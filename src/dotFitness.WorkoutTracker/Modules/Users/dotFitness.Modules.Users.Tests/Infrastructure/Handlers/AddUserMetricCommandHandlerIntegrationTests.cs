using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Moq;
using dotFitness.Modules.Users.Application.Commands;
using dotFitness.Modules.Users.Application.Mappers;
using dotFitness.Modules.Users.Domain.Entities;
using dotFitness.Modules.Users.Infrastructure.Data;
using dotFitness.Modules.Users.Infrastructure.Handlers;
using dotFitness.SharedKernel.Tests.PostgreSQL;

namespace dotFitness.Modules.Users.Tests.Infrastructure.Handlers;

public class AddUserMetricCommandHandlerIntegrationTests : IAsyncLifetime
{
    private readonly PostgreSqlFixture _fixture;
    private readonly UserMetricMapper _userMetricMapper;
    private readonly ILogger<AddUserMetricCommandHandler> _logger;
    private UsersDbContext _context = null!;
    private AddUserMetricCommandHandler _handler = null!;

    public AddUserMetricCommandHandlerIntegrationTests()
    {
        _fixture = new PostgreSqlFixture();
        _userMetricMapper = new UserMetricMapper();
        _logger = new Mock<ILogger<AddUserMetricCommandHandler>>().Object;
    }

    public async Task InitializeAsync()
    {
        await _fixture.InitializeAsync();
        _context = _fixture.CreateInMemoryDbContext<UsersDbContext>();
        await _context.Database.EnsureCreatedAsync();
        
        _handler = new AddUserMetricCommandHandler(
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
    public async Task Should_Add_User_Metric_Successfully()
    {
        // Arrange
        var user = new User
        {
            Id = "507f1f77bcf86cd799439011",
            Email = "test@example.com",
            DisplayName = "Test User",
            UnitPreference = UnitPreference.Metric,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var command = new AddUserMetricCommand
        {
            UserId = user.PostgresId,
            Date = DateTime.UtcNow.Date,
            Weight = 75.5m,
            Height = 180.0m,
            Notes = "Weekly weigh-in"
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.UserId.Should().Be(user.PostgresId);
        result.Value.Weight.Should().Be(75.5m);
        result.Value.Height.Should().Be(180.0m);
        result.Value.Notes.Should().Be("Weekly weigh-in");
        result.Value.Bmi.Should().BeGreaterThan(0); // BMI should be calculated

        // Verify metric was saved to database
        var savedMetric = await _context.UserMetrics.FirstAsync(um => um.UserId == user.Id);
        savedMetric.Weight.Should().Be(75.5m);
        savedMetric.Height.Should().Be(180.0m);
        savedMetric.Notes.Should().Be("Weekly weigh-in");
        savedMetric.Bmi.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task Should_Return_Error_When_User_Does_Not_Exist()
    {
        // Arrange
        var command = new AddUserMetricCommand
        {
            UserId = 999, // Non-existent user
            Date = DateTime.UtcNow.Date,
            Weight = 75.5m,
            Height = 180.0m,
            Notes = "Test metric"
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("User not found");

        // Verify no metric was saved
        var metrics = await _context.UserMetrics.ToListAsync();
        metrics.Should().BeEmpty();
    }

    [Fact]
    public async Task Should_Return_Error_When_Metric_Already_Exists_For_Date()
    {
        // Arrange
        var user = new User
        {
            Id = "507f1f77bcf86cd799439011",
            Email = "test@example.com",
            DisplayName = "Test User",
            UnitPreference = UnitPreference.Metric,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var existingMetric = new UserMetric
        {
            Id = "507f1f77bcf86cd799439012",
            UserId = user.Id,
            Date = DateTime.UtcNow.Date,
            Weight = 80.0m,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        _context.UserMetrics.Add(existingMetric);
        await _context.SaveChangesAsync();

        var command = new AddUserMetricCommand
        {
            UserId = user.PostgresId,
            Date = DateTime.UtcNow.Date, // Same date as existing metric
            Weight = 75.5m,
            Height = 180.0m,
            Notes = "Duplicate metric"
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("A metric already exists for this date. Please update the existing metric instead.");

        // Verify only the original metric exists
        var metrics = await _context.UserMetrics.ToListAsync();
        metrics.Should().HaveCount(1);
        metrics[0].Weight.Should().Be(80.0m); // Original weight unchanged
    }

    [Fact]
    public async Task Should_Calculate_BMI_When_Both_Weight_And_Height_Provided()
    {
        // Arrange
        var user = new User
        {
            Id = "507f1f77bcf86cd799439011",
            Email = "test@example.com",
            DisplayName = "Test User",
            UnitPreference = UnitPreference.Metric,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var command = new AddUserMetricCommand
        {
            UserId = user.PostgresId,
            Date = DateTime.UtcNow.Date,
            Weight = 70.0m, // 70 kg
            Height = 175.0m, // 175 cm
            Notes = "BMI calculation test"
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        
        // Expected BMI = 70 / (1.75^2) = 22.86 (approximately)
        var expectedBmi = 70.0m / (1.75m * 1.75m);
        result.Value.Bmi.Should().BeApproximately((double)expectedBmi, 0.1);

        // Verify BMI is saved correctly
        var savedMetric = await _context.UserMetrics.FirstAsync();
        savedMetric.Bmi.Should().BeApproximately((double)expectedBmi, 0.1);
    }

    [Fact]
    public async Task Should_Not_Calculate_BMI_When_Height_Is_Missing()
    {
        // Arrange
        var user = new User
        {
            Id = "507f1f77bcf86cd799439011",
            Email = "test@example.com",
            DisplayName = "Test User",
            UnitPreference = UnitPreference.Metric,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var command = new AddUserMetricCommand
        {
            UserId = user.PostgresId,
            Date = DateTime.UtcNow.Date,
            Weight = 70.0m,
            Height = null, // No height provided
            Notes = "Weight only"
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Bmi.Should().BeNull();

        // Verify BMI is null in database
        var savedMetric = await _context.UserMetrics.FirstAsync();
        savedMetric.Bmi.Should().BeNull();
    }
}
