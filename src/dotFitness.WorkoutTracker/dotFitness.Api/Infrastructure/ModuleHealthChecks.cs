using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace dotFitness.Api.Infrastructure;

/// <summary>
/// Health checks for individual modules to monitor their status and dependencies
/// </summary>
public static class ModuleHealthChecks
{
    /// <summary>
    /// Adds health checks for all registered modules
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddModuleHealthChecks(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddCheck<UsersModuleHealthCheck>("users_module", tags: new[] { "module", "users" })
            .AddCheck<ExercisesModuleHealthCheck>("exercises_module", tags: new[] { "module", "exercises" })
            .AddCheck<ModuleRegistryHealthCheck>("module_registry", tags: new[] { "module", "registry" });

        return services;
    }
}

/// <summary>
/// Health check for the Users module
/// </summary>
public class UsersModuleHealthCheck : IHealthCheck
{
    private readonly ILogger<UsersModuleHealthCheck> _logger;

    public UsersModuleHealthCheck(ILogger<UsersModuleHealthCheck> logger)
    {
        _logger = logger;
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            // Check if Users module assemblies are loaded
            var applicationAssembly = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(a => a.GetName().Name == "dotFitness.Modules.Users.Application");
            
            var infrastructureAssembly = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(a => a.GetName().Name == "dotFitness.Modules.Users.Infrastructure");

            if (applicationAssembly == null || infrastructureAssembly == null)
            {
                _logger.LogWarning("Users module assemblies not found");
                return Task.FromResult(HealthCheckResult.Unhealthy("Users module assemblies not loaded"));
            }

            // Check if key types are available
            var userRepositoryType = infrastructureAssembly.GetType("dotFitness.Modules.Users.Infrastructure.Repositories.UserRepository");
            var userControllerType = applicationAssembly.GetType("dotFitness.Modules.Users.Application.Commands.LoginWithGoogleCommand");

            if (userRepositoryType == null || userControllerType == null)
            {
                _logger.LogWarning("Users module key types not found");
                return Task.FromResult(HealthCheckResult.Degraded("Users module key types not available"));
            }

            _logger.LogDebug("Users module health check passed");
            return Task.FromResult(HealthCheckResult.Healthy("Users module is healthy"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during Users module health check");
            return Task.FromResult(HealthCheckResult.Unhealthy("Users module health check failed", ex));
        }
    }
}

/// <summary>
/// Health check for the Exercises module
/// </summary>
public class ExercisesModuleHealthCheck : IHealthCheck
{
    private readonly ILogger<ExercisesModuleHealthCheck> _logger;

    public ExercisesModuleHealthCheck(ILogger<ExercisesModuleHealthCheck> logger)
    {
        _logger = logger;
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            // Check if Exercises module assemblies are loaded
            var applicationAssembly = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(a => a.GetName().Name == "dotFitness.Modules.Exercises.Application");
            
            var infrastructureAssembly = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(a => a.GetName().Name == "dotFitness.Modules.Exercises.Infrastructure");

            if (applicationAssembly == null || infrastructureAssembly == null)
            {
                _logger.LogWarning("Exercises module assemblies not found");
                return Task.FromResult(HealthCheckResult.Unhealthy("Exercises module assemblies not loaded"));
            }

            // Check if key types are available
            var exerciseRepositoryType = infrastructureAssembly.GetType("dotFitness.Modules.Exercises.Infrastructure.Repositories.ExerciseRepository");
            var createExerciseCommandType = applicationAssembly.GetType("dotFitness.Modules.Exercises.Application.Commands.CreateExerciseCommand");

            if (exerciseRepositoryType == null || createExerciseCommandType == null)
            {
                _logger.LogWarning("Exercises module key types not found");
                return Task.FromResult(HealthCheckResult.Degraded("Exercises module key types not available"));
            }

            _logger.LogDebug("Exercises module health check passed");
            return Task.FromResult(HealthCheckResult.Healthy("Exercises module is healthy"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during Exercises module health check");
            return Task.FromResult(HealthCheckResult.Unhealthy("Exercises module health check failed", ex));
        }
    }
}

/// <summary>
/// Health check for the module registry system
/// </summary>
public class ModuleRegistryHealthCheck : IHealthCheck
{
    private readonly ILogger<ModuleRegistryHealthCheck> _logger;

    public ModuleRegistryHealthCheck(ILogger<ModuleRegistryHealthCheck> logger)
    {
        _logger = logger;
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var moduleNames = ModuleRegistry.ModuleNames;
            var loadedModules = new List<string>();
            var missingModules = new List<string>();

            foreach (var moduleName in moduleNames)
            {
                var applicationAssembly = AppDomain.CurrentDomain.GetAssemblies()
                    .FirstOrDefault(a => a.GetName().Name == $"dotFitness.Modules.{moduleName}.Application");
                
                var infrastructureAssembly = AppDomain.CurrentDomain.GetAssemblies()
                    .FirstOrDefault(a => a.GetName().Name == $"dotFitness.Modules.{moduleName}.Infrastructure");

                if (applicationAssembly != null && infrastructureAssembly != null)
                {
                    loadedModules.Add(moduleName);
                }
                else
                {
                    missingModules.Add(moduleName);
                }
            }

            var data = new Dictionary<string, object>
            {
                { "total_modules", moduleNames.Length },
                { "loaded_modules", loadedModules.Count },
                { "missing_modules", missingModules.Count },
                { "loaded_module_names", loadedModules },
                { "missing_module_names", missingModules }
            };

            if (missingModules.Count == 0)
            {
                _logger.LogDebug("All modules loaded successfully");
                return Task.FromResult(HealthCheckResult.Healthy("All modules loaded successfully", data));
            }
            else if (loadedModules.Count > 0)
            {
                _logger.LogWarning("Some modules are missing: {MissingModules}", string.Join(", ", missingModules));
                return Task.FromResult(HealthCheckResult.Degraded("Some modules are missing", data: data));
            }
            else
            {
                _logger.LogError("No modules loaded");
                return Task.FromResult(HealthCheckResult.Unhealthy("No modules loaded", data: data));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during module registry health check");
            return Task.FromResult(HealthCheckResult.Unhealthy("Module registry health check failed", ex));
        }
    }
} 