using dotFitness.Common.Results;
using dotFitness.Modules.Exercises.Application.DTOs;
using MediatR;

namespace dotFitness.Modules.Exercises.Application.Queries;

public record GetExerciseByIdQuery(
    string ExerciseId,
    Guid UserId
) : IRequest<Result<ExerciseDto?>>;
