using Microsoft.Extensions.DependencyInjection;
using dotFitness.Modules.Users.Domain.Repositories;
using dotFitness.Modules.Users.Infrastructure.Repositories;

namespace dotFitness.Modules.Users.Infrastructure.Configuration;

/// <summary>
/// Configuration class for Users module infrastructure services
/// </summary>
public static class UsersInfrastructureModule
{
    /// <summary>
    /// Adds Users infrastructure services to the dependency injection container
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddUsersInfrastructure(this IServiceCollection services)
    {
        // Register repositories with proper dependency injection
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserMetricsRepository, UserMetricsRepository>();

        return services;
    }
}
