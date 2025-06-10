using MediatR;
using dotFitness.Modules.Users.Application.DTOs;
using dotFitness.SharedKernel.Results;
using System.ComponentModel.DataAnnotations;

namespace dotFitness.Modules.Users.Application.Queries;

public class GetUserMetricsQuery : IRequest<Result<IEnumerable<UserMetricDto>>>
{
    public GetUserMetricsQuery()
    {
        
    }

    public GetUserMetricsQuery(string userId, DateTime? fromDate, DateTime? toDate)
    {
        UserId = userId;
        StartDate = fromDate;
        EndDate = toDate;
    }
    [Required]
    public string UserId { get; set; } = string.Empty;
    
    [Range(0, int.MaxValue)]
    public int Skip { get; set; } = 0;
    
    [Range(1, 100)]
    public int Take { get; set; } = 50;
    
    public DateTime? StartDate { get; set; }
    
    public DateTime? EndDate { get; set; }
}
