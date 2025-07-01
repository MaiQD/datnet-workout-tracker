namespace dotFitness.Modules.Exercises.Domain.Events;

public class ExerciseCreatedEvent
{
    public string ExerciseId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public List<string> MuscleGroups { get; set; } = new();
    public List<string> Equipment { get; set; } = new();
    public bool IsGlobal { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ExerciseCreatedEvent() { }

    public ExerciseCreatedEvent(string exerciseId, string userId, string name, List<string> muscleGroups, List<string> equipment, bool isGlobal)
    {
        ExerciseId = exerciseId;
        UserId = userId;
        Name = name;
        MuscleGroups = muscleGroups;
        Equipment = equipment;
        IsGlobal = isGlobal;
        CreatedAt = DateTime.UtcNow;
    }
}
