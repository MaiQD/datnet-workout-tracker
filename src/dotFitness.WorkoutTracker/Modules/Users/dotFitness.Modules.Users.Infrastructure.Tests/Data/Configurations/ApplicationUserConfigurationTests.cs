using dotFitness.Modules.Users.Domain.Entities;
using dotFitness.Modules.Users.Infrastructure.Data.Configurations;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace dotFitness.Modules.Users.Infrastructure.Tests.Data.Configurations;

public class ApplicationUserConfigurationTests
{
    [Fact]
    public void Configure_Should_Set_Correct_Table_Name()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<DbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new DbContext(options);
        var modelBuilder = new ModelBuilder();
        
        // Set default schema to match DbContext behavior
        modelBuilder.HasDefaultSchema("users");
        
        var configuration = new ApplicationUserConfiguration();

        // Act
        configuration.Configure(modelBuilder.Entity<ApplicationUser>());
        var model = modelBuilder.Model;
        var entityType = model.FindEntityType(typeof(ApplicationUser));

        // Assert
        entityType.Should().NotBeNull();
        entityType!.GetTableName().Should().Be("AspNetUsers");
        entityType.GetSchema().Should().Be("users"); // Uses default schema from DbContext
    }

    [Fact]
    public void Configure_Should_Set_Correct_Property_Configurations()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<DbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new DbContext(options);
        var modelBuilder = new ModelBuilder();
        var configuration = new ApplicationUserConfiguration();

        // Act
        configuration.Configure(modelBuilder.Entity<ApplicationUser>());
        var model = modelBuilder.Model;
        var entityType = model.FindEntityType(typeof(ApplicationUser));

        // Assert
        entityType.Should().NotBeNull();
        
        // Check DisplayName property
        var displayNameProperty = entityType!.FindProperty(nameof(ApplicationUser.DisplayName));
        displayNameProperty.Should().NotBeNull();
        displayNameProperty!.GetMaxLength().Should().Be(256);
        displayNameProperty.IsRequired().Should().BeTrue();

        // Check GoogleId property
        var googleIdProperty = entityType.FindProperty(nameof(ApplicationUser.GoogleId));
        googleIdProperty.Should().NotBeNull();
        googleIdProperty!.GetMaxLength().Should().Be(256);
        googleIdProperty.IsRequired().Should().BeFalse();

        // Check ProfilePicture property
        var profilePictureProperty = entityType.FindProperty(nameof(ApplicationUser.ProfilePicture));
        profilePictureProperty.Should().NotBeNull();
        profilePictureProperty!.GetMaxLength().Should().Be(512);
        profilePictureProperty.IsRequired().Should().BeFalse();

        // Check LoginMethod property
        var loginMethodProperty = entityType.FindProperty(nameof(ApplicationUser.LoginMethod));
        loginMethodProperty.Should().NotBeNull();
        loginMethodProperty!.GetMaxLength().Should().Be(50);
        loginMethodProperty.IsRequired().Should().BeTrue();

        // Check Gender property
        var genderProperty = entityType.FindProperty(nameof(ApplicationUser.Gender));
        genderProperty.Should().NotBeNull();
        genderProperty!.GetMaxLength().Should().Be(50);
        genderProperty.IsRequired().Should().BeFalse();

        // Check DateOfBirth property
        var dateOfBirthProperty = entityType.FindProperty(nameof(ApplicationUser.DateOfBirth));
        dateOfBirthProperty.Should().NotBeNull();
        dateOfBirthProperty!.GetColumnType().Should().Be("date");

        // Check UnitPreference property
        var unitPreferenceProperty = entityType.FindProperty(nameof(ApplicationUser.UnitPreference));
        unitPreferenceProperty.Should().NotBeNull();
        unitPreferenceProperty!.GetMaxLength().Should().Be(50);
        unitPreferenceProperty.IsRequired().Should().BeTrue();

        // Check CreatedAt property
        var createdAtProperty = entityType.FindProperty(nameof(ApplicationUser.CreatedAt));
        createdAtProperty.Should().NotBeNull();
        createdAtProperty!.GetColumnType().Should().Be("timestamp with time zone");
        createdAtProperty.IsRequired().Should().BeTrue();

        // Check UpdatedAt property
        var updatedAtProperty = entityType.FindProperty(nameof(ApplicationUser.UpdatedAt));
        updatedAtProperty.Should().NotBeNull();
        updatedAtProperty!.GetColumnType().Should().Be("timestamp with time zone");
        updatedAtProperty.IsRequired().Should().BeTrue();

        // Check IsOnboarded property
        var isOnboardedProperty = entityType.FindProperty(nameof(ApplicationUser.IsOnboarded));
        isOnboardedProperty.Should().NotBeNull();
        isOnboardedProperty!.IsRequired().Should().BeTrue();

        // Check OnboardingCompletedAt property
        var onboardingCompletedAtProperty = entityType.FindProperty(nameof(ApplicationUser.OnboardingCompletedAt));
        onboardingCompletedAtProperty.Should().NotBeNull();
        onboardingCompletedAtProperty!.GetColumnType().Should().Be("timestamp with time zone");
        onboardingCompletedAtProperty.IsRequired().Should().BeFalse();

        // Check AvailableEquipmentIds property
        var availableEquipmentIdsProperty = entityType.FindProperty(nameof(ApplicationUser.AvailableEquipmentIds));
        availableEquipmentIdsProperty.Should().NotBeNull();
        availableEquipmentIdsProperty!.GetColumnType().Should().Be("jsonb");

        // Check FocusMuscleGroupIds property
        var focusMuscleGroupIdsProperty = entityType.FindProperty(nameof(ApplicationUser.FocusMuscleGroupIds));
        focusMuscleGroupIdsProperty.Should().NotBeNull();
        focusMuscleGroupIdsProperty!.GetColumnType().Should().Be("jsonb");
    }

    [Fact]
    public void Configure_Should_Set_Correct_Relationship_Configurations()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<DbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new DbContext(options);
        var modelBuilder = new ModelBuilder();
        var configuration = new ApplicationUserConfiguration();

        // Act
        configuration.Configure(modelBuilder.Entity<ApplicationUser>());
        var model = modelBuilder.Model;
        var entityType = model.FindEntityType(typeof(ApplicationUser));

        // Assert
        entityType.Should().NotBeNull();
        
        // Check PT relationship
        var assignedPTNavigation = entityType!.FindNavigation(nameof(ApplicationUser.AssignedPT));
        assignedPTNavigation.Should().NotBeNull();
        assignedPTNavigation!.IsRequired().Should().BeFalse();

        // Check Clients relationship
        var clientsNavigation = entityType.FindNavigation(nameof(ApplicationUser.Clients));
        clientsNavigation.Should().NotBeNull();
        clientsNavigation!.IsCollection.Should().BeTrue();
    }
}
