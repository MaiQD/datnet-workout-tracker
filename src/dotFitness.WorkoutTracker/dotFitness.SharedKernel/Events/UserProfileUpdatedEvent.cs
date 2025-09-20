namespace dotFitness.SharedKernel.Events;

public class UserProfileUpdatedEvent : BaseDomainEvent
{
    public int UserId { get; }
    public string DisplayName { get; }
    public string? Gender { get; }
    public DateTime? DateOfBirth { get; }
    public string UnitPreference { get; }
    public DateTime UpdatedAt { get; }

    public UserProfileUpdatedEvent(
        int userId,
        string displayName,
        string? gender,
        DateTime? dateOfBirth,
        string unitPreference,
        DateTime updatedAt,
        string? correlationId = null,
        string? traceId = null) : base(correlationId, traceId)
    {
        UserId = userId;
        DisplayName = displayName;
        Gender = gender;
        DateOfBirth = dateOfBirth;
        UnitPreference = unitPreference;
        UpdatedAt = updatedAt;
    }
}
