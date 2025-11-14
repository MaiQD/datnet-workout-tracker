using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using FluentAssertions;
using dotFitness.Modules.Users.Application.Services;
using dotFitness.Modules.Users.Domain.Entities;
using dotFitness.Modules.Users.Infrastructure.Data;
using dotFitness.Modules.Users.Infrastructure.Services;
using dotFitness.Modules.Users.Infrastructure.Settings;
using dotFitness.Modules.Users.Tests.Infrastructure.Extensions;
using dotFitness.Modules.Users.Tests.Infrastructure.Fixtures;

namespace dotFitness.Modules.Users.Tests.Infrastructure.Services;

public class UserServiceTests : IAsyncLifetime
{
    private readonly UsersUnitTestFixture _fixture = new();
    private readonly Mock<ILogger<UserService>> _loggerMock = new();
    private readonly Mock<IOptions<AdminSettings>> _adminSettingsMock = new();
    private readonly AdminSettings _adminSettings = new()
    {
        AdminEmails = ["admin@dotfitness.com"]
    };
    private UserService _service = null!;
    private UsersDbContext _context = null!;

    public async Task InitializeAsync()
    {
        _adminSettingsMock.Setup(x => x.Value).Returns(_adminSettings);

        _service = new UserService(
            _context,
            _adminSettingsMock.Object,
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
    public async Task GetOrCreateUserAsync_Should_Return_Existing_User_When_Found()
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

        var googleUserInfo = new GoogleUserInfo(
            "google123",
            email,
            "Test User",
            "https://example.com/profile.jpg"
        );

        // Act
        var result = await _service.GetOrCreateUserAsync(googleUserInfo);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Id.Should().Be(existingUser.Id);
        result.Value.Email.Should().Be(email);
    }

    [Fact]
    public async Task GetOrCreateUserAsync_Should_Create_New_User_When_Not_Found()
    {
        // Arrange
        var email = this.GenerateUniqueEmail();
        var googleUserInfo = new GoogleUserInfo(
            "newgoogle123",
            email,
            "New User",
            "https://example.com/profile.jpg"
        );

        // Act
        var result = await _service.GetOrCreateUserAsync(googleUserInfo);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Email.Should().Be(email);
        result.Value.GoogleId.Should().Be("newgoogle123");
        result.Value.DisplayName.Should().Be("New User");
        result.Value.ProfilePicture.Should().Be("https://example.com/profile.jpg");
        result.Value.LoginMethod.Should().Be(LoginMethod.Google);
        result.Value.EmailConfirmed.Should().BeTrue();
    }

    [Fact]
    public async Task GetOrCreateUserAsync_Should_Update_Profile_Picture_When_Changed()
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
            ProfilePicture = "https://example.com/old-profile.jpg",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Users.Add(existingUser);
        await _context.SaveChangesAsync();

        var googleUserInfo = new GoogleUserInfo(
            "google123",
            email,
            "Test User",
            "https://example.com/new-profile.jpg"
        );

        // Act
        var result = await _service.GetOrCreateUserAsync(googleUserInfo);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.ProfilePicture.Should().Be("https://example.com/new-profile.jpg");
        result.Value.UpdatedAt.Should().BeAfter(existingUser.UpdatedAt);
    }

    [Fact]
    public async Task GetOrCreateUserAsync_Should_Not_Update_Profile_Picture_When_Unchanged()
    {
        // Arrange
        var email = this.GenerateUniqueEmail();
        var profilePicture = "https://example.com/profile.jpg";
        var existingUser = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            Email = email,
            UserName = email,
            DisplayName = "Test User",
            GoogleId = "google123",
            ProfilePicture = profilePicture,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Users.Add(existingUser);
        await _context.SaveChangesAsync();

        var googleUserInfo = new GoogleUserInfo(
            "google123",
            email,
            "Test User",
            profilePicture
        );

        // Act
        var result = await _service.GetOrCreateUserAsync(googleUserInfo);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.ProfilePicture.Should().Be(profilePicture);
        result.Value.UpdatedAt.Should().Be(existingUser.UpdatedAt);
    }

    [Fact]
    public async Task GetOrCreateUserAsync_Should_Handle_Admin_Email()
    {
        // Arrange
        var email = "admin@dotfitness.com";
        var googleUserInfo = new GoogleUserInfo(
            "admingoogle123",
            email,
            "Admin User",
            "https://example.com/admin.jpg"
        );

        // Act
        var result = await _service.GetOrCreateUserAsync(googleUserInfo);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Email.Should().Be(email);
        result.Value.GoogleId.Should().Be("admingoogle123");
        result.Value.DisplayName.Should().Be("Admin User");
    }

    [Fact]
    public async Task GetOrCreateUserAsync_Should_Handle_Database_Errors()
    {
        // Arrange
        var email = this.GenerateUniqueEmail();
        var googleUserInfo = new GoogleUserInfo(
            "google123",
            email,
            "Test User",
            "https://example.com/profile.jpg"
        );

        // Dispose context to simulate database error
        await _context.DisposeAsync();

        // Act
        var result = await _service.GetOrCreateUserAsync(googleUserInfo);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("User management failed");
    }
}
