using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using dotFitness.Modules.Users.Infrastructure.Data;
using dotFitness.Modules.Users.Domain.Entities;

namespace dotFitness.Modules.Users.Tests.Infrastructure.Data;

public class UsersDbContextTests
{
    [Fact]
    public void Should_Create_UsersDbContext()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<UsersDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        // Act
        using var context = new UsersDbContext(options);

        // Assert
        context.Should().NotBeNull();
        context.UserMetrics.Should().NotBeNull();
        context.OutboxMessages.Should().NotBeNull();
    }

    [Fact]
    public void Should_Have_Correct_DbSets()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<UsersDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        // Act
        using var context = new UsersDbContext(options);

        // Assert
        context.UserMetrics.Should().NotBeNull();
        context.OutboxMessages.Should().NotBeNull();
    }

    [Fact]
    public async Task SaveChangesAsync_Should_Update_UpdatedAt_For_ApplicationUser()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<UsersDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new UsersDbContext(options);
        await context.Database.EnsureCreatedAsync();

        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            UserName = "test@example.com",
            DisplayName = "Test User",
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            UpdatedAt = DateTime.UtcNow.AddDays(-1)
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        var originalUpdatedAt = user.UpdatedAt;

        // Act
        user.DisplayName = "Updated User";
        await context.SaveChangesAsync();

        // Assert
        user.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    [Fact]
    public async Task SaveChangesAsync_Should_Not_Update_UpdatedAt_For_Other_Entities()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<UsersDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new UsersDbContext(options);
        await context.Database.EnsureCreatedAsync();

        var userMetric = new UserMetric
        {
            UserId = Guid.NewGuid(),
            Date = DateTime.UtcNow,
            Weight = 70.0,
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            UpdatedAt = DateTime.UtcNow.AddDays(-1)
        };

        context.UserMetrics.Add(userMetric);
        await context.SaveChangesAsync();

        var originalUpdatedAt = userMetric.UpdatedAt;

        // Act
        userMetric.Weight = 71.0;
        await context.SaveChangesAsync();

        // Assert
        userMetric.UpdatedAt.Should().Be(originalUpdatedAt);
    }
}
