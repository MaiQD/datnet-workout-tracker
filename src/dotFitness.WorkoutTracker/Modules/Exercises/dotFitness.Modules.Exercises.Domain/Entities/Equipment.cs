using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using dotFitness.SharedKernel.Interfaces;

namespace dotFitness.Modules.Exercises.Domain.Entities;

public class Equipment : IEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    [BsonElement("name")]
    [BsonRequired]
    public string Name { get; set; } = string.Empty;

    [BsonElement("description")]
    public string? Description { get; set; }

    [BsonElement("category")]
    public string? Category { get; set; } // e.g., "Weights", "Cardio", "Bodyweight", "Resistance"

    [BsonElement("isGlobal")]
    public bool IsGlobal { get; set; } = false;

    [BsonElement("userId")]
    public string? UserId { get; set; } // null for global equipment

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Updates equipment properties and sets UpdatedAt timestamp
    /// </summary>
    public void UpdateEquipment(string? name = null, string? description = null, string? category = null)
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

        if (category != null)
        {
            Category = category;
            isUpdated = true;
        }

        if (isUpdated)
        {
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
