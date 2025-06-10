using FluentAssertions;
using dotFitness.Modules.Users.Domain.Entities;

namespace dotFitness.Modules.Users.Tests.Domain.Entities;

public class UserSimpleTests
{
    [Fact]
    public void Should_Create_Valid_User_With_Required_Properties()
    {
        // Arrange & Act
        var user = new User
        {
            Email = "test@example.com",
            DisplayName = "Test User"
        };

        // Assert
        user.Id.Should().NotBeNullOrEmpty();
        user.Email.Should().Be("test@example.com");
        user.DisplayName.Should().Be("Test User");
        user.LoginMethod.Should().Be(LoginMethod.Google);
        user.UnitPreference.Should().Be(UnitPreference.Metric);
        user.Roles.Should().Contain("User");
        user.IsAdmin.Should().BeFalse();
    }

    [Fact]
    public void Should_Add_Role_Successfully()
    {
        // Arrange
        var user = new User { Email = "test@example.com", DisplayName = "Test User" };

        // Act
        user.AddRole("Admin");

        // Assert
        user.Roles.Should().Contain("Admin");
        user.IsAdmin.Should().BeTrue();
    }

    [Fact]
    public void Should_Update_Profile_Successfully()
    {
        // Arrange
        var user = new User { Email = "test@example.com", DisplayName = "Old Name" };

        // Act
        user.UpdateProfile(displayName: "New Name", gender: Gender.Male);

        // Assert
        user.DisplayName.Should().Be("New Name");
        user.Gender.Should().Be(Gender.Male);
    }
}
