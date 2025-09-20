using FluentAssertions;
using MongoDB.Driver;
using MongoDB.Bson;
using dotFitness.Modules.Exercises.Domain.Entities;
using dotFitness.Modules.Exercises.Infrastructure.Repositories;
using dotFitness.SharedKernel.Tests.MongoDB;

namespace dotFitness.Modules.Exercises.Tests.Infrastructure.Repositories;

[Collection("MongoDB")]
public class UserPreferencesProjectionRepositoryTests(MongoDbFixture fixture) : IAsyncLifetime
{
    private IMongoDatabase _database = null!;
    private IMongoCollection<UserPreferencesProjection> _collection = null!;
    private UserPreferencesProjectionRepository _repository = null!;

    public async Task InitializeAsync()
    {
        _database = fixture.CreateFreshDatabase();
        _collection = _database.GetCollection<UserPreferencesProjection>("userPreferencesProjections");
        _repository = new UserPreferencesProjectionRepository(_database);
        await Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        await fixture.CleanupDatabaseAsync();
    }

    [Fact]
    public async Task Should_Return_Null_When_Not_Found()
    {
        var userId = 1;
        var result = await _repository.GetByUserIdAsync(userId);
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeNull();
    }

    [Fact]
    public async Task Should_Return_Preferences_When_Found()
    {
        var userId = 1;
        var pref = new UserPreferencesProjection
        {
            UserId = userId,
            FocusMuscleGroupIds = [ObjectId.GenerateNewId().ToString(), ObjectId.GenerateNewId().ToString()],
            AvailableEquipmentIds = [ObjectId.GenerateNewId().ToString(), ObjectId.GenerateNewId().ToString()],
            UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        await _collection.InsertOneAsync(pref);

        var result = await _repository.GetByUserIdAsync(userId);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.UserId.Should().Be(userId);
        result.Value!.FocusMuscleGroupIds.Should().HaveCount(2);
        result.Value!.AvailableEquipmentIds.Should().HaveCount(2);
    }
}
