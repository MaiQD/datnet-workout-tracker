using dotFitness.Modules.Users.Application.DTOs;
using dotFitness.SharedKernel.Results;
using MediatR;

namespace dotFitness.Modules.Users.Application.Commands;

public record UpdateUserProfileCommand(
    string UserId,
    UpdateUserProfileRequest Request
) : IRequest<Result<UserDto>>;