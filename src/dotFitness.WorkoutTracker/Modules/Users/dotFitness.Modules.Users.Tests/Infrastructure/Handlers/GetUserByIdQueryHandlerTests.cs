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

public class GetUserByIdQueryHandlerTests : IAsyncLifetime
{
    private readonly PostgreSqlFixture _fixture;
    private readonly UserMapper _userMapper;
    private readonly ILogger<GetUserByIdQueryHandler> _logger;
    private UsersDbContext _context = null!;
    private GetUserByIdQueryHandler _handler = null!;

    public GetUserByIdQueryHandlerTests()
    {
        _fixture = PostgreSqlFixture.Instance;
        _userMapper = new UserMapper();
        _logger = new Mock<ILogger<GetUserByIdQueryHandler>>().Object;
    }

    public async Task InitializeAsync()
    {
        await _fixture.InitializeAsync();
        _context = _fixture.CreateDbContext<UsersDbContext>();
        await _context.Database.EnsureCreatedAsync();
        
        _handler = new GetUserByIdQueryHandler(
            _context,
            _userMapper,
            _logger
        );
    }

    public async Task DisposeAsync()
    {
        await _context.DisposeAsync();
        await _fixture.DisposeAsync();
    }

    [Fact]
    public async Task Should_Return_User_When_Found()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            Email = "test@example.com",
            DisplayName = "Test User",
            Gender = Gender.Male,
            UnitPreference = UnitPreference.Metric,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var query = new GetUserByIdQuery(user.Id);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Id.Should().Be(user.Id);
        result.Value.Email.Should().Be("test@example.com");
        result.Value.DisplayName.Should().Be("Test User");
    }

    [Fact]
    public async Task Should_Return_NotFound_When_User_DoesNot_Exist()
    {
        // Arrange
        var query = new GetUserByIdQuery(999); // Non-existent user

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("User not found");
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

        var query = new GetUserByIdQuery(user.Id);

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
        var query = new GetUserByIdQuery(0); // Invalid user ID

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("User not found");
    }
}