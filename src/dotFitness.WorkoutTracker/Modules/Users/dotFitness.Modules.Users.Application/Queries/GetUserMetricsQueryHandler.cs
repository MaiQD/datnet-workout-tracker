using dotFitness.Common.Results;
using dotFitness.Modules.Users.Application.DTOs;
using dotFitness.Modules.Users.Application.Mappers;
using dotFitness.Modules.Users.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace dotFitness.Modules.Users.Application.Queries;

public class GetUserMetricsQueryHandler : IRequestHandler<GetUserMetricsQuery, Result<IEnumerable<UserMetricDto>>>
{
    private readonly IUserMetricsRepository _userMetricsRepository;
    private readonly ILogger<GetUserMetricsQueryHandler> _logger;

    public GetUserMetricsQueryHandler(
        IUserMetricsRepository userMetricsRepository,
        ILogger<GetUserMetricsQueryHandler> logger)
    {
        _userMetricsRepository = userMetricsRepository;
        _logger = logger;
    }

    public async Task<Result<IEnumerable<UserMetricDto>>> Handle(GetUserMetricsQuery request, CancellationToken cancellationToken)
    {
        Result<IEnumerable<Domain.Entities.UserMetric>> metricsResult;

        if (request.StartDate.HasValue && request.EndDate.HasValue)
        {
            metricsResult = await _userMetricsRepository.GetByUserIdAndDateRangeAsync(
                request.UserId, 
                request.StartDate.Value, 
                request.EndDate.Value, 
                cancellationToken);
        }
        else
        {
            metricsResult = await _userMetricsRepository.GetByUserIdAsync(
                request.UserId, 
                request.Skip, 
                request.Take, 
                cancellationToken);
        }

        if (metricsResult.IsFailure)
        {
            return Result.Failure<IEnumerable<UserMetricDto>>(metricsResult.Error ?? "Failed to get user metrics");
        }

        var metricDtos = UserMetricMapper.ToDto(metricsResult.Value!);
        return Result.Success<IEnumerable<UserMetricDto>>(metricDtos);
    }
}

