using MongoDB.Driver;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Microsoft.Extensions.Options;
using dotFitness.SharedKernel.Outbox;
using dotFitness.SharedKernel.Events;
using dotFitness.Modules.Users.Infrastructure.Data;
using dotFitness.Api.Infrastructure.Settings;

namespace dotFitness.Api.Infrastructure.Services;

/// <summary>
/// Background service that processes outbox messages from both MongoDB and PostgreSQL
/// and publishes events to event handlers for cross-module communication
/// </summary>
public class OutboxProcessorService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OutboxProcessorService> _logger;
    private readonly OutboxProcessorSettings _settings;
    private readonly TimeSpan _processingInterval;

    public OutboxProcessorService(
        IServiceProvider serviceProvider,
        ILogger<OutboxProcessorService> logger,
        IOptions<OutboxProcessorSettings> settings)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _settings = settings.Value;
        
        // Configure processing interval
        _processingInterval = TimeSpan.FromSeconds(_settings.IntervalSeconds);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Outbox Processor Service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessOutboxMessagesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing outbox messages");
            }

            await Task.Delay(_processingInterval, stoppingToken);
        }

        _logger.LogInformation("Outbox Processor Service stopped");
    }

    private async Task ProcessOutboxMessagesAsync(CancellationToken cancellationToken)
    {
        // Process both MongoDB and PostgreSQL outbox messages
        await ProcessMongoDbOutboxAsync(cancellationToken);
        await ProcessPostgreSqlOutboxAsync(cancellationToken);
    }

    private async Task ProcessMongoDbOutboxAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var outboxCollection = scope.ServiceProvider.GetRequiredService<IMongoCollection<OutboxMessage>>();

            // Get unprocessed messages that haven't exceeded max retry count
            var filter = Builders<OutboxMessage>.Filter.And(
                Builders<OutboxMessage>.Filter.Eq(x => x.IsProcessed, false),
                Builders<OutboxMessage>.Filter.Lt(x => x.RetryCount, _settings.MaxRetryAttempts)
            );
            var sort = Builders<OutboxMessage>.Sort.Ascending(x => x.CreatedAt);
            
            var unprocessedMessages = await outboxCollection
                .Find(filter)
                .Sort(sort)
                .Limit(_settings.BatchSize)
                .ToListAsync(cancellationToken);

            if (!unprocessedMessages.Any())
                return;

            _logger.LogDebug("Processing {Count} MongoDB outbox messages", unprocessedMessages.Count);

            foreach (var message in unprocessedMessages)
            {
                try
                {
                    await ProcessEventAsync(message.EventType, message.EventData, scope.ServiceProvider, cancellationToken);

                    // Mark as processed
                    var updateFilter = Builders<OutboxMessage>.Filter.Eq(x => x.Id, message.Id);
                    var update = Builders<OutboxMessage>.Update
                        .Set(x => x.IsProcessed, true)
                        .Set(x => x.ProcessedAt, DateTime.UtcNow);

                    await outboxCollection.UpdateOneAsync(updateFilter, update, cancellationToken: cancellationToken);

                    _logger.LogDebug("Successfully processed MongoDB outbox message {MessageId} of type {EventType}", 
                        message.Id, message.EventType);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to process MongoDB outbox message {MessageId} of type {EventType}: {Error}", 
                        message.Id, message.EventType, ex.Message);
                    
                    // Update retry count and error info
                    var updateFilter = Builders<OutboxMessage>.Filter.Eq(x => x.Id, message.Id);
                    var errorMessage = ex.Message.Length > 1000 ? ex.Message.Substring(0, 1000) : ex.Message;
                    
                    UpdateDefinition<OutboxMessage> update;
                    if (message.RetryCount + 1 >= _settings.MaxRetryAttempts)
                    {
                        // Mark as processed to prevent infinite retries
                        update = Builders<OutboxMessage>.Update
                            .Set(x => x.RetryCount, message.RetryCount + 1)
                            .Set(x => x.LastError, errorMessage)
                            .Set(x => x.IsProcessed, true)
                            .Set(x => x.ProcessedAt, DateTime.UtcNow);
                            
                        _logger.LogError("MongoDB outbox message {MessageId} has permanently failed after {RetryCount} attempts and will no longer be retried. " +
                            "Event type: {EventType}, Last error: {Error}", 
                            message.Id, message.RetryCount + 1, message.EventType, errorMessage);
                    }
                    else
                    {
                        // Increment retry count for retry
                        update = Builders<OutboxMessage>.Update
                            .Set(x => x.RetryCount, message.RetryCount + 1)
                            .Set(x => x.LastError, errorMessage);
                            
                        _logger.LogWarning("MongoDB outbox message {MessageId} failed (attempt {RetryCount}/{MaxRetryCount}). " +
                            "Event type: {EventType}, Will retry. Error: {Error}", 
                            message.Id, message.RetryCount + 1, _settings.MaxRetryAttempts, message.EventType, ex.Message);
                    }
                    
                    await outboxCollection.UpdateOneAsync(updateFilter, update, cancellationToken: cancellationToken);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing MongoDB outbox messages: {Error}", ex.Message);
        }
    }

    private async Task ProcessPostgreSqlOutboxAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var usersContext = scope.ServiceProvider.GetRequiredService<UsersDbContext>();

            // Get unprocessed messages that haven't exceeded max retry count
            var unprocessedMessages = await usersContext.OutboxMessages
                .Where(x => !x.IsProcessed && x.RetryCount < _settings.MaxRetryAttempts)
                .OrderBy(x => x.CreatedAt)
                .Take(_settings.BatchSize)
                .ToListAsync(cancellationToken);

            if (!unprocessedMessages.Any())
                return;

            _logger.LogDebug("Processing {Count} PostgreSQL outbox messages", unprocessedMessages.Count);

            foreach (var message in unprocessedMessages)
            {
                try
                {
                    await ProcessEventAsync(message.EventType, message.EventData, scope.ServiceProvider, cancellationToken);

                    // Mark as processed
                    message.IsProcessed = true;
                    message.ProcessedAt = DateTime.UtcNow;

                    await usersContext.SaveChangesAsync(cancellationToken);

                    _logger.LogDebug("Successfully processed PostgreSQL outbox message {MessageId} of type {EventType}", 
                        message.EventId, message.EventType);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to process PostgreSQL outbox message {MessageId} of type {EventType}: {Error}", 
                        message.EventId, message.EventType, ex.Message);
                    
                    // Update retry count and error info
                    message.RetryCount++;
                    message.LastError = ex.Message.Length > 1000 ? ex.Message.Substring(0, 1000) : ex.Message;
                    
                    // Stop retrying after reaching max attempts
                    if (message.RetryCount >= _settings.MaxRetryAttempts)
                    {
                        message.IsProcessed = true; // Mark as processed to prevent infinite retries
                        _logger.LogError("PostgreSQL outbox message {MessageId} has permanently failed after {RetryCount} attempts and will no longer be retried. " +
                            "Event type: {EventType}, Last error: {Error}", 
                            message.EventId, message.RetryCount, message.EventType, message.LastError);
                    }
                    else
                    {
                        _logger.LogWarning("PostgreSQL outbox message {MessageId} failed (attempt {RetryCount}/{MaxRetryCount}). " +
                            "Event type: {EventType}, Will retry. Error: {Error}", 
                            message.EventId, message.RetryCount, _settings.MaxRetryAttempts, message.EventType, ex.Message);
                    }

                    await usersContext.SaveChangesAsync(cancellationToken);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing PostgreSQL outbox messages: {Error}", ex.Message);
        }
    }

    private async Task ProcessEventAsync(string eventType, string eventData, IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        // Try to find and invoke the appropriate event handler
        var handlerType = typeof(IEventHandler<>);
        
        // Look for event types in all loaded assemblies
        var eventTypeObj = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .FirstOrDefault(t => t.Name == eventType && typeof(IDomainEvent).IsAssignableFrom(t));

        if (eventTypeObj == null)
        {
            _logger.LogWarning("Could not find event type {EventType} in loaded assemblies", eventType);
            return;
        }

        // Deserialize the event
        var domainEvent = JsonSerializer.Deserialize(eventData, eventTypeObj);
        if (domainEvent == null)
        {
            _logger.LogWarning("Could not deserialize event data for type {EventType}", eventType);
            return;
        }

        // Find all handlers for this event type
        var specificHandlerType = handlerType.MakeGenericType(eventTypeObj);
        var handlers = serviceProvider.GetServices(specificHandlerType);

        foreach (var handler in handlers.Where(p=> p != null))
        {
            try
            {
                
                var handleMethod = specificHandlerType.GetMethod("HandleAsync");
                if (handleMethod != null)
                {
                    var task = (Task?)handleMethod.Invoke(handler, [domainEvent, cancellationToken]);
                    if (task != null)
                    {
                        await task;
                        _logger.LogDebug("Successfully handled event {EventType} with handler {HandlerType}", 
                            eventType, handler!.GetType().Name);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling event {EventType} with handler {HandlerType}: {Error}", 
                    eventType, handler!.GetType().Name, ex.Message);
                throw; // Re-throw to mark the message as failed
            }
        }
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Outbox Processor Service is stopping...");
        return base.StopAsync(cancellationToken);
    }
}
