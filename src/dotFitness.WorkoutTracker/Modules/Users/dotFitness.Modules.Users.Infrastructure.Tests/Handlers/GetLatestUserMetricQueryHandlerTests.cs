using dotFitness.Common.Results;
using dotFitness.Modules.Users.Application.Queries;
using dotFitness.Modules.Users.Domain.Entities;
using dotFitness.Modules.Users.Domain.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace dotFitness.Modules.Users.Infrastructure.Tests.Handlers;

public class GetLatestUserMetricQueryHandlerTests
{
    private readonly Mock<IUserMetricsRepository> _userMetricsRepositoryMock = new();
    private readonly Mock<ILogger<GetLatestUserMetricQueryHandler>> _loggerMock = new();
    private readonly GetLatestUserMetricQueryHandler _handler;

    public GetLatestUserMetricQueryHandlerTests()
    {
        _handler = new GetLatestUserMetricQueryHandler(
            _userMetricsRepositoryMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task Should_Return_Latest_Metric_When_Found()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var latestMetric = new UserMetric
        {
            Id = 1,
            UserId = userId,
            Date = DateTime.UtcNow.Date,
            Weight = 72.0,
            Height = 175.0,
            Bmi = 23.51,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _userMetricsRepositoryMock
            .Setup(r => r.GetLatestByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(latestMetric));

        var query = new GetLatestUserMetricQuery(userId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.UserId.Should().Be(userId);
        result.Value.Weight.Should().Be(72.0);
        result.Value.Height.Should().Be(175.0);
        result.Value.Bmi.Should().Be(23.51);

        _userMetricsRepositoryMock.Verify(r => r.GetLatestByUserIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task Should_Return_NotFound_When_No_Metrics_Exist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _userMetricsRepositoryMock
            .Setup(r => r.GetLatestByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<UserMetric>("No metrics found for user"));

        var query = new GetLatestUserMetricQuery(userId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("No metrics found for user");
    }
}
