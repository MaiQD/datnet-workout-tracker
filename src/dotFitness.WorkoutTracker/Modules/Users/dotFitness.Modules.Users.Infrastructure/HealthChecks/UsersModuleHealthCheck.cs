using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using dotFitness.Modules.Users.Infrastructure.Data;
using Microsoft.Extensions.Logging;

namespace dotFitness.Modules.Users.Infrastructure.HealthChecks;

/// <summary>
/// Health check for the Users module - validates PostgreSQL connection
/// </summary>
public class UsersModuleHealthCheck(
    UsersDbContext postgresContext,
    ILogger<UsersModuleHealthCheck> logger)
    : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var data = new Dictionary<string, object>
        {
            ["module"] = "Users",
            ["database"] = "PostgreSQL"
        };

        try
        {
            // Test PostgreSQL connection
            var canConnectPostgres = await postgresContext.Database.CanConnectAsync(cancellationToken);
            if (!canConnectPostgres)
            {
                data["postgresError"] = "Cannot connect to PostgreSQL";
                return HealthCheckResult.Unhealthy("Users module: PostgreSQL connection failed", data: data);
            }

            // Test PostgreSQL user count
            var userCount = await postgresContext.Users.CountAsync(cancellationToken);

            // Test PostgreSQL outbox
            var outboxCount = await postgresContext.OutboxMessages.CountAsync(cancellationToken);

            data["postgresConnection"] = "OK";
            data["userCount"] = userCount;
            data["outboxMessageCount"] = outboxCount;

            logger.LogDebug("Users module health check passed. Users: {UserCount}, Outbox: {OutboxCount}", 
                userCount, outboxCount);

            return HealthCheckResult.Healthy($"Users module healthy. Users: {userCount}, Outbox: {outboxCount}", data);
        }
        catch (Exception ex)
        {
            data["error"] = ex.Message;
            logger.LogError(ex, "Users module health check failed");
            return HealthCheckResult.Unhealthy($"Users module unhealthy: {ex.Message}", ex, data);
        }
    }
}
