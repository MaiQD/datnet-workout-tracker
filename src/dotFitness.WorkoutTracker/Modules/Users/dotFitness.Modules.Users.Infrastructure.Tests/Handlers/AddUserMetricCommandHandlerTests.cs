using dotFitness.Common.Results;
using dotFitness.Modules.Users.Application.Commands;
using dotFitness.Modules.Users.Domain.Entities;
using dotFitness.Modules.Users.Domain.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace dotFitness.Modules.Users.Infrastructure.Tests.Handlers;

public class AddUserMetricCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<IUserMetricsRepository> _userMetricsRepositoryMock = new();
    private readonly Mock<ILogger<AddUserMetricCommandHandler>> _loggerMock = new();
    private readonly AddUserMetricCommandHandler _handler;

    public AddUserMetricCommandHandlerTests()
    {
        _handler = new AddUserMetricCommandHandler(
            _userRepositoryMock.Object,
            _userMetricsRepositoryMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task Should_Handle_Valid_Command_Successfully()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new ApplicationUser
        {
            Id = userId,
            Email = "test@example.com",
            UserName = "test@example.com",
            DisplayName = "Test User",
            UnitPreference = UnitPreference.Metric,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var testDate = DateTime.UtcNow.Date;
        var userMetric = new UserMetric
        {
            Id = 1,
            UserId = userId,
            Date = testDate,
            Weight = 70.5,
            Height = 175.0,
            Notes = "Morning measurement",
            Bmi = 22.86,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(user));

        _userMetricsRepositoryMock
            .Setup(r => r.ExistsForUserAndDateAsync(userId, testDate, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(false));

        _userMetricsRepositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<UserMetric>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(userMetric));

        var command = new AddUserMetricCommand
        {
            UserId = userId,
            Date = testDate,
            Weight = 70.5,
            Height = 175.0,
            Notes = "Morning measurement"
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.UserId.Should().Be(userId);
        result.Value.Weight.Should().Be(70.5);
        result.Value.Height.Should().Be(175.0);
        result.Value.Notes.Should().Be("Morning measurement");
        result.Value.Bmi.Should().BeGreaterThan(0);

        _userRepositoryMock.Verify(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
        _userMetricsRepositoryMock.Verify(r => r.ExistsForUserAndDateAsync(userId, testDate, It.IsAny<CancellationToken>()), Times.Once);
        _userMetricsRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<UserMetric>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task Should_Handle_Weight_Only_Measurement()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new ApplicationUser
        {
            Id = userId,
            Email = "test@example.com",
            UserName = "test@example.com",
            DisplayName = "Test User",
            UnitPreference = UnitPreference.Metric,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var testDate = DateTime.UtcNow.Date;
        var userMetric = new UserMetric
        {
            Id = 1,
            UserId = userId,
            Date = testDate,
            Weight = 70.5,
            Height = null,
            Notes = "Weight only",
            Bmi = null,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(user));

        _userMetricsRepositoryMock
            .Setup(r => r.ExistsForUserAndDateAsync(userId, testDate, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(false));

        _userMetricsRepositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<UserMetric>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(userMetric));

        var command = new AddUserMetricCommand
        {
            UserId = userId,
            Date = testDate,
            Weight = 70.5,
            Height = null,
            Notes = "Weight only"
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Weight.Should().Be(70.5);
        result.Value.Height.Should().BeNull();
        result.Value.Bmi.Should().BeNull();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task Should_Calculate_BMI_When_Both_Weight_And_Height_Provided()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new ApplicationUser
        {
            Id = userId,
            Email = "test@example.com",
            UserName = "test@example.com",
            DisplayName = "Test User",
            UnitPreference = UnitPreference.Metric,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var testDate = DateTime.UtcNow.Date;
        var userMetric = new UserMetric
        {
            Id = 1,
            UserId = userId,
            Date = testDate,
            Weight = 70.0,
            Height = 175.0,
            Notes = "Complete measurement",
            Bmi = 22.86,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(user));

        _userMetricsRepositoryMock
            .Setup(r => r.ExistsForUserAndDateAsync(userId, testDate, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(false));

        _userMetricsRepositoryMock
            .Setup(r => r.CreateAsync(It.Is<UserMetric>(m => m.Weight == 70.0 && m.Height == 175.0), It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserMetric m, CancellationToken ct) => Result.Success(userMetric));

        var command = new AddUserMetricCommand
        {
            UserId = userId,
            Date = testDate,
            Weight = 70.0,
            Height = 175.0,
            Notes = "Complete measurement"
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Bmi.Should().BeApproximately(22.86, 0.01);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task Should_Return_Failure_When_User_Not_Found()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<ApplicationUser>("User not found"));

        var command = new AddUserMetricCommand
        {
            UserId = userId,
            Date = DateTime.UtcNow.Date,
            Weight = 70.0,
            Height = 175.0,
            Notes = "Test measurement"
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("User not found");

        _userMetricsRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<UserMetric>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task Should_Return_Failure_When_Metric_Already_Exists_For_Date()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new ApplicationUser
        {
            Id = userId,
            Email = "test@example.com",
            UserName = "test@example.com",
            DisplayName = "Test User",
            UnitPreference = UnitPreference.Metric,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var testDate = DateTime.UtcNow.Date;

        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(user));

        _userMetricsRepositoryMock
            .Setup(r => r.ExistsForUserAndDateAsync(userId, testDate, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(true));

        var command = new AddUserMetricCommand
        {
            UserId = userId,
            Date = testDate,
            Weight = 70.0,
            Height = 175.0,
            Notes = "Duplicate metric"
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("A metric already exists for this date. Please update the existing metric instead.");

        _userMetricsRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<UserMetric>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
