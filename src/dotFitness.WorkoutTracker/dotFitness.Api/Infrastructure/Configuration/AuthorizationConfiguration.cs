using Microsoft.AspNetCore.Authorization;
using dotFitness.Api.Infrastructure.Authorization;
using dotFitness.Common.Authorization;
using dotFitness.Common.Services;

namespace dotFitness.Api.Infrastructure.Configuration;

public static class AuthorizationConfiguration
{
    public static IServiceCollection AddApiAuthorization(this IServiceCollection services)
    {
        services.AddAuthorizationBuilder()
            .AddPolicy(AuthorizationPolicies.AdminOnly, policy => 
                policy.RequireRole(Roles.Admin))
            .AddPolicy(AuthorizationPolicies.PtOnly, policy => 
                policy.RequireRole(Roles.Pt))
            .AddPolicy(AuthorizationPolicies.UserOnly, policy => 
                policy.RequireRole(Roles.User))
            .AddPolicy(AuthorizationPolicies.SelfOrAdmin, policy => 
                policy.Requirements.Add(new SelfOrAdminRequirement()))
            .AddPolicy(AuthorizationPolicies.ResourceOwner, policy => 
                policy.Requirements.Add(new ResourceOwnerRequirement()))
            .AddPolicy(AuthorizationPolicies.PtAssignedOrAdmin, policy => 
                policy.Requirements.Add(new PTClientAccessRequirement()))
            .AddPolicy(AuthorizationPolicies.OwnerOrPtOrAdmin, policy => 
                policy.Requirements.Add(new OwnerOrPTOrAdminRequirement()));

        // Register security audit service
        services.AddScoped<ISecurityAuditService, SecurityAuditService>();

        // Register authorization handlers
        services.AddScoped<IAuthorizationHandler, SelfOrAdminHandler>();
        services.AddScoped<IAuthorizationHandler, ResourceOwnerHandler>();
        services.AddScoped<IAuthorizationHandler, PTClientAccessHandler>();
        services.AddScoped<IAuthorizationHandler, OwnerOrPTOrAdminHandler>();

        return services;
    }
}
