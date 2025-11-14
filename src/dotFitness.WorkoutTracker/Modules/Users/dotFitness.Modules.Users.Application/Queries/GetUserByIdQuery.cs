using dotFitness.Common.Results;
using MediatR;
using dotFitness.Modules.Users.Application.DTOs;

namespace dotFitness.Modules.Users.Application.Queries;

public record GetUserByIdQuery(Guid UserId) : IRequest<Result<UserDto>>;
