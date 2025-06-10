using MongoDB.Driver;
using Testcontainers.MongoDb;

namespace dotFitness.Modules.Users.Tests.Infrastructure.MongoDB;

/// <summary>
/// Shared MongoDB test fixture that provides a single container instance
/// for all repository tests to improve performance and reduce resource usage.
/// </summary>
public class MongoDbFixture : IAsyncLifetime
{
    private readonly MongoDbContainer _mongoContainer;
    
    public IMongoDatabase Database { get; private set; } = null!;
    public string ConnectionString { get; private set; } = null!;

    public MongoDbFixture()
    {
        _mongoContainer = new MongoDbBuilder()
            .WithImage("mongo:8.0")
            .WithPortBinding(0, true)
            .WithName($"test-mongo-{Guid.NewGuid():N}")
            .Build();
    }

    public async Task InitializeAsync()
    {
        await _mongoContainer.StartAsync();
        
        ConnectionString = _mongoContainer.GetConnectionString();
        var client = new MongoClient(ConnectionString);
        Database = client.GetDatabase($"testDb_{Guid.NewGuid():N}");
    }

    public async Task DisposeAsync()
    {
        await _mongoContainer.StopAsync();
        await _mongoContainer.DisposeAsync();
    }

    /// <summary>
    /// Creates a fresh database for each test class to ensure test isolation
    /// </summary>
    public IMongoDatabase CreateFreshDatabase()
    {
        var client = new MongoClient(ConnectionString);
        return client.GetDatabase($"testDb_{Guid.NewGuid():N}");
    }

    /// <summary>
    /// Cleans up all data in the current database
    /// </summary>
    public async Task CleanupDatabaseAsync()
    {
        var collections = await Database.ListCollectionNamesAsync();
        await collections.ForEachAsync(async collectionName =>
        {
            await Database.DropCollectionAsync(collectionName);
        });
    }
}

/// <summary>
/// Collection definition for xUnit to share the fixture across test classes
/// </summary>
[CollectionDefinition("MongoDB")]
public class MongoDbCollectionFixture : ICollectionFixture<MongoDbFixture>
{
}
