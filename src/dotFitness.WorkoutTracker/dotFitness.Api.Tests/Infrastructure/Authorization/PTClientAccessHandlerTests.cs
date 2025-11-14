using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using FluentAssertions;
using dotFitness.Api.Infrastructure.Authorization;
using dotFitness.Modules.Users.Infrastructure.Data;
using System.Security.Claims;
using dotFitness.Common.Services;

namespace dotFitness.Api.Tests.Infrastructure.Authorization;

public class PTClientAccessHandlerTests
{
    private readonly Mock<ISecurityAuditService> _auditServiceMock;
    private readonly Mock<UsersDbContext> _contextMock;
    private readonly PTClientAccessHandler _handler;

    public PTClientAccessHandlerTests()
    {
        _auditServiceMock = new Mock<ISecurityAuditService>();
        var options = new DbContextOptionsBuilder<UsersDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _contextMock = new Mock<UsersDbContext>(options);
        _handler = new PTClientAccessHandler(_contextMock.Object, _auditServiceMock.Object);
    }

    [Fact]
    public async Task HandleRequirementAsync_Should_Succeed_When_User_Is_Admin()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var clientUserId = Guid.NewGuid();
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Role, "Admin")
        }, "test"));

        var httpContext = new DefaultHttpContext();
        httpContext.Request.RouteValues["userId"] = clientUserId.ToString();
        httpContext.Connection.RemoteIpAddress = System.Net.IPAddress.Parse("127.0.0.1");
        httpContext.Request.Headers.UserAgent = "TestAgent";

        var context = new AuthorizationHandlerContext(
            new[] { new PTClientAccessRequirement() },
            claimsPrincipal,
            httpContext);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        context.HasSucceeded.Should().BeTrue();
        _auditServiceMock.Verify(
            x => x.LogAuthorizationAttemptAsync(
                userId,
                "PTClient",
                clientUserId.ToString(),
                "PTClientAccess",
                true,
                "Admin access granted",
                "127.0.0.1",
                "TestAgent"),
            Times.Once);
    }

    [Fact]
    public async Task HandleRequirementAsync_Should_Succeed_When_User_Is_Assigned_PT()
    {
        // Arrange
        var ptUserId = Guid.NewGuid();
        var clientUserId = Guid.NewGuid();
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, ptUserId.ToString()),
            new Claim(ClaimTypes.Role, "PT")
        }, "test"));

        var httpContext = new DefaultHttpContext();
        httpContext.Request.RouteValues["userId"] = clientUserId.ToString();
        httpContext.Connection.RemoteIpAddress = System.Net.IPAddress.Parse("127.0.0.1");
        httpContext.Request.Headers.UserAgent = "TestAgent";

        // Create a real in-memory database context for this test
        var options = new DbContextOptionsBuilder<UsersDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        var realContext = new UsersDbContext(options);
        
        // Add a client user with the PT assigned
        var clientUser = new dotFitness.Modules.Users.Domain.Entities.ApplicationUser
        {
            Id = clientUserId,
            Email = "client@test.com",
            UserName = "client@test.com",
            DisplayName = "Client User",
            AssignedPtId = ptUserId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        realContext.Users.Add(clientUser);
        await realContext.SaveChangesAsync();

        var handler = new PTClientAccessHandler(realContext, _auditServiceMock.Object);

        var context = new AuthorizationHandlerContext(
            new[] { new PTClientAccessRequirement() },
            claimsPrincipal,
            httpContext);

        // Act
        await handler.HandleAsync(context);

        // Assert
        context.HasSucceeded.Should().BeTrue();
        _auditServiceMock.Verify(
            x => x.LogAuthorizationAttemptAsync(
                ptUserId,
                "PTClient",
                clientUserId.ToString(),
                "PTClientAccess",
                true,
                "PT client access granted",
                "127.0.0.1",
                "TestAgent"),
            Times.Once);

        await realContext.DisposeAsync();
    }

    [Fact]
    public async Task HandleRequirementAsync_Should_Fail_When_PT_Is_Not_Assigned_To_Client()
    {
        // Arrange
        var ptUserId = Guid.NewGuid();
        var clientUserId = Guid.NewGuid();
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, ptUserId.ToString()),
            new Claim(ClaimTypes.Role, "PT")
        }, "test"));

        var httpContext = new DefaultHttpContext();
        httpContext.Request.RouteValues["userId"] = clientUserId.ToString();
        httpContext.Connection.RemoteIpAddress = System.Net.IPAddress.Parse("127.0.0.1");
        httpContext.Request.Headers.UserAgent = "TestAgent";

        // Create a real in-memory database context for this test
        var options = new DbContextOptionsBuilder<UsersDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        var realContext = new UsersDbContext(options);
        
        // Add a client user with a different PT assigned
        var clientUser = new dotFitness.Modules.Users.Domain.Entities.ApplicationUser
        {
            Id = clientUserId,
            Email = "client@test.com",
            UserName = "client@test.com",
            DisplayName = "Client User",
            AssignedPtId = Guid.NewGuid(), // Different PT
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        realContext.Users.Add(clientUser);
        await realContext.SaveChangesAsync();

        var handler = new PTClientAccessHandler(realContext, _auditServiceMock.Object);

        var context = new AuthorizationHandlerContext(
            new[] { new PTClientAccessRequirement() },
            claimsPrincipal,
            httpContext);

        // Act
        await handler.HandleAsync(context);

        // Assert
        context.HasFailed.Should().BeTrue();
        _auditServiceMock.Verify(
            x => x.LogAuthorizationAttemptAsync(
                ptUserId,
                "PTClient",
                clientUserId.ToString(),
                "PTClientAccess",
                false,
                "Access denied - not assigned PT",
                "127.0.0.1",
                "TestAgent"),
            Times.Once);

        await realContext.DisposeAsync();
    }

    [Fact]
    public async Task HandleRequirementAsync_Should_Fail_When_User_Is_Not_PT_Or_Admin()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var clientUserId = Guid.NewGuid();
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Role, "User")
        }, "test"));

        var httpContext = new DefaultHttpContext();
        httpContext.Request.RouteValues["userId"] = clientUserId.ToString();
        httpContext.Connection.RemoteIpAddress = System.Net.IPAddress.Parse("127.0.0.1");
        httpContext.Request.Headers.UserAgent = "TestAgent";

        var context = new AuthorizationHandlerContext(
            new[] { new PTClientAccessRequirement() },
            claimsPrincipal,
            httpContext);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        context.HasFailed.Should().BeTrue();
        _auditServiceMock.Verify(
            x => x.LogAuthorizationAttemptAsync(
                userId,
                "PTClient",
                clientUserId.ToString(),
                "PTClientAccess",
                false,
                "Access denied - not PT or admin",
                "127.0.0.1",
                "TestAgent"),
            Times.Once);
    }

    [Fact]
    public async Task HandleRequirementAsync_Should_Handle_Query_Parameter_UserId()
    {
        // Arrange
        var ptUserId = Guid.NewGuid();
        var clientUserId = Guid.NewGuid();
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, ptUserId.ToString()),
            new Claim(ClaimTypes.Role, "PT")
        }, "test"));

        var httpContext = new DefaultHttpContext();
        httpContext.Request.QueryString = new QueryString($"?userId={clientUserId}");
        httpContext.Connection.RemoteIpAddress = System.Net.IPAddress.Parse("127.0.0.1");
        httpContext.Request.Headers.UserAgent = "TestAgent";

        // Create a real in-memory database context for this test
        var options = new DbContextOptionsBuilder<UsersDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        var realContext = new UsersDbContext(options);
        
        // Add a client user with the PT assigned
        var clientUser = new dotFitness.Modules.Users.Domain.Entities.ApplicationUser
        {
            Id = clientUserId,
            Email = "client@test.com",
            UserName = "client@test.com",
            DisplayName = "Client User",
            AssignedPtId = ptUserId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        realContext.Users.Add(clientUser);
        await realContext.SaveChangesAsync();

        var handler = new PTClientAccessHandler(realContext, _auditServiceMock.Object);

        var context = new AuthorizationHandlerContext(
            new[] { new PTClientAccessRequirement() },
            claimsPrincipal,
            httpContext);

        // Act
        await handler.HandleAsync(context);

        // Assert
        context.HasSucceeded.Should().BeTrue();
        _auditServiceMock.Verify(
            x => x.LogAuthorizationAttemptAsync(
                ptUserId,
                "PTClient",
                clientUserId.ToString(),
                "PTClientAccess",
                true,
                "PT client access granted",
                "127.0.0.1",
                "TestAgent"),
            Times.Once);

        await realContext.DisposeAsync();
    }

    [Fact]
    public async Task HandleRequirementAsync_Should_Fail_When_No_UserId_In_Route_Or_Query()
    {
        // Arrange
        var ptUserId = Guid.NewGuid();
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, ptUserId.ToString()),
            new Claim(ClaimTypes.Role, "PT")
        }, "test"));

        var httpContext = new DefaultHttpContext();
        // No userId in route or query
        httpContext.Connection.RemoteIpAddress = System.Net.IPAddress.Parse("127.0.0.1");
        httpContext.Request.Headers.UserAgent = "TestAgent";

        var context = new AuthorizationHandlerContext(
            new[] { new PTClientAccessRequirement() },
            claimsPrincipal,
            httpContext);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        context.HasFailed.Should().BeTrue();
        _auditServiceMock.Verify(
            x => x.LogAuthorizationAttemptAsync(
                ptUserId,
                "PTClient",
                "Unknown",
                "PTClientAccess",
                false,
                "Client user ID not found in request",
                "127.0.0.1",
                "TestAgent"),
            Times.Once);
    }
}
