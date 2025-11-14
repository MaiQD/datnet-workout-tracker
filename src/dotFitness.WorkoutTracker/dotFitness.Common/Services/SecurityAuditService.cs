using dotFitness.Common.Security;
using Microsoft.Extensions.Logging;

namespace dotFitness.Common.Services;

/// <summary>
/// Implementation of ISecurityAuditService using structured logging
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
        var logData = new Dictionary<string, object>
        {
            ["EventType"] = eventType,
            ["Timestamp"] = DateTime.UtcNow,
            ["CorrelationId"] = correlationId ?? Guid.NewGuid().ToString(),
            ["TraceId"] = traceId ?? Guid.NewGuid().ToString()
        };

        if (userId.HasValue)
        {
            logData["UserId"] = userId.Value;
        }

        if (!string.IsNullOrEmpty(ipAddress))
        {
            logData["IpAddress"] = ipAddress;
        }

        if (!string.IsNullOrEmpty(userAgent))
        {
            logData["UserAgent"] = userAgent;
        }

        if (details != null)
        {
            foreach (var detail in details)
            {
                logData[detail.Key] = detail.Value;
            }
        }

        // Log with appropriate level based on event type
        var logLevel = GetLogLevelForEventType(eventType);
        
        _logger.Log(logLevel, "Security Event: {EventType} - {LogData}", eventType, logData);

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
        var logData = new Dictionary<string, object>
        {
            ["UserId"] = userId,
            ["ResourceType"] = resourceType,
            ["ResourceId"] = resourceId,
            ["Action"] = action,
            ["Authorized"] = authorized,
            ["Timestamp"] = DateTime.UtcNow
        };

        if (!string.IsNullOrEmpty(reason))
        {
            logData["Reason"] = reason;
        }

        if (!string.IsNullOrEmpty(ipAddress))
        {
            logData["IpAddress"] = ipAddress;
        }

        if (!string.IsNullOrEmpty(userAgent))
        {
            logData["UserAgent"] = userAgent;
        }

        var logLevel = authorized ? LogLevel.Information : LogLevel.Warning;
        _logger.Log(logLevel, "Authorization Attempt: {Action} on {ResourceType}:{ResourceId} - Authorized: {Authorized} - {LogData}", 
            action, resourceType, resourceId, authorized, logData);

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
        var logData = new Dictionary<string, object>
        {
            ["UserId"] = userId,
            ["ResourceType"] = resourceType,
            ["ResourceId"] = resourceId,
            ["Action"] = action,
            ["Timestamp"] = DateTime.UtcNow
        };

        if (!string.IsNullOrEmpty(ipAddress))
        {
            logData["IpAddress"] = ipAddress;
        }

        if (!string.IsNullOrEmpty(userAgent))
        {
            logData["UserAgent"] = userAgent;
        }

        _logger.LogInformation("Resource Access: {Action} on {ResourceType}:{ResourceId} - {LogData}", 
            action, resourceType, resourceId, logData);

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
        var logData = new Dictionary<string, object>
        {
            ["EventType"] = eventType,
            ["Severity"] = severity,
            ["Timestamp"] = DateTime.UtcNow
        };

        if (userId.HasValue)
        {
            logData["UserId"] = userId.Value;
        }

        if (!string.IsNullOrEmpty(ipAddress))
        {
            logData["IpAddress"] = ipAddress;
        }

        if (!string.IsNullOrEmpty(userAgent))
        {
            logData["UserAgent"] = userAgent;
        }

        if (details != null)
        {
            foreach (var detail in details)
            {
                logData[detail.Key] = detail.Value;
            }
        }

        var logLevel = GetLogLevelForSeverity(severity);
        _logger.Log(logLevel, "Suspicious Activity: {EventType} - Severity: {Severity} - {LogData}", 
            eventType, severity, logData);

        await Task.CompletedTask;
    }

    private static LogLevel GetLogLevelForEventType(string eventType)
    {
        return eventType switch
        {
            SecurityEventTypes.LoginSuccess => LogLevel.Information,
            SecurityEventTypes.LoginFailure => LogLevel.Warning,
            SecurityEventTypes.Logout => LogLevel.Information,
            SecurityEventTypes.TokenRefresh => LogLevel.Information,
            SecurityEventTypes.UnauthorizedAccess => LogLevel.Warning,
            SecurityEventTypes.ForbiddenAccess => LogLevel.Warning,
            SecurityEventTypes.ResourceAccessAttempt => LogLevel.Information,
            SecurityEventTypes.ResourceModificationAttempt => LogLevel.Information,
            SecurityEventTypes.UserCreation => LogLevel.Information,
            SecurityEventTypes.UserUpdate => LogLevel.Information,
            SecurityEventTypes.RoleAssignment => LogLevel.Information,
            SecurityEventTypes.PasswordChange => LogLevel.Information,
            SecurityEventTypes.AccountLockout => LogLevel.Warning,
            SecurityEventTypes.AccountUnlock => LogLevel.Information,
            SecurityEventTypes.RateLimitExceeded => LogLevel.Warning,
            _ => LogLevel.Information
        };
    }

    private static LogLevel GetLogLevelForSeverity(string severity)
    {
        return severity.ToLowerInvariant() switch
        {
            "low" => LogLevel.Information,
            "medium" => LogLevel.Warning,
            "high" => LogLevel.Error,
            "critical" => LogLevel.Critical,
            _ => LogLevel.Warning
        };
    }
}
