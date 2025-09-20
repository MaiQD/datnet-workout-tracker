using MediatR;
using dotFitness.Modules.Users.Application.DTOs;
using dotFitness.SharedKernel.Results;
using System.ComponentModel.DataAnnotations;

namespace dotFitness.Modules.Users.Application.Queries;

public class GetLatestUserMetricQuery(int userId) : IRequest<Result<UserMetricDto>>
{
    public GetLatestUserMetricQuery() : this(0)
    {
    }
    [Required]
    public int UserId { get; set; } = userId;
};