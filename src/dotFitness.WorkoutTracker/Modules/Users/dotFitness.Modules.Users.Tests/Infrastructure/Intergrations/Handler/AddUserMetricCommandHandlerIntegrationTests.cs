using dotFitness.Modules.Users.Application.Commands;
using dotFitness.Modules.Users.Application.Mappers;
using dotFitness.Modules.Users.Domain.Entities;
using dotFitness.Modules.Users.Infrastructure.Data;
using dotFitness.Modules.Users.Infrastructure.Handlers;
using dotFitness.Modules.Users.Tests.Infrastructure.Extensions;
using dotFitness.Modules.Users.Tests.Infrastructure.Fixtures;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace dotFitness.Modules.Users.Tests.Infrastructure.Intergrations.Handler;

[Collection("UsersPostgreSQL.Shared")]
public class AddUserMetricCommandHandlerIntegrationTests(UsersPostgresSqlFixture fixture)
{
    private readonly ILogger<AddUserMetricCommandHandler> _logger = new Mock<ILogger<AddUserMetricCommandHandler>>().Object;

    private async Task<(UsersDbContext context, AddUserMetricCommandHandler handler)> CreateHandlerAsync()
    {
        await fixture.InitializeAsync();
        var context = fixture.CreateFreshUsersDbContext();
        await context.Database.EnsureCreatedAsync();
        
        // Clear any existing data to ensure test isolation
        context.Users.RemoveRange(context.Users);
        context.UserMetrics.RemoveRange(context.UserMetrics);
        await context.SaveChangesAsync();
        
        var handler = new AddUserMetricCommandHandler(
            context,
            _logger
        );

        return (context, handler);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task Should_Add_User_Metric_Successfully()
    {
        // Arrange
        var (context, handler) = await CreateHandlerAsync();
        var user = new User
        {
            Email = this.GenerateUniqueEmail(),
            DisplayName = "Test User",
            UnitPreference = UnitPreference.Metric,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        var command = new AddUserMetricCommand
        {
            UserId = user.Id,
            Date = this.GenerateUniqueDate(),
            Weight = 75.5,
            Height = 180.0,
            Notes = "Weekly weigh-in"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.UserId.Should().Be(user.Id);
        result.Value.Weight.Should().Be(75.5);
        result.Value.Height.Should().Be(180.0);
        result.Value.Notes.Should().Be("Weekly weigh-in");
        result.Value.Bmi.Should().BeGreaterThan(0); // BMI should be calculated

        // Verify metric was saved to database
        var savedMetric = await context.UserMetrics.FirstAsync(um => um.UserId == user.Id);
        savedMetric.Weight.Should().Be(75.5);
        savedMetric.Height.Should().Be(180.0);
        savedMetric.Notes.Should().Be("Weekly weigh-in");
        savedMetric.Bmi.Should().BeGreaterThan(0);

        // Cleanup
        await context.DisposeAsync();
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task Should_Return_Error_When_User_Does_Not_Exist()
    {
        // Arrange
        var (context, handler) = await CreateHandlerAsync();
        var command = new AddUserMetricCommand
        {
            UserId = this.GenerateUniqueUserId(), // Non-existent user
            Date = this.GenerateUniqueDate(),
            Weight = 75.5,
            Height = 180.0,
            Notes = "Test metric"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("User not found");

        // Verify no metric was saved
        var metrics = await context.UserMetrics.ToListAsync();
        metrics.Should().BeEmpty();

        // Cleanup
        await context.DisposeAsync();
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task Should_Return_Error_When_Metric_Already_Exists_For_Date()
    {
        // Arrange
        var (context, handler) = await CreateHandlerAsync();
        var user = new User
        {
            Email = this.GenerateUniqueEmail(),
            DisplayName = "Test User",
            UnitPreference = UnitPreference.Metric,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();
        
        var testDate = this.GenerateUniqueDate();
        var existingMetric = new UserMetric
        {
            UserId = user.Id,
            Date = testDate,
            Weight = 80.0,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        context.UserMetrics.Add(existingMetric);
        await context.SaveChangesAsync();

        var command = new AddUserMetricCommand
        {
            UserId = user.Id,
            Date = testDate, // Same date as existing metric
            Weight = 75.5,
            Height = 180.0,
            Notes = "Duplicate metric"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("A metric already exists for this date. Please update the existing metric instead.");

        // Verify only the original metric exists
        var metrics = await context.UserMetrics.ToListAsync();
        metrics.Should().HaveCount(1);
        metrics[0].Weight.Should().Be(80.0); // Original weight unchanged

        // Cleanup
        await context.DisposeAsync();
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task Should_Calculate_BMI_When_Both_Weight_And_Height_Provided()
    {
        // Arrange
        var (context, handler) = await CreateHandlerAsync();
        var user = new User
        {
            Email = this.GenerateUniqueEmail(),
            DisplayName = "Test User",
            UnitPreference = UnitPreference.Metric,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        var command = new AddUserMetricCommand
        {
            UserId = user.Id,
            Date = this.GenerateUniqueDate(),
            Weight = 70.0, // 70 kg
            Height = 175.0, // 175 cm
            Notes = "BMI calculation test"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        
        // Expected BMI = 70 / (1.75^2) = 22.86 (approximately)
        var expectedBmi = 70.0 / (1.75 * 1.75);
        result.Value.Bmi.Should().BeApproximately(expectedBmi, 0.1);

        // Verify BMI is saved correctly
        var savedMetric = await context.UserMetrics.FirstAsync();
        savedMetric.Bmi.Should().BeApproximately(expectedBmi, 0.1);

        // Cleanup
        await context.DisposeAsync();
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task Should_Not_Calculate_BMI_When_Height_Is_Missing()
    {
        // Arrange
        var (context, handler) = await CreateHandlerAsync();
        var user = new User
        {
            Email = this.GenerateUniqueEmail(),
            DisplayName = "Test User",
            UnitPreference = UnitPreference.Metric,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        var command = new AddUserMetricCommand
        {
            UserId = user.Id,
            Date = this.GenerateUniqueDate(),
            Weight = 70.0,
            Height = null, // No height provided
            Notes = "Weight only"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Bmi.Should().BeNull();

        // Verify BMI is null in database
        var savedMetric = await context.UserMetrics.FirstAsync();
        savedMetric.Bmi.Should().BeNull();

        // Cleanup
        await context.DisposeAsync();
    }
}
