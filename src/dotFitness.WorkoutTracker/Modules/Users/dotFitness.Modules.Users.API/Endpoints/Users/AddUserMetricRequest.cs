namespace dotFitness.Modules.Users.API.Endpoints.Users;

/// <summary>
/// Request DTO for adding a user metric
/// </summary>
public class AddUserMetricRequest
{
    public DateTime Date { get; set; }
    public double? Weight { get; set; }
    public double? Height { get; set; }
    public string? Notes { get; set; }
}

