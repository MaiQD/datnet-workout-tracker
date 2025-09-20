using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Moq;
using dotFitness.Modules.Users.Application.DTOs;
using dotFitness.Modules.Users.Application.Mappers;
using dotFitness.Modules.Users.Application.Queries;
using dotFitness.Modules.Users.Domain.Entities;
using dotFitness.Modules.Users.Infrastructure.Data;
using dotFitness.Modules.Users.Infrastructure.Handlers;
using dotFitness.SharedKernel.Results;
using dotFitness.SharedKernel.Tests.PostgreSQL;

namespace dotFitness.Modules.Users.Tests.Infrastructure.Handlers;

public class GetUserMetricsQueryHandlerTests : IAsyncLifetime
{
    private readonly PostgreSqlFixture _fixture;
    private readonly UserMetricMapper _userMetricMapper;
    private readonly ILogger<GetUserMetricsQueryHandler> _logger;
    private UsersDbContext _context = null!;
    private GetUserMetricsQueryHandler _handler = null!;

    public GetUserMetricsQueryHandlerTests()
    {
        _fixture = PostgreSqlFixture.Instance;
        _userMetricMapper = new UserMetricMapper();
        _logger = new Mock<ILogger<GetUserMetricsQueryHandler>>().Object;
    }

    public async Task InitializeAsync()
    {
        await _fixture.InitializeAsync();
        _context = _fixture.CreateDbContext<UsersDbContext>();
        await _context.Database.EnsureCreatedAsync();
        
        _handler = new GetUserMetricsQueryHandler(
            _context,
            _userMetricMapper,
            _logger
        );
    }

    public async Task DisposeAsync()
    {
        await _context.DisposeAsync();
        await _fixture.DisposeAsync();
    }

    [Fact]
    public async Task Should_Return_All_Metrics_When_No_Date_Range_Specified()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            Email = "test@example.com",
            DisplayName = "Test User",
            UnitPreference = UnitPreference.Metric,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var metrics = new List<UserMetric>
        {
            new()
            {
                Id = 1,
                UserId = user.Id,
                Date = new DateTime(2024, 1, 1),
                Weight = 70.0,
                Bmi = 22.86,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = 2,
                UserId = user.Id,
                Date = new DateTime(2024, 1, 15),
                Weight = 72.0,
                Bmi = 23.51,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        _context.Users.Add(user);
        _context.UserMetrics.AddRange(metrics);
        await _context.SaveChangesAsync();

        var query = new GetUserMetricsQuery(user.Id, null, null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value.Should().OnlyContain(dto => dto.UserId == user.Id);
    }

    [Fact]
    public async Task Should_Return_Metrics_Within_Date_Range_When_Specified()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            Email = "test@example.com",
            DisplayName = "Test User",
            UnitPreference = UnitPreference.Metric,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var fromDate = new DateTime(2024, 1, 1);
        var toDate = new DateTime(2024, 1, 31);
        
        var metricsInRange = new List<UserMetric>
        {
            new()
            {
                Id = 1,
                UserId = user.Id,
                Date = new DateTime(2024, 1, 15),
                Weight = 70.0,
                Bmi = 22.86,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        _context.Users.Add(user);
        _context.UserMetrics.AddRange(metricsInRange);
        await _context.SaveChangesAsync();

        var query = new GetUserMetricsQuery(user.Id, fromDate, toDate);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value!.First().Date.Should().Be(new DateTime(2024, 1, 15));
    }

    [Fact]
    public async Task Should_Return_Empty_List_When_No_Metrics_Found()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            Email = "test@example.com",
            DisplayName = "Test User",
            UnitPreference = UnitPreference.Metric,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var query = new GetUserMetricsQuery(user.Id, null, null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task Should_Handle_Repository_Errors_Gracefully()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            Email = "test@example.com",
            DisplayName = "Test User",
            UnitPreference = UnitPreference.Metric,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Dispose context to simulate database error
        await _context.DisposeAsync();

        var query = new GetUserMetricsQuery(user.Id, null, null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("User management failed");
    }

    [Fact]
    public async Task Should_Apply_Date_Range_Filters_Correctly()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            Email = "test@example.com",
            DisplayName = "Test User",
            UnitPreference = UnitPreference.Metric,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var fromDate = new DateTime(2024, 1, 10);
        var toDate = new DateTime(2024, 1, 20);
        
        var filteredMetrics = new List<UserMetric>
        {
            new()
            {
                Id = 1,
                UserId = user.Id,
                Date = new DateTime(2024, 1, 15),
                Weight = 70.0,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        _context.Users.Add(user);
        _context.UserMetrics.AddRange(filteredMetrics);
        await _context.SaveChangesAsync();

        var query = new GetUserMetricsQuery(user.Id, fromDate, toDate);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value!.First().Date.Should().BeBefore(toDate.AddDays(1));
        result.Value!.First().Date.Should().BeOnOrAfter(fromDate);
    }
}