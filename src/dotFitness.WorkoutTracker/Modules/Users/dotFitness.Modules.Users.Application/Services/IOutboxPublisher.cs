namespace dotFitness.Modules.Users.Application.Services;

/// <summary>
/// Interface for publishing domain events to the outbox
/// </summary>
public interface IOutboxPublisher
{
    /// <summary>
    /// Publishes a domain event to the outbox for reliable delivery
    /// </summary>
    Task PublishAsync<T>(T domainEvent, CancellationToken cancellationToken = default) where T : class;
}

