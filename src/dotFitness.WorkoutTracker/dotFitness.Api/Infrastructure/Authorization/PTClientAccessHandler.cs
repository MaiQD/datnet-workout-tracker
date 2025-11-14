using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using dotFitness.Api.Infrastructure.Extensions;
using dotFitness.Common.Services;
using dotFitness.Modules.Users.Infrastructure.Data;

namespace dotFitness.Api.Infrastructure.Authorization;

/// <summary>
/// Authorization handler for PTClientAccess policy - allows access if user is admin or PT assigned to the client
/// </summary>
public class PTClientAccessHandler(UsersDbContext context, ISecurityAuditService auditService)
    : AuthorizationHandler<PTClientAccessRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context1, PTClientAccessRequirement requirement)
    {
        var httpContext = context1.Resource as HttpContext;
        var currentUserId = context1.User.GetRequiredUserId();
        var clientUserId = GetClientUserIdFromRoute(httpContext!);
        var ipAddress = httpContext?.Connection.RemoteIpAddress?.ToString();
        var userAgent = httpContext?.Request.Headers.UserAgent.ToString();

        // Check if user is admin
        if (context1.User.IsInRole("Admin"))
        {
            await auditService.LogAuthorizationAttemptAsync(
                currentUserId,
                "PTClient",
                clientUserId?.ToString() ?? "Unknown",
                "PTClientAccess",
                true,
                "Admin access granted",
                ipAddress,
                userAgent);

            context1.Succeed(requirement);
            return;
        }

        // Check if user is PT
        if (!context1.User.IsInRole("PT"))
        {
            await auditService.LogAuthorizationAttemptAsync(
                currentUserId,
                "PTClient",
                clientUserId?.ToString() ?? "Unknown",
                "PTClientAccess",
                false,
                "Access denied - not PT or admin",
                ipAddress,
                userAgent);

            context1.Fail();
            return;
        }

        if (httpContext == null)
        {
            await auditService.LogAuthorizationAttemptAsync(
                currentUserId,
                "PTClient",
                clientUserId?.ToString() ?? "Unknown",
                "PTClientAccess",
                false,
                "HTTP context not available",
                ipAddress,
                userAgent);

            context1.Fail();
            return;
        }

        if (!clientUserId.HasValue)
        {
            await auditService.LogAuthorizationAttemptAsync(
                currentUserId,
                "PTClient",
                "Unknown",
                "PTClientAccess",
                false,
                "Client user ID not found in request",
                ipAddress,
                userAgent);

            context1.Fail();
            return;
        }

        // Check if current PT is assigned to the client
        var client = await context.Users
            .FirstOrDefaultAsync(u => u.Id == clientUserId.Value);

        if (client != null && client.AssignedPtId == currentUserId)
        {
            await auditService.LogAuthorizationAttemptAsync(
                currentUserId,
                "PTClient",
                clientUserId.Value.ToString(),
                "PTClientAccess",
                true,
                "PT client access granted",
                ipAddress,
                userAgent);

            context1.Succeed(requirement);
            return;
        }

        await auditService.LogAuthorizationAttemptAsync(
            currentUserId,
            "PTClient",
            clientUserId.Value.ToString(),
            "PTClientAccess",
            false,
            "Access denied - not assigned PT",
            ipAddress,
            userAgent);

        context1.Fail();
    }

    private static Guid? GetClientUserIdFromRoute(HttpContext httpContext)
    {
        // Try to get client user ID from route parameters
        if (httpContext.Request.RouteValues.TryGetValue("userId", out var userIdValue) ||
            httpContext.Request.RouteValues.TryGetValue("clientId", out userIdValue))
        {
            if (Guid.TryParse(userIdValue?.ToString(), out var userId))
            {
                return userId;
            }
        }

        // Try to get client user ID from query parameters
        if (httpContext.Request.Query.TryGetValue("userId", out var queryUserId) ||
            httpContext.Request.Query.TryGetValue("clientId", out queryUserId))
        {
            if (Guid.TryParse(queryUserId, out var userId))
            {
                return userId;
            }
        }

        return null;
    }
}
