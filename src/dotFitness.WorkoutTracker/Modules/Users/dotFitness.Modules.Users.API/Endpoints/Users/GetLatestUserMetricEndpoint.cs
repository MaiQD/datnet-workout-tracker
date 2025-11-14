using FastEndpoints;
using MediatR;
using dotFitness.Modules.Users.Application.Queries;
using dotFitness.Modules.Users.Application.DTOs;
using dotFitness.Common.API.Extensions;

namespace dotFitness.Modules.Users.API.Endpoints.Users;

/// <summary>
/// Endpoint for getting the most recent metric entry for the current user
/// </summary>
public class GetLatestUserMetricEndpoint(IMediator mediator) : Endpoint<EmptyRequest, UserMetricDto>
{
    public override void Configure()
    {
        Get("/api/v1/users/metrics/latest");
        Policies("UserOnly");
        Version(1);
        Summary(s =>
        {
            s.Summary = "Gets the most recent metric entry for the current user";
            s.Description = "Retrieves the latest metric entry (weight, height, etc.) for the authenticated user";
            s.Responses[200] = "Latest metric retrieved successfully";
            s.Responses[401] = "Unauthorized - User not authenticated";
            s.Responses[404] = "No metrics found for user";
            s.Responses[500] = "Internal server error";
        });
    }

    public override async Task HandleAsync(EmptyRequest req, CancellationToken ct)
    {
        var userId = HttpContext.User.GetRequiredUserId();
        var query = new GetLatestUserMetricQuery(userId);
        var result = await mediator.Send(query, ct);
        
        await this.SendResultAsync(result, ct);
    }
}

