using dotFitness.Common.Results;
using dotFitness.Modules.Users.Application.DTOs;
using dotFitness.Modules.Users.Application.Mappers;
using dotFitness.Modules.Users.Domain.Entities;
using dotFitness.Modules.Users.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace dotFitness.Modules.Users.Application.Commands;

public class AddUserMetricCommandHandler : IRequestHandler<AddUserMetricCommand, Result<UserMetricDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserMetricsRepository _userMetricsRepository;
    private readonly ILogger<AddUserMetricCommandHandler> _logger;

    public AddUserMetricCommandHandler(
        IUserRepository userRepository,
        IUserMetricsRepository userMetricsRepository,
        ILogger<AddUserMetricCommandHandler> logger)
    {
        _userRepository = userRepository;
        _userMetricsRepository = userMetricsRepository;
        _logger = logger;
    }

    public async Task<Result<UserMetricDto>> Handle(AddUserMetricCommand request, CancellationToken cancellationToken)
    {
        // 1. Get user to verify existence and get unit preference
        var userResult = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (userResult.IsFailure)
        {
            return Result.Failure<UserMetricDto>("User not found");
        }

        var user = userResult.Value!;

        // 2. Check if metric already exists for this date
        var existsResult = await _userMetricsRepository.ExistsForUserAndDateAsync(request.UserId, request.Date, cancellationToken);
        if (existsResult.IsFailure)
        {
            return Result.Failure<UserMetricDto>(existsResult.Error ?? "Failed to check for existing metric.");
        }

        if (existsResult.Value)
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

        // 5. Save the metric via repository
        var createResult = await _userMetricsRepository.CreateAsync(userMetric, cancellationToken);
        if (createResult.IsFailure)
        {
            return Result.Failure<UserMetricDto>(createResult.Error ?? "Failed to create user metric.");
        }

        var metricDto = UserMetricMapper.ToDto(createResult.Value!);

        _logger.LogInformation("User metric added successfully for user {UserId} on {Date}", 
            request.UserId, request.Date.ToString("yyyy-MM-dd"));
        
        return Result.Success(metricDto);
    }
}

