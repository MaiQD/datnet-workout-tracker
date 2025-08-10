using MediatR;
using dotFitness.Modules.Users.Application.DTOs;
using dotFitness.SharedKernel.Results;

namespace dotFitness.Modules.Users.Application.Queries;

public record GetUserByIdQuery(string UserId) : IRequest<Result<UserDto>>;
