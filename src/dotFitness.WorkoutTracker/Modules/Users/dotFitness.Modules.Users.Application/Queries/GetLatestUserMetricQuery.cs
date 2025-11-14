using MediatR;
using dotFitness.Modules.Users.Application.DTOs;
using System.ComponentModel.DataAnnotations;
using dotFitness.Common.Results;

namespace dotFitness.Modules.Users.Application.Queries;

public class GetLatestUserMetricQuery(Guid userId) : IRequest<Result<UserMetricDto>>
{
    public GetLatestUserMetricQuery() : this(Guid.Empty)
    {
    }
    [Required]
    public Guid UserId { get; set; } = userId;
};