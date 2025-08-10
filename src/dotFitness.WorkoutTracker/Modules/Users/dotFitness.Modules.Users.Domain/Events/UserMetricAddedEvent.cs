namespace dotFitness.Modules.Users.Domain.Events;

public class UserMetricAddedEvent
{
    public string UserMetricId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public double? Weight { get; set; }
    public double? Height { get; set; }
    public double? Bmi { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public UserMetricAddedEvent()
    {
    }

    public UserMetricAddedEvent(string userMetricId, string userId, DateTime date, double? weight, double? height,
        double? bmi)
    {
        UserMetricId = userMetricId;
        UserId = userId;
        Date = date;
        Weight = weight;
        Height = height;
        Bmi = bmi;
        CreatedAt = DateTime.UtcNow;
    }
}