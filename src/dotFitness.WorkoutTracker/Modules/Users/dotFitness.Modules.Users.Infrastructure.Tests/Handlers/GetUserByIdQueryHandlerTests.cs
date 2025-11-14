using dotFitness.Common.Results;
using dotFitness.Modules.Users.Application.Queries;
using dotFitness.Modules.Users.Domain.Entities;
using dotFitness.Modules.Users.Domain.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace dotFitness.Modules.Users.Infrastructure.Tests.Handlers;

public class GetUserByIdQueryHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<ILogger<GetUserByIdQueryHandler>> _loggerMock = new();
    private readonly GetUserByIdQueryHandler _handler;

    public GetUserByIdQueryHandlerTests()
    {
        _handler = new GetUserByIdQueryHandler(
            _userRepositoryMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task Should_Return_User_When_Found()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new ApplicationUser
        {
            Id = userId,
            Email = "test@example.com",
            UserName = "test@example.com",
            DisplayName = "Test User",
            Gender = Gender.Male,
            UnitPreference = UnitPreference.Metric,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(user));

        var query = new GetUserByIdQuery(userId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Id.Should().Be(user.Id);
        result.Value.Email.Should().Be(user.Email);
        result.Value.DisplayName.Should().Be("Test User");

        _userRepositoryMock.Verify(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task Should_Return_NotFound_When_User_DoesNot_Exist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<ApplicationUser>("User not found"));

        var query = new GetUserByIdQuery(userId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("User not found");

        _userRepositoryMock.Verify(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task Should_Handle_Invalid_User_Id()
    {
        // Arrange
        var userId = Guid.Empty;
        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<ApplicationUser>("User not found"));

        var query = new GetUserByIdQuery(userId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("User not found");
    }
}
