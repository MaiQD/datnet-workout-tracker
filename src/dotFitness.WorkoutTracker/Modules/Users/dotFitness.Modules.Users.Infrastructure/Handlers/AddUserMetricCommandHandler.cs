using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using dotFitness.Modules.Users.Application.Commands;
using dotFitness.Modules.Users.Application.DTOs;
using dotFitness.Modules.Users.Application.Mappers;
using dotFitness.Modules.Users.Domain.Entities;
using dotFitness.Modules.Users.Infrastructure.Data;
using dotFitness.SharedKernel.Results;

namespace dotFitness.Modules.Users.Infrastructure.Handlers;

public class AddUserMetricCommandHandler : IRequestHandler<AddUserMetricCommand, Result<UserMetricDto>>
{
    private readonly UsersDbContext _context;
    private readonly ILogger<AddUserMetricCommandHandler> _logger;

    public AddUserMetricCommandHandler(
        UsersDbContext context,
        ILogger<AddUserMetricCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<UserMetricDto>> Handle(AddUserMetricCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Use execution strategy to handle retries and transactions together
            var strategy = _context.Database.CreateExecutionStrategy();
            return await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
                
                try
                {
                    // 1. Get user to verify existence and get unit preference
                    var user = await _context.Users
                        .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);
                    
                    if (user == null)
                    {
                        return Result.Failure<UserMetricDto>("User not found");
                    }

                    // 2. Check if metric already exists for this date
                    var existingMetric = await _context.UserMetrics
                        .FirstOrDefaultAsync(um => um.UserId == request.UserId && um.Date.Date == request.Date.Date, cancellationToken);
                    
                    if (existingMetric != null)
                    {
                        return Result.Failure<UserMetricDto>("A metric already exists for this date. Please update the existing metric instead.");
                    }

                    // 3. Create new user metric
                    var userMetric = new UserMetric
                    {
                        UserId = request.UserId,
                        Date = request.Date,
                        Weight = request.Weight,
                        Height = request.Height,
                        Notes = request.Notes,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    // 4. Calculate BMI if both weight and height are provided
                    if (request.Weight.HasValue && request.Height.HasValue)
                    {
                        userMetric.CalculateBmi(user.UnitPreference);
                    }

                    // 5. Save the metric
                    _context.UserMetrics.Add(userMetric);
                    await _context.SaveChangesAsync(cancellationToken);

                    await transaction.CommitAsync(cancellationToken);

                    var metricDto = UserMetricMapper.ToDto(userMetric);

                    _logger.LogInformation("User metric added successfully for user {UserId} on {Date}", 
                        request.UserId, request.Date.ToString("yyyy-MM-dd"));
                    
                    return Result.Success(metricDto);
                }
                catch
                {
                    await transaction.RollbackAsync(cancellationToken);
                    throw;
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add user metric for user {UserId} on {Date}", 
                request.UserId, request.Date.ToString("yyyy-MM-dd"));
            return Result.Failure<UserMetricDto>($"Failed to add user metric: {ex.Message}");
        }
    }
}
