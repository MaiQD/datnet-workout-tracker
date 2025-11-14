using dotFitness.Common.Security;
using Microsoft.Extensions.Logging;
using Moq;

namespace dotFitness.Api.Tests.Infrastructure.Services;

public class SecurityAuditServiceTests
{
    private readonly Mock<ILogger<dotFitness.Api.Infrastructure.Services.SecurityAuditService>> _loggerMock;
    private readonly dotFitness.Api.Infrastructure.Services.SecurityAuditService _service;

    public SecurityAuditServiceTests()
    {
        _loggerMock = new Mock<ILogger<dotFitness.Api.Infrastructure.Services.SecurityAuditService>>();
        _service = new dotFitness.Api.Infrastructure.Services.SecurityAuditService(_loggerMock.Object);
    }

    [Fact]
    public async Task LogAuthorizationAttemptAsync_Should_Log_Successful_Access()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var resourceType = "Exercise";
        var resourceId = Guid.NewGuid().ToString();
        var action = "Read";
        var isSuccess = true;
        var reason = "Self access granted";
        var ipAddress = "127.0.0.1";
        var userAgent = "TestAgent";

        // Act
        await _service.LogAuthorizationAttemptAsync(
            userId,
            resourceType,
            resourceId,
            action,
            isSuccess,
            reason,
            ipAddress,
            userAgent);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Authorization attempt")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task LogAuthorizationAttemptAsync_Should_Log_Failed_Access()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var resourceType = "Exercise";
        var resourceId = Guid.NewGuid().ToString();
        var action = "Read";
        var isSuccess = false;
        var reason = "Access denied - not self or admin";
        var ipAddress = "127.0.0.1";
        var userAgent = "TestAgent";

        // Act
        await _service.LogAuthorizationAttemptAsync(
            userId,
            resourceType,
            resourceId,
            action,
            isSuccess,
            reason,
            ipAddress,
            userAgent);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Authorization attempt")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task LogSecurityEventAsync_Should_Log_Security_Event()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var eventType = SecurityEventTypes.LoginSuccess;
        var details = new Dictionary<string, object> { ["message"] = "User logged in successfully" };
        var ipAddress = "127.0.0.1";
        var userAgent = "TestAgent";

        // Act
        await _service.LogSecurityEventAsync(
            eventType,
            userId,
            details,
            ipAddress,
            userAgent);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Security event")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task LogResourceAccessAsync_Should_Log_Resource_Access()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var resourceType = "Exercise";
        var resourceId = Guid.NewGuid().ToString();
        var action = "Read";
        var ipAddress = "127.0.0.1";
        var userAgent = "TestAgent";

        // Act
        await _service.LogResourceAccessAsync(
            userId,
            resourceType,
            resourceId,
            action,
            ipAddress,
            userAgent);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Resource access")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
