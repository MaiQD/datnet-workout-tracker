using dotFitness.Common.Results;
using MediatR;
using dotFitness.Modules.Users.Application.DTOs;

namespace dotFitness.Modules.Users.Application.Commands;

public record LoginWithGoogleCommand(LoginWithGoogleRequest Request) : IRequest<Result<LoginResponseDto>>;
