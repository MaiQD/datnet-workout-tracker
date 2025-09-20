using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using dotFitness.Modules.Users.Infrastructure.Data;

namespace dotFitness.Modules.Users.Infrastructure.Services;

/// <summary>
/// Background service that automatically applies EF Core migrations for the Users module
/// </summary>
public class DatabaseMigrationService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DatabaseMigrationService> _logger;
    private readonly IConfiguration _configuration;

    public DatabaseMigrationService(
        IServiceProvider serviceProvider,
        ILogger<DatabaseMigrationService> logger,
        IConfiguration configuration)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        // Check if auto-migration is enabled
        var autoMigrateEnabled = _configuration.GetValue<bool>("Database:AutoMigrate", defaultValue: true);
        
        if (!autoMigrateEnabled)
        {
            _logger.LogInformation("Auto-migration is disabled for Users module");
            return;
        }

        _logger.LogInformation("Starting Users module database migration service...");

        try
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<UsersDbContext>();

            // Wait for database to be ready (useful when using Docker/Aspire)
            var retryCount = 0;
            const int maxRetries = 30; // 30 seconds with 1-second intervals
            
            while (retryCount < maxRetries)
            {
                try
                {
                    var canConnect = await context.Database.CanConnectAsync(cancellationToken);
                    if (canConnect)
                    {
                        _logger.LogInformation("Users module database connection established");
                        break;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogDebug("Database connection attempt {Attempt}/{MaxAttempts} failed: {Error}", 
                        retryCount + 1, maxRetries, ex.Message);
                }

                retryCount++;
                if (retryCount >= maxRetries)
                {
                    throw new TimeoutException($"Failed to connect to database after {maxRetries} attempts");
                }

                await Task.Delay(1000, cancellationToken); // Wait 1 second before retry
            }

            // Check for pending migrations
            var pendingMigrations = await context.Database.GetPendingMigrationsAsync(cancellationToken);
            var pendingMigrationsList = pendingMigrations.ToList();

            if (pendingMigrationsList.Any())
            {
                _logger.LogInformation("Found {Count} pending migrations for Users module: {Migrations}", 
                    pendingMigrationsList.Count, 
                    string.Join(", ", pendingMigrationsList));

                // Apply pending migrations
                await context.Database.MigrateAsync(cancellationToken);
                
                _logger.LogInformation("Successfully applied {Count} migrations for Users module", 
                    pendingMigrationsList.Count);
            }
            else
            {
                _logger.LogInformation("No pending migrations found for Users module");
            }

            // Final verification
            var finalCanConnect = await context.Database.CanConnectAsync(cancellationToken);
            if (finalCanConnect)
            {
                _logger.LogInformation("Users module database migration service completed successfully");
            }
            else
            {
                _logger.LogWarning("Users module database connection verification failed after migration");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to apply migrations for Users module: {ErrorMessage}", ex.Message);
            
            // In development, we might want to fail fast, but in production, we might want to continue
            var failOnMigrationError = _configuration.GetValue<bool>("Database:FailOnMigrationError", defaultValue: true);
            if (failOnMigrationError)
            {
                throw; // Re-throw to fail fast if database migration fails
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping Users module database migration service...");
        return Task.CompletedTask;
    }
}
