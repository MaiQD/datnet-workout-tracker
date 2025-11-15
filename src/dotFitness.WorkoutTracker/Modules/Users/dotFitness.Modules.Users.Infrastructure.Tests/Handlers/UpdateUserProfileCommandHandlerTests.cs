using dotFitness.Common.Events;
using dotFitness.Common.Results;
using dotFitness.Modules.Users.Application.Commands;
using dotFitness.Modules.Users.Application.Services;
using dotFitness.Modules.Users.Domain.Entities;
using dotFitness.Modules.Users.Domain.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace dotFitness.Modules.Users.Infrastructure.Tests.Handlers;

public class UpdateUserProfileCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<IOutboxPublisher> _outboxPublisherMock = new();
    private readonly Mock<ILogger<UpdateUserProfileCommandHandler>> _loggerMock = new();
    private readonly UpdateUserProfileCommandHandler _handler;

    public UpdateUserProfileCommandHandlerTests()
    {
        _handler = new UpdateUserProfileCommandHandler(
            _userRepositoryMock.Object,
            _outboxPublisherMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task Should_Handle_Valid_Command_Successfully()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var existingUser = new ApplicationUser
        {
            Id = userId,
            Email = "test@example.com",
            UserName = "test@example.com",
            DisplayName = "Original Name",
            Gender = Gender.Female,
            DateOfBirth = new DateTime(1985, 5, 5),
            UnitPreference = UnitPreference.Metric,
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            UpdatedAt = DateTime.UtcNow.AddDays(-1)
        };

        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(existingUser));

        var updatedUser = new ApplicationUser
        {
            Id = userId,
            Email = existingUser.Email,
            UserName = existingUser.UserName,
            DisplayName = "Updated Name",
            Gender = Gender.Male,
            DateOfBirth = new DateTime(1990, 1, 1),
            UnitPreference = UnitPreference.Imperial,
            CreatedAt = existingUser.CreatedAt,
            UpdatedAt = DateTime.UtcNow
        };

        _userRepositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<ApplicationUser>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(updatedUser));

        var command = new UpdateUserProfileCommand(
            userId,
            new UpdateUserProfileRequest
            {
                DisplayName = "Updated Name",
                Gender = Gender.Male,
                DateOfBirth = new DateTime(1990, 1, 1),
                UnitPreference = UnitPreference.Imperial
            }
        );

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.DisplayName.Should().Be("Updated Name");
        result.Value.Gender.Should().Be("Male");
        result.Value.DateOfBirth.Should().Be(new DateTime(1990, 1, 1));
        result.Value.UnitPreference.Should().Be("Imperial");

        _userRepositoryMock.Verify(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
        _userRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<ApplicationUser>(), It.IsAny<CancellationToken>()), Times.Once);
        _outboxPublisherMock.Verify(p => p.PublishAsync(It.IsAny<UserProfileUpdatedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task Should_Return_NotFound_When_User_Does_Not_Exist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<ApplicationUser>("User not found"));

        var command = new UpdateUserProfileCommand(
            userId,
            new UpdateUserProfileRequest
            {
                DisplayName = "New Name",
                Gender = null,
                DateOfBirth = null,
                UnitPreference = UnitPreference.Metric
            }
        );

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("User not found.");

        _userRepositoryMock.Verify(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
        _userRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<ApplicationUser>(), It.IsAny<CancellationToken>()), Times.Never);
        _outboxPublisherMock.Verify(p => p.PublishAsync(It.IsAny<UserProfileUpdatedEvent>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task Should_Update_Only_Provided_Fields()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var existingUser = new ApplicationUser
        {
            Id = userId,
            Email = "test@example.com",
            UserName = "test@example.com",
            DisplayName = "Original Name",
            Gender = Gender.Male,
            DateOfBirth = new DateTime(1985, 5, 15),
            UnitPreference = UnitPreference.Metric,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(existingUser));

        var updatedUser = new ApplicationUser
        {
            Id = userId,
            Email = existingUser.Email,
            UserName = existingUser.UserName,
            DisplayName = "New Name",
            Gender = Gender.Male, // Unchanged
            DateOfBirth = new DateTime(1985, 5, 15), // Unchanged
            UnitPreference = UnitPreference.Imperial, // Updated
            CreatedAt = existingUser.CreatedAt,
            UpdatedAt = DateTime.UtcNow
        };

        _userRepositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<ApplicationUser>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(updatedUser));

        var command = new UpdateUserProfileCommand(
            userId,
            new UpdateUserProfileRequest
            {
                DisplayName = "New Name",
                Gender = null,
                DateOfBirth = null,
                UnitPreference = UnitPreference.Imperial
            }
        );

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.DisplayName.Should().Be("New Name");
        result.Value.Gender.Should().Be(nameof(Gender.Male));
        result.Value.DateOfBirth.Should().Be(new DateTime(1985, 5, 15));
        result.Value.UnitPreference.Should().Be(nameof(UnitPreference.Imperial));
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task Should_Not_Update_Display_Name_When_Invalid(string? invalidDisplayName)
    {
        // Arrange
        var userId = Guid.NewGuid();
        var existingUser = new ApplicationUser
        {
            Id = userId,
            Email = "test@example.com",
            UserName = "test@example.com",
            DisplayName = "Original Name",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(existingUser));

        var command = new UpdateUserProfileCommand(
            userId,
            new UpdateUserProfileRequest
            {
                DisplayName = invalidDisplayName!,
                Gender = null,
                DateOfBirth = null,
                UnitPreference = UnitPreference.Metric
            }
        );

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.DisplayName.Should().Be("Original Name");
    }
}
