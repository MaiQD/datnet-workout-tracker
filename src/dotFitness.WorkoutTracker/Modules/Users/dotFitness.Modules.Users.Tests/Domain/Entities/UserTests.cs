using FluentAssertions;
using dotFitness.Modules.Users.Domain.Entities;

namespace dotFitness.Modules.Users.Tests.Domain.Entities;

public class UserTests
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
        user.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        user.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        user.IsAdmin.Should().BeFalse();
    }

    [Fact]
    public void Should_Add_Role_Successfully()
    {
        // Arrange
        var user = new User { Email = "test@example.com", DisplayName = "Test User" };
        var originalUpdatedAt = user.UpdatedAt;

        // Act
        user.AddRole("Admin");

        // Assert
        user.Roles.Should().Contain("Admin");
        user.IsAdmin.Should().BeTrue();
        user.UpdatedAt.Should().BeOnOrAfter(originalUpdatedAt);
    }

    [Fact]
    public void Should_Not_Add_Duplicate_Role()
    {
        // Arrange
        var user = new User { Email = "test@example.com", DisplayName = "Test User" };
        user.AddRole("Admin");
        var originalUpdatedAt = user.UpdatedAt;
        var originalRoleCount = user.Roles.Count;

        Thread.Sleep(10); // Ensure time difference

        // Act
        user.AddRole("Admin");

        // Assert
        user.Roles.Count.Should().Be(originalRoleCount);
        user.UpdatedAt.Should().Be(originalUpdatedAt);
    }

    [Fact]
    public void Should_Remove_Role_Successfully()
    {
        // Arrange
        var user = new User { Email = "test@example.com", DisplayName = "Test User" };
        user.AddRole("Admin");
        var originalUpdatedAt = user.UpdatedAt;

        Thread.Sleep(10); // Ensure time difference

        // Act
        user.RemoveRole("Admin");

        // Assert
        user.Roles.Should().NotContain("Admin");
        user.IsAdmin.Should().BeFalse();
        user.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    [Fact]
    public void Should_Not_Remove_Base_User_Role()
    {
        // Arrange
        var user = new User { Email = "test@example.com", DisplayName = "Test User" };
        var originalUpdatedAt = user.UpdatedAt;

        Thread.Sleep(10); // Ensure time difference

        // Act
        user.RemoveRole("User");

        // Assert
        user.Roles.Should().Contain("User");
        user.UpdatedAt.Should().Be(originalUpdatedAt);
    }

    [Theory]
    [InlineData("New Display Name")]
    [InlineData("Another Name")]
    public void Should_Update_Display_Name(string newDisplayName)
    {
        // Arrange
        var user = new User { Email = "test@example.com", DisplayName = "Old Name" };
        var originalUpdatedAt = user.UpdatedAt;

        Thread.Sleep(10); // Ensure time difference

        // Act
        user.UpdateProfile(displayName: newDisplayName);

        // Assert
        user.DisplayName.Should().Be(newDisplayName);
        user.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    [Theory]
    [InlineData(Gender.Male)]
    [InlineData(Gender.Female)]
    [InlineData(Gender.Other)]
    [InlineData(Gender.PreferNotToSay)]
    public void Should_Update_Gender(Gender gender)
    {
        // Arrange
        var user = new User { Email = "test@example.com", DisplayName = "Test User" };
        var originalUpdatedAt = user.UpdatedAt;

        Thread.Sleep(10); // Ensure time difference

        // Act
        user.UpdateProfile(gender: gender);

        // Assert
        user.Gender.Should().Be(gender);
        user.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    [Fact]
    public void Should_Update_Date_Of_Birth()
    {
        // Arrange
        var user = new User { Email = "test@example.com", DisplayName = "Test User" };
        var dateOfBirth = new DateTime(1990, 1, 1);
        var originalUpdatedAt = user.UpdatedAt;

        Thread.Sleep(10); // Ensure time difference

        // Act
        user.UpdateProfile(dateOfBirth: dateOfBirth);

        // Assert
        user.DateOfBirth.Should().Be(dateOfBirth);
        user.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    [Theory]
    [InlineData(UnitPreference.Metric)]
    [InlineData(UnitPreference.Imperial)]
    public void Should_Update_Unit_Preference(UnitPreference unitPreference)
    {
        // Arrange
        var user = new User { Email = "test@example.com", DisplayName = "Test User" };
        var originalUpdatedAt = user.UpdatedAt;

        Thread.Sleep(10); // Ensure time difference

        // Act
        user.UpdateProfile(unitPreference: unitPreference);

        // Assert
        user.UnitPreference.Should().Be(unitPreference);
        user.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    [Fact]
    public void Should_Not_Update_Profile_With_Empty_Display_Name()
    {
        // Arrange
        var user = new User { Email = "test@example.com", DisplayName = "Original Name" };
        var originalUpdatedAt = user.UpdatedAt;

        Thread.Sleep(10); // Ensure time difference

        // Act
        user.UpdateProfile(displayName: "");

        // Assert
        user.DisplayName.Should().Be("Original Name");
        user.UpdatedAt.Should().Be(originalUpdatedAt);
    }

    [Fact]
    public void Should_Update_Multiple_Profile_Properties()
    {
        // Arrange
        var user = new User { Email = "test@example.com", DisplayName = "Old Name" };
        var dateOfBirth = new DateTime(1990, 1, 1);
        var originalUpdatedAt = user.UpdatedAt;

        Thread.Sleep(10); // Ensure time difference

        // Act
        user.UpdateProfile(
            displayName: "New Name",
            gender: Gender.Male,
            dateOfBirth: dateOfBirth,
            unitPreference: UnitPreference.Imperial
        );

        // Assert
        user.DisplayName.Should().Be("New Name");
        user.Gender.Should().Be(Gender.Male);
        user.DateOfBirth.Should().Be(dateOfBirth);
        user.UnitPreference.Should().Be(UnitPreference.Imperial);
        user.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }
}
