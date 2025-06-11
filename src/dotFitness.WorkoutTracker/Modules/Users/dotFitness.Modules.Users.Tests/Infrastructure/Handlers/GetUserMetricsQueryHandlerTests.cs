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

public class GetUserMetricsQueryHandlerTests
{
    private readonly Mock<IUserMetricsRepository> _userMetricsRepositoryMock;
    private readonly UserMetricMapper _userMetricMapper;
    private readonly GetUserMetricsQueryHandler _handler;

    public GetUserMetricsQueryHandlerTests()
    {
        _userMetricsRepositoryMock = new Mock<IUserMetricsRepository>();
        _userMetricMapper = new UserMetricMapper();
        var loggerMock = new Mock<ILogger<GetUserMetricsQueryHandler>>();

        _handler = new GetUserMetricsQueryHandler(
            _userMetricsRepositoryMock.Object,
            _userMetricMapper,
            loggerMock.Object
        );
    }

    [Fact]
    public async Task Should_Return_All_Metrics_When_No_Date_Range_Specified()
    {
        // Arrange
        var query = new GetUserMetricsQuery("user123", null, null);
        var metrics = new List<UserMetric>
        {
            new()
            {
                Id = "metric1",
                UserId = "user123",
                Date = new DateTime(2024, 1, 1),
                Weight = 70.0,
                Bmi = 22.86
            },
            new()
            {
                Id = "metric2",
                UserId = "user123",
                Date = new DateTime(2024, 1, 15),
                Weight = 72.0,
                Bmi = 23.51
            }
        };

        var expectedDtos = new List<UserMetricDto>
        {
            new()
            {
                Id = "metric1",
                UserId = "user123",
                Date = new DateTime(2024, 1, 1),
                Weight = 70.0,
                Height = null,
                Bmi = 22.86,
                BmiCategory = "Normal weight",
                Notes = null,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = "metric2",
                UserId = "user123",
                Date = new DateTime(2024, 1, 15),
                Weight = 72.0,
                Height = null,
                Bmi = 23.51,
                BmiCategory = "Normal weight",
                Notes = null,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        _userMetricsRepositoryMock
            .Setup(x => x.GetByUserIdAsync("user123", 0, 50, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<IEnumerable<UserMetric>>(metrics));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value.Should().OnlyContain(dto => dto.UserId == "user123");

        _userMetricsRepositoryMock.Verify(x => x.GetByUserIdAsync("user123", 0, 50, It.IsAny<CancellationToken>()),
            Times.Once);
        _userMetricsRepositoryMock.Verify(
            x => x.GetByUserIdAndDateRangeAsync(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Should_Return_Metrics_Within_Date_Range_When_Specified()
    {
        // Arrange
        var fromDate = new DateTime(2024, 1, 1);
        var toDate = new DateTime(2024, 1, 31);
        var query = new GetUserMetricsQuery
        {
            UserId = "user123", StartDate = fromDate, EndDate = toDate
        };

        var metricsInRange = new List<UserMetric>
        {
            new()
            {
                Id = "metric1",
                UserId = "user123",
                Date = new DateTime(2024, 1, 15),
                Weight = 70.0,
                Bmi = 22.86
            }
        };

        var expectedDto = new UserMetricDto
        {
            Id = "metric1",
            UserId = "user123",
            Date = new DateTime(2024, 1, 15),
            Weight = 70.0,
            Height = null,
            Bmi = 22.86,
            BmiCategory = "Normal weight",
            Notes = null,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _userMetricsRepositoryMock
            .Setup(x => x.GetByUserIdAndDateRangeAsync("user123", fromDate, toDate, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<IEnumerable<UserMetric>>(metricsInRange));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value!.First().Date.Should().Be(new DateTime(2024, 1, 15));

        _userMetricsRepositoryMock.Verify(x => x.GetByUserIdAndDateRangeAsync("user123", fromDate, toDate, It.IsAny<CancellationToken>()), Times.Once);
        _userMetricsRepositoryMock.Verify(x => x.GetByUserIdAsync(It.IsAny<string>(), 0, 50, It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Should_Return_Empty_List_When_No_Metrics_Found()
    {
        // Arrange
        var query = new GetUserMetricsQuery("user123", null, null);

        _userMetricsRepositoryMock
            .Setup(x => x.GetByUserIdAsync("user123",0, 50, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<IEnumerable<UserMetric>>(new List<UserMetric>()));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();

        _userMetricsRepositoryMock.Verify(x => x.GetByUserIdAsync("user123", 0, 50, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Should_Handle_Repository_Errors_Gracefully()
    {
        // Arrange
        var query = new GetUserMetricsQuery("user123", null, null);

        _userMetricsRepositoryMock
            .Setup(x => x.GetByUserIdAsync("user123", 0, 50, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<IEnumerable<UserMetric>>("Database connection failed"));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Database connection failed");

        _userMetricsRepositoryMock.Verify(x => x.GetByUserIdAsync("user123", 0, 50, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Should_Apply_Date_Range_Filters_Correctly()
    {
        // Arrange
        var fromDate = new DateTime(2024, 1, 10);
        var toDate = new DateTime(2024, 1, 20);
        var query = new GetUserMetricsQuery("user123", fromDate, toDate);

        var filteredMetrics = new List<UserMetric>
        {
            new()
            {
                Id = "metric1",
                UserId = "user123",
                Date = new DateTime(2024, 1, 15),
                Weight = 70.0
            }
        };

        _userMetricsRepositoryMock
            .Setup(x => x.GetByUserIdAndDateRangeAsync("user123", fromDate, toDate, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<IEnumerable<UserMetric>>(filteredMetrics));

        var expectedDto = new UserMetricDto
        {
            Id = "metric1",
            UserId = "user123",
            Date = new DateTime(2024, 1, 15),
            Weight = 70.0,
            Height = null,
            Bmi = null,
            BmiCategory = "Unknown",
            Notes = null,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value!.First().Date.Should().BeBefore(toDate.AddDays(1));
        result.Value!.First().Date.Should().BeOnOrAfter(fromDate);

        _userMetricsRepositoryMock.Verify(
            x => x.GetByUserIdAndDateRangeAsync("user123", fromDate, toDate, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task Should_Handle_Invalid_User_Id(string? invalidUserId)
    {
        // Arrange
        var query = new GetUserMetricsQuery(invalidUserId!, null, null);

        _userMetricsRepositoryMock
            .Setup(x => x.GetByUserIdAsync(invalidUserId!, 0, 50, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<IEnumerable<UserMetric>>("Invalid user ID"));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNullOrEmpty();
    }
}