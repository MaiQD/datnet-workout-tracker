using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using dotFitness.Modules.Users.Application.Queries;
using dotFitness.Modules.Users.Application.DTOs;
using dotFitness.Modules.Users.Application.Mappers;
using dotFitness.Modules.Users.Infrastructure.Data;
using dotFitness.SharedKernel.Results;

namespace dotFitness.Modules.Users.Infrastructure.Handlers;

public class GetLatestUserMetricQueryHandler : IRequestHandler<GetLatestUserMetricQuery, Result<UserMetricDto>>
{
    private readonly UsersDbContext _context;
    private readonly UserMetricMapper _userMetricMapper;
    private readonly ILogger<GetLatestUserMetricQueryHandler> _logger;

    public GetLatestUserMetricQueryHandler(
        UsersDbContext context,
        UserMetricMapper userMetricMapper,
        ILogger<GetLatestUserMetricQueryHandler> logger)
    {
        _context = context;
        _userMetricMapper = userMetricMapper;
        _logger = logger;
    }

    public async Task<Result<UserMetricDto>> Handle(GetLatestUserMetricQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var metric = await _context.UserMetrics
                .Where(um => um.UserId == request.UserId)
                .OrderByDescending(um => um.Date)
                .ThenByDescending(um => um.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);
            
            if (metric == null)
            {
                return Result.Failure<UserMetricDto>("No metrics found for user");
            }

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
