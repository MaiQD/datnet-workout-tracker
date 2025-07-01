using MediatR;
using Microsoft.Extensions.Logging;
using dotFitness.Modules.Exercises.Application.DTOs;
using dotFitness.Modules.Exercises.Application.Mappers;
using dotFitness.Modules.Exercises.Application.Queries;
using dotFitness.Modules.Exercises.Domain.Repositories;
using dotFitness.SharedKernel.Results;

namespace dotFitness.Modules.Exercises.Infrastructure.Handlers;

public class GetExerciseByIdQueryHandler : IRequestHandler<GetExerciseByIdQuery, Result<ExerciseDto?>>
{
    private readonly IExerciseRepository _exerciseRepository;
    private readonly ILogger<GetExerciseByIdQueryHandler> _logger;

    public GetExerciseByIdQueryHandler(
        IExerciseRepository exerciseRepository,
        ILogger<GetExerciseByIdQueryHandler> logger)
    {
        _exerciseRepository = exerciseRepository;
        _logger = logger;
    }

    public async Task<Result<ExerciseDto?>> Handle(GetExerciseByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Getting exercise {ExerciseId} for user {UserId}", request.ExerciseId, request.UserId);

            var result = await _exerciseRepository.GetByIdAsync(request.ExerciseId, cancellationToken);
            if (result.IsFailure)
            {
                _logger.LogError("Failed to get exercise {ExerciseId}: {Error}", request.ExerciseId, result.Error);
                return Result.Failure<ExerciseDto?>(result.Error!);
            }

            var exercise = result.Value;
            if (exercise == null)
            {
                _logger.LogInformation("Exercise {ExerciseId} not found", request.ExerciseId);
                return Result.Success<ExerciseDto?>(null);
            }

            // Check if user has access to this exercise (global or owned by user)
            if (!exercise.IsGlobal && exercise.UserId != request.UserId)
            {
                _logger.LogWarning("User {UserId} attempted to access exercise {ExerciseId} they don't own", 
                    request.UserId, request.ExerciseId);
                return Result.Success<ExerciseDto?>(null);
            }

            var exerciseDto = ExerciseMapper.ToDto(exercise);
            _logger.LogInformation("Successfully retrieved exercise {ExerciseId} for user {UserId}", request.ExerciseId, request.UserId);
            
            return Result.Success<ExerciseDto?>(exerciseDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting exercise {ExerciseId} for user {UserId}: {ErrorMessage}", 
                request.ExerciseId, request.UserId, ex.Message);
            return Result.Failure<ExerciseDto?>($"Failed to get exercise: {ex.Message}");
        }
    }
}
