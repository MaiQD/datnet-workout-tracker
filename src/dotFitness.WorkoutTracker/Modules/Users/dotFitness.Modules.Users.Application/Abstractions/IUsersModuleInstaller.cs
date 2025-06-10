using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace dotFitness.Modules.Users.Application.Abstractions;

/// <summary>
/// Interface for Users module infrastructure registration
/// This allows the Application layer to define the contract without depending on Infrastructure
/// </summary>
public interface IUsersModuleInstaller
{
    /// <summary>
    /// Registers all Users module infrastructure services
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="configuration">Configuration</param>
    /// <returns>Service collection for chaining</returns>
    IServiceCollection Install(IServiceCollection services, IConfiguration configuration);

    /// <summary>
    /// Configures MongoDB indexes for Users module
    /// </summary>
    /// <param name="services">Service provider</param>
    /// <returns>Task representing the async operation</returns>
    Task ConfigureIndexes(IServiceProvider services);
}
