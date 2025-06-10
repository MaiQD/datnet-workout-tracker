namespace dotFitness.Modules.Users.Domain.Events;

public class UserCreatedEvent
{
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public UserCreatedEvent() { }

    public UserCreatedEvent(string userId, string email, string displayName, List<string> roles)
    {
        UserId = userId;
        Email = email;
        DisplayName = displayName;
        Roles = roles;
        CreatedAt = DateTime.UtcNow;
    }
}

public class UserMetricAddedEvent
{
    public string UserMetricId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public double? Weight { get; set; }
    public double? Height { get; set; }
    public double? Bmi { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public UserMetricAddedEvent() { }

    public UserMetricAddedEvent(string userMetricId, string userId, DateTime date, double? weight, double? height, double? bmi)
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
