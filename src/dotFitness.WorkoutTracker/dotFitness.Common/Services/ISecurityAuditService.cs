namespace dotFitness.Common.Services;

/// <summary>
/// Service for logging security events and audit trails
/// </summary>
public interface ISecurityAuditService
{
    /// <summary>
    /// Logs a security event with structured data
    /// </summary>
    /// <param name="eventType">Type of security event</param>
    /// <param name="userId">User ID (if applicable)</param>
    /// <param name="details">Additional event details</param>
    /// <param name="ipAddress">Client IP address</param>
    /// <param name="userAgent">Client user agent</param>
    /// <param name="correlationId">Correlation ID for tracing</param>
    /// <param name="traceId">Trace ID for distributed tracing</param>
    Task LogSecurityEventAsync(
        string eventType,
        Guid? userId = null,
        Dictionary<string, object>? details = null,
        string? ipAddress = null,
        string? userAgent = null,
        string? correlationId = null,
        string? traceId = null);

    /// <summary>
    /// Logs an authorization attempt
    /// </summary>
    /// <param name="userId">User ID attempting access</param>
    /// <param name="resourceType">Type of resource being accessed</param>
    /// <param name="resourceId">ID of the resource</param>
    /// <param name="action">Action being performed</param>
    /// <param name="authorized">Whether access was granted</param>
    /// <param name="reason">Reason for denial (if applicable)</param>
    /// <param name="ipAddress">Client IP address</param>
    /// <param name="userAgent">Client user agent</param>
    Task LogAuthorizationAttemptAsync(
        Guid userId,
        string resourceType,
        string resourceId,
        string action,
        bool authorized,
        string? reason = null,
        string? ipAddress = null,
        string? userAgent = null);

    /// <summary>
    /// Logs a resource access attempt
    /// </summary>
    /// <param name="userId">User ID accessing resource</param>
    /// <param name="resourceType">Type of resource</param>
    /// <param name="resourceId">ID of the resource</param>
    /// <param name="action">Action performed</param>
    /// <param name="ipAddress">Client IP address</param>
    /// <param name="userAgent">Client user agent</param>
    Task LogResourceAccessAsync(
        Guid userId,
        string resourceType,
        string resourceId,
        string action,
        string? ipAddress = null,
        string? userAgent = null);

    /// <summary>
    /// Logs a suspicious activity
    /// </summary>
    /// <param name="eventType">Type of suspicious event</param>
    /// <param name="userId">User ID (if applicable)</param>
    /// <param name="details">Event details</param>
    /// <param name="severity">Severity level</param>
    /// <param name="ipAddress">Client IP address</param>
    /// <param name="userAgent">Client user agent</param>
    Task LogSuspiciousActivityAsync(
        string eventType,
        Guid? userId = null,
        Dictionary<string, object>? details = null,
        string severity = "Medium",
        string? ipAddress = null,
        string? userAgent = null);
}
