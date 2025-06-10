using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace dotFitness.Modules.Users.Application.Configuration;

/// <summary>
/// Interface for configuring the Users module services and infrastructure
/// </summary>
public interface IUsersModuleConfiguration
{
    /// <summary>
    /// Adds all Users module services and configuration to the dependency injection container
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration to bind settings from</param>
    /// <returns>The service collection for chaining</returns>
    IServiceCollection AddUsersModule(IServiceCollection services, IConfiguration configuration);

    /// <summary>
    /// Configures MongoDB indexes for Users module entities
    /// </summary>
    /// <param name="services">The service provider</param>
    /// <returns>Task representing the async operation</returns>
    Task ConfigureUsersModuleIndexes(IServiceProvider services);
}
