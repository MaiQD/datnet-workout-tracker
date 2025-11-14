using dotFitness.Common.Results;
using dotFitness.Modules.Users.Application.DTOs;
using MediatR;

namespace dotFitness.Modules.Users.Application.Commands;

public record UpdateUserProfileCommand(
    Guid UserId,
    UpdateUserProfileRequest Request) : IRequest<Result<UserDto>>;