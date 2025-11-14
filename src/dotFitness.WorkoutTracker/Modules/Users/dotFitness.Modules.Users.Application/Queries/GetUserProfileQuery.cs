using MediatR;
using dotFitness.Modules.Users.Application.DTOs;
using System.ComponentModel.DataAnnotations;
using dotFitness.Common.Results;

namespace dotFitness.Modules.Users.Application.Queries;

public class GetUserProfileQuery : IRequest<Result<UserDto>>
{
    [Required]
    public Guid UserId { get; set; }
}
