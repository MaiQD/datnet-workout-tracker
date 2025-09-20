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

public class GetLatestUserMetricQueryHandlerTests : IAsyncLifetime
{
    private readonly PostgreSqlFixture _fixture;
    private readonly UserMetricMapper _userMetricMapper;
    private readonly ILogger<GetLatestUserMetricQueryHandler> _logger;
    private UsersDbContext _context = null!;
    private GetLatestUserMetricQueryHandler _handler = null!;

    public GetLatestUserMetricQueryHandlerTests()
    {
        _fixture = PostgreSqlFixture.Instance;
        _userMetricMapper = new UserMetricMapper();
        _logger = new Mock<ILogger<GetLatestUserMetricQueryHandler>>().Object;
    }

    public async Task InitializeAsync()
    {
        await _fixture.InitializeAsync();
        _context = _fixture.CreateDbContext<UsersDbContext>();
        await _context.Database.EnsureCreatedAsync();
        
        _handler = new GetLatestUserMetricQueryHandler(
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
    public async Task Should_Return_Latest_Metric_When_Found()
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

        var latestMetric = new UserMetric
        {
            Id = 1,
            UserId = user.Id,
            Date = new DateTime(2024, 1, 15),
            Weight = 72.0,
            Height = 175.0,
            Bmi = 23.51,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        _context.UserMetrics.Add(latestMetric);
        await _context.SaveChangesAsync();

        var query = new GetLatestUserMetricQuery(user.Id);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.UserId.Should().Be(user.Id);
        result.Value.Weight.Should().Be(72.0);
        result.Value.Height.Should().Be(175.0);
        result.Value.Bmi.Should().Be(23.51);
    }

    [Fact]
    public async Task Should_Return_NotFound_When_No_Metrics_Exist()
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

        var query = new GetLatestUserMetricQuery(user.Id);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("No metrics found for user");
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

        var query = new GetLatestUserMetricQuery(user.Id);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("User management failed");
    }

    [Fact]
    public async Task Should_Handle_Invalid_User_Id()
    {
        // Arrange
        var query = new GetLatestUserMetricQuery(999); // Non-existent user

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("User not found");
    }
}
