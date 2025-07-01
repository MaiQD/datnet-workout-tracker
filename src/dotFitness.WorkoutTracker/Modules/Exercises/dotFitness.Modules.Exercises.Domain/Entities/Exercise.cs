using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using dotFitness.SharedKernel.Interfaces;

namespace dotFitness.Modules.Exercises.Domain.Entities;

public class Exercise : IEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    [BsonElement("name")]
    [BsonRequired]
    public string Name { get; set; } = string.Empty;

    [BsonElement("description")]
    public string? Description { get; set; }

    [BsonElement("muscleGroups")]
    public List<string> MuscleGroups { get; set; } = new();

    [BsonElement("equipment")]
    public List<string> Equipment { get; set; } = new();

    [BsonElement("instructions")]
    public List<string> Instructions { get; set; } = new();

    [BsonElement("difficulty")]
    [BsonRepresentation(BsonType.String)]
    public ExerciseDifficulty Difficulty { get; set; } = ExerciseDifficulty.Beginner;

    [BsonElement("videoUrl")]
    public string? VideoUrl { get; set; }

    [BsonElement("imageUrl")]
    public string? ImageUrl { get; set; }

    [BsonElement("isGlobal")]
    public bool IsGlobal { get; set; } = false;

    [BsonElement("userId")]
    public string? UserId { get; set; } // null for global exercises

    [BsonElement("tags")]
    public List<string> Tags { get; set; } = new();

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Updates exercise properties and sets UpdatedAt timestamp
    /// </summary>
    public void UpdateExercise(string? name = null, string? description = null, 
        List<string>? muscleGroups = null, List<string>? equipment = null, 
        List<string>? instructions = null, ExerciseDifficulty? difficulty = null,
        string? videoUrl = null, string? imageUrl = null, List<string>? tags = null)
    {
        var isUpdated = false;

        if (!string.IsNullOrWhiteSpace(name))
        {
            Name = name;
            isUpdated = true;
        }

        if (description != null)
        {
            Description = description;
            isUpdated = true;
        }

        if (muscleGroups != null)
        {
            MuscleGroups = muscleGroups;
            isUpdated = true;
        }

        if (equipment != null)
        {
            Equipment = equipment;
            isUpdated = true;
        }

        if (instructions != null)
        {
            Instructions = instructions;
            isUpdated = true;
        }

        if (difficulty.HasValue)
        {
            Difficulty = difficulty.Value;
            isUpdated = true;
        }

        if (videoUrl != null)
        {
            VideoUrl = videoUrl;
            isUpdated = true;
        }

        if (imageUrl != null)
        {
            ImageUrl = imageUrl;
            isUpdated = true;
        }

        if (tags != null)
        {
            Tags = tags;
            isUpdated = true;
        }

        if (isUpdated)
        {
            UpdatedAt = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Adds a muscle group to the exercise
    /// </summary>
    public void AddMuscleGroup(string muscleGroup)
    {
        if (!string.IsNullOrWhiteSpace(muscleGroup) && !MuscleGroups.Contains(muscleGroup))
        {
            MuscleGroups.Add(muscleGroup);
            UpdatedAt = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Removes a muscle group from the exercise
    /// </summary>
    public void RemoveMuscleGroup(string muscleGroup)
    {
        if (MuscleGroups.Contains(muscleGroup))
        {
            MuscleGroups.Remove(muscleGroup);
            UpdatedAt = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Adds equipment to the exercise
    /// </summary>
    public void AddEquipment(string equipment)
    {
        if (!string.IsNullOrWhiteSpace(equipment) && !Equipment.Contains(equipment))
        {
            Equipment.Add(equipment);
            UpdatedAt = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Removes equipment from the exercise
    /// </summary>
    public void RemoveEquipment(string equipment)
    {
        if (Equipment.Contains(equipment))
        {
            Equipment.Remove(equipment);
            UpdatedAt = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Adds a tag to the exercise
    /// </summary>
    public void AddTag(string tag)
    {
        if (!string.IsNullOrWhiteSpace(tag) && !Tags.Contains(tag))
        {
            Tags.Add(tag);
            UpdatedAt = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Removes a tag from the exercise
    /// </summary>
    public void RemoveTag(string tag)
    {
        if (Tags.Contains(tag))
        {
            Tags.Remove(tag);
            UpdatedAt = DateTime.UtcNow;
        }
    }
}

public enum ExerciseDifficulty
{
    Beginner,
    Intermediate,
    Advanced,
    Expert
}
