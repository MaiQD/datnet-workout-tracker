using dotFitness.Common.Results;
using dotFitness.Modules.Exercises.Application.DTOs;
using MediatR;

namespace dotFitness.Modules.Exercises.Application.Queries;

public record GetAllEquipmentQuery(
    Guid UserId
) : IRequest<Result<IEnumerable<EquipmentDto>>>;
