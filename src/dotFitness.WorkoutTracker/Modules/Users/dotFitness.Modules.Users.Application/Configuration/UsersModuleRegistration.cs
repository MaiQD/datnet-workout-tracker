using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace dotFitness.Modules.Users.Application.Configuration;

/// <summary>
/// Static class for registering Users module from the Application layer perspective
/// </summary>
public static class UsersModuleRegistration
{
    /// <summary>
    /// Adds the Users module to the dependency injection container
    /// This method uses reflection to find and invoke the Infrastructure layer's registration
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration to bind settings from</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddUsersModule(this IServiceCollection services, IConfiguration configuration)
    {
        try
        {
            // Try to find the Infrastructure assembly from already loaded assemblies
            var infrastructureAssembly = System.AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(a => a.GetName().Name == "dotFitness.Modules.Users.Infrastructure");
            
            if (infrastructureAssembly == null)
            {
                // Fallback: try to load it directly
                infrastructureAssembly = System.Reflection.Assembly.Load("dotFitness.Modules.Users.Infrastructure");
            }
            
            var moduleType = infrastructureAssembly.GetType("dotFitness.Modules.Users.Infrastructure.Configuration.UsersInfrastructureModule");
            var addModuleMethod = moduleType?.GetMethod("AddUsersModule", 
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            
            if (addModuleMethod != null)
            {
                addModuleMethod.Invoke(null, new object[] { services, configuration });
                return services;
            }
            else
            {
                throw new InvalidOperationException("Could not find AddUsersModule method in Users Infrastructure layer");
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Could not load Users Infrastructure module: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Configures MongoDB indexes for Users module entities
    /// </summary>
    /// <param name="services">The service provider</param>
    /// <returns>Task representing the async operation</returns>
    public static async Task ConfigureUsersModuleIndexes(IServiceProvider services)
    {
        try
        {
            // Try to find the Infrastructure assembly from already loaded assemblies
            var infrastructureAssembly = System.AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(a => a.GetName().Name == "dotFitness.Modules.Users.Infrastructure");
            
            if (infrastructureAssembly == null)
            {
                // Fallback: try to load it directly
                infrastructureAssembly = System.Reflection.Assembly.Load("dotFitness.Modules.Users.Infrastructure");
            }
            
            var moduleType = infrastructureAssembly.GetType("dotFitness.Modules.Users.Infrastructure.Configuration.UsersInfrastructureModule");
            var configureIndexesMethod = moduleType?.GetMethod("ConfigureUsersModuleIndexes", 
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            
            if (configureIndexesMethod != null)
            {
                var task = (Task?)configureIndexesMethod.Invoke(null, new object[] { services });
                if (task != null)
                {
                    await task;
                }
            }
            else
            {
                throw new InvalidOperationException("Could not find ConfigureUsersModuleIndexes method in Users Infrastructure layer");
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Could not configure Users module indexes: {ex.Message}", ex);
        }
    }
}
