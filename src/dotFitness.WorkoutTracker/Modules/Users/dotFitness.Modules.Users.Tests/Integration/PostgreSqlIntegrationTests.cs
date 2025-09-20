using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using dotFitness.Modules.Users.Domain.Entities;
using dotFitness.Modules.Users.Infrastructure.Data;
using dotFitness.Modules.Users.Infrastructure.Data.Entities;
using dotFitness.SharedKernel.Tests.PostgreSQL;

namespace dotFitness.Modules.Users.Tests.Integration;

public class PostgreSqlIntegrationTests : IAsyncLifetime
{
    private readonly PostgreSqlFixture _fixture;
    private UsersDbContext _context = null!;

    public PostgreSqlIntegrationTests()
    {
        _fixture = new PostgreSqlFixture();
    }

    public async Task InitializeAsync()
    {
        await _fixture.InitializeAsync();
        _context = _fixture.CreateDbContext<UsersDbContext>();
        
        // Apply migrations to create the schema
        await _context.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        await _context.DisposeAsync();
        await _fixture.DisposeAsync();
    }

    [Fact]
    public async Task Should_Create_And_Retrieve_User_With_PostgreSQL_Database()
    {
        // Arrange
        var user = new User
        {
            Id = "507f1f77bcf86cd799439011",
            GoogleId = "google_integration_test",
            Email = "integration@test.com",
            DisplayName = "Integration Test User",
            Gender = Gender.Male,
            DateOfBirth = new DateTime(1990, 1, 1),
            UnitPreference = UnitPreference.Metric,
            ProfilePicture = "https://example.com/profile.jpg",
            LoginMethod = LoginMethod.Google,
            Roles = ["User", "Admin"],
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Act
        _context.Users.Add(user);
        var savedCount = await _context.SaveChangesAsync();

        // Assert
        savedCount.Should().Be(1);

        // Retrieve and verify
        var retrievedUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == "integration@test.com");

        retrievedUser.Should().NotBeNull();
        retrievedUser!.PostgresId.Should().BeGreaterThan(0); // Auto-generated
        retrievedUser.Id.Should().Be("507f1f77bcf86cd799439011"); // MongoDB ObjectId preserved
        retrievedUser.GoogleId.Should().Be("google_integration_test");
        retrievedUser.DisplayName.Should().Be("Integration Test User");
        retrievedUser.Gender.Should().Be(Gender.Male);
        retrievedUser.UnitPreference.Should().Be(UnitPreference.Metric);
        retrievedUser.Roles.Should().Equal(["User", "Admin"]);
    }

    [Fact]
    public async Task Should_Create_And_Retrieve_UserMetric_With_PostgreSQL_Database()
    {
        // Arrange - First create a user
        var user = new User
        {
            Id = "507f1f77bcf86cd799439012",
            GoogleId = "google_metric_test",
            Email = "metric@test.com",
            DisplayName = "Metric Test User",
            UnitPreference = UnitPreference.Imperial,
            LoginMethod = LoginMethod.Google,
            Roles = ["User"],
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var userMetric = new UserMetric
        {
            Id = "507f1f77bcf86cd799439013",
            UserId = user.Id, // Reference to MongoDB ObjectId
            Date = DateTime.UtcNow.Date,
            Weight = 75.5m,
            Height = 180.0m,
            Bmi = 23.3,
            Notes = "Weekly measurement",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Act
        _context.UserMetrics.Add(userMetric);
        var savedCount = await _context.SaveChangesAsync();

        // Assert
        savedCount.Should().Be(1);

        // Retrieve and verify
        var retrievedMetric = await _context.UserMetrics
            .FirstOrDefaultAsync(um => um.UserId == user.Id);

        retrievedMetric.Should().NotBeNull();
        retrievedMetric!.PostgresId.Should().BeGreaterThan(0); // Auto-generated
        retrievedMetric.Id.Should().Be("507f1f77bcf86cd799439013"); // MongoDB ObjectId preserved
        retrievedMetric.UserId.Should().Be(user.Id);
        retrievedMetric.Weight.Should().Be(75.5m);
        retrievedMetric.Height.Should().Be(180.0m);
        retrievedMetric.Bmi.Should().Be(23.3);
        retrievedMetric.Notes.Should().Be("Weekly measurement");
    }

    [Fact]
    public async Task Should_Create_And_Retrieve_OutboxMessage_With_PostgreSQL_Database()
    {
        // Arrange
        var outboxMessage = new OutboxMessageEntity
        {
            EventId = Guid.NewGuid().ToString(),
            EventType = "UserProfileUpdatedEvent",
            EventData = """{"UserId": 123, "DisplayName": "Test User"}""",
            CreatedAt = DateTime.UtcNow,
            IsProcessed = false,
            RetryCount = 0,
            CorrelationId = Guid.NewGuid().ToString(),
            TraceId = Guid.NewGuid().ToString()
        };

        // Act
        _context.OutboxMessages.Add(outboxMessage);
        var savedCount = await _context.SaveChangesAsync();

        // Assert
        savedCount.Should().Be(1);

        // Retrieve and verify
        var retrievedMessage = await _context.OutboxMessages
            .FirstOrDefaultAsync(om => om.EventId == outboxMessage.EventId);

        retrievedMessage.Should().NotBeNull();
        retrievedMessage!.Id.Should().BeGreaterThan(0); // Auto-generated
        retrievedMessage.EventType.Should().Be("UserProfileUpdatedEvent");
        retrievedMessage.EventData.Should().Contain("Test User");
        retrievedMessage.IsProcessed.Should().BeFalse();
        retrievedMessage.RetryCount.Should().Be(0);
    }

    [Fact]
    public async Task Should_Handle_User_Updates_And_Track_UpdatedAt()
    {
        // Arrange
        var user = new User
        {
            Id = "507f1f77bcf86cd799439014",
            GoogleId = "google_update_test",
            Email = "update@test.com",
            DisplayName = "Original Name",
            UnitPreference = UnitPreference.Metric,
            LoginMethod = LoginMethod.Google,
            Roles = ["User"],
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var originalUpdateTime = user.UpdatedAt;
        await Task.Delay(100); // Ensure time difference

        // Act
        user.DisplayName = "Updated Name";
        user.UnitPreference = UnitPreference.Imperial;
        
        await _context.SaveChangesAsync(); // This should trigger automatic UpdatedAt update

        // Assert
        var updatedUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == user.Id);

        updatedUser.Should().NotBeNull();
        updatedUser!.DisplayName.Should().Be("Updated Name");
        updatedUser.UnitPreference.Should().Be(UnitPreference.Imperial);
        updatedUser.UpdatedAt.Should().BeAfter(originalUpdateTime);
    }

    [Fact]
    public async Task Should_Handle_Complex_Query_With_Indexes()
    {
        // Arrange - Create multiple users
        var users = new[]
        {
            new User
            {
                Id = "507f1f77bcf86cd799439015",
                GoogleId = "google_query1",
                Email = "query1@test.com",
                DisplayName = "Query User 1",
                Gender = Gender.Male,
                UnitPreference = UnitPreference.Metric,
                LoginMethod = LoginMethod.Google,
                Roles = ["User"],
                CreatedAt = DateTime.UtcNow.AddDays(-10),
                UpdatedAt = DateTime.UtcNow.AddDays(-10)
            },
            new User
            {
                Id = "507f1f77bcf86cd799439016",
                GoogleId = "google_query2",
                Email = "query2@test.com",
                DisplayName = "Query User 2",
                Gender = Gender.Female,
                UnitPreference = UnitPreference.Imperial,
                LoginMethod = LoginMethod.Google,
                Roles = ["User", "Admin"],
                CreatedAt = DateTime.UtcNow.AddDays(-5),
                UpdatedAt = DateTime.UtcNow.AddDays(-5)
            }
        };

        _context.Users.AddRange(users);
        await _context.SaveChangesAsync();

        // Act & Assert - Test various queries that should use indexes

        // Query by email (unique index)
        var userByEmail = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == "query1@test.com");
        userByEmail.Should().NotBeNull();
        userByEmail!.DisplayName.Should().Be("Query User 1");

        // Query by GoogleId (unique index)
        var userByGoogleId = await _context.Users
            .FirstOrDefaultAsync(u => u.GoogleId == "google_query2");
        userByGoogleId.Should().NotBeNull();
        userByGoogleId!.DisplayName.Should().Be("Query User 2");

        // Query by roles (array contains)
        var adminUsers = await _context.Users
            .Where(u => u.Roles.Contains("Admin"))
            .ToListAsync();
        adminUsers.Should().HaveCount(1);
        adminUsers[0].Email.Should().Be("query2@test.com");

        // Complex query with multiple conditions
        var filteredUsers = await _context.Users
            .Where(u => u.Gender == Gender.Male && u.UnitPreference == UnitPreference.Metric)
            .OrderBy(u => u.CreatedAt)
            .ToListAsync();
        filteredUsers.Should().HaveCount(1);
        filteredUsers[0].Email.Should().Be("query1@test.com");
    }

    [Fact]
    public async Task Should_Handle_Transaction_Rollback_On_Error()
    {
        // Arrange
        var user1 = new User
        {
            Id = "507f1f77bcf86cd799439017",
            GoogleId = "google_transaction1",
            Email = "transaction1@test.com",
            DisplayName = "Transaction User 1",
            LoginMethod = LoginMethod.Google,
            Roles = ["User"],
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var user2 = new User
        {
            Id = "507f1f77bcf86cd799439018",
            GoogleId = "google_transaction1", // Duplicate GoogleId - should cause constraint violation
            Email = "transaction2@test.com",
            DisplayName = "Transaction User 2",
            LoginMethod = LoginMethod.Google,
            Roles = ["User"],
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Act & Assert
        using var transaction = await _context.Database.BeginTransactionAsync();
        
        _context.Users.Add(user1);
        await _context.SaveChangesAsync(); // This should succeed
        
        _context.Users.Add(user2);
        
        // This should fail due to unique constraint on GoogleId
        var exception = await Assert.ThrowsAsync<DbUpdateException>(async () =>
        {
            await _context.SaveChangesAsync();
        });

        await transaction.RollbackAsync();

        // Verify rollback - neither user should exist
        var userCount = await _context.Users
            .CountAsync(u => u.GoogleId == "google_transaction1");
        userCount.Should().Be(0);
    }

    [Fact]
    public async Task Should_Support_Concurrent_Access()
    {
        // Arrange
        var baseUser = new User
        {
            Id = "507f1f77bcf86cd799439019",
            GoogleId = "google_concurrent",
            Email = "concurrent@test.com",
            DisplayName = "Concurrent User",
            LoginMethod = LoginMethod.Google,
            Roles = ["User"],
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Users.Add(baseUser);
        await _context.SaveChangesAsync();

        // Act - Simulate concurrent access
        var tasks = Enumerable.Range(1, 5).Select(async i =>
        {
            using var concurrentContext = _fixture.CreateDbContext<UsersDbContext>();
            
            var metric = new UserMetric
            {
                Id = $"507f1f77bcf86cd79943902{i:D1}",
                UserId = baseUser.Id,
                Date = DateTime.UtcNow.Date.AddDays(-i),
                Weight = 70.0m + i,
                Height = 180.0m,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            concurrentContext.UserMetrics.Add(metric);
            return await concurrentContext.SaveChangesAsync();
        });

        var results = await Task.WhenAll(tasks);

        // Assert
        results.Should().AllSatisfy(result => result.Should().Be(1));

        // Verify all metrics were saved
        var savedMetrics = await _context.UserMetrics
            .Where(um => um.UserId == baseUser.Id)
            .ToListAsync();
        savedMetrics.Should().HaveCount(5);
    }
}
