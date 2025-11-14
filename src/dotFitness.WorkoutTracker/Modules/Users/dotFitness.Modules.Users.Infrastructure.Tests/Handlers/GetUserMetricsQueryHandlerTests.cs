using dotFitness.Common.Results;
using dotFitness.Modules.Users.Application.Queries;
using dotFitness.Modules.Users.Domain.Entities;
using dotFitness.Modules.Users.Domain.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace dotFitness.Modules.Users.Infrastructure.Tests.Handlers;

public class GetUserMetricsQueryHandlerTests
{
    private readonly Mock<IUserMetricsRepository> _userMetricsRepositoryMock = new();
    private readonly Mock<ILogger<GetUserMetricsQueryHandler>> _loggerMock = new();
    private readonly GetUserMetricsQueryHandler _handler;

    public GetUserMetricsQueryHandlerTests()
    {
        _handler = new GetUserMetricsQueryHandler(
            _userMetricsRepositoryMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task Should_Return_All_Metrics_When_No_Date_Range_Specified()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var metrics = new List<UserMetric>
        {
            new()
            {
                Id = 1,
                UserId = userId,
                Date = DateTime.UtcNow.Date,
                Weight = 70.0,
                Bmi = 22.86,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = 2,
                UserId = userId,
                Date = DateTime.UtcNow.Date.AddDays(-1),
                Weight = 72.0,
                Bmi = 23.51,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        _userMetricsRepositoryMock
            .Setup(r => r.GetByUserIdAsync(userId, 0, 50, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(metrics.AsEnumerable()));

        var query = new GetUserMetricsQuery(userId, null, null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value.Should().OnlyContain(dto => dto.UserId == userId);

        _userMetricsRepositoryMock.Verify(r => r.GetByUserIdAsync(userId, 0, 50, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task Should_Return_Metrics_Within_Date_Range_When_Specified()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var fromDate = new DateTime(2024, 1, 1).Date;
        var toDate = new DateTime(2024, 1, 31).Date;
        var testDate = new DateTime(2024, 1, 15).Date;

        var metricsInRange = new List<UserMetric>
        {
            new()
            {
                Id = 1,
                UserId = userId,
                Date = testDate,
                Weight = 70.0,
                Bmi = 22.86,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        _userMetricsRepositoryMock
            .Setup(r => r.GetByUserIdAndDateRangeAsync(userId, fromDate, toDate, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(metricsInRange.AsEnumerable()));

        var query = new GetUserMetricsQuery(userId, fromDate, toDate);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value!.First().Date.Should().Be(testDate);

        _userMetricsRepositoryMock.Verify(r => r.GetByUserIdAndDateRangeAsync(userId, fromDate, toDate, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task Should_Return_Empty_List_When_No_Metrics_Found()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _userMetricsRepositoryMock
            .Setup(r => r.GetByUserIdAsync(userId, 0, 50, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(Enumerable.Empty<UserMetric>()));

        var query = new GetUserMetricsQuery(userId, null, null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }
}
