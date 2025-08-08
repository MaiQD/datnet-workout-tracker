using dotFitness.Modules.Exercises.Application.DTOs;
using dotFitness.SharedKernel.Results;
using MediatR;

namespace dotFitness.Modules.Exercises.Application.Queries;

public record GetSmartExerciseSuggestionsQuery(
    string UserId,
    int Limit = 10
) : IRequest<Result<IEnumerable<ExerciseDto>>>;


