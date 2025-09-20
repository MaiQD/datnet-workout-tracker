namespace dotFitness.SharedKernel.Events;

/// <summary>
/// Base implementation for domain events
/// </summary>
public abstract class BaseDomainEvent : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public string EventType => GetType().Name;
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
    public string? CorrelationId { get; set; }
    public string? TraceId { get; set; }

    protected BaseDomainEvent()
    {
    }

    protected BaseDomainEvent(string? correlationId, string? traceId)
    {
        CorrelationId = correlationId;
        TraceId = traceId;
    }
}
