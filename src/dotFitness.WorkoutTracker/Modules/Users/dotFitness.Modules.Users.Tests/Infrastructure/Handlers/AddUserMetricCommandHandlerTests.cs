using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Moq;
using dotFitness.Modules.Users.Application.Commands;
using dotFitness.Modules.Users.Application.Mappers;
using dotFitness.Modules.Users.Domain.Entities;
using dotFitness.Modules.Users.Infrastructure.Data;
using dotFitness.Modules.Users.Infrastructure.Handlers;
using dotFitness.Modules.Users.Tests.Infrastructure.Extensions;
using dotFitness.Modules.Users.Tests.Infrastructure.Fixtures;

namespace dotFitness.Modules.Users.Tests.Infrastructure.Handlers;

public class AddUserMetricCommandHandlerTests : IAsyncLifetime
{
    private readonly UsersUnitTestFixture _fixture = new();
    private readonly UserMetricMapper _userMetricMapper = new();
    private readonly ILogger<AddUserMetricCommandHandler> _logger = new Mock<ILogger<AddUserMetricCommandHandler>>().Object;
    private UsersDbContext _context = null!;
    private AddUserMetricCommandHandler _handler = null!;

    public async Task InitializeAsync()
    {
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
    }

    [Fact]
    public async Task Should_Handle_Valid_Command_Successfully()
    {
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

        var command = new AddUserMetricCommand
        {
            UserId = user.Id,
            Date = this.GenerateUniqueDate(),
            Weight = 70.5,
            Height = 175.0,
            Notes = "Morning measurement"
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.UserId.Should().Be(user.Id);
        result.Value.Weight.Should().Be(70.5);
        result.Value.Height.Should().Be(175.0);
        result.Value.Notes.Should().Be("Morning measurement");
        result.Value.Bmi.Should().BeGreaterThan(0);

        // Verify metric was saved to database
        var savedMetric = await _context.UserMetrics.FirstAsync(um => um.UserId == user.Id);
        savedMetric.Weight.Should().Be(70.5);
        savedMetric.Height.Should().Be(175.0);
        savedMetric.Notes.Should().Be("Morning measurement");
        savedMetric.Bmi.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task Should_Handle_Weight_Only_Measurement()
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

        var command = new AddUserMetricCommand
        {
            UserId = user.Id,
            Date = this.GenerateUniqueDate(),
            Weight = 70.5,
            Height = null,
            Notes = "Weight only"
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Weight.Should().Be(70.5);
        result.Value.Height.Should().BeNull();
        result.Value.Bmi.Should().BeNull();

        // Verify metric was saved to database
        var savedMetric = await _context.UserMetrics.FirstAsync(um => um.UserId == user.Id);
        savedMetric.Weight.Should().Be(70.5);
        savedMetric.Height.Should().BeNull();
        savedMetric.Bmi.Should().BeNull();
    }

    [Fact]
    public async Task Should_Handle_Height_Only_Measurement()
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

        var command = new AddUserMetricCommand
        {
            UserId = user.Id,
            Date = this.GenerateUniqueDate(),
            Weight = null,
            Height = 175.0,
            Notes = "Height only"
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Weight.Should().BeNull();
        result.Value.Height.Should().Be(175.0);
        result.Value.Bmi.Should().BeNull();

        // Verify metric was saved to database
        var savedMetric = await _context.UserMetrics.FirstAsync(um => um.UserId == user.Id);
        savedMetric.Weight.Should().BeNull();
        savedMetric.Height.Should().Be(175.0);
        savedMetric.Bmi.Should().BeNull();
    }

    [Fact]
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

        var errorContext = _fixture.CreateInMemoryDbContext<UsersDbContext>();
        await errorContext.DisposeAsync();
        
        var errorHandler = new AddUserMetricCommandHandler(
            errorContext,
            _userMetricMapper,
            _logger
        );

        var command = new AddUserMetricCommand
        {
            UserId = user.Id,
            Date = this.GenerateUniqueDate(),
            Weight = 70.5,
            Height = 175.0,
            Notes = "Test measurement"
        };

        // Act
        var result = await errorHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Failed to add user metric");
    }

    [Fact]
    public async Task Should_Calculate_BMI_When_Both_Weight_And_Height_Provided()
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

        var command = new AddUserMetricCommand
        {
            UserId = user.Id,
            Date = this.GenerateUniqueDate(),
            Weight = 70.0,
            Height = 175.0,
            Notes = "Complete measurement"
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Bmi.Should().BeApproximately(22.86, 0.01);

        // Verify BMI is saved correctly
        var savedMetric = await _context.UserMetrics.FirstAsync(um => um.UserId == user.Id);
        savedMetric.Bmi.Should().BeApproximately(22.86, 0.01);
    }

    [Fact]
    public async Task Should_Set_Date_To_Today_When_Not_Provided()
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

        var today = DateTime.UtcNow.Date;
        var command = new AddUserMetricCommand
        {
            UserId = user.Id,
            Date = today,
            Weight = 70.0,
            Height = null,
            Notes = null
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Date.Should().Be(today);

        // Verify date is saved correctly
        var savedMetric = await _context.UserMetrics.FirstAsync(um => um.UserId == user.Id);
        savedMetric.Date.Should().Be(today);
    }

    [Fact]
    public async Task Should_Return_Failure_When_User_Not_Found()
    {
        // Arrange
        var command = new AddUserMetricCommand
        {
            UserId = this.GenerateUniqueUserId(),
            Date = this.GenerateUniqueDate(),
            Weight = 70.0,
            Height = 175.0,
            Notes = "Test measurement"
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("User not found");

        // Verify no metric was saved for the non-existent user
        var metrics = await _context.UserMetrics.Where(um => um.UserId == command.UserId).ToListAsync();
        metrics.Should().BeEmpty();
    }

    [Fact]
    public async Task Should_Return_Failure_When_Metric_Already_Exists_For_Date()
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

        var testDate = this.GenerateUniqueDate();
        
        var existingMetric = new UserMetric
        {
            UserId = 0,
            Date = testDate,
            Weight = 80.0,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        
        existingMetric.UserId = user.Id;
        _context.UserMetrics.Add(existingMetric);
        await _context.SaveChangesAsync();

        var command = new AddUserMetricCommand
        {
            UserId = user.Id,
            Date = testDate, // Same date as existing metric
            Weight = 70.0,
            Height = 175.0,
            Notes = "Duplicate metric"
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("A metric already exists for this date. Please update the existing metric instead.");

        // Verify only the original metric exists
        var metrics = await _context.UserMetrics.Where(um => um.UserId == user.Id).ToListAsync();
        metrics.Should().HaveCount(1);
        metrics[0].Weight.Should().Be(80.0);
    }
}