using dotFitness.SharedKernel.Results;
using MediatR;

namespace dotFitness.Modules.Exercises.Application.Commands;

public record DeleteExerciseCommand(
    string ExerciseId,
    string UserId
) : IRequest<Result>;
