using Microsoft.AspNetCore.Authorization;
using dotFitness.Api.Infrastructure.Extensions;
using dotFitness.Common.Services;

namespace dotFitness.Api.Infrastructure.Authorization;

/// <summary>
/// Authorization handler for SelfOrAdmin policy - allows access if user is accessing their own resource or is an admin
/// </summary>
public class SelfOrAdminHandler(ISecurityAuditService auditService) : AuthorizationHandler<SelfOrAdminRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, SelfOrAdminRequirement requirement)
    {
        var httpContext = context.Resource as HttpContext;
        var currentUserId = context.User.GetRequiredUserId();
        var requestedUserId = GetUserIdFromRoute(httpContext);
        var ipAddress = httpContext?.Connection.RemoteIpAddress?.ToString();
        var userAgent = httpContext?.Request.Headers.UserAgent.ToString();

        // Check if user is admin
        if (context.User.IsInRole("Admin"))
        {
            await auditService.LogAuthorizationAttemptAsync(
                currentUserId,
                "User",
                requestedUserId?.ToString() ?? "Unknown",
                "SelfOrAdmin",
                true,
                "Admin access granted",
                ipAddress,
                userAgent);

            context.Succeed(requirement);
            return;
        }

        // Check if user is accessing their own resource
        if (requestedUserId.HasValue && currentUserId == requestedUserId.Value)
        {
            await auditService.LogAuthorizationAttemptAsync(
                currentUserId,
                "User",
                requestedUserId.Value.ToString(),
                "SelfOrAdmin",
                true,
                "Self access granted",
                ipAddress,
                userAgent);

            context.Succeed(requirement);
            return;
        }

        await auditService.LogAuthorizationAttemptAsync(
            currentUserId,
            "User",
            requestedUserId?.ToString() ?? "Unknown",
            "SelfOrAdmin",
            false,
            "Access denied - not self or admin",
            ipAddress,
            userAgent);

        context.Fail();
    }

    private static Guid? GetUserIdFromRoute(HttpContext? httpContext)
    {
        if (httpContext == null) return null;

        // Try to get userId from route parameters
        if (httpContext.Request.RouteValues.TryGetValue("userId", out var userIdValue) ||
            httpContext.Request.RouteValues.TryGetValue("id", out userIdValue))
        {
            if (Guid.TryParse(userIdValue?.ToString(), out var userId))
            {
                return userId;
            }
        }

        // Try to get userId from query parameters
        if (httpContext.Request.Query.TryGetValue("userId", out var queryUserId))
        {
            if (Guid.TryParse(queryUserId, out var userId))
            {
                return userId;
            }
        }

        return null;
    }
}
