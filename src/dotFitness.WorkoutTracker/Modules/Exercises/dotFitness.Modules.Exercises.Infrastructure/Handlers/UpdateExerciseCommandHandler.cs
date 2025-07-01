using MediatR;
using Microsoft.Extensions.Logging;
using dotFitness.Modules.Exercises.Application.Commands;
using dotFitness.Modules.Exercises.Application.DTOs;
using dotFitness.Modules.Exercises.Application.Mappers;
using dotFitness.Modules.Exercises.Domain.Repositories;
using dotFitness.SharedKernel.Results;

namespace dotFitness.Modules.Exercises.Infrastructure.Handlers;

public class UpdateExerciseCommandHandler : IRequestHandler<UpdateExerciseCommand, Result<ExerciseDto>>
{
    private readonly IExerciseRepository _exerciseRepository;
    private readonly ILogger<UpdateExerciseCommandHandler> _logger;

    public UpdateExerciseCommandHandler(
        IExerciseRepository exerciseRepository,
        ILogger<UpdateExerciseCommandHandler> logger)
    {
        _exerciseRepository = exerciseRepository;
        _logger = logger;
    }

    public async Task<Result<ExerciseDto>> Handle(UpdateExerciseCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Updating exercise {ExerciseId} for user {UserId}", request.ExerciseId, request.UserId);

            // Check if exercise exists and user has permission to update it
            var existingExerciseResult = await _exerciseRepository.GetByIdAsync(request.ExerciseId, cancellationToken);
            if (existingExerciseResult.IsFailure)
            {
                _logger.LogWarning("Failed to retrieve exercise {ExerciseId}: {Error}", request.ExerciseId, existingExerciseResult.Error);
                return Result.Failure<ExerciseDto>(existingExerciseResult.Error!);
            }

            var existingExercise = existingExerciseResult.Value;
            if (existingExercise == null)
            {
                _logger.LogWarning("Exercise {ExerciseId} not found", request.ExerciseId);
                return Result.Failure<ExerciseDto>("Exercise not found");
            }

            // Check if user owns the exercise (global exercises cannot be updated by users)
            if (existingExercise.IsGlobal || existingExercise.UserId != request.UserId)
            {
                _logger.LogWarning("User {UserId} attempted to update exercise {ExerciseId} they don't own", 
                    request.UserId, request.ExerciseId);
                return Result.Failure<ExerciseDto>("You don't have permission to update this exercise");
            }

            // Update exercise properties
            existingExercise.UpdateExercise(
                request.Name,
                request.Description,
                request.MuscleGroups,
                request.Equipment,
                request.Instructions,
                request.Difficulty,
                request.VideoUrl,
                request.ImageUrl,
                request.Tags
            );

            var result = await _exerciseRepository.UpdateAsync(existingExercise, cancellationToken);
            if (result.IsFailure)
            {
                _logger.LogError("Failed to update exercise {ExerciseId}: {Error}", request.ExerciseId, result.Error);
                return Result.Failure<ExerciseDto>(result.Error!);
            }

            var exerciseDto = ExerciseMapper.ToDto(result.Value!);
            _logger.LogInformation("Successfully updated exercise {ExerciseId} for user {UserId}", request.ExerciseId, request.UserId);
            
            return Result.Success(exerciseDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating exercise {ExerciseId} for user {UserId}: {ErrorMessage}", 
                request.ExerciseId, request.UserId, ex.Message);
            return Result.Failure<ExerciseDto>($"Failed to update exercise: {ex.Message}");
        }
    }
}
