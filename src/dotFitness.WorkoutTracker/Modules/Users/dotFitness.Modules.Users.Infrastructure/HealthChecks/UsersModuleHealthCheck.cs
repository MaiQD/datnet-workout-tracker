using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using dotFitness.Modules.Users.Infrastructure.Data;
using dotFitness.Modules.Users.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace dotFitness.Modules.Users.Infrastructure.HealthChecks;

/// <summary>
/// Health check for the Users module - validates both PostgreSQL and MongoDB connections
/// </summary>
public class UsersModuleHealthCheck : IHealthCheck
{
    private readonly UsersDbContext _postgresContext;
    private readonly IMongoCollection<User> _usersCollection;
    private readonly ILogger<UsersModuleHealthCheck> _logger;

    public UsersModuleHealthCheck(
        UsersDbContext postgresContext,
        IMongoCollection<User> usersCollection,
        ILogger<UsersModuleHealthCheck> logger)
    {
        _postgresContext = postgresContext;
        _usersCollection = usersCollection;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var data = new Dictionary<string, object>
        {
            ["module"] = "Users",
            ["database"] = "Hybrid (PostgreSQL + MongoDB)"
        };

        try
        {
            // Test PostgreSQL connection
            var canConnectPostgres = await _postgresContext.Database.CanConnectAsync(cancellationToken);
            if (!canConnectPostgres)
            {
                data["postgresError"] = "Cannot connect to PostgreSQL";
                return HealthCheckResult.Unhealthy("Users module: PostgreSQL connection failed", data: data);
            }

            // Test MongoDB connection
            var userCount = await _usersCollection.CountDocumentsAsync(
                FilterDefinition<User>.Empty, 
                cancellationToken: cancellationToken);

            // Test PostgreSQL outbox
            var outboxCount = await _postgresContext.OutboxMessages.CountAsync(cancellationToken);

            data["postgresConnection"] = "OK";
            data["mongoConnection"] = "OK";
            data["userCount"] = userCount;
            data["outboxMessageCount"] = outboxCount;

            _logger.LogDebug("Users module health check passed. Users: {UserCount}, Outbox: {OutboxCount}", 
                userCount, outboxCount);

            return HealthCheckResult.Healthy($"Users module healthy. Users: {userCount}, Outbox: {outboxCount}", data);
        }
        catch (Exception ex)
        {
            data["error"] = ex.Message;
            _logger.LogError(ex, "Users module health check failed");
            return HealthCheckResult.Unhealthy($"Users module unhealthy: {ex.Message}", ex, data);
        }
    }
}
