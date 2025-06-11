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

public class GetLatestUserMetricQueryHandlerTests
{
    private readonly Mock<IUserMetricsRepository> _userMetricsRepositoryMock;
    private readonly UserMetricMapper _userMetricMapper;
    private readonly Mock<ILogger<GetLatestUserMetricQueryHandler>> _loggerMock;
    private readonly GetLatestUserMetricQueryHandler _handler;

    public GetLatestUserMetricQueryHandlerTests()
    {
        _userMetricsRepositoryMock = new Mock<IUserMetricsRepository>();
        _userMetricMapper = new UserMetricMapper();
        _loggerMock = new Mock<ILogger<GetLatestUserMetricQueryHandler>>();

        _handler = new GetLatestUserMetricQueryHandler(
            _userMetricsRepositoryMock.Object,
            _userMetricMapper,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task Should_Return_Latest_Metric_When_Found()
    {
        // Arrange
        var query = new GetLatestUserMetricQuery("user123");
        var latestMetric = new UserMetric
        {
            Id = "metric123",
            UserId = "user123",
            Date = new DateTime(2024, 1, 15),
            Weight = 72.0,
            Height = 175.0,
            Bmi = 23.51
        };

        var expectedDto = new UserMetricDto
        {
            Id = "metric123",
            UserId = "user123",
            Date = new DateTime(2024, 1, 15),
            Weight = 72.0,
            Height = 175.0,
            Bmi = 23.51,
            BmiCategory = "Normal weight",
            Notes = null,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _userMetricsRepositoryMock
            .Setup(x => x.GetLatestByUserIdAsync("user123", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(latestMetric));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.UserId.Should().Be("user123");
        result.Value.Weight.Should().Be(72.0);
        result.Value.Height.Should().Be(175.0);
        result.Value.Bmi.Should().Be(23.51);

        _userMetricsRepositoryMock.Verify(x => x.GetLatestByUserIdAsync("user123", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Should_Return_NotFound_When_No_Metrics_Exist()
    {
        // Arrange
        var query = new GetLatestUserMetricQuery("user123");

        _userMetricsRepositoryMock
            .Setup(x => x.GetLatestByUserIdAsync("user123", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<UserMetric>("No metrics found for user"));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("No metrics found for user");

        _userMetricsRepositoryMock.Verify(x => x.GetLatestByUserIdAsync("user123", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Should_Handle_Repository_Errors_Gracefully()
    {
        // Arrange
        var query = new GetLatestUserMetricQuery("user123");

        _userMetricsRepositoryMock
            .Setup(x => x.GetLatestByUserIdAsync("user123", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<UserMetric>("Database connection failed"));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("No metrics found for user");

        _userMetricsRepositoryMock.Verify(x => x.GetLatestByUserIdAsync("user123", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task Should_Handle_Invalid_User_Id(string? invalidUserId)
    {
        // Arrange
        var query = new GetLatestUserMetricQuery(invalidUserId!);

        _userMetricsRepositoryMock
            .Setup(x => x.GetLatestByUserIdAsync(invalidUserId!, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<UserMetric>("Invalid user ID"));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNullOrEmpty();
    }
}
