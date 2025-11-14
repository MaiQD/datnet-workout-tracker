using dotFitness.Modules.Users.Infrastructure.Data.Configurations;
using dotFitness.Modules.Users.Infrastructure.Data.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace dotFitness.Modules.Users.Infrastructure.Tests.Data.Configurations;

public class OutboxMessageConfigurationTests
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
        var configuration = new OutboxMessageConfiguration();

        // Act
        configuration.Configure(modelBuilder.Entity<OutboxMessageEntity>());
        var model = modelBuilder.Model;
        var entityType = model.FindEntityType(typeof(OutboxMessageEntity));

        // Assert
        entityType.Should().NotBeNull();
        entityType!.GetTableName().Should().Be("OutboxMessages");
        entityType.GetSchema().Should().Be("users");
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
        var configuration = new OutboxMessageConfiguration();

        // Act
        configuration.Configure(modelBuilder.Entity<OutboxMessageEntity>());
        var model = modelBuilder.Model;
        var entityType = model.FindEntityType(typeof(OutboxMessageEntity));

        // Assert
        entityType.Should().NotBeNull();
        
        // Check Id property
        var idProperty = entityType!.FindProperty(nameof(OutboxMessageEntity.Id));
        idProperty.Should().NotBeNull();
        idProperty!.GetColumnType().Should().Be("uuid");
        idProperty.IsRequired().Should().BeTrue();

        // Check OccurredOn property
        var occurredOnProperty = entityType.FindProperty(nameof(OutboxMessageEntity.OccurredOn));
        occurredOnProperty.Should().NotBeNull();
        occurredOnProperty!.GetColumnType().Should().Be("timestamp with time zone");
        occurredOnProperty.IsRequired().Should().BeTrue();

        // Check Type property
        var typeProperty = entityType.FindProperty(nameof(OutboxMessageEntity.Type));
        typeProperty.Should().NotBeNull();
        typeProperty!.GetMaxLength().Should().Be(255);
        typeProperty.IsRequired().Should().BeTrue();

        // Check Data property
        var dataProperty = entityType.FindProperty(nameof(OutboxMessageEntity.Data));
        dataProperty.Should().NotBeNull();
        dataProperty!.GetColumnType().Should().Be("jsonb");
        dataProperty.IsRequired().Should().BeTrue();

        // Check ProcessedDate property
        var processedDateProperty = entityType.FindProperty(nameof(OutboxMessageEntity.ProcessedDate));
        processedDateProperty.Should().NotBeNull();
        processedDateProperty!.GetColumnType().Should().Be("timestamp with time zone");
        processedDateProperty.IsRequired().Should().BeFalse();

        // Check Error property
        var errorProperty = entityType.FindProperty(nameof(OutboxMessageEntity.Error));
        errorProperty.Should().NotBeNull();
        errorProperty!.GetColumnType().Should().Be("text");
        errorProperty.IsRequired().Should().BeFalse();
    }

    [Fact]
    public void Configure_Should_Set_Correct_Indexes()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<DbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new DbContext(options);
        var modelBuilder = new ModelBuilder();
        var configuration = new OutboxMessageConfiguration();

        // Act
        configuration.Configure(modelBuilder.Entity<OutboxMessageEntity>());
        var model = modelBuilder.Model;
        var entityType = model.FindEntityType(typeof(OutboxMessageEntity));

        // Assert
        entityType.Should().NotBeNull();
        
        // Check OccurredOn index
        var occurredOnIndex = entityType!.FindIndex(entityType.FindProperty(nameof(OutboxMessageEntity.OccurredOn))!);
        occurredOnIndex.Should().NotBeNull();
        occurredOnIndex!.GetDatabaseName().Should().Be("IX_OutboxMessages_OccurredOn");

        // Check ProcessedDate index
        var processedDateIndex = entityType.FindIndex(entityType.FindProperty(nameof(OutboxMessageEntity.ProcessedDate))!);
        processedDateIndex.Should().NotBeNull();
        processedDateIndex!.GetDatabaseName().Should().Be("IX_OutboxMessages_ProcessedDate");
    }
}
