using Microsoft.Extensions.Logging;

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

        await ModuleRegistry.SeedAllModuleData(services);
        logger.LogInformation("MongoDB seed data configured successfully");
    }
}


