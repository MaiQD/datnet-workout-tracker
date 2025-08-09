using MediatR;
using dotFitness.Modules.Users.Application.DTOs;
using dotFitness.SharedKernel.Results;

namespace dotFitness.Modules.Users.Application.Commands;

public record LoginWithGoogleCommand(LoginWithGoogleRequest Request) : IRequest<Result<LoginResponseDto>>;
