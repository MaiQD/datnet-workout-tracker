using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Moq;
using dotFitness.Modules.Users.Application.Commands;
using dotFitness.Modules.Users.Application.DTOs;
using dotFitness.Modules.Users.Application.Mappers;
using dotFitness.Modules.Users.Domain.Entities;
using dotFitness.Modules.Users.Infrastructure.Data;
using dotFitness.Modules.Users.Infrastructure.Handlers;
using dotFitness.SharedKernel.Results;
using dotFitness.SharedKernel.Tests.PostgreSQL;

namespace dotFitness.Modules.Users.Tests.Infrastructure.Handlers;

public class UpdateUserProfileCommandHandlerTests : IAsyncLifetime
{
    private readonly PostgreSqlFixture _fixture;
    private readonly UserMapper _userMapper;
    private readonly ILogger<UpdateUserProfileCommandHandler> _logger;
    private UsersDbContext _context = null!;
    private UpdateUserProfileCommandHandler _handler = null!;

    public UpdateUserProfileCommandHandlerTests()
    {
        _fixture = PostgreSqlFixture.Instance;
        _userMapper = new UserMapper();
        _logger = new Mock<ILogger<UpdateUserProfileCommandHandler>>().Object;
    }

    public async Task InitializeAsync()
    {
        await _fixture.InitializeAsync();
        _context = _fixture.CreateInMemoryDbContext<UsersDbContext>();
        await _context.Database.EnsureCreatedAsync();
        
        _handler = new UpdateUserProfileCommandHandler(
            _context,
            _userMapper,
            _logger
        );
    }

    public async Task DisposeAsync()
    {
        await _context.DisposeAsync();
        await _fixture.DisposeAsync();
    }

    [Fact]
    public async Task Should_Handle_Valid_Command_Successfully()
    {
        // Arrange
        var existingUser = new User
        {
            Id = 1,
            Email = "test@example.com",
            DisplayName = "Original Name",
            Gender = Gender.Female,
            DateOfBirth = new DateTime(1985, 5, 5),
            UnitPreference = UnitPreference.Metric,
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            UpdatedAt = DateTime.UtcNow.AddDays(-1)
        };

        _context.Users.Add(existingUser);
        await _context.SaveChangesAsync();

        var command = new UpdateUserProfileCommand(
            existingUser.Id,
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
        result.Value.DisplayName.Should().Be("Updated Name");
        result.Value.Gender.Should().Be("Male");
        result.Value.DateOfBirth.Should().Be(new DateTime(1990, 1, 1));
        result.Value.UnitPreference.Should().Be("Imperial");

        // Verify the user was updated in the database
        var updatedUser = await _context.Users.FirstAsync(u => u.Id == existingUser.Id);
        updatedUser.DisplayName.Should().Be("Updated Name");
        updatedUser.Gender.Should().Be(Gender.Male);
        updatedUser.DateOfBirth.Should().Be(new DateTime(1990, 1, 1));
        updatedUser.UnitPreference.Should().Be(UnitPreference.Imperial);
        updatedUser.UpdatedAt.Should().BeAfter(existingUser.UpdatedAt);

        // Verify outbox message was created
        var outboxMessages = await _context.OutboxMessages.ToListAsync();
        outboxMessages.Should().HaveCount(1);
        outboxMessages[0].EventType.Should().Be("UserProfileUpdatedEvent");
        outboxMessages[0].IsProcessed.Should().BeFalse();
    }

    [Fact]
    public async Task Should_Return_NotFound_When_User_Does_Not_Exist()
    {
        // Arrange
        var command = new UpdateUserProfileCommand(
            999, // Non-existent user ID
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

        // Verify no outbox message was created
        var outboxMessages = await _context.OutboxMessages.ToListAsync();
        outboxMessages.Should().BeEmpty();
    }

    [Fact]
    public async Task Should_Handle_Repository_Update_Errors_Gracefully()
    {
        // Arrange
        var existingUser = new User
        {
            Id = 1,
            Email = "test@example.com",
            DisplayName = "Original Name",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Users.Add(existingUser);
        await _context.SaveChangesAsync();

        // Dispose context to simulate database error
        await _context.DisposeAsync();

        var command = new UpdateUserProfileCommand(
            existingUser.Id,
            new UpdateUserProfileRequest
            {
                DisplayName = "Updated Name",
                Gender = null,
                DateOfBirth = null,
                UnitPreference = UnitPreference.Metric
            }
        );

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("User management failed");
    }

    [Fact]
    public async Task Should_Update_Only_Provided_Fields()
    {
        // Arrange
        var existingUser = new User
        {
            Id = 1,
            Email = "test@example.com",
            DisplayName = "Original Name",
            Gender = Gender.Male, // Should remain unchanged
            DateOfBirth = new DateTime(1985, 5, 15), // Should remain unchanged
            UnitPreference = UnitPreference.Metric,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Users.Add(existingUser);
        await _context.SaveChangesAsync();

        var command = new UpdateUserProfileCommand(
            existingUser.Id,
            new UpdateUserProfileRequest
            {
                DisplayName = "New Name",
                Gender = null, // Not updating gender
                DateOfBirth = null, // Not updating date of birth
                UnitPreference = UnitPreference.Imperial // Updating unit preference
            }
        );

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.DisplayName.Should().Be("New Name");
        result.Value.Gender.Should().Be(nameof(Gender.Male)); // Should remain unchanged
        result.Value.DateOfBirth.Should().Be(new DateTime(1985, 5, 15)); // Should remain unchanged
        result.Value.UnitPreference.Should().Be(nameof(UnitPreference.Imperial)); // Should be updated

        // Verify the user was updated in the database
        var updatedUser = await _context.Users.FirstAsync(u => u.Id == existingUser.Id);
        updatedUser.DisplayName.Should().Be("New Name");
        updatedUser.Gender.Should().Be(Gender.Male); // Unchanged
        updatedUser.DateOfBirth.Should().Be(new DateTime(1985, 5, 15)); // Unchanged
        updatedUser.UnitPreference.Should().Be(UnitPreference.Imperial); // Updated
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task Should_Not_Update_Display_Name_When_Invalid(string? invalidDisplayName)
    {
        // Arrange
        var existingUser = new User
        {
            Id = 1,
            Email = "test@example.com",
            DisplayName = "Original Name",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Users.Add(existingUser);
        await _context.SaveChangesAsync();

        var command = new UpdateUserProfileCommand(
            existingUser.Id,
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
        result.Value!.DisplayName.Should().Be("Original Name"); // Should remain unchanged

        // Verify the user was not updated in the database
        var userInDb = await _context.Users.FirstAsync(u => u.Id == existingUser.Id);
        userInDb.DisplayName.Should().Be("Original Name");
    }
}