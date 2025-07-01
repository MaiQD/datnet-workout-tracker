using MediatR;
using Microsoft.Extensions.Logging;
using dotFitness.Modules.Exercises.Application.DTOs;
using dotFitness.Modules.Exercises.Application.Mappers;
using dotFitness.Modules.Exercises.Application.Queries;
using dotFitness.Modules.Exercises.Domain.Repositories;
using dotFitness.SharedKernel.Results;

namespace dotFitness.Modules.Exercises.Infrastructure.Handlers;

public class GetAllExercisesQueryHandler : IRequestHandler<GetAllExercisesQuery, Result<IEnumerable<ExerciseDto>>>
{
    private readonly IExerciseRepository _exerciseRepository;
    private readonly ILogger<GetAllExercisesQueryHandler> _logger;

    public GetAllExercisesQueryHandler(
        IExerciseRepository exerciseRepository,
        ILogger<GetAllExercisesQueryHandler> logger)
    {
        _exerciseRepository = exerciseRepository;
        _logger = logger;
    }

    public async Task<Result<IEnumerable<ExerciseDto>>> Handle(GetAllExercisesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Getting all exercises for user {UserId} with search criteria", request.UserId);

            Result<IEnumerable<Domain.Entities.Exercise>> result;

            // If no search criteria provided, get all exercises for user
            if (string.IsNullOrWhiteSpace(request.SearchTerm) && 
                (request.MuscleGroups?.Any() != true) && 
                (request.Equipment?.Any() != true) && 
                !request.Difficulty.HasValue)
            {
                result = await _exerciseRepository.GetAllForUserAsync(request.UserId, cancellationToken);
            }
            else
            {
                // Use search functionality
                result = await _exerciseRepository.SearchAsync(
                    request.UserId, 
                    request.SearchTerm, 
                    request.MuscleGroups, 
                    request.Equipment, 
                    request.Difficulty, 
                    cancellationToken);
            }

            if (result.IsFailure)
            {
                _logger.LogError("Failed to get exercises for user {UserId}: {Error}", request.UserId, result.Error);
                return Result.Failure<IEnumerable<ExerciseDto>>(result.Error!);
            }

            var exerciseDtos = ExerciseMapper.ToDto(result.Value!);
            _logger.LogInformation("Successfully retrieved {Count} exercises for user {UserId}", 
                exerciseDtos.Count(), request.UserId);
            
            return Result.Success(exerciseDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting exercises for user {UserId}: {ErrorMessage}", 
                request.UserId, ex.Message);
            return Result.Failure<IEnumerable<ExerciseDto>>($"Failed to get exercises: {ex.Message}");
        }
    }
}
