namespace dotFitness.SharedKernel.Events;

/// <summary>
/// Marker interface for domain events
/// </summary>
public interface IDomainEvent
{
    string EventId { get; }
    string EventType { get; }
    DateTime OccurredOn { get; }
    string? CorrelationId { get; }
    string? TraceId { get; }
}
