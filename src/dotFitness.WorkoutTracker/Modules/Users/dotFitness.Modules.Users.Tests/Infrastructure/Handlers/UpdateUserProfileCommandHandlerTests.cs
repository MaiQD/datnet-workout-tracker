using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using dotFitness.Modules.Users.Application.Commands;
using dotFitness.Modules.Users.Application.DTOs;
using dotFitness.Modules.Users.Application.Mappers;
using dotFitness.Modules.Users.Domain.Entities;
using dotFitness.Modules.Users.Domain.Repositories;
using dotFitness.Modules.Users.Infrastructure.Handlers;
using dotFitness.SharedKernel.Results;

namespace dotFitness.Modules.Users.Tests.Infrastructure.Handlers;

public class UpdateUserProfileCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly UserMapper _userMapper;
    private readonly UpdateUserProfileCommandHandler _handler;

    public UpdateUserProfileCommandHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _userMapper = new UserMapper();
        var loggerMock = new Mock<ILogger<UpdateUserProfileCommandHandler>>();

        _handler = new UpdateUserProfileCommandHandler(
            _userRepositoryMock.Object,
            _userMapper,
            loggerMock.Object
        );
    }

    [Fact]
    public async Task Should_Handle_Valid_Command_Successfully()
    {
        // Arrange
        var command = new UpdateUserProfileCommand(
            userId: "user123",
            displayName: "Updated Name",
            gender: nameof(Gender.Male),
            dateOfBirth: new DateTime(1990, 1, 1),
            unitPreference: nameof(UnitPreference.Imperial)
        );

        var existingUser = new User
        {
            Id = "user123",
            Email = "test@example.com",
            DisplayName = "Original Name",
            Gender = Gender.Female,
            UnitPreference = UnitPreference.Metric
        };

        var updatedUser = new User
        {
            Id = "user123",
            Email = "test@example.com",
            DisplayName = "Updated Name",
            Gender = Gender.Male,
            DateOfBirth = new DateTime(1990, 1, 1),
            UnitPreference = UnitPreference.Imperial
        };

        var expectedDto = new UserDto
        {
            Id = "user123",
            Email = "test@example.com",
            DisplayName = "Updated Name",
            Gender = nameof(Gender.Male),
            DateOfBirth = new DateTime(1990, 1, 1),
            UnitPreference = nameof(UnitPreference.Imperial),
            Roles = new List<string> { "User" },
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync("user123", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(existingUser));

        _userRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(updatedUser));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.DisplayName.Should().Be("Updated Name");
        result.Value.Gender.Should().Be(nameof(Gender.Male));
        result.Value.UnitPreference.Should().Be(nameof(UnitPreference.Imperial));

        _userRepositoryMock.Verify(x => x.GetByIdAsync("user123", It.IsAny<CancellationToken>()), Times.Once);
        _userRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Should_Return_NotFound_When_User_Does_Not_Exist()
    {
        // Arrange
        var command = new UpdateUserProfileCommand(
            userId: "nonexistent",
            displayName: "New Name",
            gender: null,
            dateOfBirth: null,
            unitPreference: string.Empty
        );

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync("nonexistent", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<User>("User not found"));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("User not found");

        _userRepositoryMock.Verify(x => x.GetByIdAsync("nonexistent", It.IsAny<CancellationToken>()), Times.Once);
        _userRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Should_Handle_Repository_Update_Errors_Gracefully()
    {
        // Arrange
        var command = new UpdateUserProfileCommand(
            userId: "user123",
            displayName: "Updated Name",
            gender: null,
            dateOfBirth: null);

        var existingUser = new User
        {
            Id = "user123",
            Email = "test@example.com",
            DisplayName = "Original Name"
        };

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync("user123", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(existingUser));

        _userRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<User>("Database update failed"));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Database update failed");

        _userRepositoryMock.Verify(x => x.GetByIdAsync("user123", It.IsAny<CancellationToken>()), Times.Once);
        _userRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Should_Update_Only_Provided_Fields()
    {
        // Arrange
        var command = new UpdateUserProfileCommand(
            userId: "user123",
            displayName: "New Name",
            gender: null, // Not updating gender
            dateOfBirth: null, // Not updating date of birth
            unitPreference: nameof(UnitPreference.Imperial) // Updating unit preference
        );

        var existingUser = new User
        {
            Id = "user123",
            Email = "test@example.com",
            DisplayName = "Original Name",
            Gender = Gender.Male, // Should remain unchanged
            DateOfBirth = new DateTime(1985, 5, 15), // Should remain unchanged
            UnitPreference = UnitPreference.Metric
        };

        var updatedUser = new User
        {
            Id = "user123",
            Email = "test@example.com",
            DisplayName = "New Name",
            Gender = Gender.Male, // Unchanged
            DateOfBirth = new DateTime(1985, 5, 15), // Unchanged
            UnitPreference = UnitPreference.Imperial // Updated
        };

        var expectedDto = new UserDto
        {
            Id = "user123",
            Email = "test@example.com",
            DisplayName = "New Name",
            Gender = nameof(Gender.Male),
            DateOfBirth = new DateTime(1985, 5, 15),
            UnitPreference = nameof(UnitPreference.Imperial),
            Roles = new List<string> { "User" },
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync("user123", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(existingUser));

        _userRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(updatedUser));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.DisplayName.Should().Be("New Name");
        result.Value.Gender.Should().Be(nameof(Gender.Male)); // Should remain unchanged
        result.Value.DateOfBirth.Should().Be(new DateTime(1985, 5, 15)); // Should remain unchanged
        result.Value.UnitPreference.Should().Be(nameof(UnitPreference.Imperial)); // Should be updated
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task Should_Not_Update_Display_Name_When_Invalid(string? invalidDisplayName)
    {
        // Arrange
        var command = new UpdateUserProfileCommand(
            userId: "user123",
            displayName: invalidDisplayName,
            gender: null,
            dateOfBirth: null
        );

        var existingUser = new User
        {
            Id = "user123",
            Email = "test@example.com",
            DisplayName = "Original Name"
        };

        var expectedDto = new UserDto
        {
            Id = "user123",
            Email = "test@example.com",
            DisplayName = "Original Name", // Should remain unchanged
            Gender = null,
            DateOfBirth = null,
            UnitPreference = nameof(UnitPreference.Metric),
            Roles = ["User"],
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync("user123", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(existingUser));

        _userRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(existingUser));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.DisplayName.Should().Be("Original Name");
    }
}