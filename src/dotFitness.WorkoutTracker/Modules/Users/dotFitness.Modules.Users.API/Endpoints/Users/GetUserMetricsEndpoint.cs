using FastEndpoints;
using MediatR;
using dotFitness.Modules.Users.Application.Queries;
using dotFitness.Modules.Users.Application.DTOs;
using dotFitness.Common.API.Extensions;

namespace dotFitness.Modules.Users.API.Endpoints.Users;

/// <summary>
/// Endpoint for getting all metrics for the current user with optional filtering and pagination
/// </summary>
public class GetUserMetricsEndpoint(IMediator mediator) : Endpoint<GetUserMetricsRequest, IEnumerable<UserMetricDto>>
{
    public override void Configure()
    {
        Get("/api/v1/users/metrics");
        Policies("UserOnly");
        Version(1);
        Summary(s =>
        {
            s.Summary = "Gets all metrics for the current user with optional filtering and pagination";
            s.Description = "Retrieves a list of user metrics with optional date filtering and pagination (max 100 records per request)";
            s.Responses[200] = "Metrics retrieved successfully";
            s.Responses[400] = "Invalid request or validation failed";
            s.Responses[401] = "Unauthorized - User not authenticated";
            s.Responses[500] = "Internal server error";
        });
    }

    public override async Task HandleAsync(GetUserMetricsRequest req, CancellationToken ct)
    {
        var userId = HttpContext.User.GetRequiredUserId();

        // Limit the maximum number of records that can be retrieved
        var take = Math.Min(req.Take, 100);

        var query = new GetUserMetricsQuery
        {
            UserId = userId,
            StartDate = req.StartDate,
            EndDate = req.EndDate,
            Skip = req.Skip,
            Take = take
        };

        var result = await mediator.Send(query, ct);
        
        await this.SendResultAsync(result, ct);
    }
}

