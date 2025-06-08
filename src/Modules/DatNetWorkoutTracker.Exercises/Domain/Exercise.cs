using MongoDB.Bson.Serialization.Attributes;
using DatNetWorkoutTracker.Shared.Domain;

namespace DatNetWorkoutTracker.Exercises.Domain;

public class Exercise : BaseEntity
{
    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("description")]
    public string Description { get; set; } = string.Empty;

    [BsonElement("category")]
    public ExerciseCategory Category { get; set; }

    [BsonElement("targetMuscleGroups")]
    public List<MuscleGroup> TargetMuscleGroups { get; set; } = new();

    [BsonElement("equipment")]
    public List<Equipment> Equipment { get; set; } = new();

    [BsonElement("instructions")]
    public List<string> Instructions { get; set; } = new();

    [BsonElement("youtubeUrl")]
    public string? YoutubeUrl { get; set; }

    [BsonElement("thumbnailUrl")]
    public string? ThumbnailUrl { get; set; }

    [BsonElement("difficulty")]
    public DifficultyLevel Difficulty { get; set; }

    [BsonElement("tags")]
    public List<string> Tags { get; set; } = new();

    [BsonElement("isCustom")]
    public bool IsCustom { get; set; } = false;

    [BsonElement("createdByUserId")]
    public string? CreatedByUserId { get; set; }
}

public enum ExerciseCategory
{
    Strength,
    Cardio,
    Flexibility,
    Balance,
    Sports,
    Functional
}

public enum MuscleGroup
{
    Chest,
    Back,
    Shoulders,
    Arms,
    Legs,
    Glutes,
    Core,
    Calves,
    Forearms,
    Traps,
    FullBody
}

public enum Equipment
{
    None,
    Barbell,
    Dumbbell,
    Kettlebell,
    Machine,
    Cable,
    Bands,
    Bodyweight,
    Bench,
    PullUpBar,
    Other
}

public enum DifficultyLevel
{
    Beginner,
    Intermediate,
    Advanced,
    Expert
}
