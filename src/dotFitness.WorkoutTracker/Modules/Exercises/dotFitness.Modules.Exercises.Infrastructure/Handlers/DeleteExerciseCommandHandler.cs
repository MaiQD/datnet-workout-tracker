using MediatR;
using Microsoft.Extensions.Logging;
using dotFitness.Modules.Exercises.Application.Commands;
using dotFitness.Modules.Exercises.Domain.Repositories;
using dotFitness.SharedKernel.Results;

namespace dotFitness.Modules.Exercises.Infrastructure.Handlers;

public class DeleteExerciseCommandHandler : IRequestHandler<DeleteExerciseCommand, Result>
{
    private readonly IExerciseRepository _exerciseRepository;
    private readonly ILogger<DeleteExerciseCommandHandler> _logger;

    public DeleteExerciseCommandHandler(
        IExerciseRepository exerciseRepository,
        ILogger<DeleteExerciseCommandHandler> logger)
    {
        _exerciseRepository = exerciseRepository;
        _logger = logger;
    }

    public async Task<Result> Handle(DeleteExerciseCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Deleting exercise {ExerciseId} for user {UserId}", request.ExerciseId, request.UserId);

            // Check if exercise exists and user has permission to delete it
            var existingExerciseResult = await _exerciseRepository.GetByIdAsync(request.ExerciseId, cancellationToken);
            if (existingExerciseResult.IsFailure)
            {
                _logger.LogWarning("Failed to retrieve exercise {ExerciseId}: {Error}", request.ExerciseId, existingExerciseResult.Error);
                return Result.Failure(existingExerciseResult.Error!);
            }

            var existingExercise = existingExerciseResult.Value;
            if (existingExercise == null)
            {
                _logger.LogWarning("Exercise {ExerciseId} not found", request.ExerciseId);
                return Result.Failure("Exercise not found");
            }

            // Check if user owns the exercise (global exercises cannot be deleted by users)
            if (existingExercise.IsGlobal || existingExercise.UserId != request.UserId)
            {
                _logger.LogWarning("User {UserId} attempted to delete exercise {ExerciseId} they don't own", 
                    request.UserId, request.ExerciseId);
                return Result.Failure("You don't have permission to delete this exercise");
            }

            var result = await _exerciseRepository.DeleteAsync(request.ExerciseId, cancellationToken);
            if (result.IsFailure)
            {
                _logger.LogError("Failed to delete exercise {ExerciseId}: {Error}", request.ExerciseId, result.Error);
                return Result.Failure(result.Error!);
            }

            _logger.LogInformation("Successfully deleted exercise {ExerciseId} for user {UserId}", request.ExerciseId, request.UserId);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting exercise {ExerciseId} for user {UserId}: {ErrorMessage}", 
                request.ExerciseId, request.UserId, ex.Message);
            return Result.Failure($"Failed to delete exercise: {ex.Message}");
        }
    }
}
