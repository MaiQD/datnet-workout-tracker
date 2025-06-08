using MongoDB.Bson.Serialization.Attributes;
using DatNetWorkoutTracker.Shared.Domain;

namespace DatNetWorkoutTracker.Workouts.Domain;

public class Workout : BaseEntity
{
    [BsonElement("userId")]
    public string UserId { get; set; } = string.Empty;

    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("description")]
    public string? Description { get; set; }

    [BsonElement("startTime")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime StartTime { get; set; }

    [BsonElement("endTime")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? EndTime { get; set; }

    [BsonElement("duration")]
    public TimeSpan? Duration { get; set; }

    [BsonElement("exercises")]
    public List<WorkoutExercise> Exercises { get; set; } = new();

    [BsonElement("notes")]
    public string? Notes { get; set; }

    [BsonElement("status")]
    public WorkoutStatus Status { get; set; } = WorkoutStatus.Planned;

    [BsonElement("routineId")]
    public string? RoutineId { get; set; }
}

public class WorkoutExercise
{
    [BsonElement("exerciseId")]
    public string ExerciseId { get; set; } = string.Empty;

    [BsonElement("exerciseName")]
    public string ExerciseName { get; set; } = string.Empty;

    [BsonElement("order")]
    public int Order { get; set; }

    [BsonElement("sets")]
    public List<WorkoutSet> Sets { get; set; } = new();

    [BsonElement("notes")]
    public string? Notes { get; set; }

    [BsonElement("restTime")]
    public TimeSpan? RestTime { get; set; }
}

public class WorkoutSet
{
    [BsonElement("setNumber")]
    public int SetNumber { get; set; }

    [BsonElement("reps")]
    public int? Reps { get; set; }

    [BsonElement("weight")]
    public decimal? Weight { get; set; }

    [BsonElement("distance")]
    public decimal? Distance { get; set; }

    [BsonElement("duration")]
    public TimeSpan? Duration { get; set; }

    [BsonElement("calories")]
    public int? Calories { get; set; }

    [BsonElement("isCompleted")]
    public bool IsCompleted { get; set; } = false;

    [BsonElement("notes")]
    public string? Notes { get; set; }

    [BsonElement("rpe")]
    public int? RPE { get; set; } // Rate of Perceived Exertion (1-10)
}

public enum WorkoutStatus
{
    Planned,
    InProgress,
    Completed,
    Cancelled
}
