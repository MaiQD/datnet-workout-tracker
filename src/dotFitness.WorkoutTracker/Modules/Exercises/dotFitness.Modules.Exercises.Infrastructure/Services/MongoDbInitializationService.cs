using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using dotFitness.Modules.Exercises.Infrastructure.Configuration;

namespace dotFitness.Modules.Exercises.Infrastructure.Services;

/// <summary>
/// Background service that automatically configures MongoDB indexes and seeds data for the Exercises module
/// </summary>
public class MongoDbInitializationService(
    IServiceProvider serviceProvider,
    ILogger<MongoDbInitializationService> logger,
    IConfiguration configuration)
    : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting Exercises module MongoDB initialization service...");

        try
        {
            using var scope = serviceProvider.CreateScope();
            var database = scope.ServiceProvider.GetRequiredService<IMongoDatabase>();

            // Wait for database to be ready (useful when using Docker/Aspire)
            var retryCount = 0;
            const int maxRetries = 30; // 30 seconds with 1-second intervals
            
            while (retryCount < maxRetries)
            {
                try
                {
                    await database.RunCommandAsync<BsonDocument>(
                        new BsonDocument("ping", 1), 
                        cancellationToken: cancellationToken);
                    
                    logger.LogInformation("Exercises module MongoDB connection established");
                    break;
                }
                catch (Exception ex)
                {
                    logger.LogDebug("MongoDB connection attempt {Attempt}/{MaxAttempts} failed: {Error}", 
                        retryCount + 1, maxRetries, ex.Message);
                }

                retryCount++;
                if (retryCount >= maxRetries)
                {
                    throw new TimeoutException($"Failed to connect to MongoDB after {maxRetries} attempts");
                }

                await Task.Delay(1000, cancellationToken); // Wait 1 second before retry
            }

            // Configure indexes
            logger.LogInformation("Configuring MongoDB indexes for Exercises module...");
            ExercisesMongoIndexConfigurator.Configure(database);
            logger.LogInformation("MongoDB indexes configured successfully for Exercises module");

            // Seed data
            logger.LogInformation("Seeding data for Exercises module...");
            ExercisesMongoSeeder.Seed(database);
            logger.LogInformation("Data seeded successfully for Exercises module");

            // Final verification
            try
            {
                await database.RunCommandAsync<BsonDocument>(
                    new BsonDocument("ping", 1), 
                    cancellationToken: cancellationToken);
                
                logger.LogInformation("Exercises module MongoDB initialization service completed successfully");
            }
            catch (Exception ex)
            {
                logger.LogWarning("Exercises module MongoDB connection verification failed after initialization: {Error}", ex.Message);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to initialize MongoDB for Exercises module: {ErrorMessage}", ex.Message);
            throw;
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Stopping Exercises module MongoDB initialization service...");
        return Task.CompletedTask;
    }
}

