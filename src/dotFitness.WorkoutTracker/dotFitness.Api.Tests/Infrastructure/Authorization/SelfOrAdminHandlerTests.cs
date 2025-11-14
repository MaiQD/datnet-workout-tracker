using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Moq;
using FluentAssertions;
using dotFitness.Api.Infrastructure.Authorization;
using System.Security.Claims;
using dotFitness.Common.Services;

namespace dotFitness.Api.Tests.Infrastructure.Authorization;

public class SelfOrAdminHandlerTests
{
    private readonly Mock<ISecurityAuditService> _auditServiceMock;
    private readonly SelfOrAdminHandler _handler;

    public SelfOrAdminHandlerTests()
    {
        _auditServiceMock = new Mock<ISecurityAuditService>();
        _handler = new SelfOrAdminHandler(_auditServiceMock.Object);
    }

    [Fact]
    public async Task HandleRequirementAsync_Should_Succeed_When_User_Is_Admin()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Role, "Admin")
        }, "test"));

        var httpContext = new DefaultHttpContext();
        httpContext.Request.RouteValues["userId"] = Guid.NewGuid().ToString();
        httpContext.Connection.RemoteIpAddress = System.Net.IPAddress.Parse("127.0.0.1");
        httpContext.Request.Headers.UserAgent = "TestAgent";

        var context = new AuthorizationHandlerContext(
            new[] { new SelfOrAdminRequirement() },
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
                It.IsAny<string>(),
                "SelfOrAdmin",
                true,
                "Admin access granted",
                "127.0.0.1",
                "TestAgent"),
            Times.Once);
    }

    [Fact]
    public async Task HandleRequirementAsync_Should_Succeed_When_User_Accesses_Own_Resource()
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
            new[] { new SelfOrAdminRequirement() },
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
                "SelfOrAdmin",
                true,
                "Self access granted",
                "127.0.0.1",
                "TestAgent"),
            Times.Once);
    }

    [Fact]
    public async Task HandleRequirementAsync_Should_Fail_When_User_Accesses_Other_User_Resource()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Role, "User")
        }, "test"));

        var httpContext = new DefaultHttpContext();
        httpContext.Request.RouteValues["userId"] = otherUserId.ToString();
        httpContext.Connection.RemoteIpAddress = System.Net.IPAddress.Parse("127.0.0.1");
        httpContext.Request.Headers.UserAgent = "TestAgent";

        var context = new AuthorizationHandlerContext(
            new[] { new SelfOrAdminRequirement() },
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
                otherUserId.ToString(),
                "SelfOrAdmin",
                false,
                "Access denied - not self or admin",
                "127.0.0.1",
                "TestAgent"),
            Times.Once);
    }

    [Fact]
    public async Task HandleRequirementAsync_Should_Fail_When_No_UserId_In_Route()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Role, "User")
        }, "test"));

        var httpContext = new DefaultHttpContext();
        // No userId in route
        httpContext.Connection.RemoteIpAddress = System.Net.IPAddress.Parse("127.0.0.1");
        httpContext.Request.Headers.UserAgent = "TestAgent";

        var context = new AuthorizationHandlerContext(
            new[] { new SelfOrAdminRequirement() },
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
                "Unknown",
                "SelfOrAdmin",
                false,
                "Access denied - not self or admin",
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
            new[] { new SelfOrAdminRequirement() },
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
                "SelfOrAdmin",
                true,
                "Self access granted",
                "127.0.0.1",
                "TestAgent"),
            Times.Once);
    }
}
