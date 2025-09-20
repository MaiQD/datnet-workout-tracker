using dotFitness.Modules.Exercises.Application.DTOs;
using dotFitness.SharedKernel.Results;
using MediatR;

namespace dotFitness.Modules.Exercises.Application.Queries;

public record GetExerciseByIdQuery(
    string ExerciseId,
    int UserId
) : IRequest<Result<ExerciseDto?>>;
