using MediatR;
using dotFitness.Modules.Users.Application.DTOs;
using dotFitness.SharedKernel.Results;
using System.ComponentModel.DataAnnotations;

namespace dotFitness.Modules.Users.Application.Commands;

public class LoginWithGoogleCommand : IRequest<Result<LoginResponseDto>>
{
    [Required]
    public string GoogleToken { get; set; } = string.Empty;
}
