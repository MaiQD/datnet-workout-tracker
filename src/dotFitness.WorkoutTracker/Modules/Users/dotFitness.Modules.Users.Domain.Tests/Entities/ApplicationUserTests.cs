using dotFitness.Modules.Users.Domain.Entities;
using FluentAssertions;

namespace dotFitness.Modules.Users.Domain.Tests.Entities;

public class ApplicationUserTests
{
    [Fact]
    public void Should_Create_ApplicationUser_With_Default_Values()
    {
        // Act
        var user = new ApplicationUser();

        // Assert
        user.Should().NotBeNull();
        user.LoginMethod.Should().Be(LoginMethod.Google);
        user.UnitPreference.Should().Be(UnitPreference.Metric);
        user.IsOnboarded.Should().BeFalse();
        user.AvailableEquipmentIds.Should().BeEmpty();
        user.FocusMuscleGroupIds.Should().BeEmpty();
        user.Clients.Should().BeEmpty();
    }

    [Fact]
    public void Should_Set_Properties_Correctly()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = "test@example.com";
        var displayName = "Test User";
        var googleId = "google123";
        var profilePicture = "https://example.com/profile.jpg";

        // Act
        var user = new ApplicationUser
        {
            Id = userId,
            Email = email,
            UserName = email,
            DisplayName = displayName,
            GoogleId = googleId,
            ProfilePicture = profilePicture,
            LoginMethod = LoginMethod.Google,
            Gender = Gender.Male,
            DateOfBirth = new DateTime(1990, 1, 1),
            UnitPreference = UnitPreference.Imperial,
            IsOnboarded = true,
            OnboardingCompletedAt = DateTime.UtcNow
        };

        // Assert
        user.Id.Should().Be(userId);
        user.Email.Should().Be(email);
        user.UserName.Should().Be(email);
        user.DisplayName.Should().Be(displayName);
        user.GoogleId.Should().Be(googleId);
        user.ProfilePicture.Should().Be(profilePicture);
        user.LoginMethod.Should().Be(LoginMethod.Google);
        user.Gender.Should().Be(Gender.Male);
        user.DateOfBirth.Should().Be(new DateTime(1990, 1, 1));
        user.UnitPreference.Should().Be(UnitPreference.Imperial);
        user.IsOnboarded.Should().BeTrue();
        user.OnboardingCompletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Should_Handle_PT_Relationship()
    {
        // Arrange
        var pt = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            Email = "pt@example.com",
            UserName = "pt@example.com",
            DisplayName = "Personal Trainer"
        };

        var client = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            Email = "client@example.com",
            UserName = "client@example.com",
            DisplayName = "Client User",
            AssignedPtId = pt.Id,
            AssignedPt = pt
        };

        // Act
        pt.Clients.Add(client);

        // Assert
        client.AssignedPt.Should().Be(pt);
        client.AssignedPtId.Should().Be(pt.Id);
        pt.Clients.Should().Contain(client);
    }

    [Fact]
    public void Should_Handle_Equipment_And_Muscle_Group_Ids()
    {
        // Arrange
        var user = new ApplicationUser();
        var equipmentIds = new List<string> { "equipment1", "equipment2" };
        var muscleGroupIds = new List<string> { "muscle1", "muscle2" };

        // Act
        user.AvailableEquipmentIds = equipmentIds;
        user.FocusMuscleGroupIds = muscleGroupIds;

        // Assert
        user.AvailableEquipmentIds.Should().BeEquivalentTo(equipmentIds);
        user.FocusMuscleGroupIds.Should().BeEquivalentTo(muscleGroupIds);
    }

    [Fact]
    public void Should_Update_Timestamps_On_Creation()
    {
        // Act
        var user = new ApplicationUser();

        // Assert
        user.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        user.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }
}
