using MediatR;
using Microsoft.Extensions.Logging;
using dotFitness.Modules.Exercises.Application.Commands;
using dotFitness.Modules.Exercises.Application.DTOs;
using dotFitness.Modules.Exercises.Application.Mappers;
using dotFitness.Modules.Exercises.Domain.Entities;
using dotFitness.Modules.Exercises.Domain.Repositories;
using dotFitness.SharedKernel.Results;

namespace dotFitness.Modules.Exercises.Infrastructure.Handlers;

public class CreateExerciseCommandHandler : IRequestHandler<CreateExerciseCommand, Result<ExerciseDto>>
{
    private readonly IExerciseRepository _exerciseRepository;
    private readonly ILogger<CreateExerciseCommandHandler> _logger;

    public CreateExerciseCommandHandler(
        IExerciseRepository exerciseRepository,
        ILogger<CreateExerciseCommandHandler> logger)
    {
        _exerciseRepository = exerciseRepository;
        _logger = logger;
    }

    public async Task<Result<ExerciseDto>> Handle(CreateExerciseCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Creating exercise {ExerciseName} for user {UserId}", request.Name, request.UserId);

            var exercise = new Exercise
            {
                Name = request.Name,
                Description = request.Description,
                MuscleGroups = request.MuscleGroups,
                Equipment = request.Equipment,
                Instructions = request.Instructions,
                Difficulty = request.Difficulty,
                VideoUrl = request.VideoUrl,
                ImageUrl = request.ImageUrl,
                Tags = request.Tags,
                IsGlobal = false, // User-created exercises are never global
                UserId = request.UserId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var result = await _exerciseRepository.CreateAsync(exercise, cancellationToken);
            if (result.IsFailure)
            {
                _logger.LogError("Failed to create exercise {ExerciseName}: {Error}", request.Name, result.Error);
                return Result.Failure<ExerciseDto>(result.Error!);
            }

            var exerciseDto = ExerciseMapper.ToDto(result.Value!);
            _logger.LogInformation("Successfully created exercise {ExerciseId} for user {UserId}", exercise.Id, request.UserId);
            
            return Result.Success(exerciseDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating exercise {ExerciseName} for user {UserId}: {ErrorMessage}", 
                request.Name, request.UserId, ex.Message);
            return Result.Failure<ExerciseDto>($"Failed to create exercise: {ex.Message}");
        }
    }
}
