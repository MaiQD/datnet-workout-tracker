using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace dotFitness.Modules.Exercises.Application.Configuration;

/// <summary>
/// Static class for registering Exercises module from the Application layer perspective
/// </summary>
public static class ExercisesModuleRegistration
{
    /// <summary>
    /// Adds the Exercises module to the dependency injection container
    /// This method uses reflection to find and invoke the Infrastructure layer's registration
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration to bind settings from</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddExercisesModule(this IServiceCollection services, IConfiguration configuration)
    {
        try
        {
            // Try to find the Infrastructure assembly from already loaded assemblies
            var infrastructureAssembly = System.AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(a => a.GetName().Name == "dotFitness.Modules.Exercises.Infrastructure");
            
            if (infrastructureAssembly == null)
            {
                // Fallback: try to load it directly
                infrastructureAssembly = System.Reflection.Assembly.Load("dotFitness.Modules.Exercises.Infrastructure");
            }
            
            var moduleType = infrastructureAssembly.GetType("dotFitness.Modules.Exercises.Infrastructure.Configuration.ExercisesInfrastructureModule");
            var addModuleMethod = moduleType?.GetMethod("AddExercisesModule", 
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            
            if (addModuleMethod != null)
            {
                addModuleMethod.Invoke(null, new object[] { services, configuration });
                return services;
            }
            else
            {
                throw new InvalidOperationException("Could not find AddExercisesModule method in Exercises Infrastructure layer");
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Could not load Exercises Infrastructure module: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Configures MongoDB indexes for Exercises module entities
    /// </summary>
    /// <param name="services">The service provider</param>
    /// <returns>Task representing the async operation</returns>
    public static async Task ConfigureExercisesModuleIndexes(IServiceProvider services)
    {
        try
        {
            // Load the Infrastructure assembly by name
            var infrastructureAssembly = System.Reflection.Assembly.Load("dotFitness.Modules.Exercises.Infrastructure");
            var moduleType = infrastructureAssembly.GetType("dotFitness.Modules.Exercises.Infrastructure.Configuration.ExercisesInfrastructureModule");
            var configureIndexesMethod = moduleType?.GetMethod("ConfigureExercisesModuleIndexes", 
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
                throw new InvalidOperationException("Could not find ConfigureExercisesModuleIndexes method in Exercises Infrastructure layer");
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Could not configure Exercises module indexes: {ex.Message}", ex);
        }
    }
}
