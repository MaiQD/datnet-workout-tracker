using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using FluentAssertions;
using dotFitness.Modules.Users.Infrastructure.Data;
using dotFitness.Modules.Users.Infrastructure.Services;
using Microsoft.Extensions.Configuration;

namespace dotFitness.Modules.Users.Tests.Infrastructure.Configuration;

public class DatabaseMigrationServiceTests
{
    [Fact]
    public async Task StartAsync_Should_Apply_Migrations_Successfully()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddDbContext<UsersDbContext>(options =>
            options.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()));

        var serviceProvider = services.BuildServiceProvider();
        var context = serviceProvider.GetRequiredService<UsersDbContext>();
        await context.Database.EnsureCreatedAsync();

        var loggerMock = new Mock<ILogger<DatabaseMigrationService>>();
        var migrationService =
            new DatabaseMigrationService(serviceProvider, loggerMock.Object, Mock.Of<IConfiguration>());

        // Act
        await migrationService.StartAsync(CancellationToken.None);

        // Assert
        // Verify that the database is accessible
        var canConnect = await context.Database.CanConnectAsync();
        canConnect.Should().BeTrue();
    }

    [Fact]
    public async Task StartAsync_Should_Handle_Database_Errors_Gracefully()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddDbContext<UsersDbContext>(options =>
            options.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()));

        var serviceProvider = services.BuildServiceProvider();
        var context = serviceProvider.GetRequiredService<UsersDbContext>();
        await context.DisposeAsync(); // Dispose to simulate database error

        var loggerMock = new Mock<ILogger<DatabaseMigrationService>>();
        var migrationService =
            new DatabaseMigrationService(serviceProvider, loggerMock.Object, Mock.Of<IConfiguration>());

        // Act & Assert
        await migrationService.StartAsync(CancellationToken.None);

        // Should not throw exception
        loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Failed to apply database migrations")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task StopAsync_Should_Complete_Successfully()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddDbContext<UsersDbContext>(options =>
            options.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()));

        var serviceProvider = services.BuildServiceProvider();
        var loggerMock = new Mock<ILogger<DatabaseMigrationService>>();
        var migrationService =
            new DatabaseMigrationService(serviceProvider, loggerMock.Object, Mock.Of<IConfiguration>());

        // Act
        await migrationService.StopAsync(CancellationToken.None);

        // Assert
        // Should complete without throwing
    }
}