using MediatR;
using Microsoft.Extensions.Logging;
using dotFitness.Modules.Users.Application.Commands;
using dotFitness.Modules.Users.Application.DTOs;
using dotFitness.Modules.Users.Application.Mappers;
using dotFitness.Modules.Users.Domain.Entities;
using dotFitness.Modules.Users.Domain.Repositories;
using dotFitness.SharedKernel.Results;

namespace dotFitness.Modules.Users.Infrastructure.Handlers;

public class AddUserMetricCommandHandler : IRequestHandler<AddUserMetricCommand, Result<UserMetricDto>>
{
    private readonly IUserMetricsRepository _userMetricsRepository;
    private readonly IUserRepository _userRepository;
    private readonly UserMetricMapper _userMetricMapper;
    private readonly ILogger<AddUserMetricCommandHandler> _logger;

    public AddUserMetricCommandHandler(
        IUserMetricsRepository userMetricsRepository,
        IUserRepository userRepository,
        UserMetricMapper userMetricMapper,
        ILogger<AddUserMetricCommandHandler> logger)
    {
        _userMetricsRepository = userMetricsRepository;
        _userRepository = userRepository;
        _userMetricMapper = userMetricMapper;
        _logger = logger;
    }

    public async Task<Result<UserMetricDto>> Handle(AddUserMetricCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Get user to verify existence and get unit preference
            var userResult = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (userResult.IsFailure)
            {
                return Result.Failure<UserMetricDto>("User not found");
            }

            var user = userResult.Value!;

            // Check if metric already exists for this date
            var existingMetricResult = await _userMetricsRepository.ExistsForUserAndDateAsync(
                request.UserId, request.Date, cancellationToken);
            
            if (existingMetricResult.IsSuccess && existingMetricResult.Value)
            {
                return Result.Failure<UserMetricDto>("A metric already exists for this date. Please update the existing metric instead.");
            }

            // Create new user metric
            var userMetric = new UserMetric
            {
                Id = Guid.NewGuid().ToString(),
                UserId = request.UserId,
                Date = request.Date,
                Weight = request.Weight,
                Height = request.Height,
                Notes = request.Notes,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Calculate BMI if both weight and height are provided
            if (request.Weight.HasValue && request.Height.HasValue)
            {
                userMetric.CalculateBmi(user.UnitPreference);
            }

            // Save the metric
            var createResult = await _userMetricsRepository.CreateAsync(userMetric, cancellationToken);
            if (createResult.IsFailure)
            {
                return Result.Failure<UserMetricDto>(createResult.Error!);
            }

            var createdMetric = createResult.Value!;
            var metricDto = _userMetricMapper.ToDto(createdMetric);

            _logger.LogInformation("User metric added successfully for user {UserId} on {Date}", 
                request.UserId, request.Date.ToString("yyyy-MM-dd"));
            
            return Result.Success(metricDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add user metric for user {UserId} on {Date}", 
                request.UserId, request.Date.ToString("yyyy-MM-dd"));
            return Result.Failure<UserMetricDto>($"Failed to add user metric: {ex.Message}");
        }
    }
}
