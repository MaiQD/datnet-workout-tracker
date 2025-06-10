using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using System.Security.Claims;
using dotFitness.Modules.Users.Application.Commands;
using dotFitness.Modules.Users.Application.Queries;
using dotFitness.Modules.Users.Application.DTOs;
using dotFitness.SharedKernel.Results;

namespace dotFitness.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/users")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IMediator mediator, ILogger<UsersController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Gets the current user's profile information
    /// </summary>
    /// <returns>User profile data</returns>
    [HttpGet("profile")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetProfile()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { error = "User ID not found in token" });
            }

            var query = new GetUserProfileQuery { UserId = userId };
            var result = await _mediator.Send(query);

            if (result.IsFailure)
            {
                _logger.LogWarning("Failed to get profile for user {UserId}: {Error}", userId, result.Error);
                return NotFound(new { error = result.Error });
            }

            return Ok(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while getting user profile");
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { error = "An internal server error occurred" });
        }
    }

    /// <summary>
    /// Updates the current user's profile information
    /// </summary>
    /// <param name="command">The profile update data</param>
    /// <returns>Updated user profile</returns>
    [HttpPut("profile")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserProfileCommand command)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { error = "User ID not found in token" });
            }

            // Set the user ID from the token to ensure user can only update their own profile
            command.UserId = userId;

            var result = await _mediator.Send(command);

            if (result.IsFailure)
            {
                _logger.LogWarning("Failed to update profile for user {UserId}: {Error}", userId, result.Error);
                
                if (result.Error!.Contains("not found"))
                    return NotFound(new { error = result.Error });
                
                return BadRequest(new { error = result.Error });
            }

            _logger.LogInformation("Profile updated successfully for user {UserId}", userId);
            return Ok(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while updating user profile");
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { error = "An internal server error occurred" });
        }
    }

    /// <summary>
    /// Adds a new body metric entry for the current user
    /// </summary>
    /// <param name="command">The metric data to add</param>
    /// <returns>Created metric data</returns>
    [HttpPost("metrics")]
    [ProducesResponseType(typeof(UserMetricDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AddMetric([FromBody] AddUserMetricCommand command)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { error = "User ID not found in token" });
            }

            // Set the user ID from the token to ensure user can only add metrics for themselves
            command.UserId = userId;

            var result = await _mediator.Send(command);

            if (result.IsFailure)
            {
                _logger.LogWarning("Failed to add metric for user {UserId}: {Error}", userId, result.Error);
                
                if (result.Error!.Contains("already exists"))
                    return Conflict(new { error = result.Error });
                
                return BadRequest(new { error = result.Error });
            }

            _logger.LogInformation("Metric added successfully for user {UserId} on {Date}", 
                userId, command.Date.ToString("yyyy-MM-dd"));
            
            return CreatedAtAction(nameof(GetLatestMetric), new { }, result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while adding user metric");
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { error = "An internal server error occurred" });
        }
    }

    /// <summary>
    /// Gets all metrics for the current user with optional filtering and pagination
    /// </summary>
    /// <param name="startDate">Optional start date filter</param>
    /// <param name="endDate">Optional end date filter</param>
    /// <param name="skip">Number of records to skip for pagination (default: 0)</param>
    /// <param name="take">Number of records to take for pagination (default: 50, max: 100)</param>
    /// <returns>List of user metrics</returns>
    [HttpGet("metrics")]
    [ProducesResponseType(typeof(IEnumerable<UserMetricDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetMetrics(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 50)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { error = "User ID not found in token" });
            }

            // Limit the maximum number of records that can be retrieved
            take = Math.Min(take, 100);

            var query = new GetUserMetricsQuery
            {
                UserId = userId,
                StartDate = startDate,
                EndDate = endDate,
                Skip = skip,
                Take = take
            };

            var result = await _mediator.Send(query);

            if (result.IsFailure)
            {
                _logger.LogWarning("Failed to get metrics for user {UserId}: {Error}", userId, result.Error);
                return BadRequest(new { error = result.Error });
            }

            return Ok(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while getting user metrics");
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { error = "An internal server error occurred" });
        }
    }

    /// <summary>
    /// Gets the most recent metric entry for the current user
    /// </summary>
    /// <returns>Latest user metric or null if no metrics exist</returns>
    [HttpGet("metrics/latest")]
    [ProducesResponseType(typeof(UserMetricDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetLatestMetric()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { error = "User ID not found in token" });
            }

            var query = new GetLatestUserMetricQuery { UserId = userId };
            var result = await _mediator.Send(query);

            if (result.IsFailure)
            {
                _logger.LogWarning("Failed to get latest metric for user {UserId}: {Error}", userId, result.Error);
                return NotFound(new { error = result.Error });
            }

            return Ok(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while getting latest user metric");
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { error = "An internal server error occurred" });
        }
    }
}
