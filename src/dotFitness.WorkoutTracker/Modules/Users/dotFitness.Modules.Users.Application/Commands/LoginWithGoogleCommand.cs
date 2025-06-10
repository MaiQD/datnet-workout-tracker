using MediatR;
using dotFitness.Modules.Users.Application.DTOs;
using dotFitness.SharedKernel.Results;
using System.ComponentModel.DataAnnotations;

namespace dotFitness.Modules.Users.Application.Commands;

public record LoginWithGoogleCommand([Required] string GoogleToken) : IRequest<Result<LoginResponseDto>>;
