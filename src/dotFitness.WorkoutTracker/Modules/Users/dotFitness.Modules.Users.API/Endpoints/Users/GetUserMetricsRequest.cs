namespace dotFitness.Modules.Users.API.Endpoints.Users;

/// <summary>
/// Request DTO for getting user metrics with optional filtering and pagination
/// FastEndpoints automatically binds query parameters from properties
/// </summary>
public class GetUserMetricsRequest
{
    public DateTime? StartDate { get; set; }
    
    public DateTime? EndDate { get; set; }
    
    public int Skip { get; set; } = 0;
    
    public int Take { get; set; } = 50;
}

