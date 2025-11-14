using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using dotFitness.Api.Infrastructure.Extensions;
using dotFitness.Common.Services;
using dotFitness.Modules.Users.Infrastructure.Data;

namespace dotFitness.Api.Infrastructure.Authorization;

/// <summary>
/// Authorization handler for OwnerOrPTOrAdmin policy - allows access if user owns the resource, is assigned PT, or is admin
/// </summary>
public class OwnerOrPTOrAdminHandler(UsersDbContext context, ISecurityAuditService auditService)
    : AuthorizationHandler<OwnerOrPTOrAdminRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context1, OwnerOrPTOrAdminRequirement requirement)
    {
        var httpContext = context1.Resource as HttpContext;
        var currentUserId = context1.User.GetRequiredUserId();
        var resourceUserId = GetResourceUserIdFromRoute(httpContext!);
        var ipAddress = httpContext?.Connection.RemoteIpAddress?.ToString();
        var userAgent = httpContext?.Request.Headers.UserAgent.ToString();

        // Check if user is admin
        if (context1.User.IsInRole("Admin"))
        {
            await auditService.LogAuthorizationAttemptAsync(
                currentUserId,
                "Resource",
                resourceUserId?.ToString() ?? "Unknown",
                "OwnerOrPTOrAdmin",
                true,
                "Admin access granted",
                ipAddress,
                userAgent);

            context1.Succeed(requirement);
            return;
        }

        if (httpContext == null)
        {
            await auditService.LogAuthorizationAttemptAsync(
                currentUserId,
                "Resource",
                resourceUserId?.ToString() ?? "Unknown",
                "OwnerOrPTOrAdmin",
                false,
                "HTTP context not available",
                ipAddress,
                userAgent);

            context1.Fail();
            return;
        }

        if (!resourceUserId.HasValue)
        {
            await auditService.LogAuthorizationAttemptAsync(
                currentUserId,
                "Resource",
                "Unknown",
                "OwnerOrPTOrAdmin",
                false,
                "Resource user ID not found in request",
                ipAddress,
                userAgent);

            context1.Fail();
            return;
        }

        // Check if user owns the resource
        if (currentUserId == resourceUserId.Value)
        {
            await auditService.LogAuthorizationAttemptAsync(
                currentUserId,
                "Resource",
                resourceUserId.Value.ToString(),
                "OwnerOrPTOrAdmin",
                true,
                "Resource owner access granted",
                ipAddress,
                userAgent);

            context1.Succeed(requirement);
            return;
        }

        // Check if user is PT assigned to the resource owner
        if (context1.User.IsInRole("PT"))
        {
            var resourceOwner = await context.Users
                .FirstOrDefaultAsync(u => u.Id == resourceUserId.Value);

            if (resourceOwner != null && resourceOwner.AssignedPtId == currentUserId)
            {
                await auditService.LogAuthorizationAttemptAsync(
                    currentUserId,
                    "Resource",
                    resourceUserId.Value.ToString(),
                    "OwnerOrPTOrAdmin",
                    true,
                    "PT assigned access granted",
                    ipAddress,
                    userAgent);

                context1.Succeed(requirement);
                return;
            }
        }

        await auditService.LogAuthorizationAttemptAsync(
            currentUserId,
            "Resource",
            resourceUserId.Value.ToString(),
            "OwnerOrPTOrAdmin",
            false,
            "Access denied - not owner, assigned PT, or admin",
            ipAddress,
            userAgent);

        context1.Fail();
    }

    private static Guid? GetResourceUserIdFromRoute(HttpContext httpContext)
    {
        // Try to get resource user ID from route parameters
        if (httpContext.Request.RouteValues.TryGetValue("userId", out var userIdValue) ||
            httpContext.Request.RouteValues.TryGetValue("id", out userIdValue))
        {
            if (Guid.TryParse(userIdValue?.ToString(), out var userId))
            {
                return userId;
            }
        }

        // Try to get resource user ID from query parameters
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
