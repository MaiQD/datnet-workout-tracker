using MediatR;
using dotFitness.Modules.Users.Application.DTOs;
using dotFitness.SharedKernel.Results;
using System.ComponentModel.DataAnnotations;

namespace dotFitness.Modules.Users.Application.Queries;

public class GetLatestUserMetricQuery : IRequest<Result<UserMetricDto>>
{
    [Required]
    public string UserId { get; set; } = string.Empty;
}
