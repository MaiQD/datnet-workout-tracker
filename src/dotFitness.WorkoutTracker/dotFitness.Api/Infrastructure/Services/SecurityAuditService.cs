using dotFitness.Common.Security;
using dotFitness.Common.Services;

namespace dotFitness.Api.Infrastructure.Services;

/// <summary>
/// Service for logging security events and authorization attempts
/// </summary>
public class SecurityAuditService : ISecurityAuditService
{
    private readonly ILogger<SecurityAuditService> _logger;

    public SecurityAuditService(ILogger<SecurityAuditService> logger)
    {
        _logger = logger;
    }

    public async Task LogSecurityEventAsync(
        string eventType,
        Guid? userId = null,
        Dictionary<string, object>? details = null,
        string? ipAddress = null,
        string? userAgent = null,
        string? correlationId = null,
        string? traceId = null)
    {
        var logLevel = LogLevel.Information;
        
        _logger.Log(logLevel,
            "Security event: {EventType} for user {UserId}. Details: {Details}. IP: {IpAddress}, UserAgent: {UserAgent}, CorrelationId: {CorrelationId}, TraceId: {TraceId}",
            eventType, userId, details, ipAddress, userAgent, correlationId, traceId);

        await Task.CompletedTask;
    }

    public async Task LogAuthorizationAttemptAsync(
        Guid userId,
        string resourceType,
        string resourceId,
        string action,
        bool authorized,
        string? reason = null,
        string? ipAddress = null,
        string? userAgent = null)
    {
        var logLevel = authorized ? LogLevel.Information : LogLevel.Warning;
        var eventType = authorized ? SecurityEventTypes.ResourceAccessAttempt : SecurityEventTypes.UnauthorizedAccess;

        _logger.Log(logLevel,
            "Authorization attempt: User {UserId} attempted {Action} on {ResourceType} {ResourceId}. Authorized: {Authorized}. Reason: {Reason}. IP: {IpAddress}, UserAgent: {UserAgent}",
            userId, action, resourceType, resourceId, authorized, reason, ipAddress, userAgent);

        await Task.CompletedTask;
    }

    public async Task LogResourceAccessAsync(
        Guid userId,
        string resourceType,
        string resourceId,
        string action,
        string? ipAddress = null,
        string? userAgent = null)
    {
        var logLevel = LogLevel.Information;

        _logger.Log(logLevel,
            "Resource access: User {UserId} performed {Action} on {ResourceType} {ResourceId}. IP: {IpAddress}, UserAgent: {UserAgent}",
            userId, action, resourceType, resourceId, ipAddress, userAgent);

        await Task.CompletedTask;
    }

    public async Task LogSuspiciousActivityAsync(
        string eventType,
        Guid? userId = null,
        Dictionary<string, object>? details = null,
        string severity = "Medium",
        string? ipAddress = null,
        string? userAgent = null)
    {
        var logLevel = severity switch
        {
            "Low" => LogLevel.Information,
            "Medium" => LogLevel.Warning,
            "High" => LogLevel.Error,
            "Critical" => LogLevel.Critical,
            _ => LogLevel.Warning
        };

        _logger.Log(logLevel,
            "Suspicious activity: {EventType} for user {UserId}. Severity: {Severity}. Details: {Details}. IP: {IpAddress}, UserAgent: {UserAgent}",
            eventType, userId, severity, details, ipAddress, userAgent);

        await Task.CompletedTask;
    }
}
