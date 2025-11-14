using dotFitness.Common.Results;
using dotFitness.Modules.Exercises.Application.DTOs;
using dotFitness.Modules.Exercises.Domain.Entities;
using MediatR;

namespace dotFitness.Modules.Exercises.Application.Queries;

public record GetAllExercisesQuery(
    Guid UserId,
    string? SearchTerm = null,
    List<string>? MuscleGroups = null,
    List<string>? Equipment = null,
    ExerciseDifficulty? Difficulty = null
) : IRequest<Result<IEnumerable<ExerciseDto>>>;
