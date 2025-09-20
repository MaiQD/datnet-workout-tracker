using dotFitness.SharedKernel.Results;
using MediatR;

namespace dotFitness.Modules.Exercises.Application.Commands;

public record DeleteExerciseCommand(
    string ExerciseId,
    int UserId
) : IRequest<Result>;
