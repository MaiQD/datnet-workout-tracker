using dotFitness.Modules.Exercises.Application.DTOs;
using dotFitness.SharedKernel.Results;
using MediatR;

namespace dotFitness.Modules.Exercises.Application.Queries;

public record GetAllEquipmentQuery(
    int UserId
) : IRequest<Result<IEnumerable<EquipmentDto>>>;
