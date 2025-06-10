using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json;

namespace dotFitness.SharedKernel.Outbox;

public class OutboxMessage
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    [BsonElement("eventType")]
    public string EventType { get; set; } = string.Empty;

    [BsonElement("eventData")]
    public string EventData { get; set; } = string.Empty;

    [BsonElement("isProcessed")]
    public bool IsProcessed { get; set; } = false;

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("processedAt")]
    public DateTime? ProcessedAt { get; set; }

    public static OutboxMessage Create<T>(T eventData)
    {
        return new OutboxMessage
        {
            EventType = typeof(T).Name,
            EventData = JsonSerializer.Serialize(eventData),
            CreatedAt = DateTime.UtcNow
        };
    }

    public T? Deserialize<T>()
    {
        return JsonSerializer.Deserialize<T>(EventData);
    }
}
