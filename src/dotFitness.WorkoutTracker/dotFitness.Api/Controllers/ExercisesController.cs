using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using System.Security.Claims;
using dotFitness.Modules.Exercises.Application.Commands;
using dotFitness.Modules.Exercises.Application.Queries;
using dotFitness.Modules.Exercises.Application.DTOs;
using dotFitness.Modules.Exercises.Domain.Entities;
using dotFitness.SharedKernel.Results;

namespace dotFitness.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/exercises")]
[Authorize]
public class ExercisesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ExercisesController> _logger;

    public ExercisesController(IMediator mediator, ILogger<ExercisesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Gets all exercises available to the current user (global + user-specific)
    /// </summary>
    /// <param name="searchTerm">Optional search term to filter exercises</param>
    /// <param name="muscleGroups">Optional muscle groups to filter by</param>
    /// <param name="equipment">Optional equipment to filter by</param>
    /// <param name="difficulty">Optional difficulty level to filter by</param>
    /// <returns>List of exercises</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ExerciseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllExercises(
        [FromQuery] string? searchTerm = null,
        [FromQuery] List<string>? muscleGroups = null,
        [FromQuery] List<string>? equipment = null,
        [FromQuery] ExerciseDifficulty? difficulty = null)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { error = "User ID not found in token" });
            }

            var query = new GetAllExercisesQuery(userId, searchTerm, muscleGroups, equipment, difficulty);
            var result = await _mediator.Send(query);

            if (result.IsFailure)
            {
                _logger.LogError("Failed to get exercises for user {UserId}: {Error}", userId, result.Error);
                return StatusCode(500, new { error = "Failed to retrieve exercises" });
            }

            return Ok(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error getting exercises");
            return StatusCode(500, new { error = "An unexpected error occurred" });
        }
    }

    /// <summary>
    /// Gets a specific exercise by ID
    /// </summary>
    /// <param name="id">The exercise ID</param>
    /// <returns>Exercise details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ExerciseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetExerciseById(string id)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { error = "User ID not found in token" });
            }

            var query = new GetExerciseByIdQuery(id, userId);
            var result = await _mediator.Send(query);

            if (result.IsFailure)
            {
                _logger.LogError("Failed to get exercise {ExerciseId} for user {UserId}: {Error}", id, userId, result.Error);
                return StatusCode(500, new { error = "Failed to retrieve exercise" });
            }

            if (result.Value == null)
            {
                return NotFound(new { error = "Exercise not found" });
            }

            return Ok(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error getting exercise {ExerciseId}", id);
            return StatusCode(500, new { error = "An unexpected error occurred" });
        }
    }

    /// <summary>
    /// Creates a new user-specific exercise
    /// </summary>
    /// <param name="request">The exercise creation request</param>
    /// <returns>Created exercise</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ExerciseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateExercise([FromBody] CreateExerciseRequest request)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { error = "User ID not found in token" });
            }

            var command = new CreateExerciseCommand(
                userId,
                request.Name,
                request.Description,
                request.MuscleGroups,
                request.Equipment,
                request.Instructions,
                request.Difficulty,
                request.VideoUrl,
                request.ImageUrl,
                request.Tags ?? new List<string>()
            );

            var result = await _mediator.Send(command);

            if (result.IsFailure)
            {
                _logger.LogError("Failed to create exercise for user {UserId}: {Error}", userId, result.Error);
                return BadRequest(new { error = result.Error });
            }

            return CreatedAtAction(nameof(GetExerciseById), new { id = result.Value!.Id }, result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error creating exercise");
            return StatusCode(500, new { error = "An unexpected error occurred" });
        }
    }

    /// <summary>
    /// Updates an existing user-specific exercise
    /// </summary>
    /// <param name="id">The exercise ID</param>
    /// <param name="request">The exercise update request</param>
    /// <returns>Updated exercise</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ExerciseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateExercise(string id, [FromBody] UpdateExerciseRequest request)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { error = "User ID not found in token" });
            }

            var command = new UpdateExerciseCommand(
                id,
                userId,
                request.Name,
                request.Description,
                request.MuscleGroups,
                request.Equipment,
                request.Instructions,
                request.Difficulty,
                request.VideoUrl,
                request.ImageUrl,
                request.Tags ?? new List<string>()
            );

            var result = await _mediator.Send(command);

            if (result.IsFailure)
            {
                _logger.LogError("Failed to update exercise {ExerciseId} for user {UserId}: {Error}", id, userId, result.Error);
                
                if (result.Error!.Contains("not found"))
                    return NotFound(new { error = result.Error });
                if (result.Error.Contains("permission"))
                    return Forbid();
                
                return BadRequest(new { error = result.Error });
            }

            return Ok(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error updating exercise {ExerciseId}", id);
            return StatusCode(500, new { error = "An unexpected error occurred" });
        }
    }

    /// <summary>
    /// Deletes a user-specific exercise
    /// </summary>
    /// <param name="id">The exercise ID</param>
    /// <returns>No content on success</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteExercise(string id)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { error = "User ID not found in token" });
            }

            var command = new DeleteExerciseCommand(id, userId);
            var result = await _mediator.Send(command);

            if (result.IsFailure)
            {
                _logger.LogError("Failed to delete exercise {ExerciseId} for user {UserId}: {Error}", id, userId, result.Error);
                
                if (result.Error!.Contains("not found"))
                    return NotFound(new { error = result.Error });
                if (result.Error.Contains("permission"))
                    return Forbid();
                
                return BadRequest(new { error = result.Error });
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error deleting exercise {ExerciseId}", id);
            return StatusCode(500, new { error = "An unexpected error occurred" });
        }
    }

    /// <summary>
    /// Gets all muscle groups available to the current user
    /// </summary>
    /// <returns>List of muscle groups</returns>
    [HttpGet("muscle-groups")]
    [ProducesResponseType(typeof(IEnumerable<MuscleGroupDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllMuscleGroups()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { error = "User ID not found in token" });
            }

            var query = new GetAllMuscleGroupsQuery(userId);
            var result = await _mediator.Send(query);

            if (result.IsFailure)
            {
                _logger.LogError("Failed to get muscle groups for user {UserId}: {Error}", userId, result.Error);
                return StatusCode(500, new { error = "Failed to retrieve muscle groups" });
            }

            return Ok(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error getting muscle groups");
            return StatusCode(500, new { error = "An unexpected error occurred" });
        }
    }

    /// <summary>
    /// Gets all equipment available to the current user
    /// </summary>
    /// <returns>List of equipment</returns>
    [HttpGet("equipment")]
    [ProducesResponseType(typeof(IEnumerable<EquipmentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllEquipment()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { error = "User ID not found in token" });
            }

            var query = new GetAllEquipmentQuery(userId);
            var result = await _mediator.Send(query);

            if (result.IsFailure)
            {
                _logger.LogError("Failed to get equipment for user {UserId}: {Error}", userId, result.Error);
                return StatusCode(500, new { error = "Failed to retrieve equipment" });
            }

            return Ok(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error getting equipment");
            return StatusCode(500, new { error = "An unexpected error occurred" });
        }
    }
}
