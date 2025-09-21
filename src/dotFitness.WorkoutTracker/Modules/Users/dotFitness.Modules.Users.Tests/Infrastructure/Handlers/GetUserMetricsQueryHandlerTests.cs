using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using dotFitness.Modules.Users.Application.Mappers;
using dotFitness.Modules.Users.Application.Queries;
using dotFitness.Modules.Users.Domain.Entities;
using dotFitness.Modules.Users.Infrastructure.Data;
using dotFitness.Modules.Users.Infrastructure.Handlers;
using dotFitness.Modules.Users.Tests.Infrastructure.Extensions;
using dotFitness.Modules.Users.Tests.Infrastructure.Fixtures;

namespace dotFitness.Modules.Users.Tests.Infrastructure.Handlers;

public class GetUserMetricsQueryHandlerTests: IAsyncLifetime
{
    private readonly UsersUnitTestFixture _fixture = new();
    private readonly ILogger<GetUserMetricsQueryHandler> _logger = new Mock<ILogger<GetUserMetricsQueryHandler>>().Object;
    private UsersDbContext _context = null!;
    private GetUserMetricsQueryHandler _handler = null!;

    public async Task InitializeAsync()
    {
        _context = _fixture.CreateInMemoryDbContext<UsersDbContext>();
        await _context.Database.EnsureCreatedAsync();
        
        _handler = new GetUserMetricsQueryHandler(
            _context,
            _logger
        );
    }

    public async Task DisposeAsync()
    {
        await _context.DisposeAsync();
        await _fixture.DisposeAsync();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task Should_Return_All_Metrics_When_No_Date_Range_Specified()
    {
        // Arrange
        var user = new User
        {
            Email = this.GenerateUniqueEmail(),
            DisplayName = "Test User",
            UnitPreference = UnitPreference.Metric,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var metrics = new List<UserMetric>
        {
            new()
            {
                UserId = user.Id,
                Date = this.GenerateUniqueDate(),
                Weight = 70.0,
                Bmi = 22.86,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new()
            {
                UserId = user.Id,
                Date = this.GenerateUniqueDate(),
                Weight = 72.0,
                Bmi = 23.51,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

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
    [Trait("Category", "Unit")]
    public async Task Should_Return_Metrics_Within_Date_Range_When_Specified()
    {
        // Arrange
        var user = new User
        {
            Email = this.GenerateUniqueEmail(),
            DisplayName = "Test User",
            UnitPreference = UnitPreference.Metric,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var fromDate = new DateTime(2024, 1, 1).Date;
        var toDate = new DateTime(2024, 1, 31).Date;
        var testDate = new DateTime(2024, 1, 15).Date;

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        
        var metricsInRange = new List<UserMetric>
        {
            new()
            {
                UserId = user.Id, // Now user.Id has the correct value after SaveChangesAsync
                Date = testDate,
                Weight = 70.0,
                Bmi = 22.86,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        _context.UserMetrics.AddRange(metricsInRange);
        await _context.SaveChangesAsync();

        var query = new GetUserMetricsQuery(user.Id, fromDate, toDate);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value!.First().Date.Should().Be(testDate);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task Should_Return_Empty_List_When_No_Metrics_Found()
    {
        // Arrange
        var user = new User
        {
            Email = this.GenerateUniqueEmail(),
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
    [Trait("Category", "Unit")]
    public async Task Should_Handle_Repository_Errors_Gracefully()
    {
        // Arrange
        var user = new User
        {
            Email = this.GenerateUniqueEmail(),
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
        result.Error.Should().Contain("Failed to get user metrics");
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task Should_Apply_Date_Range_Filters_Correctly()
    {
        // Arrange
        var user = new User
        {
            Email = this.GenerateUniqueEmail(),
            DisplayName = "Test User",
            UnitPreference = UnitPreference.Metric,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var fromDate = new DateTime(2024, 1, 10).Date;
        var toDate = new DateTime(2024, 1, 20).Date;
        var testDate = new DateTime(2024, 1, 15).Date;

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        
        var filteredMetrics = new List<UserMetric>
        {
            new()
            {
                UserId = user.Id, // Now user.Id has the correct value after SaveChangesAsync
                Date = testDate,
                Weight = 70.0,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

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
