using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using dotFitness.Modules.Users.Application.DTOs;
using dotFitness.Modules.Users.Application.Mappers;
using dotFitness.Modules.Users.Application.Queries;
using dotFitness.Modules.Users.Domain.Entities;
using dotFitness.Modules.Users.Domain.Repositories;
using dotFitness.Modules.Users.Infrastructure.Handlers;
using dotFitness.SharedKernel.Results;

namespace dotFitness.Modules.Users.Tests.Infrastructure.Handlers;

public class GetUserByIdQueryHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly UserMapper _userMapper;
    private readonly GetUserByIdQueryHandler _handler;

    public GetUserByIdQueryHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _userMapper = new UserMapper();
        var loggerMock = new Mock<ILogger<GetUserByIdQueryHandler>>();

        _handler = new GetUserByIdQueryHandler(
            _userRepositoryMock.Object,
            _userMapper,
            loggerMock.Object
        );
    }

    [Fact]
    public async Task Should_Return_User_When_Found()
    {
        // Arrange
        var query = new GetUserByIdQuery("user123");
        var user = new User
        {
            Id = "user123",
            Email = "test@example.com",
            DisplayName = "Test User",
            Gender = Gender.Male,
            UnitPreference = UnitPreference.Metric
        };

        var expectedDto = new UserDto
        {
            Id = "user123",
            Email = "test@example.com",
            DisplayName = "Test User",
            Gender = nameof(Gender.Male),
            DateOfBirth = null,
            UnitPreference = nameof(UnitPreference.Metric),
            Roles = ["User"],
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync("user123", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(user));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Id.Should().Be("user123");
        result.Value.Email.Should().Be("test@example.com");
        result.Value.DisplayName.Should().Be("Test User");

        _userRepositoryMock.Verify(x => x.GetByIdAsync("user123", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Should_Return_NotFound_When_User_DoesNot_Exist()
    {
        // Arrange
        var query = new GetUserByIdQuery("nonexistent");

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync("nonexistent", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<User>("User not found"));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("User not found");

        _userRepositoryMock.Verify(x => x.GetByIdAsync("nonexistent", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Should_Handle_Repository_Errors_Gracefully()
    {
        // Arrange
        var query = new GetUserByIdQuery("user123");

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync("user123", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<User>("Database connection failed"));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("User not found");

        _userRepositoryMock.Verify(x => x.GetByIdAsync("user123", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task Should_Handle_Invalid_User_Id(string? invalidId)
    {
        // Arrange
        var query = new GetUserByIdQuery(invalidId!);

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(invalidId!, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<User>("Invalid user ID"));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNullOrEmpty();
    }
}