using MongoDB.Bson.Serialization.Attributes;
using DatNetWorkoutTracker.Shared.Domain;

namespace DatNetWorkoutTracker.Routines.Domain;

public class Routine : BaseEntity
{
    [BsonElement("userId")]
    public string UserId { get; set; } = string.Empty;

    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("description")]
    public string? Description { get; set; }

    [BsonElement("exercises")]
    public List<RoutineExercise> Exercises { get; set; } = new();

    [BsonElement("tags")]
    public List<string> Tags { get; set; } = new();

    [BsonElement("estimatedDuration")]
    public TimeSpan? EstimatedDuration { get; set; }

    [BsonElement("difficulty")]
    public RoutineDifficulty Difficulty { get; set; }

    [BsonElement("isPublic")]
    public bool IsPublic { get; set; } = false;

    [BsonElement("timesUsed")]
    public int TimesUsed { get; set; } = 0;

    [BsonElement("lastUsed")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? LastUsed { get; set; }
}

public class RoutineExercise
{
    [BsonElement("exerciseId")]
    public string ExerciseId { get; set; } = string.Empty;

    [BsonElement("exerciseName")]
    public string ExerciseName { get; set; } = string.Empty;

    [BsonElement("order")]
    public int Order { get; set; }

    [BsonElement("plannedSets")]
    public List<PlannedSet> PlannedSets { get; set; } = new();

    [BsonElement("notes")]
    public string? Notes { get; set; }

    [BsonElement("restTime")]
    public TimeSpan? RestTime { get; set; }

    [BsonElement("superset")]
    public string? Superset { get; set; } // Group exercises in supersets
}

public class PlannedSet
{
    [BsonElement("setNumber")]
    public int SetNumber { get; set; }

    [BsonElement("targetReps")]
    public int? TargetReps { get; set; }

    [BsonElement("targetWeight")]
    public decimal? TargetWeight { get; set; }

    [BsonElement("targetDistance")]
    public decimal? TargetDistance { get; set; }

    [BsonElement("targetDuration")]
    public TimeSpan? TargetDuration { get; set; }

    [BsonElement("notes")]
    public string? Notes { get; set; }
}

public enum RoutineDifficulty
{
    Beginner,
    Intermediate,
    Advanced,
    Expert
}
