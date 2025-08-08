using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace dotFitness.Modules.Exercises.Domain.Entities;

public class UserPreferencesProjection
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    [BsonElement("userId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string UserId { get; set; } = string.Empty;

    [BsonElement("focusMuscleGroupIds")]
    [BsonRepresentation(BsonType.ObjectId)]
    public List<string> FocusMuscleGroupIds { get; set; } = new();

    [BsonElement("availableEquipmentIds")]
    [BsonRepresentation(BsonType.ObjectId)]
    public List<string> AvailableEquipmentIds { get; set; } = new();

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}


