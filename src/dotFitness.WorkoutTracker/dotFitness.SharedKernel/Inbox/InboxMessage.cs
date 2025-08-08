using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace dotFitness.SharedKernel.Inbox;

public class InboxMessage
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    [BsonElement("eventId")]
    public string EventId { get; set; } = string.Empty;

    [BsonElement("eventType")]
    public string EventType { get; set; } = string.Empty;

    [BsonElement("occurredOn")]
    public DateTime OccurredOn { get; set; } = DateTime.UtcNow;

    [BsonElement("payload")]
    public string Payload { get; set; } = string.Empty;

    [BsonElement("consumer")]
    public string Consumer { get; set; } = string.Empty;

    [BsonElement("status")]
    public string Status { get; set; } = "pending"; // pending|processed|failed

    [BsonElement("processedAt")]
    public DateTime? ProcessedAt { get; set; }

    [BsonElement("attempts")]
    public int Attempts { get; set; } = 0;

    [BsonElement("error")]
    public string? Error { get; set; }

    [BsonElement("correlationId")]
    public string? CorrelationId { get; set; }

    [BsonElement("traceId")]
    public string? TraceId { get; set; }

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}


