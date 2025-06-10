using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace dotFitness.Api.Infrastructure;

/// <summary>
/// Centralized module registry for automatic discovery and registration of all modules
/// </summary>
public static class ModuleRegistry
{
    /// <summary>
    /// Gets all module names that should be registered
    /// This can be extended as new modules are added
    /// </summary>
    public static readonly string[] ModuleNames = 
    {
        "Users",
        "Exercises", 
        "Routines",
        "WorkoutLogs"
    };

    /// <summary>
    /// Registers all module services automatically
    /// </summary>
    /// <param name="services">Service collection</param>  
    /// <param name="configuration">Configuration</param>
    public static void RegisterAllModules(IServiceCollection services, IConfiguration configuration)
    {
        foreach (var moduleName in ModuleNames)
        {
            try
            {
                RegisterModule(services, configuration, moduleName);
            }
            catch (Exception ex)
            {
                Log.Warning("Could not register module {ModuleName}: {Error}", moduleName, ex.Message);
            }
        }
    }

    /// <summary>
    /// Registers a specific module using reflection
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="configuration">Configuration</param>
    /// <param name="moduleName">Name of the module (e.g., "Users", "Exercises")</param>
    private static void RegisterModule(IServiceCollection services, IConfiguration configuration, string moduleName)
    {
        try
        {
            // Try to load the Application assembly and find the registration method
            var applicationAssemblyName = $"dotFitness.Modules.{moduleName}.Application";
            var applicationAssembly = System.Reflection.Assembly.Load(applicationAssemblyName);
            
            // Look for module registration class
            var registrationTypeName = $"dotFitness.Modules.{moduleName}.Application.Configuration.{moduleName}ModuleRegistration";
            var registrationType = applicationAssembly.GetType(registrationTypeName);
            
            if (registrationType != null)
            {
                // Look for AddXModule extension method
                var addModuleMethod = registrationType.GetMethod($"Add{moduleName}Module", 
                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                
                if (addModuleMethod != null)
                {
                    addModuleMethod.Invoke(null, new object[] { services, configuration });
                    Log.Information("Successfully registered module: {ModuleName}", moduleName);
                    return;
                }
            }
            
            Log.Warning("Could not find registration method for module: {ModuleName}", moduleName);
        }
        catch (Exception ex)
        {
            Log.Warning("Failed to register module {ModuleName}: {Error}", moduleName, ex.Message);
        }
    }

    /// <summary>
    /// Automatically discovers and registers MediatR assemblies for all modules
    /// </summary>
    /// <param name="cfg">MediatR configuration</param>
    public static void RegisterModuleAssemblies(Microsoft.Extensions.DependencyInjection.MediatRServiceConfiguration cfg)
    {
        try
        {
            var moduleAssemblies = new List<System.Reflection.Assembly>();

            foreach (var moduleName in ModuleNames)
            {
                // Try to load Application assembly
                try
                {
                    var applicationAssemblyName = $"dotFitness.Modules.{moduleName}.Application";
                    var applicationAssembly = System.Reflection.Assembly.Load(applicationAssemblyName);
                    moduleAssemblies.Add(applicationAssembly);
                    Log.Information("Loaded {ModuleName} Application assembly for MediatR", moduleName);
                }
                catch (Exception ex)
                {
                    Log.Warning("Could not load {ModuleName} Application assembly: {Error}", moduleName, ex.Message);
                }

                // Try to load Infrastructure assembly  
                try
                {
                    var infrastructureAssemblyName = $"dotFitness.Modules.{moduleName}.Infrastructure";
                    var infrastructureAssembly = System.Reflection.Assembly.Load(infrastructureAssemblyName);
                    moduleAssemblies.Add(infrastructureAssembly);
                    Log.Information("Loaded {ModuleName} Infrastructure assembly for MediatR", moduleName);
                }
                catch (Exception ex)
                {
                    Log.Warning("Could not load {ModuleName} Infrastructure assembly: {Error}", moduleName, ex.Message);
                }
            }

            // Register all discovered assemblies with MediatR
            foreach (var assembly in moduleAssemblies)
            {
                cfg.RegisterServicesFromAssembly(assembly);
                Log.Information("Registered MediatR services from assembly: {AssemblyName}", assembly.GetName().Name);
            }
            
            Log.Information("MediatR registration completed for {Count} module assemblies", moduleAssemblies.Count);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error during MediatR module assembly registration");
            throw;
        }
    }

    /// <summary>
    /// Configures MongoDB indexes for all modules
    /// </summary>
    /// <param name="services">Service provider</param>
    public static async Task ConfigureAllModuleIndexes(IServiceProvider services)
    {
        foreach (var moduleName in ModuleNames)
        {
            try
            {
                await ConfigureModuleIndexes(services, moduleName);
            }
            catch (Exception ex)
            {
                Log.Warning("Could not configure indexes for module {ModuleName}: {Error}", moduleName, ex.Message);
            }
        }
    }

    /// <summary>
    /// Configures MongoDB indexes for a specific module
    /// </summary>
    /// <param name="services">Service provider</param>
    /// <param name="moduleName">Name of the module</param>
    private static async Task ConfigureModuleIndexes(IServiceProvider services, string moduleName)
    {
        try
        {
            // Try to load the Application assembly and find the index configuration method
            var applicationAssemblyName = $"dotFitness.Modules.{moduleName}.Application";
            var applicationAssembly = System.Reflection.Assembly.Load(applicationAssemblyName);
            
            // Look for module registration class
            var registrationTypeName = $"dotFitness.Modules.{moduleName}.Application.Configuration.{moduleName}ModuleRegistration";
            var registrationType = applicationAssembly.GetType(registrationTypeName);
            
            if (registrationType != null)
            {
                // Look for Configure*ModuleIndexes method
                var configureIndexesMethod = registrationType.GetMethod($"Configure{moduleName}ModuleIndexes", 
                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                
                if (configureIndexesMethod != null)
                {
                    var task = (Task?)configureIndexesMethod.Invoke(null, new object[] { services });
                    if (task != null)
                    {
                        await task;
                        Log.Information("Successfully configured indexes for module: {ModuleName}", moduleName);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Log.Warning("Failed to configure indexes for module {ModuleName}: {Error}", moduleName, ex.Message);
        }
    }
}
