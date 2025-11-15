using FastEndpoints;
using MediatR;
using dotFitness.Modules.Users.Application.Queries;
using dotFitness.Modules.Users.Application.DTOs;
using dotFitness.Common.API.Extensions;

namespace dotFitness.Modules.Users.API.Endpoints.Users;

/// <summary>
/// Endpoint for getting the current user's profile information
/// </summary>
public class GetUserProfileEndpoint(IMediator mediator) : Endpoint<EmptyRequest, UserDto>
{
    public override void Configure()
    {
        Get("/api/v1/users/profile");
        Policies("UserOnly");
        Summary(s =>
        {
            s.Summary = "Gets the current user's profile information";
            s.Description = "Retrieves the profile data for the authenticated user";
            s.Responses[200] = "User profile retrieved successfully";
            s.Responses[401] = "Unauthorized - User not authenticated";
            s.Responses[404] = "User profile not found";
            s.Responses[500] = "Internal server error";
        });
    }

    public override async Task HandleAsync(EmptyRequest req, CancellationToken ct)
    {
        var userId = HttpContext.User.GetRequiredUserId();
        var query = new GetUserProfileQuery { UserId = userId };
        var result = await mediator.Send(query, ct);
        
        await this.SendResultAsync(result, ct);
    }
}

