using FluentAssertions;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Moq;
using dotFitness.Modules.Users.Domain.Entities;
using dotFitness.Modules.Users.Infrastructure.Repositories;

namespace dotFitness.Modules.Users.Tests.Infrastructure.MongoDB;

[Collection("MongoDB")]
public class UserRepositoryTests(MongoDbFixture fixture) : IAsyncLifetime
{
    private IMongoDatabase _database = null!;
    private UserRepository _repository = null!;
    private Mock<ILogger<UserRepository>> _loggerMock = null!;

    public async Task InitializeAsync()
    {
        // Create a fresh database for this test class to ensure isolation
        _database = fixture.CreateFreshDatabase();
        
        // Ensure the users collection exists
        _database.GetCollection<User>("users");
        
        _loggerMock = new Mock<ILogger<UserRepository>>();
        _repository = new UserRepository(_database, _loggerMock.Object);
        
        await Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        // Clean up the database after tests complete
        await fixture.CleanupDatabaseAsync();
    }

    [Fact]
    public async Task Should_Create_User_Successfully()
    {
        // Arrange
        var user = new User
        {
            Email = "test@example.com",
            DisplayName = "Test User",
            GoogleId = "google123"
        };

        // Act
        var result = await _repository.CreateAsync(user);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Id.Should().NotBeNullOrEmpty();
        result.Value.Email.Should().Be("test@example.com");
        result.Value.DisplayName.Should().Be("Test User");
        result.Value.GoogleId.Should().Be("google123");
    }

    [Fact]
    public async Task Should_Retrieve_User_By_Id()
    {
        // Arrange
        var user = new User
        {
            Email = "test@example.com",
            DisplayName = "Test User"
        };
        var createResult = await _repository.CreateAsync(user);
        var userId = createResult.Value!.Id;

        // Act
        var result = await _repository.GetByIdAsync(userId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Id.Should().Be(userId);
        result.Value.Email.Should().Be("test@example.com");
        result.Value.DisplayName.Should().Be("Test User");
    }

    [Fact]
    public async Task Should_Return_NotFound_For_NonExistent_User()
    {
        // Arrange
        var nonExistentId = "507f1f77bcf86cd799439011";

        // Act
        var result = await _repository.GetByIdAsync(nonExistentId);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("User not found");
    }

    [Fact]
    public async Task Should_Update_User_Successfully()
    {
        // Arrange
        var user = new User
        {
            Email = "test@example.com",
            DisplayName = "Original Name"
        };
        var createResult = await _repository.CreateAsync(user);
        var createdUser = createResult.Value;
        
        createdUser!.UpdateProfile(displayName: "Updated Name");

        // Act
        var result = await _repository.UpdateAsync(createdUser);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.DisplayName.Should().Be("Updated Name");
        
        // Verify persistence
        var retrievedResult = await _repository.GetByIdAsync(createdUser.Id);
        retrievedResult.Value!.DisplayName.Should().Be("Updated Name");
    }

    [Fact]
    public async Task Should_Delete_User_Successfully()
    {
        // Arrange
        var user = new User
        {
            Email = "test@example.com",
            DisplayName = "Test User"
        };
        var createResult = await _repository.CreateAsync(user);
        var userId = createResult.Value!.Id;

        // Act
        var result = await _repository.DeleteAsync(userId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        
        // Verify deletion
        var retrievedResult = await _repository.GetByIdAsync(userId);
        retrievedResult.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task Should_Return_False_When_Deleting_NonExistent_User()
    {
        // Arrange
        var nonExistentId = "507f1f77bcf86cd799439011";

        // Act
        var result = await _repository.DeleteAsync(nonExistentId);

        // Assert
        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task Should_Get_User_By_Email()
    {
        // Arrange
        var user = new User
        {
            Email = "unique@example.com",
            DisplayName = "Test User"
        };
        await _repository.CreateAsync(user);

        // Act
        var result = await _repository.GetByEmailAsync("unique@example.com");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Email.Should().Be("unique@example.com");
    }

    [Fact]
    public async Task Should_Return_NotFound_For_Unknown_Email()
    {
        // Act
        var result = await _repository.GetByEmailAsync("unknown@example.com");

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("User not found");
    }

    [Fact]
    public async Task Should_Get_User_By_GoogleId()
    {
        // Arrange
        var user = new User
        {
            Email = "test@example.com",
            DisplayName = "Test User",
            GoogleId = "uniqueGoogleId123"
        };
        await _repository.CreateAsync(user);

        // Act
        var result = await _repository.GetByGoogleIdAsync("uniqueGoogleId123");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.GoogleId.Should().Be("uniqueGoogleId123");
    }

    [Fact]
    public async Task Should_Return_NotFound_For_Unknown_GoogleId()
    {
        // Act
        var result = await _repository.GetByGoogleIdAsync("unknownGoogleId");

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("User not found");
    }

    
}
