using FluentAssertions;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Moq;
using Testcontainers.MongoDb;
using dotFitness.Modules.Users.Domain.Entities;
using dotFitness.Modules.Users.Infrastructure.Repositories;

namespace dotFitness.Modules.Users.Tests.Infrastructure.MongoDB;

public class UserMetricsRepositoryTests : IAsyncLifetime
{
    private readonly MongoDbContainer _mongoContainer;
    private IMongoDatabase _database = null!;
    private UserMetricsRepository _repository = null!;
    private Mock<ILogger<UserMetricsRepository>> _loggerMock = null!;

    public UserMetricsRepositoryTests()
    {
        _mongoContainer = new MongoDbBuilder()
            .WithImage("mongo:8.0")
            .WithPortBinding(0, true)
            .Build();
    }
    
    public async Task InitializeAsync()
    {
        await _mongoContainer.StartAsync();
        
        var connectionString = _mongoContainer.GetConnectionString();
        var client = new MongoClient(connectionString);
        _database = client.GetDatabase("testDb");
        
        _database.GetCollection<UserMetric>("userMetrics");
        _loggerMock = new Mock<ILogger<UserMetricsRepository>>();
        _repository = new UserMetricsRepository(_database, _loggerMock.Object);
    }

    public async Task DisposeAsync()
    {
        await _mongoContainer.StopAsync();
        await _mongoContainer.DisposeAsync();
    }

    [Fact]
    public async Task Should_Create_UserMetric_Successfully()
    {
        // Arrange
        var userMetric = new UserMetric
        {
            UserId = "user123",
            Date = new DateTime(2024, 1, 1),
            Weight = 70.5,
            Height = 175.0,
            Notes = "Test measurement"
        };

        // Act
        var result = await _repository.CreateAsync(userMetric);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Id.Should().NotBeNullOrEmpty();
        result.Value.UserId.Should().Be("user123");
        result.Value.Weight.Should().Be(70.5);
        result.Value.Height.Should().Be(175.0);
        result.Value.Notes.Should().Be("Test measurement");
    }

    [Fact]
    public async Task Should_Retrieve_UserMetric_By_Id()
    {
        // Arrange
        var userMetric = new UserMetric
        {
            UserId = "user123",
            Date = new DateTime(2024, 1, 1),
            Weight = 70.5
        };
        var createResult = await _repository.CreateAsync(userMetric);
        var metricId = createResult.Value.Id;

        // Act
        var result = await _repository.GetByIdAsync(metricId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Id.Should().Be(metricId);
        result.Value.UserId.Should().Be("user123");
        result.Value.Weight.Should().Be(70.5);
    }

    [Fact]
    public async Task Should_Return_NotFound_For_NonExistent_UserMetric()
    {
        // Arrange
        var nonExistentId = "507f1f77bcf86cd799439011";

        // Act
        var result = await _repository.GetByIdAsync(nonExistentId);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("User metric not found");
    }

    [Fact]
    public async Task Should_Update_UserMetric_Successfully()
    {
        // Arrange
        var userMetric = new UserMetric
        {
            UserId = "user123",
            Date = new DateTime(2024, 1, 1),
            Weight = 70.0
        };
        var createResult = await _repository.CreateAsync(userMetric);
        var createdMetric = createResult.Value;
        
        createdMetric.UpdateMetrics(weight: 72.5, notes: "Updated measurement");

        // Act
        var result = await _repository.UpdateAsync(createdMetric);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Weight.Should().Be(72.5);
        result.Value.Notes.Should().Be("Updated measurement");
        
        // Verify persistence
        var retrievedResult = await _repository.GetByIdAsync(createdMetric.Id);
        retrievedResult.Value.Weight.Should().Be(72.5);
        retrievedResult.Value.Notes.Should().Be("Updated measurement");
    }

    [Fact]
    public async Task Should_Delete_UserMetric_Successfully()
    {
        // Arrange
        var userMetric = new UserMetric
        {
            UserId = "user123",
            Date = new DateTime(2024, 1, 1),
            Weight = 70.0
        };
        var createResult = await _repository.CreateAsync(userMetric);
        var metricId = createResult.Value.Id;

        // Act
        var result = await _repository.DeleteAsync(metricId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        
        // Verify deletion
        var retrievedResult = await _repository.GetByIdAsync(metricId);
        retrievedResult.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task Should_Return_False_When_Deleting_NonExistent_UserMetric()
    {
        // Arrange
        var nonExistentId = "507f1f77bcf86cd799439011";

        // Act
        var result = await _repository.DeleteAsync(nonExistentId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Error.Should().BeEmpty();
    }

    [Fact]
    public async Task Should_Get_UserMetrics_By_UserId()
    {
        // Arrange
        var userId = "user123";
        var metric1 = new UserMetric
        {
            UserId = userId,
            Date = new DateTime(2024, 1, 1),
            Weight = 70.0
        };
        var metric2 = new UserMetric
        {
            UserId = userId,
            Date = new DateTime(2024, 1, 2),
            Weight = 70.5
        };
        var metric3 = new UserMetric
        {
            UserId = "differentUser",
            Date = new DateTime(2024, 1, 1),
            Weight = 80.0
        };

        await _repository.CreateAsync(metric1);
        await _repository.CreateAsync(metric2);
        await _repository.CreateAsync(metric3);

        // Act
        var result = await _repository.GetByUserIdAsync(userId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value.Should().OnlyContain(m => m.UserId == userId);
    }

    [Fact]
    public async Task Should_Get_Latest_UserMetric_By_UserId()
    {
        // Arrange
        var userId = "user123";
        var olderMetric = new UserMetric
        {
            UserId = userId,
            Date = new DateTime(2024, 1, 1),
            Weight = 70.0
        };
        var newerMetric = new UserMetric
        {
            UserId = userId,
            Date = new DateTime(2024, 1, 5),
            Weight = 72.0
        };

        await _repository.CreateAsync(olderMetric);
        await _repository.CreateAsync(newerMetric);

        // Act
        var result = await _repository.GetLatestByUserIdAsync(userId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Weight.Should().Be(72.0);
        result.Value.Date.Should().Be(new DateTime(2024, 1, 5));
    }

    [Fact]
    public async Task Should_Return_NotFound_For_Latest_When_No_Metrics_Exist()
    {
        // Act
        var result = await _repository.GetLatestByUserIdAsync("nonExistentUser");

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("No user metrics found");
    }

    [Fact]
    public async Task Should_Get_UserMetrics_Within_Date_Range()
    {
        // Arrange
        var userId = "user123";
        var metric1 = new UserMetric
        {
            UserId = userId,
            Date = new DateTime(2024, 1, 1),
            Weight = 70.0
        };
        var metric2 = new UserMetric
        {
            UserId = userId,
            Date = new DateTime(2024, 1, 15),
            Weight = 71.0
        };
        var metric3 = new UserMetric
        {
            UserId = userId,
            Date = new DateTime(2024, 2, 1),
            Weight = 72.0
        };

        await _repository.CreateAsync(metric1);
        await _repository.CreateAsync(metric2);
        await _repository.CreateAsync(metric3);

        // Act
        var result = await _repository.GetByUserIdAndDateRangeAsync(
            userId, 
            new DateTime(2024, 1, 1), 
            new DateTime(2024, 1, 31)
        );

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value.Should().OnlyContain(m => m.Date >= new DateTime(2024, 1, 1) && m.Date <= new DateTime(2024, 1, 31));
    }

    [Fact]
    public async Task Should_Return_Empty_List_For_Date_Range_With_No_Metrics()
    {
        // Act
        var result = await _repository.GetByUserIdAndDateRangeAsync(
            "user123", 
            new DateTime(2024, 6, 1), 
            new DateTime(2024, 6, 30)
        );

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task Should_Handle_Database_Connection_Errors_Gracefully()
    {
        // Arrange
        var invalidRepository = new UserMetricsRepository(_database, _loggerMock.Object);
        
        // Force a connection issue by stopping the container
        await _mongoContainer.StopAsync();

        // Act
        var result = await invalidRepository.GetByIdAsync("507f1f77bcf86cd799439011");

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Failed to retrieve user metric"); // Based on the error handling in the repository
    }
}
