using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using dotFitness.Api.Infrastructure.Metrics;

namespace dotFitness.Api.Infrastructure;

/// <summary>
/// Centralized module registry for automatic discovery and registration of all modules
/// </summary>
public static class ModuleRegistry
{
    private static ILogger? _logger;

    /// <summary>
    /// Sets the logger instance for the module registry
    /// </summary>
    /// <param name="logger">Logger instance</param>
    public static void SetLogger(ILogger logger)
    {
        _logger = logger;
    }

    private static void LogInformation(string message, params object[] args)
    {
        _logger?.LogInformation(message, args);
    }

    private static void LogWarning(string message, params object[] args)
    {
        _logger?.LogWarning(message, args);
    }

    private static void LogError(Exception ex, string message, params object[] args)
    {
        _logger?.LogError(ex, message, args);
    }

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
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                RegisterModule(services, configuration, moduleName);
                stopwatch.Stop();
                ModuleMetrics.RecordModuleRegistration(moduleName, true, stopwatch.Elapsed, _logger!);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                ModuleMetrics.RecordModuleRegistration(moduleName, false, stopwatch.Elapsed, _logger!);
                LogWarning("Could not register module {ModuleName}: {Error}", moduleName, ex.Message);
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
            
            // Pre-load Infrastructure assembly from the file system to ensure it's available
            var infrastructureAssemblyName = $"dotFitness.Modules.{moduleName}.Infrastructure";
            var infrastructureAssemblyPath = Path.Combine(AppContext.BaseDirectory, $"{infrastructureAssemblyName}.dll");
            if (File.Exists(infrastructureAssemblyPath))
            {
                try
                {
                    System.Reflection.Assembly.LoadFrom(infrastructureAssemblyPath);
                    LogInformation("Loaded Infrastructure assembly for module: {ModuleName}", moduleName);
                }
                catch (Exception ex)
                {
                    LogWarning("Could not load Infrastructure assembly for module {ModuleName}: {Error}", moduleName, ex.Message);
                }
            }
            else
            {
                LogWarning("Infrastructure assembly not found at path: {Path}", infrastructureAssemblyPath);
            }
            
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
                    LogInformation("Successfully registered module: {ModuleName}", moduleName);
                    return;
                }
            }
            
            LogWarning("Could not find registration method for module: {ModuleName}", moduleName);
        }
        catch (Exception ex)
        {
            LogWarning("Failed to register module {ModuleName}: {Error}", moduleName, ex.Message);
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
                    var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                    
                    // Use LoadFrom for Application assemblies to ensure they're loaded from the file system
                    var applicationAssemblyPath = Path.Combine(AppContext.BaseDirectory, $"{applicationAssemblyName}.dll");
                    System.Reflection.Assembly applicationAssembly;
                    
                    if (File.Exists(applicationAssemblyPath))
                    {
                        applicationAssembly = System.Reflection.Assembly.LoadFrom(applicationAssemblyPath);
                    }
                    else
                    {
                        // Fallback to Load if LoadFrom fails
                        applicationAssembly = System.Reflection.Assembly.Load(applicationAssemblyName);
                    }
                    
                    stopwatch.Stop();
                    ModuleMetrics.RecordAssemblyLoading(applicationAssemblyName, true, stopwatch.Elapsed, _logger!);
                    moduleAssemblies.Add(applicationAssembly);
                    LogInformation("Loaded {ModuleName} Application assembly for MediatR", moduleName);
                }
                catch (Exception ex)
                {
                    var applicationAssemblyName = $"dotFitness.Modules.{moduleName}.Application";
                    ModuleMetrics.RecordAssemblyLoading(applicationAssemblyName, false, TimeSpan.Zero, _logger!);
                    LogWarning("Could not load {ModuleName} Application assembly: {Error}", moduleName, ex.Message);
                }

                // Try to load Infrastructure assembly  
                try
                {
                    var infrastructureAssemblyName = $"dotFitness.Modules.{moduleName}.Infrastructure";
                    var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                    
                    // Use LoadFrom for Infrastructure assemblies to ensure they're loaded from the file system
                    var infrastructureAssemblyPath = Path.Combine(AppContext.BaseDirectory, $"{infrastructureAssemblyName}.dll");
                    System.Reflection.Assembly infrastructureAssembly;
                    
                    if (File.Exists(infrastructureAssemblyPath))
                    {
                        infrastructureAssembly = System.Reflection.Assembly.LoadFrom(infrastructureAssemblyPath);
                    }
                    else
                    {
                        // Fallback to Load if LoadFrom fails
                        infrastructureAssembly = System.Reflection.Assembly.Load(infrastructureAssemblyName);
                    }
                    
                    stopwatch.Stop();
                    ModuleMetrics.RecordAssemblyLoading(infrastructureAssemblyName, true, stopwatch.Elapsed, _logger!);
                    moduleAssemblies.Add(infrastructureAssembly);
                    LogInformation("Loaded {ModuleName} Infrastructure assembly for MediatR", moduleName);
                }
                catch (Exception ex)
                {
                    var infrastructureAssemblyName = $"dotFitness.Modules.{moduleName}.Infrastructure";
                    ModuleMetrics.RecordAssemblyLoading(infrastructureAssemblyName, false, TimeSpan.Zero, _logger!);
                    LogWarning("Could not load {ModuleName} Infrastructure assembly: {Error}", moduleName, ex.Message);
                }
            }

            // Register all discovered assemblies with MediatR
            foreach (var assembly in moduleAssemblies)
            {
                var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                var handlerCount = assembly.GetTypes()
                    .Count(t => t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(MediatR.IRequestHandler<,>)));
                cfg.RegisterServicesFromAssembly(assembly);
                stopwatch.Stop();
                ModuleMetrics.RecordMediatRRegistration(assembly.GetName().Name!, handlerCount, stopwatch.Elapsed, _logger!);
                LogInformation("Registered MediatR services from assembly: {AssemblyName}", assembly.GetName().Name ?? "Unknown");
            }
            
            LogInformation("MediatR registration completed for {Count} module assemblies", moduleAssemblies.Count);
        }
        catch (Exception ex)
        {
            LogError(ex, "Error during MediatR module assembly registration");
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
                LogWarning("Could not configure indexes for module {ModuleName}: {Error}", moduleName, ex.Message);
            }
        }
    }

    /// <summary>
    /// Seeds data for all modules
    /// </summary>
    public static async Task SeedAllModuleData(IServiceProvider services)
    {
        foreach (var moduleName in ModuleNames)
        {
            try
            {
                await SeedModuleData(services, moduleName);
            }
            catch (Exception ex)
            {
                LogWarning("Could not seed data for module {ModuleName}: {Error}", moduleName, ex.Message);
            }
        }
    }

    private static async Task SeedModuleData(IServiceProvider services, string moduleName)
    {
        try
        {
            var infrastructureAssemblyPath = Path.Combine(AppContext.BaseDirectory, $"dotFitness.Modules.{moduleName}.Infrastructure.dll");
            if (File.Exists(infrastructureAssemblyPath))
            {
                var infrastructureAssembly = System.Reflection.Assembly.LoadFrom(infrastructureAssemblyPath);
                var seederTypeName = $"dotFitness.Modules.{moduleName}.Infrastructure.Configuration.{moduleName}InfrastructureModule";
                var seederType = infrastructureAssembly.GetType(seederTypeName);

                if (seederType != null)
                {
                    var seedMethod = seederType.GetMethod($"Seed{moduleName}ModuleData", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                    if (seedMethod != null)
                    {
                        var task = (Task?)seedMethod.Invoke(null, new object[] { services });
                        if (task != null) await task;
                        LogInformation("Successfully seeded data for module: {ModuleName}", moduleName);
                        return;
                    }
                }
            }

            LogWarning("Could not find seeder for module: {ModuleName}", moduleName);
        }
        catch (Exception ex)
        {
            LogWarning("Failed to seed module {ModuleName}: {Error}", moduleName, ex.Message);
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
            // Pre-load Infrastructure assembly from the file system
            var infrastructureAssemblyPath = Path.Combine(AppContext.BaseDirectory, $"dotFitness.Modules.{moduleName}.Infrastructure.dll");
            if (File.Exists(infrastructureAssemblyPath))
            {
                try
                {
                    var infrastructureAssembly = System.Reflection.Assembly.LoadFrom(infrastructureAssemblyPath);
                    
                    // Look for Infrastructure module configuration class
                    var configurationTypeName = $"dotFitness.Modules.{moduleName}.Infrastructure.Configuration.{moduleName}InfrastructureModule";
                    var configurationType = infrastructureAssembly.GetType(configurationTypeName);
                    
                    if (configurationType != null)
                    {
                        // Look for Configure*ModuleIndexes method
                        var configureIndexesMethod = configurationType.GetMethod($"Configure{moduleName}ModuleIndexes", 
                            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                        
                        if (configureIndexesMethod != null)
                        {
                            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                            var task = (Task?)configureIndexesMethod.Invoke(null, new object[] { services });
                            if (task != null)
                            {
                                await task;
                                stopwatch.Stop();
                                // Estimate index count based on module name (this could be made more sophisticated)
                                var estimatedIndexCount = moduleName.ToLowerInvariant() switch
                                {
                                    "users" => 8, // User + UserMetric indexes
                                    "exercises" => 9, // Exercise + MuscleGroup + Equipment indexes
                                    _ => 5 // Default estimate
                                };
                                ModuleMetrics.RecordIndexConfiguration(moduleName, estimatedIndexCount, stopwatch.Elapsed, _logger!);
                                LogInformation("Successfully configured indexes for module: {ModuleName}", moduleName);
                                return;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogWarning("Could not configure {ModuleName} module indexes: {Error}", moduleName, ex.Message);
                }
            }
            else
            {
                LogWarning("Infrastructure assembly not found for module: {ModuleName}", moduleName);
            }
        }
        catch (Exception ex)
        {
            LogWarning("Failed to configure indexes for module {ModuleName}: {Error}", moduleName, ex.Message);
        }
    }
}
