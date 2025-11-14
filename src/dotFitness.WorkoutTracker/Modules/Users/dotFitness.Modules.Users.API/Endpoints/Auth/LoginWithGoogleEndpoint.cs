using FastEndpoints;
using MediatR;
using dotFitness.Modules.Users.Application.Commands;
using dotFitness.Modules.Users.Application.DTOs;
using dotFitness.Common.API.Extensions;

namespace dotFitness.Modules.Users.API.Endpoints.Auth;

/// <summary>
/// Endpoint for authenticating users with Google OAuth token
/// </summary>
public class LoginWithGoogleEndpoint(IMediator mediator) : Endpoint<LoginWithGoogleRequest, LoginResponseDto>
{
    public override void Configure()
    {
        Post("/api/v1/auth/google-login");
        AllowAnonymous();
        Version(1);
        Summary(s =>
        {
            s.Summary = "Authenticates a user with Google OAuth token";
            s.Description = "Verifies the Google OAuth token and returns JWT token and user information";
            s.Responses[200] = "User authenticated successfully";
            s.Responses[400] = "Invalid request or authentication failed";
            s.Responses[401] = "Unauthorized - Invalid Google token";
            s.Responses[500] = "Internal server error";
        });
    }

    public override async Task HandleAsync(LoginWithGoogleRequest req, CancellationToken ct)
    {
        var command = new LoginWithGoogleCommand(req);
        var result = await mediator.Send(command, ct);
        
        await this.SendResultAsync(result, ct);
    }
}

