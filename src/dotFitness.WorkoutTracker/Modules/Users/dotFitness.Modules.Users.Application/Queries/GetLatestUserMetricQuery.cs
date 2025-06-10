using MediatR;
using dotFitness.Modules.Users.Application.DTOs;
using dotFitness.SharedKernel.Results;
using System.ComponentModel.DataAnnotations;

namespace dotFitness.Modules.Users.Application.Queries;

public class GetLatestUserMetricQuery(string userId) : IRequest<Result<UserMetricDto>>
{
    public GetLatestUserMetricQuery() : this(string.Empty)
    {
    }
    [Required]
    public string UserId { get; set; } = userId;
};