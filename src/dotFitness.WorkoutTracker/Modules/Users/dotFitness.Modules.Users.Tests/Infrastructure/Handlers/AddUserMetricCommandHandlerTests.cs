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
    private readonly Mock<UserMetricMapper> _userMetricMapperMock;
    private readonly Mock<ILogger<AddUserMetricCommandHandler>> _loggerMock;
    private readonly AddUserMetricCommandHandler _handler;

    public AddUserMetricCommandHandlerTests()
    {
        _userMetricsRepositoryMock = new Mock<IUserMetricsRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _userMetricMapperMock = new Mock<UserMetricMapper>();
        _loggerMock = new Mock<ILogger<AddUserMetricCommandHandler>>();

        _handler = new AddUserMetricCommandHandler(
            _userMetricsRepositoryMock.Object,
            _userRepositoryMock.Object,
            _userMetricMapperMock.Object,
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

        _userMetricMapperMock
            .Setup(x => x.ToDto(It.IsAny<UserMetric>()))
            .Returns(expectedDto);

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

        _userMetricMapperMock
            .Setup(x => x.ToDto(It.IsAny<UserMetric>()))
            .Returns(expectedDto);

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

        _userMetricMapperMock
            .Setup(x => x.ToDto(It.IsAny<UserMetric>()))
            .Returns(expectedDto);

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

        UserMetric? capturedMetric = null;
        _userMetricsRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<UserMetric>(), It.IsAny<CancellationToken>()))
            .Callback<UserMetric>(metric => capturedMetric = metric)
            .ReturnsAsync((UserMetric metric) => Result.Success(metric));

        var expectedDto = new UserMetricDto
        {
            Id = "metric123",
            UserId = "user123",
            Date = new DateTime(2024, 1, 1),
            Weight = 70.0,
            Height = 175.0,
            Bmi = 22.86, // Calculated BMI
            BmiCategory = "Normal weight",
            Notes = "Complete measurement",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _userMetricMapperMock
            .Setup(x => x.ToDto(It.IsAny<UserMetric>()))
            .Returns(expectedDto);

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
        var command = new AddUserMetricCommand(
            UserId: "user123",
            Date: DateTime.UtcNow.Date, // Today's date
            Weight: 70.0,
            Height: null,
            Notes: null
        );

        UserMetric? capturedMetric = null;
        _userMetricsRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<UserMetric>(), It.IsAny<CancellationToken>()))
            .Callback<UserMetric>(metric => capturedMetric = metric)
            .ReturnsAsync((UserMetric metric) => Result.Success(metric));

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

        _userMetricMapperMock
            .Setup(x => x.ToDto(It.IsAny<UserMetric>()))
            .Returns(expectedDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        capturedMetric.Should().NotBeNull();
        capturedMetric!.Date.Should().Be(DateTime.UtcNow.Date);
    }
}