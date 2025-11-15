using dotFitness.Common.Results;
using dotFitness.Modules.Users.Application.Commands;
using dotFitness.Modules.Users.Application.Services;
using dotFitness.Modules.Users.Application.Settings;
using dotFitness.Modules.Users.Domain.Entities;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace dotFitness.Modules.Users.Infrastructure.Tests.Handlers;

public class LoginWithGoogleCommandHandlerTests
{
    private readonly Mock<ILogger<LoginWithGoogleCommandHandler>> _loggerMock = new();
    private readonly Mock<IOptions<AdminSettings>> _adminSettingsMock = new();
    private readonly Mock<IGoogleAuthService> _googleAuthServiceMock = new();
    private readonly Mock<IIdentityService> _identityServiceMock = new();
    private readonly AdminSettings _adminSettings = new()
    {
        AdminEmails = ["admin@dotfitness.com"]
    };
    private readonly LoginWithGoogleCommandHandler _handler;

    public LoginWithGoogleCommandHandlerTests()
    {
        _adminSettingsMock.Setup(x => x.Value).Returns(_adminSettings);

        _handler = new LoginWithGoogleCommandHandler(
            _identityServiceMock.Object,
            _googleAuthServiceMock.Object,
            _adminSettingsMock.Object,
            _loggerMock.Object
        );
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

        _identityServiceMock
            .Setup(x => x.FindByEmailAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);

        _identityServiceMock
            .Setup(x => x.GetRolesAsync(existingUser, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<IEnumerable<string>>(new List<string> { "User" }));

        _identityServiceMock
            .Setup(x => x.SignInAsync(existingUser, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _identityServiceMock
            .Setup(x => x.GenerateUserTokenAsync(existingUser, "access_token", It.IsAny<CancellationToken>()))
            .ReturnsAsync("mock_access_token");

        _identityServiceMock
            .Setup(x => x.GenerateUserTokenAsync(existingUser, "refresh_token", It.IsAny<CancellationToken>()))
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

        _identityServiceMock
            .Setup(x => x.FindByEmailAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ApplicationUser?)null);

        _identityServiceMock
            .Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(newUser));

        _identityServiceMock
            .Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), "User", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        _identityServiceMock
            .Setup(x => x.GetRolesAsync(It.IsAny<ApplicationUser>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<IEnumerable<string>>(new List<string> { "User" }));

        _identityServiceMock
            .Setup(x => x.SignInAsync(It.IsAny<ApplicationUser>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _identityServiceMock
            .Setup(x => x.GenerateUserTokenAsync(It.IsAny<ApplicationUser>(), "access_token", It.IsAny<CancellationToken>()))
            .ReturnsAsync("mock_access_token");

        _identityServiceMock
            .Setup(x => x.GenerateUserTokenAsync(It.IsAny<ApplicationUser>(), "refresh_token", It.IsAny<CancellationToken>()))
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

        _identityServiceMock
            .Setup(x => x.FindByEmailAsync("admin@dotfitness.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(adminUser);

        _identityServiceMock
            .Setup(x => x.GetRolesAsync(adminUser, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<IEnumerable<string>>(new List<string> { "User", "Admin" }));

        _identityServiceMock
            .Setup(x => x.SignInAsync(adminUser, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _identityServiceMock
            .Setup(x => x.GenerateUserTokenAsync(adminUser, "access_token", It.IsAny<CancellationToken>()))
            .ReturnsAsync("mock_access_token");

        _identityServiceMock
            .Setup(x => x.GenerateUserTokenAsync(adminUser, "refresh_token", It.IsAny<CancellationToken>()))
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
