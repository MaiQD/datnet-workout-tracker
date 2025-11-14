using System.Text.Json;
using dotFitness.Modules.Users.Application.Services;
using dotFitness.Modules.Users.Infrastructure.Data;
using dotFitness.Modules.Users.Infrastructure.Data.Entities;
using Microsoft.Extensions.Logging;

namespace dotFitness.Modules.Users.Infrastructure.Services;

public class OutboxPublisher(UsersDbContext context, ILogger<OutboxPublisher> logger) : IOutboxPublisher
{
    public async Task PublishAsync<T>(T domainEvent, CancellationToken cancellationToken = default) where T : class
    {
        var outboxMessage = new OutboxMessageEntity
        {
            EventId = Guid.NewGuid().ToString(),
            EventType = typeof(T).Name,
            EventData = JsonSerializer.Serialize(domainEvent),
            CreatedAt = DateTime.UtcNow,
            IsProcessed = false
        };

        context.OutboxMessages.Add(outboxMessage);
        await context.SaveChangesAsync(cancellationToken);

        logger.LogDebug("Published event {EventType} with ID {EventId} to outbox", typeof(T).Name, outboxMessage.EventId);
    }
}

