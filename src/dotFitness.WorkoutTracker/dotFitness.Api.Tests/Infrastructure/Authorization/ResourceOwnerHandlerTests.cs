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

public class ResourceOwnerHandlerTests
{
    private readonly Mock<ISecurityAuditService> _auditServiceMock;
    private readonly Mock<UsersDbContext> _contextMock;
    private readonly ResourceOwnerHandler _handler;

    public ResourceOwnerHandlerTests()
    {
        _auditServiceMock = new Mock<ISecurityAuditService>();
        var options = new DbContextOptionsBuilder<UsersDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _contextMock = new Mock<UsersDbContext>(options);
        _handler = new ResourceOwnerHandler(_contextMock.Object, _auditServiceMock.Object);
    }

    [Fact]
    public async Task HandleRequirementAsync_Should_Succeed_When_User_Is_Admin()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var resourceUserId = Guid.NewGuid();
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Role, "Admin")
        }, "test"));

        var httpContext = new DefaultHttpContext();
        httpContext.Request.RouteValues["userId"] = resourceUserId.ToString();
        httpContext.Connection.RemoteIpAddress = System.Net.IPAddress.Parse("127.0.0.1");
        httpContext.Request.Headers.UserAgent = "TestAgent";

        var context = new AuthorizationHandlerContext(
            new[] { new ResourceOwnerRequirement() },
            claimsPrincipal,
            httpContext);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        context.HasSucceeded.Should().BeTrue();
        _auditServiceMock.Verify(
            x => x.LogAuthorizationAttemptAsync(
                userId,
                "Admin",
                resourceUserId.ToString(),
                "ResourceOwner",
                true,
                "Admin access granted",
                "127.0.0.1",
                "TestAgent"),
            Times.Once);
    }

    [Fact]
    public async Task HandleRequirementAsync_Should_Succeed_When_User_Is_Resource_Owner()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Role, "User")
        }, "test"));

        var httpContext = new DefaultHttpContext();
        httpContext.Request.RouteValues["userId"] = userId.ToString();
        httpContext.Connection.RemoteIpAddress = System.Net.IPAddress.Parse("127.0.0.1");
        httpContext.Request.Headers.UserAgent = "TestAgent";

        var context = new AuthorizationHandlerContext(
            new[] { new ResourceOwnerRequirement() },
            claimsPrincipal,
            httpContext);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        context.HasSucceeded.Should().BeTrue();
        _auditServiceMock.Verify(
            x => x.LogAuthorizationAttemptAsync(
                userId,
                "User",
                userId.ToString(),
                "ResourceOwner",
                true,
                "Resource owner access granted",
                "127.0.0.1",
                "TestAgent"),
            Times.Once);
    }

    [Fact]
    public async Task HandleRequirementAsync_Should_Fail_When_User_Is_Not_Resource_Owner_Or_Admin()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var resourceUserId = Guid.NewGuid();
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Role, "User")
        }, "test"));

        var httpContext = new DefaultHttpContext();
        httpContext.Request.RouteValues["userId"] = resourceUserId.ToString();
        httpContext.Connection.RemoteIpAddress = System.Net.IPAddress.Parse("127.0.0.1");
        httpContext.Request.Headers.UserAgent = "TestAgent";

        var context = new AuthorizationHandlerContext(
            new[] { new ResourceOwnerRequirement() },
            claimsPrincipal,
            httpContext);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        context.HasFailed.Should().BeTrue();
        _auditServiceMock.Verify(
            x => x.LogAuthorizationAttemptAsync(
                userId,
                "User",
                resourceUserId.ToString(),
                "ResourceOwner",
                false,
                "Access denied - not resource owner or admin",
                "127.0.0.1",
                "TestAgent"),
            Times.Once);
    }

    [Fact]
    public async Task HandleRequirementAsync_Should_Handle_Query_Parameter_UserId()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Role, "User")
        }, "test"));

        var httpContext = new DefaultHttpContext();
        httpContext.Request.QueryString = new QueryString($"?userId={userId}");
        httpContext.Connection.RemoteIpAddress = System.Net.IPAddress.Parse("127.0.0.1");
        httpContext.Request.Headers.UserAgent = "TestAgent";

        var context = new AuthorizationHandlerContext(
            new[] { new ResourceOwnerRequirement() },
            claimsPrincipal,
            httpContext);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        context.HasSucceeded.Should().BeTrue();
        _auditServiceMock.Verify(
            x => x.LogAuthorizationAttemptAsync(
                userId,
                "User",
                userId.ToString(),
                "ResourceOwner",
                true,
                "Resource owner access granted",
                "127.0.0.1",
                "TestAgent"),
            Times.Once);
    }

    [Fact]
    public async Task HandleRequirementAsync_Should_Fail_When_No_UserId_In_Route_Or_Query()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Role, "User")
        }, "test"));

        var httpContext = new DefaultHttpContext();
        // No userId in route or query
        httpContext.Connection.RemoteIpAddress = System.Net.IPAddress.Parse("127.0.0.1");
        httpContext.Request.Headers.UserAgent = "TestAgent";

        var context = new AuthorizationHandlerContext(
            new[] { new ResourceOwnerRequirement() },
            claimsPrincipal,
            httpContext);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        context.HasFailed.Should().BeTrue();
        _auditServiceMock.Verify(
            x => x.LogAuthorizationAttemptAsync(
                userId,
                "Resource",
                "Unknown",
                "ResourceOwner",
                false,
                "Resource ID not found in request",
                "127.0.0.1",
                "TestAgent"),
            Times.Once);
    }
}
