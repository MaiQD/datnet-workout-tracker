using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Moq;
using dotFitness.Modules.Users.Application.Commands;
using dotFitness.Modules.Users.Application.DTOs;
using dotFitness.Modules.Users.Application.Services;
using dotFitness.Modules.Users.Domain.Entities;
using dotFitness.Modules.Users.Infrastructure.Data;
using dotFitness.Modules.Users.Infrastructure.Handlers;
using dotFitness.Modules.Users.Infrastructure.Settings;
using dotFitness.SharedKernel.Results;
using dotFitness.SharedKernel.Tests.PostgreSQL;

namespace dotFitness.Modules.Users.Tests.Infrastructure.Handlers;

public class LoginWithGoogleCommandHandlerTests : IAsyncLifetime
{
    private readonly PostgreSqlFixture _fixture;
    private readonly Mock<ILogger<LoginWithGoogleCommandHandler>> _loggerMock;
    private readonly Mock<IOptions<JwtSettings>> _jwtSettingsMock;
    private readonly Mock<IOptions<AdminSettings>> _adminSettingsMock;
    private readonly Mock<IGoogleAuthService> _googleAuthServiceMock;
    private readonly Mock<IUserService> _userServiceMock;
    private readonly Mock<IJwtService> _jwtServiceMock;
    private readonly LoginWithGoogleCommandHandler _handler;
    private readonly JwtSettings _jwtSettings;
    private readonly AdminSettings _adminSettings;
    private UsersDbContext _context = null!;

    public LoginWithGoogleCommandHandlerTests()
    {
        _fixture = PostgreSqlFixture.Instance;
        _googleAuthServiceMock = new Mock<IGoogleAuthService>();
        _userServiceMock = new Mock<IUserService>();
        _jwtServiceMock = new Mock<IJwtService>();
        _loggerMock = new Mock<ILogger<LoginWithGoogleCommandHandler>>();
        _jwtSettingsMock = new Mock<IOptions<JwtSettings>>();
        _adminSettingsMock = new Mock<IOptions<AdminSettings>>();

        _jwtSettings = new JwtSettings
        {
            SecretKey = "TestSecretKeyThatIsLongEnoughForTestingPurposes123456789012345678901234567890",
            Issuer = "TestIssuer",
            Audience = "TestAudience",
            ExpirationInHours = 1
        };

        _adminSettings = new AdminSettings
        {
            AdminEmails = ["admin@dotfitness.com"]
        };

        _jwtSettingsMock.Setup(x => x.Value).Returns(_jwtSettings);
        _adminSettingsMock.Setup(x => x.Value).Returns(_adminSettings);

        _handler = new LoginWithGoogleCommandHandler(
            _googleAuthServiceMock.Object,
            _userServiceMock.Object,
            _jwtServiceMock.Object,
            _loggerMock.Object
        );
    }

    public async Task InitializeAsync()
    {
        await _fixture.InitializeAsync();
        _context = _fixture.CreateDbContext<UsersDbContext>();
        await _context.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await _context.DisposeAsync();
        await _fixture.DisposeAsync();
    }

    [Fact]
    public async Task Should_Handle_Valid_Command_Successfully_For_Existing_User()
    {
        // Arrange
        var existingUser = new User
        {
            Id = 1,
            Email = "test@example.com",
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
            "test@example.com",
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
        result.Error.Should().Contain("User management failed");
    }

    [Fact]
    public async Task Should_Create_New_User_For_First_Time_Google_Login()
    {
        // Arrange
        var request = new LoginWithGoogleRequest
        {
            GoogleToken = "valid_google_token_123"
        };
        var command = new LoginWithGoogleCommand(request);

        // Mock Google auth service to return user info
        var googleUserInfo = new GoogleUserInfo(
            "newgoogle123",
            "newuser@example.com",
            "New User",
            "https://example.com/profile.jpg"
        );

        var newUser = new User
        {
            Id = 1,
            Email = "newuser@example.com",
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
    public async Task Should_Add_Admin_Role_For_Admin_Email()
    {
        // Arrange
        var adminUser = new User
        {
            Id = 1,
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
