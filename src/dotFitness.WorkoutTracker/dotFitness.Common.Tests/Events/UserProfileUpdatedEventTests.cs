using dotFitness.Common.Events;
using FluentAssertions;

namespace dotFitness.Common.Tests.Events;

public class UserProfileUpdatedEventTests
{
    [Fact]
    public void Should_Create_UserProfileUpdatedEvent_With_Required_Properties()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var displayName = "Test User";
        var gender = "Male";
        var dateOfBirth = new DateTime(1990, 1, 1);
        var unitPreference = "Metric";
        var updatedAt = DateTime.UtcNow;

        // Act
        var @event = new UserProfileUpdatedEvent(
            userId,
            displayName,
            gender,
            dateOfBirth,
            unitPreference,
            updatedAt);

        // Assert
        @event.Should().NotBeNull();
        @event.UserId.Should().Be(userId);
        @event.DisplayName.Should().Be(displayName);
        @event.Gender.Should().Be(gender);
        @event.DateOfBirth.Should().Be(dateOfBirth);
        @event.UnitPreference.Should().Be(unitPreference);
        @event.UpdatedAt.Should().Be(updatedAt);
    }

    [Fact]
    public void Should_Create_UserProfileUpdatedEvent_With_Correlation_And_Trace_Id()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var displayName = "Test User";
        var gender = "Male";
        var dateOfBirth = new DateTime(1990, 1, 1);
        var unitPreference = "Metric";
        var updatedAt = DateTime.UtcNow;
        var correlationId = "correlation123";
        var traceId = "trace456";

        // Act
        var @event = new UserProfileUpdatedEvent(
            userId,
            displayName,
            gender,
            dateOfBirth,
            unitPreference,
            updatedAt,
            correlationId,
            traceId);

        // Assert
        @event.Should().NotBeNull();
        @event.CorrelationId.Should().Be(correlationId);
        @event.TraceId.Should().Be(traceId);
    }

    [Fact]
    public void Should_Create_UserProfileUpdatedEvent_With_Null_Gender()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var displayName = "Test User";
        string? gender = null;
        var dateOfBirth = new DateTime(1990, 1, 1);
        var unitPreference = "Metric";
        var updatedAt = DateTime.UtcNow;

        // Act
        var @event = new UserProfileUpdatedEvent(
            userId,
            displayName,
            gender,
            dateOfBirth,
            unitPreference,
            updatedAt);

        // Assert
        @event.Should().NotBeNull();
        @event.Gender.Should().BeNull();
    }

    [Fact]
    public void Should_Create_UserProfileUpdatedEvent_With_Null_DateOfBirth()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var displayName = "Test User";
        var gender = "Male";
        DateTime? dateOfBirth = null;
        var unitPreference = "Metric";
        var updatedAt = DateTime.UtcNow;

        // Act
        var @event = new UserProfileUpdatedEvent(
            userId,
            displayName,
            gender,
            dateOfBirth,
            unitPreference,
            updatedAt);

        // Assert
        @event.Should().NotBeNull();
        @event.DateOfBirth.Should().BeNull();
    }

    [Fact]
    public void Should_Inherit_From_BaseDomainEvent()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var displayName = "Test User";
        var gender = "Male";
        var dateOfBirth = new DateTime(1990, 1, 1);
        var unitPreference = "Metric";
        var updatedAt = DateTime.UtcNow;

        // Act
        var @event = new UserProfileUpdatedEvent(
            userId,
            displayName,
            gender,
            dateOfBirth,
            unitPreference,
            updatedAt);

        // Assert
        @event.Should().BeAssignableTo<BaseDomainEvent>();
    }
}