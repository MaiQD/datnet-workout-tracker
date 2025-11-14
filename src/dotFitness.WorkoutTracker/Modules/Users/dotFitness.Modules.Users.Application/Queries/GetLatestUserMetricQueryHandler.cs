using dotFitness.Common.Results;
using dotFitness.Modules.Users.Application.DTOs;
using dotFitness.Modules.Users.Application.Mappers;
using dotFitness.Modules.Users.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace dotFitness.Modules.Users.Application.Queries;

public class GetLatestUserMetricQueryHandler : IRequestHandler<GetLatestUserMetricQuery, Result<UserMetricDto>>
{
    private readonly IUserMetricsRepository _userMetricsRepository;
    private readonly ILogger<GetLatestUserMetricQueryHandler> _logger;

    public GetLatestUserMetricQueryHandler(
        IUserMetricsRepository userMetricsRepository,
        ILogger<GetLatestUserMetricQueryHandler> logger)
    {
        _userMetricsRepository = userMetricsRepository;
        _logger = logger;
    }

    public async Task<Result<UserMetricDto>> Handle(GetLatestUserMetricQuery request, CancellationToken cancellationToken)
    {
        var metricResult = await _userMetricsRepository.GetLatestByUserIdAsync(request.UserId, cancellationToken);
        
        if (metricResult.IsFailure)
        {
            return Result.Failure<UserMetricDto>(metricResult.Error ?? "No metrics found for user");
        }

        var metricDto = UserMetricMapper.ToDto(metricResult.Value!);
        return Result.Success(metricDto);
    }
}

