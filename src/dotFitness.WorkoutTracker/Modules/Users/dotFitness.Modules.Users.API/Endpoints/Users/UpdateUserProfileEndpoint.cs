using FastEndpoints;
using MediatR;
using dotFitness.Modules.Users.Application.Commands;
using dotFitness.Modules.Users.Application.DTOs;
using dotFitness.Common.API.Extensions;
using dotFitness.Common.Authorization;

namespace dotFitness.Modules.Users.API.Endpoints.Users;

/// <summary>
/// Endpoint for updating the current user's profile information
/// </summary>
public class UpdateUserProfileEndpoint(IMediator mediator) : Endpoint<UpdateUserProfileRequest, UserDto>
{
    public override void Configure()
    {
        Put("/api/v1/users/profile");
        Policies(AuthorizationPolicies.SelfOrAdmin);
        Version(1);
        Summary(s =>
        {
            s.Summary = "Updates the current user's profile information";
            s.Description = "Updates profile data for the authenticated user";
            s.Responses[200] = "User profile updated successfully";
            s.Responses[400] = "Invalid request or validation failed";
            s.Responses[401] = "Unauthorized - User not authenticated";
            s.Responses[404] = "User profile not found";
            s.Responses[500] = "Internal server error";
        });
    }

    public override async Task HandleAsync(UpdateUserProfileRequest req, CancellationToken ct)
    {
        var userId = HttpContext.User.GetRequiredUserId();
        var command = new UpdateUserProfileCommand(userId, req);
        var result = await mediator.Send(command, ct);
        
        await this.SendResultAsync(result, ct);
    }
}

