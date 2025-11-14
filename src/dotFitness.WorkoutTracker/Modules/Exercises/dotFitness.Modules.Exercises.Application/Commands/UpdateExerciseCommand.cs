using dotFitness.Common.Results;
using dotFitness.Modules.Exercises.Application.DTOs;
using dotFitness.Modules.Exercises.Domain.Entities;
using MediatR;

namespace dotFitness.Modules.Exercises.Application.Commands;

public record UpdateExerciseCommand(
    string ExerciseId,
    Guid UserId,
    string Name,
    string? Description,
    List<string> MuscleGroups,
    List<string> Equipment,
    List<string> Instructions,
    ExerciseDifficulty Difficulty,
    string? VideoUrl,
    string? ImageUrl,
    List<string> Tags
) : IRequest<Result<ExerciseDto>>;
