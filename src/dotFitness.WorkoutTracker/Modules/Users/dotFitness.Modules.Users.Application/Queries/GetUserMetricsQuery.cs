using MediatR;
using dotFitness.Modules.Users.Application.DTOs;
using System.ComponentModel.DataAnnotations;
using dotFitness.Common.Results;

namespace dotFitness.Modules.Users.Application.Queries;

public class GetUserMetricsQuery : IRequest<Result<IEnumerable<UserMetricDto>>>
{
    public GetUserMetricsQuery()
    {
        
    }

    public GetUserMetricsQuery(Guid userId, DateTime? fromDate, DateTime? toDate)
    {
        UserId = userId;
        StartDate = fromDate;
        EndDate = toDate;
    }
    [Required]
    public Guid UserId { get; set; }
    
    [Range(0, int.MaxValue)]
    public int Skip { get; set; } = 0;
    
    [Range(1, 100)]
    public int Take { get; set; } = 50;
    
    public DateTime? StartDate { get; set; }
    
    public DateTime? EndDate { get; set; }
}
