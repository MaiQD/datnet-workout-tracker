using dotFitness.Modules.Exercises.Application.DTOs;
using dotFitness.Modules.Exercises.Domain.Entities;
using dotFitness.SharedKernel.Results;
using MediatR;

namespace dotFitness.Modules.Exercises.Application.Commands;

public record CreateExerciseCommand(
    string UserId,
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
