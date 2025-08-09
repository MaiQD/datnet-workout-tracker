using Microsoft.Extensions.Logging;
using dotFitness.Bootstrap;

namespace dotFitness.Api.Infrastructure;

/// <summary>
/// Handles MongoDB seed data for the application
/// </summary>
public static class MongoDbSeeder
{
    /// <summary>
    /// Runs seeders for all modules
    /// </summary>
    public static async Task ConfigureSeedsAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("MongoDbSeeder");

        // Seed module data using interface-based approach
        scope.ServiceProvider.SeedAllModuleData(logger);
        logger.LogInformation("MongoDB seed data configured successfully");
    }
}


