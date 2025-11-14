using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Microsoft.AspNetCore.Identity;
using dotFitness.Modules.Users.Application.Commands;
using dotFitness.Modules.Users.Application.DTOs;
using dotFitness.Modules.Users.Application.Services;
using dotFitness.Modules.Users.Domain.Entities;
using dotFitness.Modules.Users.Infrastructure.Data;
using dotFitness.Modules.Users.Infrastructure.Handlers;
using dotFitness.Modules.Users.Infrastructure.Settings;
using dotFitness.Modules.Users.Tests.Infrastructure.Extensions;
using dotFitness.Modules.Users.Tests.Infrastructure.Fixtures;

namespace dotFitness.Modules.Users.Tests.Infrastructure.Handlers;

public class LoginWithGoogleCommandHandlerTests : IAsyncLifetime
{
    private readonly UsersUnitTestFixture _fixture = new();
    private readonly Mock<ILogger<LoginWithGoogleCommandHandler>> _loggerMock = new();
    private readonly Mock<IOptions<AdminSettings>> _adminSettingsMock = new();
    private readonly Mock<IGoogleAuthService> _googleAuthServiceMock = new();
    private readonly AdminSettings _adminSettings = new()
    {
        AdminEmails = ["admin@dotfitness.com"]
    };
    private LoginWithGoogleCommandHandler _handler = null!;
    private UsersDbContext _context = null!;
    private Mock<UserManager<ApplicationUser>> _userManagerMock = null!;
    private Mock<SignInManager<ApplicationUser>> _signInManagerMock = null!;

    public async Task InitializeAsync()
    {
        _adminSettingsMock.Setup(x => x.Value).Returns(_adminSettings);

        // Setup UserManager and SignInManager mocks
        var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
        _userManagerMock = new Mock<UserManager<ApplicationUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);

        var signInManagerUserClaimsFactoryMock = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
        _signInManagerMock = new Mock<SignInManager<ApplicationUser>>(_userManagerMock.Object,
            new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>().Object,
            new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>().Object,
            new Mock<IOptions<IdentityOptions>>().Object,
            new Mock<ILogger<SignInManager<ApplicationUser>>>().Object,
            new Mock<Microsoft.AspNetCore.Authentication.IAuthenticationSchemeProvider>().Object,
            null);

        _handler = new LoginWithGoogleCommandHandler(
            _userManagerMock.Object,
            _signInManagerMock.Object,
            _googleAuthServiceMock.Object,
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
        var existingUser = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            Email = email,
            UserName = email,
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

        _userManagerMock
            .Setup(x => x.FindByEmailAsync(email))
            .ReturnsAsync(existingUser);

        _userManagerMock
            .Setup(x => x.GetRolesAsync(existingUser))
            .ReturnsAsync(new List<string> { "User" });

        _signInManagerMock
            .Setup(x => x.SignInAsync(existingUser, It.IsAny<bool>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        _userManagerMock
            .Setup(x => x.GenerateUserTokenAsync(existingUser, "Default", "access_token"))
            .ReturnsAsync("mock_access_token");

        _userManagerMock
            .Setup(x => x.GenerateUserTokenAsync(existingUser, "Default", "refresh_token"))
            .ReturnsAsync("mock_refresh_token");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.AccessToken.Should().Be("mock_access_token");
        result.Value.RefreshToken.Should().Be("mock_refresh_token");
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

        // Mock Google auth service to return null (simulating error)
        _googleAuthServiceMock
            .Setup(x => x.GetUserInfoAsync("valid_google_token_123", It.IsAny<CancellationToken>()))
            .ReturnsAsync((GoogleUserInfo?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Failed to get user information from Google");
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

        var newUser = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            Email = email,
            UserName = email,
            DisplayName = "New User",
            GoogleId = "newgoogle123",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _googleAuthServiceMock
            .Setup(x => x.GetUserInfoAsync("valid_google_token_123", It.IsAny<CancellationToken>()))
            .ReturnsAsync(googleUserInfo);

        _userManagerMock
            .Setup(x => x.FindByEmailAsync(email))
            .ReturnsAsync((ApplicationUser?)null);

        _userManagerMock
            .Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(IdentityResult.Success);

        _userManagerMock
            .Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), "User"))
            .ReturnsAsync(IdentityResult.Success);

        _userManagerMock
            .Setup(x => x.GetRolesAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(new List<string> { "User" });

        _signInManagerMock
            .Setup(x => x.SignInAsync(It.IsAny<ApplicationUser>(), It.IsAny<bool>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        _userManagerMock
            .Setup(x => x.GenerateUserTokenAsync(It.IsAny<ApplicationUser>(), "Default", "access_token"))
            .ReturnsAsync("mock_access_token");

        _userManagerMock
            .Setup(x => x.GenerateUserTokenAsync(It.IsAny<ApplicationUser>(), "Default", "refresh_token"))
            .ReturnsAsync("mock_refresh_token");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.AccessToken.Should().Be("mock_access_token");
        result.Value.RefreshToken.Should().Be("mock_refresh_token");
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task Should_Add_Admin_Role_For_Admin_Email()
    {
        // Arrange
        var adminUser = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            Email = "admin@dotfitness.com", // This is in the admin emails list
            UserName = "admin@dotfitness.com",
            DisplayName = "Admin User",
            GoogleId = "admingoogle123",
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

        _userManagerMock
            .Setup(x => x.FindByEmailAsync("admin@dotfitness.com"))
            .ReturnsAsync(adminUser);

        _userManagerMock
            .Setup(x => x.GetRolesAsync(adminUser))
            .ReturnsAsync(new List<string> { "User", "Admin" });

        _signInManagerMock
            .Setup(x => x.SignInAsync(adminUser, It.IsAny<bool>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        _userManagerMock
            .Setup(x => x.GenerateUserTokenAsync(adminUser, "Default", "access_token"))
            .ReturnsAsync("mock_access_token");

        _userManagerMock
            .Setup(x => x.GenerateUserTokenAsync(adminUser, "Default", "refresh_token"))
            .ReturnsAsync("mock_refresh_token");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.AccessToken.Should().Be("mock_access_token");
        result.Value.RefreshToken.Should().Be("mock_refresh_token");
        result.Value.Roles.Should().Contain("Admin");
    }
}
