using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using dotFitness.SharedKernel.Interfaces;

namespace dotFitness.Modules.Exercises.Domain.Entities;

public class MuscleGroup : IEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    [BsonElement("name")]
    [BsonRequired]
    public string Name { get; set; } = string.Empty;

    [BsonElement("description")]
    public string? Description { get; set; }

    [BsonElement("bodyRegion")]
    [BsonRepresentation(BsonType.String)]
    public BodyRegion? BodyRegion { get; set; }

    [BsonElement("parentId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? ParentId { get; set; }

    [BsonElement("aliases")]
    public List<string>? Aliases { get; set; }

    [BsonElement("isGlobal")]
    public bool IsGlobal { get; set; } = false;

    [BsonElement("userId")]
    public string? UserId { get; set; } // null for global muscle groups

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Updates muscle group properties and sets UpdatedAt timestamp
    /// </summary>
    public void UpdateMuscleGroup(
        string? name = null,
        string? description = null,
        BodyRegion? bodyRegion = null,
        string? parentId = null,
        List<string>? aliases = null)
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

        if (bodyRegion.HasValue)
        {
            BodyRegion = bodyRegion.Value;
            isUpdated = true;
        }

        if (parentId != null)
        {
            ParentId = parentId;
            isUpdated = true;
        }

        if (aliases != null)
        {
            Aliases = aliases;
            isUpdated = true;
        }

        if (isUpdated)
        {
            UpdatedAt = DateTime.UtcNow;
        }
    }
}

public enum BodyRegion
{
    Upper,
    Lower,
    Core,
    FullBody
}
