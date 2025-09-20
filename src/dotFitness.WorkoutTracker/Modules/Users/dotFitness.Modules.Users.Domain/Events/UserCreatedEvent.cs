namespace dotFitness.Modules.Users.Domain.Events;

public class UserCreatedEvent
{
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = [];
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public UserCreatedEvent()
    {
    }

    public UserCreatedEvent(string userId, string email, string displayName, List<string> roles)
    {
        UserId = userId;
        Email = email;
        DisplayName = displayName;
        Roles = roles;
        CreatedAt = DateTime.UtcNow;
    }
}