using FastEndpoints;
using MediatR;
using dotFitness.Modules.Users.Application.Commands;
using dotFitness.Modules.Users.Application.DTOs;
using dotFitness.Common.API.Extensions;

namespace dotFitness.Modules.Users.API.Endpoints.Users;

/// <summary>
/// Endpoint for adding a new body metric entry for the current user
/// </summary>
public class AddUserMetricEndpoint(IMediator mediator) : Endpoint<AddUserMetricRequest, UserMetricDto>
{
    public override void Configure()
    {
        Post("/api/v1/users/metrics");
        Policies("UserOnly");
        Version(1);
        Summary(s =>
        {
            s.Summary = "Adds a new body metric entry for the current user";
            s.Description = "Creates a new metric entry (weight, height, etc.) for the authenticated user";
            s.Responses[201] = "Metric created successfully";
            s.Responses[400] = "Invalid request or validation failed";
            s.Responses[401] = "Unauthorized - User not authenticated";
            s.Responses[409] = "Conflict - Metric already exists for this date";
            s.Responses[500] = "Internal server error";
        });
    }

    public override async Task HandleAsync(AddUserMetricRequest req, CancellationToken ct)
    {
        var userId = HttpContext.User.GetRequiredUserId();
        
        // Set the user ID from the token to ensure user can only add metrics for themselves
        var command = new AddUserMetricCommand(
            userId,
            req.Date,
            req.Weight,
            req.Height,
            req.Notes
        );
        
        var result = await mediator.Send(command, ct);
        
        // Use 201 Created response with location header
        await this.SendCreatedResultAsync(result, "/api/v1/users/metrics/latest", ct);
    }
}

