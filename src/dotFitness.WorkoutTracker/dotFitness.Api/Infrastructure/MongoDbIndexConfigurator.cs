using MongoDB.Driver;
using dotFitness.SharedKernel.Outbox;
using Microsoft.Extensions.Logging;

namespace dotFitness.Api.Infrastructure;

/// <summary>
/// Handles MongoDB index configuration for the application
/// </summary>
public static class MongoDbIndexConfigurator
{
    /// <summary>
    /// Configures all MongoDB indexes for the application
    /// </summary>
    public static async Task ConfigureIndexesAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var database = scope.ServiceProvider.GetRequiredService<IMongoDatabase>();
        var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("MongoDbIndexConfigurator");
        
        // Configure shared indexes
        await ConfigureSharedIndexesAsync(database);
        
        // Configure module-specific indexes
        await ModuleRegistry.ConfigureAllModuleIndexes(services);
        
        logger.LogInformation("MongoDB indexes configured successfully");
    }

    /// <summary>
    /// Configures indexes for shared/global collections
    /// </summary>
    private static async Task ConfigureSharedIndexesAsync(IMongoDatabase database)
    {
        // Create indexes for shared/global collections (OutboxMessage)
        var outboxCollection = database.GetCollection<OutboxMessage>("outboxMessages");
        var outboxIndexBuilder = Builders<OutboxMessage>.IndexKeys;
        
        await outboxCollection.Indexes.CreateManyAsync(new[]
        {
            new CreateIndexModel<OutboxMessage>(outboxIndexBuilder.Ascending(x => x.IsProcessed)),
            new CreateIndexModel<OutboxMessage>(outboxIndexBuilder.Ascending(x => x.CreatedAt)),
            new CreateIndexModel<OutboxMessage>(outboxIndexBuilder.Ascending(x => x.EventType))
        });
    }
} 