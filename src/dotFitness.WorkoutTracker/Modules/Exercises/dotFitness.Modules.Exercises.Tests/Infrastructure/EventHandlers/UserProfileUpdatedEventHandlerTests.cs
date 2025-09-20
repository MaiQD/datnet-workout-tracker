using FluentAssertions;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Moq;
using dotFitness.Modules.Exercises.Domain.Entities;
using dotFitness.Modules.Exercises.Infrastructure.EventHandlers;
using dotFitness.SharedKernel.Events;
using dotFitness.SharedKernel.Inbox;
using dotFitness.SharedKernel.Tests.MongoDB;

namespace dotFitness.Modules.Exercises.Tests.Infrastructure.EventHandlers;

public class UserProfileUpdatedEventHandlerTests : IClassFixture<MongoDbFixture>
{
    private readonly MongoDbFixture _mongoFixture;
    private readonly ILogger<UserProfileUpdatedEventHandler> _logger;
    private readonly IMongoCollection<InboxMessage> _inboxCollection;
    private readonly IMongoCollection<UserPreferencesProjection> _userPreferencesCollection;
    private readonly UserProfileUpdatedEventHandler _handler;

    public UserProfileUpdatedEventHandlerTests(MongoDbFixture mongoFixture)
    {
        _mongoFixture = mongoFixture;
        _logger = new Mock<ILogger<UserProfileUpdatedEventHandler>>().Object;
        
        _inboxCollection = _mongoFixture.Database.GetCollection<InboxMessage>("Inbox_Exercises");
        _userPreferencesCollection = _mongoFixture.Database.GetCollection<UserPreferencesProjection>("UserPreferences");
        
        _handler = new UserProfileUpdatedEventHandler(
            _userPreferencesCollection,
            _inboxCollection,
            _logger
        );
    }

    [Fact]
    public async Task Should_Handle_UserProfileUpdatedEvent_Successfully()
    {
        // Arrange
        var userProfileUpdatedEvent = new UserProfileUpdatedEvent(
            userId: 123,
            displayName: "Updated User",
            gender: "Male",
            dateOfBirth: new DateTime(1990, 1, 1),
            unitPreference: "Imperial",
            updatedAt: DateTime.UtcNow,
            correlationId: Guid.NewGuid().ToString(),
            traceId: Guid.NewGuid().ToString()
        );

        // Act
        await _handler.HandleAsync(userProfileUpdatedEvent, CancellationToken.None);

        // Assert
        // Verify inbox message was created
        var inboxMessages = await _inboxCollection
            .Find(m => m.EventId == userProfileUpdatedEvent.EventId.ToString())
            .ToListAsync();
        
        inboxMessages.Should().HaveCount(1);
        var inboxMessage = inboxMessages[0];
        inboxMessage.Consumer.Should().Be("Exercises.UserProfileUpdatedHandler");
        inboxMessage.Status.Should().Be("completed");
        inboxMessage.ProcessedAt.Should().NotBeNull();

        // Verify user preferences projection was updated
        var userPreferences = await _userPreferencesCollection
            .Find(up => up.UserId == userProfileUpdatedEvent.UserId)
            .ToListAsync();
        
        userPreferences.Should().HaveCount(1);
        var preferences = userPreferences[0];
        preferences.UserId.Should().Be(userProfileUpdatedEvent.UserId);
        preferences.UpdatedAt.Should().BeCloseTo(userProfileUpdatedEvent.UpdatedAt, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task Should_Skip_Processing_When_Event_Already_Processed()
    {
        // Arrange
        var userProfileUpdatedEvent = new UserProfileUpdatedEvent(
            userId: 456,
            displayName: "Test User",
            gender: "Female",
            dateOfBirth: new DateTime(1985, 5, 5),
            unitPreference: "Metric",
            updatedAt: DateTime.UtcNow,
            correlationId: Guid.NewGuid().ToString(),
            traceId: Guid.NewGuid().ToString()
        );

        // Create an existing processed inbox message
        var existingInboxMessage = new InboxMessage
        {
            EventId = userProfileUpdatedEvent.EventId.ToString(),
            Consumer = "Exercises.UserProfileUpdatedHandler",
            EventType = "UserProfileUpdatedEvent",
            Status = "completed",
            ProcessedAt = DateTime.UtcNow.AddMinutes(-10),
            CorrelationId = userProfileUpdatedEvent.CorrelationId,
            TraceId = userProfileUpdatedEvent.TraceId,
            CreatedAt = DateTime.UtcNow.AddMinutes(-10)
        };

        await _inboxCollection.InsertOneAsync(existingInboxMessage);

        // Count initial user preferences
        var initialCount = await _userPreferencesCollection.CountDocumentsAsync(_ => true);

        // Act
        await _handler.HandleAsync(userProfileUpdatedEvent, CancellationToken.None);

        // Assert
        // Verify no additional inbox message was created
        var inboxMessages = await _inboxCollection
            .Find(m => m.EventId == userProfileUpdatedEvent.EventId.ToString())
            .ToListAsync();
        
        inboxMessages.Should().HaveCount(1); // Still only the original one

        // Verify no additional user preferences were created
        var finalCount = await _userPreferencesCollection.CountDocumentsAsync(_ => true);
        finalCount.Should().Be(initialCount);
    }

    [Fact]
    public async Task Should_Update_Existing_User_Preferences_Projection()
    {
        // Arrange
        var userId = 789;
        var existingPreferences = new UserPreferencesProjection
        {
            UserId = userId,
            FocusMuscleGroupIds = ["64a7b2e1c5d6f8a9b1c2d3e4", "64a7b2e1c5d6f8a9b1c2d3e5"],
            AvailableEquipmentIds = ["64a7b2e1c5d6f8a9b1c2d3e6"],
            UpdatedAt = DateTime.UtcNow.AddDays(-1)
        };

        await _userPreferencesCollection.InsertOneAsync(existingPreferences);

        var userProfileUpdatedEvent = new UserProfileUpdatedEvent(
            userId: userId,
            displayName: "Updated Name",
            gender: "Male",
            dateOfBirth: new DateTime(1992, 12, 12),
            unitPreference: "Imperial",
            updatedAt: DateTime.UtcNow,
            correlationId: Guid.NewGuid().ToString(),
            traceId: Guid.NewGuid().ToString()
        );

        // Act
        await _handler.HandleAsync(userProfileUpdatedEvent, CancellationToken.None);

        // Assert
        // Verify user preferences were updated (not duplicated)
        var userPreferences = await _userPreferencesCollection
            .Find(up => up.UserId == userId)
            .ToListAsync();
        
        userPreferences.Should().HaveCount(1);
        var preferences = userPreferences[0];
        preferences.UserId.Should().Be(userId);
        preferences.UpdatedAt.Should().BeCloseTo(userProfileUpdatedEvent.UpdatedAt, TimeSpan.FromSeconds(1));
        
        // Verify existing data is preserved
        preferences.FocusMuscleGroupIds.Should().Equal(["64a7b2e1c5d6f8a9b1c2d3e4", "64a7b2e1c5d6f8a9b1c2d3e5"]);
        preferences.AvailableEquipmentIds.Should().Equal(["64a7b2e1c5d6f8a9b1c2d3e6"]);
    }

    [Fact]
    public async Task Should_Handle_Errors_And_Mark_Inbox_As_Failed()
    {
        // Arrange
        var userProfileUpdatedEvent = new UserProfileUpdatedEvent(
            userId: 999,
            displayName: "Error User",
            gender: "Male",
            dateOfBirth: new DateTime(1980, 1, 1),
            unitPreference: "Metric",
            updatedAt: DateTime.UtcNow,
            correlationId: Guid.NewGuid().ToString(),
            traceId: Guid.NewGuid().ToString()
        );

        // Create a mock that will throw an exception during user preferences update
        var mockUserPreferencesCollection = new Mock<IMongoCollection<UserPreferencesProjection>>();
        mockUserPreferencesCollection
            .Setup(c => c.UpdateOneAsync(
                It.IsAny<FilterDefinition<UserPreferencesProjection>>(),
                It.IsAny<UpdateDefinition<UserPreferencesProjection>>(),
                It.IsAny<UpdateOptions>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new MongoException("Simulated database error"));

        var handlerWithMock = new UserProfileUpdatedEventHandler(
            mockUserPreferencesCollection.Object,
            _inboxCollection,
            _logger
        );

        // Act
        var act = async () => await handlerWithMock.HandleAsync(userProfileUpdatedEvent, CancellationToken.None);
        
        // The handler should throw the exception after marking inbox as failed
        await act.Should().ThrowAsync<MongoException>();

        // Assert
        // Verify inbox message was marked as failed
        var inboxMessages = await _inboxCollection
            .Find(m => m.EventId == userProfileUpdatedEvent.EventId.ToString())
            .ToListAsync();
        
        inboxMessages.Should().HaveCount(1);
        var inboxMessage = inboxMessages[0];
        inboxMessage.Consumer.Should().Be("Exercises.UserProfileUpdatedHandler");
        inboxMessage.Status.Should().Be("failed");
        inboxMessage.Error.Should().Contain("Simulated database error");
        inboxMessage.ProcessedAt.Should().NotBeNull();
    }

    [Theory]
    [InlineData(100, "John Doe", "Male", "Imperial")]
    [InlineData(200, "Jane Smith", "Female", "Metric")]
    [InlineData(300, "Alex Johnson", null, "Imperial")]
    public async Task Should_Handle_Various_User_Profile_Data(int userId, string displayName, string? gender, string unitPreference)
    {
        // Arrange
        var userProfileUpdatedEvent = new UserProfileUpdatedEvent(
            userId: userId,
            displayName: displayName,
            gender: gender,
            dateOfBirth: new DateTime(1990, 6, 15),
            unitPreference: unitPreference,
            updatedAt: DateTime.UtcNow,
            correlationId: Guid.NewGuid().ToString(),
            traceId: Guid.NewGuid().ToString()
        );

        // Act
        await _handler.HandleAsync(userProfileUpdatedEvent, CancellationToken.None);

        // Assert
        // Verify processing completed successfully
        var inboxMessages = await _inboxCollection
            .Find(m => m.EventId == userProfileUpdatedEvent.EventId.ToString())
            .ToListAsync();
        
        inboxMessages.Should().HaveCount(1);
        inboxMessages[0].Status.Should().Be("completed");

        // Verify user preferences projection was created/updated
        var userPreferences = await _userPreferencesCollection
            .Find(up => up.UserId == userId)
            .ToListAsync();
        
        userPreferences.Should().HaveCount(1);
        userPreferences[0].UserId.Should().Be(userId);
    }
}
