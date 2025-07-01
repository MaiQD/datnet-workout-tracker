using dotFitness.Modules.Exercises.Application.DTOs;
using dotFitness.SharedKernel.Results;
using MediatR;

namespace dotFitness.Modules.Exercises.Application.Queries;

public record GetAllMuscleGroupsQuery(
    string UserId
) : IRequest<Result<IEnumerable<MuscleGroupDto>>>;
