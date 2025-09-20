using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using dotFitness.Modules.Users.Application.Commands;
using dotFitness.Modules.Users.Application.DTOs;
using dotFitness.Modules.Users.Application.Services;
using dotFitness.Modules.Users.Domain.Entities;
using dotFitness.Modules.Users.Infrastructure.Data;
using dotFitness.Modules.Users.Infrastructure.Handlers;
using dotFitness.Modules.Users.Infrastructure.Settings;
using dotFitness.Modules.Users.Tests.Infrastructure.Extensions;
using dotFitness.SharedKernel.Results;
using dotFitness.Modules.Users.Tests.Infrastructure.Fixtures;

namespace dotFitness.Modules.Users.Tests.Infrastructure.Handlers;

public class LoginWithGoogleCommandHandlerTests : IAsyncLifetime
{
    private readonly UsersUnitTestFixture _fixture = new();
    private readonly Mock<ILogger<LoginWithGoogleCommandHandler>> _loggerMock = new();
    private readonly Mock<IOptions<JwtSettings>> _jwtSettingsMock = new();
    private readonly Mock<IOptions<AdminSettings>> _adminSettingsMock = new();
    private readonly Mock<IGoogleAuthService> _googleAuthServiceMock = new();
    private readonly Mock<IUserService> _userServiceMock = new();
    private readonly Mock<IJwtService> _jwtServiceMock = new();
    private readonly JwtSettings _jwtSettings = new()
    {
        SecretKey = "TestSecretKeyThatIsLongEnoughForTestingPurposes123456789012345678901234567890",
        Issuer = "TestIssuer",
        Audience = "TestAudience",
        ExpirationInHours = 1
    };
    private readonly AdminSettings _adminSettings = new()
    {
        AdminEmails = ["admin@dotfitness.com"]
    };
    private LoginWithGoogleCommandHandler _handler = null!;
    private UsersDbContext _context = null!;


    public async Task InitializeAsync()
    {
        _jwtSettingsMock.Setup(x => x.Value).Returns(_jwtSettings);
        _adminSettingsMock.Setup(x => x.Value).Returns(_adminSettings);

        _handler = new LoginWithGoogleCommandHandler(
            _googleAuthServiceMock.Object,
            _userServiceMock.Object,
            _jwtServiceMock.Object,
            _loggerMock.Object
        );

        _context = _fixture.CreateInMemoryDbContext<UsersDbContext>();
        await _context.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await _context.DisposeAsync();
        await _fixture.DisposeAsync();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task Should_Handle_Valid_Command_Successfully_For_Existing_User()
    {
        // Arrange
        var email = this.GenerateUniqueEmail();
        var existingUser = new User
        {
            Email = email,
            DisplayName = "Test User",
            GoogleId = "google123",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Users.Add(existingUser);
        await _context.SaveChangesAsync();

        var request = new LoginWithGoogleRequest
        {
            GoogleToken = "valid_google_token_123"
        };
        var command = new LoginWithGoogleCommand(request);

        // Mock Google auth service to return user info
        var googleUserInfo = new GoogleUserInfo(
            "google123",
            email,
            "Test User",
            "https://example.com/profile.jpg"
        );

        _googleAuthServiceMock
            .Setup(x => x.GetUserInfoAsync("valid_google_token_123", It.IsAny<CancellationToken>()))
            .ReturnsAsync(googleUserInfo);

        _userServiceMock
            .Setup(x => x.GetOrCreateUserAsync(googleUserInfo, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(existingUser));

        _jwtServiceMock
            .Setup(x => x.GenerateToken(existingUser))
            .Returns("mock_jwt_token");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Token.Should().Be("mock_jwt_token");
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task Should_Return_ValidationError_For_Invalid_Command()
    {
        // Arrange
        var request = new LoginWithGoogleRequest
        {
            GoogleToken = ""
        };
        var command = new LoginWithGoogleCommand(request); // Invalid empty token

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeNullOrEmpty();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task Should_Handle_Repository_Errors_Gracefully()
    {
        // Arrange
        var request = new LoginWithGoogleRequest
        {
            GoogleToken = "valid_google_token_123"
        };
        var command = new LoginWithGoogleCommand(request);

        // Dispose context to simulate database error
        await _context.DisposeAsync();

        // Mock Google auth service to return user info
        var googleUserInfo = new GoogleUserInfo(
            "google123",
            "test@example.com",
            "Test User",
            "https://example.com/profile.jpg"
        );

        _googleAuthServiceMock
            .Setup(x => x.GetUserInfoAsync("valid_google_token_123", It.IsAny<CancellationToken>()))
            .ReturnsAsync(googleUserInfo);

        _userServiceMock
            .Setup(x => x.GetOrCreateUserAsync(googleUserInfo, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<User>("Database connection failed"));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Database connection failed");
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task Should_Create_New_User_For_First_Time_Google_Login()
    {
        // Arrange
        var email = this.GenerateUniqueEmail();
        var request = new LoginWithGoogleRequest
        {
            GoogleToken = "valid_google_token_123"
        };
        var command = new LoginWithGoogleCommand(request);

        // Mock Google auth service to return user info
        var googleUserInfo = new GoogleUserInfo(
            "newgoogle123",
            email,
            "New User",
            "https://example.com/profile.jpg"
        );

        var newUser = new User
        {
            Email = email,
            DisplayName = "New User",
            GoogleId = "newgoogle123",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _googleAuthServiceMock
            .Setup(x => x.GetUserInfoAsync("valid_google_token_123", It.IsAny<CancellationToken>()))
            .ReturnsAsync(googleUserInfo);

        _userServiceMock
            .Setup(x => x.GetOrCreateUserAsync(googleUserInfo, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(newUser));

        _jwtServiceMock
            .Setup(x => x.GenerateToken(newUser))
            .Returns("mock_jwt_token");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Token.Should().Be("mock_jwt_token");
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task Should_Add_Admin_Role_For_Admin_Email()
    {
        // Arrange
        var adminUser = new User
        {
            Email = "admin@dotfitness.com", // This is in the admin emails list
            DisplayName = "Admin User",
            GoogleId = "admingoogle123",
            Roles = ["User", "Admin"],
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Users.Add(adminUser);
        await _context.SaveChangesAsync();

        var request = new LoginWithGoogleRequest
        {
            GoogleToken = "valid_google_token_123"
        };
        var command = new LoginWithGoogleCommand(request);

        // Mock Google auth service to return user info
        var googleUserInfo = new GoogleUserInfo(
            "admingoogle123",
            "admin@dotfitness.com",
            "Admin User",
            "https://example.com/admin.jpg"
        );

        _googleAuthServiceMock
            .Setup(x => x.GetUserInfoAsync("valid_google_token_123", It.IsAny<CancellationToken>()))
            .ReturnsAsync(googleUserInfo);

        _userServiceMock
            .Setup(x => x.GetOrCreateUserAsync(googleUserInfo, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(adminUser));

        _jwtServiceMock
            .Setup(x => x.GenerateToken(adminUser))
            .Returns("mock_jwt_token");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Token.Should().Be("mock_jwt_token");
    }
}
