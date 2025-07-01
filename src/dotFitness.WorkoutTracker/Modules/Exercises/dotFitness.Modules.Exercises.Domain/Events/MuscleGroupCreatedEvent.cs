namespace dotFitness.Modules.Exercises.Domain.Events;

public class MuscleGroupCreatedEvent
{
    public string MuscleGroupId { get; set; } = string.Empty;
    public string? UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsGlobal { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public MuscleGroupCreatedEvent() { }

    public MuscleGroupCreatedEvent(string muscleGroupId, string? userId, string name, bool isGlobal)
    {
        MuscleGroupId = muscleGroupId;
        UserId = userId;
        Name = name;
        IsGlobal = isGlobal;
        CreatedAt = DateTime.UtcNow;
    }
}
