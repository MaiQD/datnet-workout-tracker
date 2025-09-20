using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using dotFitness.Modules.Users.Application.Queries;
using dotFitness.Modules.Users.Application.DTOs;
using dotFitness.Modules.Users.Application.Mappers;
using dotFitness.Modules.Users.Infrastructure.Data;
using dotFitness.SharedKernel.Results;

namespace dotFitness.Modules.Users.Infrastructure.Handlers;

public class GetUserMetricsQueryHandler : IRequestHandler<GetUserMetricsQuery, Result<IEnumerable<UserMetricDto>>>
{
    private readonly UsersDbContext _context;
    private readonly UserMetricMapper _userMetricMapper;
    private readonly ILogger<GetUserMetricsQueryHandler> _logger;

    public GetUserMetricsQueryHandler(
        UsersDbContext context,
        UserMetricMapper userMetricMapper,
        ILogger<GetUserMetricsQueryHandler> logger)
    {
        _context = context;
        _userMetricMapper = userMetricMapper;
        _logger = logger;
    }

    public async Task<Result<IEnumerable<UserMetricDto>>> Handle(GetUserMetricsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            IQueryable<Domain.Entities.UserMetric> query = _context.UserMetrics
                .Where(um => um.UserId == request.UserId);

            if (request.StartDate.HasValue && request.EndDate.HasValue)
            {
                // Filter by date range
                query = query.Where(um => um.Date >= request.StartDate.Value && um.Date <= request.EndDate.Value);
            }

            // Apply ordering and pagination
            var metrics = await query
                .OrderByDescending(um => um.Date)
                .ThenByDescending(um => um.CreatedAt)
                .Skip(request.Skip)
                .Take(request.Take)
                .ToListAsync(cancellationToken);

            var metricDtos = metrics.Select(m => _userMetricMapper.ToDto(m)).ToList();

            return Result.Success<IEnumerable<UserMetricDto>>(metricDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get user metrics for user {UserId}", request.UserId);
            return Result.Failure<IEnumerable<UserMetricDto>>($"Failed to get user metrics: {ex.Message}");
        }
    }
}
