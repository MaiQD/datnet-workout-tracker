using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Moq;
using dotFitness.Modules.Users.Application.Services;
using dotFitness.Modules.Users.Domain.Entities;
using dotFitness.Modules.Users.Infrastructure.Data;
using dotFitness.Modules.Users.Infrastructure.Services;
using dotFitness.Modules.Users.Infrastructure.Settings;
using dotFitness.SharedKernel.Tests.PostgreSQL;

namespace dotFitness.Modules.Users.Tests.Infrastructure.Services;

public class UserServiceIntegrationTests : IAsyncLifetime
{
    private readonly PostgreSqlFixture _fixture;
    private readonly ILogger<UserService> _logger;
    private readonly AdminSettings _adminSettings;
    private UsersDbContext _context = null!;
    private UserService _userService = null!;

    public UserServiceIntegrationTests()
    {
        _fixture = new PostgreSqlFixture();
        _logger = new Mock<ILogger<UserService>>().Object;
        _adminSettings = new AdminSettings
        {
            AdminEmails = ["admin@dotfitness.com", "superuser@dotfitness.com"]
        };
    }

    public async Task InitializeAsync()
    {
        await _fixture.InitializeAsync();
        _context = _fixture.CreateInMemoryDbContext<UsersDbContext>();
        await _context.Database.EnsureCreatedAsync();
        
        var adminOptions = Options.Create(_adminSettings);
        _userService = new UserService(_context, adminOptions, _logger);
    }

    public async Task DisposeAsync()
    {
        await _context.DisposeAsync();
        await _fixture.DisposeAsync();
    }

    [Fact]
    public async Task Should_Create_New_User_When_User_Does_Not_Exist()
    {
        // Arrange
        var googleUserInfo = new GoogleUserInfo
        {
            Id = "google123",
            Email = "newuser@example.com",
            Name = "New User",
            ProfilePicture = "https://example.com/profile.jpg"
        };

        // Act
        var result = await _userService.GetOrCreateUserAsync(googleUserInfo);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.GoogleId.Should().Be("google123");
        result.Value.Email.Should().Be("newuser@example.com");
        result.Value.DisplayName.Should().Be("New User");
        result.Value.ProfilePicture.Should().Be("https://example.com/profile.jpg");
        result.Value.LoginMethod.Should().Be(LoginMethod.Google);
        result.Value.Roles.Should().Contain("User");
        result.Value.Roles.Should().NotContain("Admin");

        // Verify user was saved to database
        var savedUser = await _context.Users.FirstAsync(u => u.GoogleId == "google123");
        savedUser.Email.Should().Be("newuser@example.com");
        savedUser.DisplayName.Should().Be("New User");
    }

    [Fact]
    public async Task Should_Create_Admin_User_When_Email_Is_In_Admin_List()
    {
        // Arrange
        var googleUserInfo = new GoogleUserInfo
        {
            Id = "google456",
            Email = "admin@dotfitness.com", // This email is in the admin list
            Name = "Admin User",
            ProfilePicture = "https://example.com/admin.jpg"
        };

        // Act
        var result = await _userService.GetOrCreateUserAsync(googleUserInfo);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Email.Should().Be("admin@dotfitness.com");
        result.Value.DisplayName.Should().Be("Admin User");
        result.Value.Roles.Should().Contain("User");
        result.Value.Roles.Should().Contain("Admin");

        // Verify admin user was saved to database
        var savedUser = await _context.Users.FirstAsync(u => u.GoogleId == "google456");
        savedUser.Roles.Should().Contain("Admin");
    }

    [Fact]
    public async Task Should_Return_Existing_User_When_User_Already_Exists()
    {
        // Arrange
        var existingUser = new User
        {
            Id = "507f1f77bcf86cd799439011",
            GoogleId = "google789",
            Email = "existing@example.com",
            DisplayName = "Existing User",
            ProfilePicture = "https://example.com/old.jpg",
            LoginMethod = LoginMethod.Google,
            Roles = ["User"],
            CreatedAt = DateTime.UtcNow.AddDays(-7),
            UpdatedAt = DateTime.UtcNow.AddDays(-7)
        };

        _context.Users.Add(existingUser);
        await _context.SaveChangesAsync();

        var googleUserInfo = new GoogleUserInfo
        {
            Id = "google789", // Same Google ID as existing user
            Email = "existing@example.com",
            Name = "Existing User Updated", // Different name (should not be updated)
            ProfilePicture = "https://example.com/new.jpg" // Different profile picture (should be updated)
        };

        // Act
        var result = await _userService.GetOrCreateUserAsync(googleUserInfo);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.PostgresId.Should().Be(existingUser.PostgresId);
        result.Value.DisplayName.Should().Be("Existing User"); // Original name preserved
        result.Value.ProfilePicture.Should().Be("https://example.com/new.jpg"); // Profile picture updated
        result.Value.UpdatedAt.Should().BeAfter(existingUser.UpdatedAt);

        // Verify user was updated in database
        var updatedUser = await _context.Users.FirstAsync(u => u.GoogleId == "google789");
        updatedUser.ProfilePicture.Should().Be("https://example.com/new.jpg");
        updatedUser.UpdatedAt.Should().BeAfter(existingUser.UpdatedAt);
    }

    [Fact]
    public async Task Should_Not_Update_Profile_Picture_When_It_Hasnt_Changed()
    {
        // Arrange
        var originalUpdateTime = DateTime.UtcNow.AddDays(-1);
        var existingUser = new User
        {
            Id = "507f1f77bcf86cd799439011",
            GoogleId = "google999",
            Email = "unchanged@example.com",
            DisplayName = "Unchanged User",
            ProfilePicture = "https://example.com/same.jpg",
            LoginMethod = LoginMethod.Google,
            Roles = ["User"],
            CreatedAt = DateTime.UtcNow.AddDays(-7),
            UpdatedAt = originalUpdateTime
        };

        _context.Users.Add(existingUser);
        await _context.SaveChangesAsync();

        var googleUserInfo = new GoogleUserInfo
        {
            Id = "google999",
            Email = "unchanged@example.com",
            Name = "Unchanged User",
            ProfilePicture = "https://example.com/same.jpg" // Same profile picture
        };

        // Act
        var result = await _userService.GetOrCreateUserAsync(googleUserInfo);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.UpdatedAt.Should().Be(originalUpdateTime); // UpdatedAt should not change

        // Verify user was not updated in database
        var userInDb = await _context.Users.FirstAsync(u => u.GoogleId == "google999");
        userInDb.UpdatedAt.Should().Be(originalUpdateTime);
    }

    [Fact]
    public async Task Should_Handle_Database_Errors_Gracefully()
    {
        // Arrange
        await _context.DisposeAsync(); // Dispose context to simulate database error
        
        var googleUserInfo = new GoogleUserInfo
        {
            Id = "google_error",
            Email = "error@example.com",
            Name = "Error User",
            ProfilePicture = "https://example.com/error.jpg"
        };

        // Act
        var result = await _userService.GetOrCreateUserAsync(googleUserInfo);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("User management failed");
    }

    [Theory]
    [InlineData("admin@dotfitness.com", true)]
    [InlineData("superuser@dotfitness.com", true)]
    [InlineData("regular@example.com", false)]
    [InlineData("admin@otherdomain.com", false)]
    public async Task Should_Assign_Admin_Role_Based_On_Email_Configuration(string email, bool shouldBeAdmin)
    {
        // Arrange
        var googleUserInfo = new GoogleUserInfo
        {
            Id = $"google_{Guid.NewGuid()}",
            Email = email,
            Name = "Test User",
            ProfilePicture = "https://example.com/test.jpg"
        };

        // Act
        var result = await _userService.GetOrCreateUserAsync(googleUserInfo);

        // Assert
        result.IsSuccess.Should().BeTrue();
        
        if (shouldBeAdmin)
        {
            result.Value.Roles.Should().Contain("Admin");
        }
        else
        {
            result.Value.Roles.Should().NotContain("Admin");
        }
        
        result.Value.Roles.Should().Contain("User"); // All users should have User role
    }
}
