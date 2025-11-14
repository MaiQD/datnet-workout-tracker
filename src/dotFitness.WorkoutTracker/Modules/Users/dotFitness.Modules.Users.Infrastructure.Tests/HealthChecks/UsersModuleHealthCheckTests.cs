using dotFitness.Modules.Users.Domain.Entities;
using dotFitness.Modules.Users.Infrastructure.Data;
using dotFitness.Modules.Users.Infrastructure.Data.Entities;
using dotFitness.Modules.Users.Infrastructure.HealthChecks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace dotFitness.Modules.Users.Infrastructure.Tests.HealthChecks;

public class UsersModuleHealthCheckTests
{
    [Fact]
    public async Task CheckHealthAsync_Should_Return_Healthy_When_Database_Is_Accessible()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<UsersDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new UsersDbContext(options);
        await context.Database.EnsureCreatedAsync();

        var loggerMock = new Mock<ILogger<UsersModuleHealthCheck>>();
        var healthCheck = new UsersModuleHealthCheck(context, loggerMock.Object);

        // Act
        var result = await healthCheck.CheckHealthAsync(new HealthCheckContext());

        // Assert
        result.Status.Should().Be(HealthStatus.Healthy);
        result.Data.Should().ContainKey("module");
        result.Data.Should().ContainKey("database");
        result.Data.Should().ContainKey("postgresConnection");
        result.Data.Should().ContainKey("userCount");
        result.Data.Should().ContainKey("outboxMessageCount");
    }

    [Fact]
    public async Task CheckHealthAsync_Should_Return_Unhealthy_When_Database_Is_Not_Accessible()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<UsersDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new UsersDbContext(options);
        await context.DisposeAsync(); // Dispose to simulate database error

        var loggerMock = new Mock<ILogger<UsersModuleHealthCheck>>();
        var healthCheck = new UsersModuleHealthCheck(context, loggerMock.Object);

        // Act
        var result = await healthCheck.CheckHealthAsync(new HealthCheckContext());

        // Assert
        result.Status.Should().Be(HealthStatus.Unhealthy);
        result.Data.Should().ContainKey("module");
        result.Data.Should().ContainKey("database");
        result.Data.Should().ContainKey("error");
    }

    [Fact]
    public async Task CheckHealthAsync_Should_Include_User_Count_In_Data()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<UsersDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new UsersDbContext(options);
        await context.Database.EnsureCreatedAsync();

        // Add some test users
        var users = new[]
        {
            new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Email = "user1@example.com",
                UserName = "user1@example.com",
                DisplayName = "User 1"
            },
            new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Email = "user2@example.com",
                UserName = "user2@example.com",
                DisplayName = "User 2"
            }
        };

        context.Users.AddRange(users);
        await context.SaveChangesAsync();

        var loggerMock = new Mock<ILogger<UsersModuleHealthCheck>>();
        var healthCheck = new UsersModuleHealthCheck(context, loggerMock.Object);

        // Act
        var result = await healthCheck.CheckHealthAsync(new HealthCheckContext());

        // Assert
        result.Status.Should().Be(HealthStatus.Healthy);
        result.Data.Should().ContainKey("userCount");
        result.Data["userCount"].Should().Be(2);
    }

    [Fact]
    public async Task CheckHealthAsync_Should_Include_Outbox_Message_Count_In_Data()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<UsersDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new UsersDbContext(options);
        await context.Database.EnsureCreatedAsync();

        // Add some test outbox messages
        var outboxMessages = new[]
        {
            new OutboxMessageEntity
            {
                Id = Guid.NewGuid(),
                OccurredOn = DateTime.UtcNow,
                Type = "TestEvent1",
                Data = "{}"
            },
            new OutboxMessageEntity
            {
                Id = Guid.NewGuid(),
                OccurredOn = DateTime.UtcNow,
                Type = "TestEvent2",
                Data = "{}"
            }
        };

        context.OutboxMessages.AddRange(outboxMessages);
        await context.SaveChangesAsync();

        var loggerMock = new Mock<ILogger<UsersModuleHealthCheck>>();
        var healthCheck = new UsersModuleHealthCheck(context, loggerMock.Object);

        // Act
        var result = await healthCheck.CheckHealthAsync(new HealthCheckContext());

        // Assert
        result.Status.Should().Be(HealthStatus.Healthy);
        result.Data.Should().ContainKey("outboxMessageCount");
        result.Data["outboxMessageCount"].Should().Be(2);
    }
}
