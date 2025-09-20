using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using dotFitness.SharedKernel.Events;
using dotFitness.SharedKernel.Inbox;
using dotFitness.Modules.Exercises.Domain.Entities;

namespace dotFitness.Modules.Exercises.Infrastructure.EventHandlers;

/// <summary>
/// Handles UserProfileUpdatedEvent from Users module to update user preferences in Exercises module
/// Implements the Inbox pattern for idempotent cross-module event processing
/// </summary>
public class UserProfileUpdatedEventHandler : IEventHandler<UserProfileUpdatedEvent>
{
    private readonly IMongoCollection<UserPreferencesProjection> _userPreferencesCollection;
    private readonly IMongoCollection<InboxMessage> _inboxCollection;
    private readonly ILogger<UserProfileUpdatedEventHandler> _logger;

    public UserProfileUpdatedEventHandler(
        IMongoCollection<UserPreferencesProjection> userPreferencesCollection,
        IMongoCollection<InboxMessage> inboxCollection,
        ILogger<UserProfileUpdatedEventHandler> logger)
    {
        _userPreferencesCollection = userPreferencesCollection;
        _inboxCollection = inboxCollection;
        _logger = logger;
    }

    public async Task HandleAsync(UserProfileUpdatedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        const string consumerName = "Exercises.UserProfileUpdatedHandler";
        var eventIdString = domainEvent.EventId.ToString();
        
        try
        {
            // Check if this event has already been processed (Inbox pattern for idempotency)
            var existingInboxMessage = await _inboxCollection
                .Find(x => x.EventId == eventIdString && x.Consumer == consumerName)
                .FirstOrDefaultAsync(cancellationToken);

            if (existingInboxMessage != null)
            {
                _logger.LogDebug("Event {EventId} already processed by {Consumer}, skipping", 
                    domainEvent.EventId, consumerName);
                return;
            }

            // Create inbox message to track processing
            var inboxMessage = new InboxMessage
            {
                EventId = eventIdString,
                Consumer = consumerName,
                EventType = domainEvent.EventType,
                Status = "processing",
                CreatedAt = DateTime.UtcNow
            };

            await _inboxCollection.InsertOneAsync(inboxMessage, cancellationToken: cancellationToken);

            // Update or create user preferences projection
            var filter = Builders<UserPreferencesProjection>.Filter.Eq(x => x.UserId, domainEvent.UserId);
            var update = Builders<UserPreferencesProjection>.Update
                .Set(x => x.UserId, domainEvent.UserId)
                .Set(x => x.UpdatedAt, domainEvent.UpdatedAt);

            var options = new UpdateOptions { IsUpsert = true };
            await _userPreferencesCollection.UpdateOneAsync(filter, update, options, cancellationToken);

            // Mark inbox message as completed
            var inboxFilter = Builders<InboxMessage>.Filter.Eq(x => x.Id, inboxMessage.Id);
            var inboxUpdate = Builders<InboxMessage>.Update
                .Set(x => x.Status, "completed")
                .Set(x => x.ProcessedAt, DateTime.UtcNow);

            await _inboxCollection.UpdateOneAsync(inboxFilter, inboxUpdate, cancellationToken: cancellationToken);

            _logger.LogInformation("Successfully updated user preferences projection for user {UserId} after profile update", 
                domainEvent.UserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to handle UserProfileUpdatedEvent for user {UserId}: {Error}", 
                domainEvent.UserId, ex.Message);

            // Mark inbox message as failed if it exists
            try
            {
                var failedInboxFilter = Builders<InboxMessage>.Filter.And(
                    Builders<InboxMessage>.Filter.Eq(x => x.EventId, eventIdString),
                    Builders<InboxMessage>.Filter.Eq(x => x.Consumer, consumerName));

                var failedInboxUpdate = Builders<InboxMessage>.Update
                    .Set(x => x.Status, "failed")
                    .Set(x => x.Error, ex.Message)
                    .Set(x => x.ProcessedAt, DateTime.UtcNow);

                await _inboxCollection.UpdateOneAsync(failedInboxFilter, failedInboxUpdate, cancellationToken: cancellationToken);
            }
            catch (Exception inboxEx)
            {
                _logger.LogError(inboxEx, "Failed to update inbox message status to failed for event {EventId}", 
                    domainEvent.EventId);
            }

            throw; // Re-throw to mark the outbox message processing as failed
        }
    }
}
