using Microsoft.AspNetCore.Mvc;
using MediatR;
using dotFitness.Modules.Users.Application.Commands;
using dotFitness.Modules.Users.Application.DTOs;
using dotFitness.SharedKernel.Results;

namespace dotFitness.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IMediator mediator, ILogger<AuthController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Authenticates a user with Google OAuth token
    /// </summary>
    /// <param name="command">The Google login command containing the OAuth token</param>
    /// <returns>JWT token and user information</returns>
    [HttpPost("google-login")]
    [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> LoginWithGoogle([FromBody] LoginWithGoogleCommand command)
    {
        try
        {
            var result = await _mediator.Send(command);
            
            if (result.IsFailure)
            {
                _logger.LogWarning("Google login failed: {Error}", result.Error);
                return BadRequest(new { error = result.Error });
            }

            var loginResponse = result.Value!;
            _logger.LogInformation("User {UserId} logged in successfully", loginResponse.UserId);
            
            return Ok(loginResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred during Google login");
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { error = "An internal server error occurred" });
        }
    }
}
