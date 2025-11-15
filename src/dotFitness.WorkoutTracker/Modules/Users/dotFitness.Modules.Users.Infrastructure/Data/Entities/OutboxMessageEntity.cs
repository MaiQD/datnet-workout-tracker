namespace dotFitness.Modules.Users.Infrastructure.Data.Entities;

public class OutboxMessageEntity
{
    public long Id { get; set; }

    public string EventId { get; set; } = Guid.NewGuid().ToString();

    public string EventType { get; set; } = string.Empty;

    public string EventData { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public bool IsProcessed { get; set; }

    public DateTime? ProcessedAt { get; set; }

    public string? CorrelationId { get; set; }

    public string? TraceId { get; set; }

    public int RetryCount { get; set; }

    public string? LastError { get; set; }
}
