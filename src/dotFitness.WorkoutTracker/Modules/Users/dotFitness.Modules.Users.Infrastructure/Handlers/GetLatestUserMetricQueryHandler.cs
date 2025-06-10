using MediatR;
using Microsoft.Extensions.Logging;
using dotFitness.Modules.Users.Application.Queries;
using dotFitness.Modules.Users.Application.DTOs;
using dotFitness.Modules.Users.Application.Mappers;
using dotFitness.Modules.Users.Domain.Repositories;
using dotFitness.SharedKernel.Results;

namespace dotFitness.Modules.Users.Infrastructure.Handlers;

public class GetLatestUserMetricQueryHandler : IRequestHandler<GetLatestUserMetricQuery, Result<UserMetricDto>>
{
    private readonly IUserMetricsRepository _userMetricsRepository;
    private readonly UserMetricMapper _userMetricMapper;
    private readonly ILogger<GetLatestUserMetricQueryHandler> _logger;

    public GetLatestUserMetricQueryHandler(
        IUserMetricsRepository userMetricsRepository,
        UserMetricMapper userMetricMapper,
        ILogger<GetLatestUserMetricQueryHandler> logger)
    {
        _userMetricsRepository = userMetricsRepository;
        _userMetricMapper = userMetricMapper;
        _logger = logger;
    }

    public async Task<Result<UserMetricDto>> Handle(GetLatestUserMetricQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var metricResult = await _userMetricsRepository.GetLatestByUserIdAsync(request.UserId, cancellationToken);
            if (metricResult.IsFailure)
            {
                return Result.Failure<UserMetricDto>("No metrics found for user");
            }

            var metric = metricResult.Value!;
            var metricDto = _userMetricMapper.ToDto(metric);

            return Result.Success(metricDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get latest user metric for user {UserId}", request.UserId);
            return Result.Failure<UserMetricDto>($"Failed to get latest user metric: {ex.Message}");
        }
    }
}
