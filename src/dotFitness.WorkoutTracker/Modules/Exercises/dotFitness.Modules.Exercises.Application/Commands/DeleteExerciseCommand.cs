using dotFitness.Common.Results;
using MediatR;

namespace dotFitness.Modules.Exercises.Application.Commands;

public record DeleteExerciseCommand(
    string ExerciseId,
    Guid UserId
) : IRequest<Result>;
