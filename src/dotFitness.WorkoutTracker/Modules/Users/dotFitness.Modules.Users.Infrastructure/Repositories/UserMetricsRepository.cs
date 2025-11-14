using dotFitness.Common.Results;
using dotFitness.Modules.Users.Domain.Entities;
using dotFitness.Modules.Users.Domain.Repositories;
using dotFitness.Modules.Users.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace dotFitness.Modules.Users.Infrastructure.Repositories;

public class UserMetricsRepository(UsersDbContext context, ILogger<UserMetricsRepository> logger)
    : IUserMetricsRepository
{
    public async Task<Result<UserMetric>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var metric = await context.UserMetrics.FindAsync([id], cancellationToken);
        if (metric == null)
        {
            return Result.Failure<UserMetric>("User metric not found");
        }
        return Result.Success(metric);
    }

    public async Task<Result<UserMetric>> CreateAsync(UserMetric userMetric, CancellationToken cancellationToken = default)
    {
        context.UserMetrics.Add(userMetric);
        await context.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Successfully created user metric with ID: {MetricId}", userMetric.Id);
        return Result.Success(userMetric);
    }

    public async Task<Result<UserMetric>> UpdateAsync(UserMetric userMetric, CancellationToken cancellationToken = default)
    {
        context.UserMetrics.Update(userMetric);
        await context.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Successfully updated user metric with ID: {MetricId}", userMetric.Id);
        return Result.Success(userMetric);
    }

    public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var metric = await context.UserMetrics.FindAsync([id], cancellationToken);
        if (metric == null)
        {
            return Result.Failure("User metric not found");
        }

        context.UserMetrics.Remove(metric);
        await context.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Successfully deleted user metric with ID: {MetricId}", id);
        return Result.Success();
    }

    public async Task<Result<IEnumerable<UserMetric>>> GetByUserIdAsync(Guid userId, int skip = 0, int take = 50, CancellationToken cancellationToken = default)
    {
        var metrics = await context.UserMetrics
            .Where(um => um.UserId == userId)
            .OrderByDescending(um => um.Date)
            .ThenByDescending(um => um.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
        return Result.Success(metrics.AsEnumerable());
    }

    public async Task<Result<UserMetric>> GetLatestByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var metric = await context.UserMetrics
            .Where(um => um.UserId == userId)
            .OrderByDescending(um => um.Date)
            .ThenByDescending(um => um.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);
        
        if (metric == null)
        {
            return Result.Failure<UserMetric>("No metrics found for user");
        }
        return Result.Success(metric);
    }

    public async Task<Result<IEnumerable<UserMetric>>> GetByUserIdAndDateRangeAsync(Guid userId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        var metrics = await context.UserMetrics
            .Where(um => um.UserId == userId && um.Date >= startDate && um.Date <= endDate)
            .OrderByDescending(um => um.Date)
            .ThenByDescending(um => um.CreatedAt)
            .ToListAsync(cancellationToken);
        return Result.Success(metrics.AsEnumerable());
    }

    public async Task<Result<long>> GetCountByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var count = await context.UserMetrics
            .LongCountAsync(um => um.UserId == userId, cancellationToken);
        return Result.Success(count);
    }

    public async Task<Result<bool>> ExistsForUserAndDateAsync(Guid userId, DateTime date, CancellationToken cancellationToken = default)
    {
        var exists = await context.UserMetrics
            .AnyAsync(um => um.UserId == userId && um.Date.Date == date.Date, cancellationToken);
        return Result.Success(exists);
    }

    public async Task<Result<UserMetric>> GetByUserIdAndDateAsync(Guid userId, DateTime date, CancellationToken cancellationToken = default)
    {
        var metric = await context.UserMetrics
            .FirstOrDefaultAsync(um => um.UserId == userId && um.Date.Date == date.Date, cancellationToken);
        
        if (metric == null)
        {
            return Result.Failure<UserMetric>("User metric not found for the specified date");
        }
        return Result.Success(metric);
    }
}
