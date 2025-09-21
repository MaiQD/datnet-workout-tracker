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

public class GetLatestUserMetricQueryHandlerTests : IAsyncLifetime
{
    private readonly UsersUnitTestFixture _fixture = new();
    private readonly ILogger<GetLatestUserMetricQueryHandler> _logger = new Mock<ILogger<GetLatestUserMetricQueryHandler>>().Object;
    private UsersDbContext _context = null!;
    private GetLatestUserMetricQueryHandler _handler = null!;

    public async Task InitializeAsync()
    {
        _context = _fixture.CreateInMemoryDbContext<UsersDbContext>();
        await _context.Database.EnsureCreatedAsync();
        
        _handler = new GetLatestUserMetricQueryHandler(
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
    public async Task Should_Return_Latest_Metric_When_Found()
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

        var latestMetric = new UserMetric
        {
            UserId = user.Id,
            Date = this.GenerateUniqueDate(),
            Weight = 72.0,
            Height = 175.0,
            Bmi = 23.51,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

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
    [Trait("Category", "Unit")]
    public async Task Should_Return_NotFound_When_No_Metrics_Exist()
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

        var query = new GetLatestUserMetricQuery(user.Id);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("No metrics found for user");
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

        var query = new GetLatestUserMetricQuery(user.Id);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Failed to get latest user metric");
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task Should_Handle_Invalid_User_Id()
    {
        // Arrange
        var query = new GetLatestUserMetricQuery(this.GenerateUniqueUserId()); // Non-existent user

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("No metrics found for user");
    }
}
