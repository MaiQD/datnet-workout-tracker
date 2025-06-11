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

public class AddUserMetricCommandHandlerTests
{
    private readonly Mock<IUserMetricsRepository> _userMetricsRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly UserMetricMapper _userMetricMapper;
    private readonly Mock<ILogger<AddUserMetricCommandHandler>> _loggerMock;
    private readonly AddUserMetricCommandHandler _handler;

    public AddUserMetricCommandHandlerTests()
    {
        _userMetricsRepositoryMock = new Mock<IUserMetricsRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _userMetricMapper = new UserMetricMapper();
        _loggerMock = new Mock<ILogger<AddUserMetricCommandHandler>>();

        _handler = new AddUserMetricCommandHandler(
            _userMetricsRepositoryMock.Object,
            _userRepositoryMock.Object,
            _userMetricMapper,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task Should_Handle_Valid_Command_Successfully()
    {
        // Arrange
        var command = new AddUserMetricCommand
        (
            UserId: "user123",
            Date: new DateTime(2024, 1, 1),
            Weight: 70.5,
            Height: 175.0,
            Notes: "Morning measurement");

        // Setup user repository
        var user = new User { Id = "user123", UnitPreference = UnitPreference.Metric };
        _userRepositoryMock
            .Setup(x => x.GetByIdAsync("user123", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(user));

        // Setup metric repository to check if metric exists
        _userMetricsRepositoryMock
            .Setup(x => x.ExistsForUserAndDateAsync("user123", new DateTime(2024, 1, 1), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(false));

        var createdMetric = new UserMetric
        {
            Id = "metric123",
            UserId = "user123",
            Date = new DateTime(2024, 1, 1),
            Weight = 70.5,
            Height = 175.0,
            Notes = "Morning measurement",
            Bmi = 23.02
        };

        var expectedDto = new UserMetricDto
        {
            Id = "metric123",
            UserId = "user123",
            Date = new DateTime(2024, 1, 1),
            Weight = 70.5,
            Height = 175.0,
            Bmi = 23.02,
            BmiCategory = "Normal weight",
            Notes = "Morning measurement",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _userMetricsRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<UserMetric>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(createdMetric));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.UserId.Should().Be("user123");
        result.Value.Weight.Should().Be(70.5);
        result.Value.Height.Should().Be(175.0);
        result.Value.Notes.Should().Be("Morning measurement");

        _userMetricsRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<UserMetric>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Should_Handle_Weight_Only_Measurement()
    {
        // Arrange
        var command = new AddUserMetricCommand(
            UserId: "user123",
            Date: new DateTime(2024, 1, 1),
            Weight: 70.5,
            Height: null,
            Notes: "Weight only"
        );

        // Setup user repository
        var user = new User { Id = "user123", UnitPreference = UnitPreference.Metric };
        _userRepositoryMock
            .Setup(x => x.GetByIdAsync("user123", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(user));

        // Setup metric repository to check if metric exists
        _userMetricsRepositoryMock
            .Setup(x => x.ExistsForUserAndDateAsync("user123", new DateTime(2024, 1, 1), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(false));

        var createdMetric = new UserMetric
        {
            Id = "metric123",
            UserId = "user123",
            Date = new DateTime(2024, 1, 1),
            Weight = 70.5,
            Height = null,
            Notes = "Weight only",
            Bmi = null // No BMI without height
        };

        var expectedDto = new UserMetricDto
        {
            Id = "metric123",
            UserId = "user123",
            Date = new DateTime(2024, 1, 1),
            Weight = 70.5,
            Height = null,
            Bmi = null,
            BmiCategory = "Unknown",
            Notes = "Weight only",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _userMetricsRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<UserMetric>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(createdMetric));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Weight.Should().Be(70.5);
        result.Value.Height.Should().BeNull();
        result.Value.Bmi.Should().BeNull();
    }

    [Fact]
    public async Task Should_Handle_Height_Only_Measurement()
    {
        // Arrange
        var command = new AddUserMetricCommand(
            UserId: "user123",
            Date: new DateTime(2024, 1, 1),
            Weight: null,
            Height: 175.0,
            Notes: "Height only"
        );

        // Setup user repository
        var user = new User { Id = "user123", UnitPreference = UnitPreference.Metric };
        _userRepositoryMock
            .Setup(x => x.GetByIdAsync("user123", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(user));

        // Setup metric repository to check if metric exists
        _userMetricsRepositoryMock
            .Setup(x => x.ExistsForUserAndDateAsync("user123", new DateTime(2024, 1, 1), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(false));

        var createdMetric = new UserMetric
        {
            Id = "metric123",
            UserId = "user123",
            Date = new DateTime(2024, 1, 1),
            Weight = null,
            Height = 175.0,
            Notes = "Height only",
            Bmi = null // No BMI without weight
        };

        var expectedDto = new UserMetricDto
        {
            Id = "metric123",
            UserId = "user123",
            Date = new DateTime(2024, 1, 1),
            Weight = null,
            Height = 175.0,
            Bmi = null,
            BmiCategory = "Unknown",
            Notes = "Height only",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _userMetricsRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<UserMetric>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(createdMetric));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Weight.Should().BeNull();
        result.Value.Height.Should().Be(175.0);
        result.Value.Bmi.Should().BeNull();
    }

    [Fact]
    public async Task Should_Handle_Repository_Errors_Gracefully()
    {
        // Arrange
        var command = new AddUserMetricCommand(
            UserId: "user123",
            Date: new DateTime(2024, 1, 1),
            Weight: 70.5,
            Height: 175.0,
            Notes: "Test measurement"
        );

        // Setup user repository
        var user = new User { Id = "user123", UnitPreference = UnitPreference.Metric };
        _userRepositoryMock
            .Setup(x => x.GetByIdAsync("user123", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(user));

        // Setup metric repository to check if metric exists
        _userMetricsRepositoryMock
            .Setup(x => x.ExistsForUserAndDateAsync("user123", new DateTime(2024, 1, 1), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(false));

        _userMetricsRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<UserMetric>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<UserMetric>("Database connection failed"));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Database connection failed");

        _userMetricsRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<UserMetric>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Should_Calculate_BMI_When_Both_Weight_And_Height_Provided()
    {
        // Arrange
        var command = new AddUserMetricCommand(
            UserId: "user123",
            Date: new DateTime(2024, 1, 1),
            Weight: 70.0,
            Height: 175.0,
            Notes: "Complete measurement"
        );

        // Setup user repository
        var user = new User { Id = "user123", UnitPreference = UnitPreference.Metric };
        _userRepositoryMock
            .Setup(x => x.GetByIdAsync("user123", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(user));

        // Setup metric repository to check if metric exists
        _userMetricsRepositoryMock
            .Setup(x => x.ExistsForUserAndDateAsync("user123", new DateTime(2024, 1, 1), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(false));

        UserMetric? capturedMetric = null;
        _userMetricsRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<UserMetric>(), It.IsAny<CancellationToken>()))
            .Callback<UserMetric, CancellationToken>((metric, token) => capturedMetric = metric)
            .ReturnsAsync((UserMetric metric, CancellationToken token) => Result.Success(metric));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        capturedMetric.Should().NotBeNull();
        capturedMetric!.Bmi.Should().BeApproximately(22.86, 0.01);
    }

    [Fact]
    public async Task Should_Set_Date_To_Today_When_Not_Provided()
    {
        // Arrange
        var today = DateTime.UtcNow.Date;
        var command = new AddUserMetricCommand(
            UserId: "user123",
            Date: today, // Today's date
            Weight: 70.0,
            Height: null,
            Notes: null
        );

        // Setup user repository
        var user = new User { Id = "user123", UnitPreference = UnitPreference.Metric };
        _userRepositoryMock
            .Setup(x => x.GetByIdAsync("user123", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(user));

        // Setup metric repository to check if metric exists
        _userMetricsRepositoryMock
            .Setup(x => x.ExistsForUserAndDateAsync("user123", today, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(false));

        UserMetric? capturedMetric = null;
        _userMetricsRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<UserMetric>(), It.IsAny<CancellationToken>()))
            .Callback<UserMetric, CancellationToken>((metric, token) => capturedMetric = metric)
            .ReturnsAsync((UserMetric metric, CancellationToken token) => Result.Success(metric));

        var expectedDto = new UserMetricDto
        {
            Id = "metric123",
            UserId = "user123",
            Date = DateTime.UtcNow.Date,
            Weight = 70.0,
            Height = null,
            Bmi = null,
            BmiCategory = "Unknown",
            Notes = null,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        capturedMetric.Should().NotBeNull();
        capturedMetric!.Date.Should().Be(DateTime.UtcNow.Date);
    }

    [Fact]
    public async Task Should_Return_Failure_When_User_Not_Found()
    {
        // Arrange
        var command = new AddUserMetricCommand(
            UserId: "nonexistent",
            Date: new DateTime(2024, 1, 1),
            Weight: 70.0,
            Height: 175.0,
            Notes: "Test measurement"
        );

        // Setup user repository to return failure
        _userRepositoryMock
            .Setup(x => x.GetByIdAsync("nonexistent", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<User>("User not found"));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("User not found");

        // Verify repository was never called
        _userMetricsRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<UserMetric>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Should_Return_Failure_When_Metric_Already_Exists_For_Date()
    {
        // Arrange
        var command = new AddUserMetricCommand(
            UserId: "user123",
            Date: new DateTime(2024, 1, 1),
            Weight: 70.0,
            Height: 175.0,
            Notes: "Test measurement"
        );

        // Setup user repository
        var user = new User { Id = "user123", UnitPreference = UnitPreference.Metric };
        _userRepositoryMock
            .Setup(x => x.GetByIdAsync("user123", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(user));

        // Setup metric repository to indicate metric already exists
        _userMetricsRepositoryMock
            .Setup(x => x.ExistsForUserAndDateAsync("user123", new DateTime(2024, 1, 1), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(true));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("A metric already exists for this date. Please update the existing metric instead.");

        // Verify repository create was never called
        _userMetricsRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<UserMetric>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }
}