using Microsoft.Extensions.Diagnostics.HealthChecks;
using dotFitness.ModuleContracts;

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
    private readonly IEnumerable<IModuleInstaller> _installers;

    public UsersModuleHealthCheck(ILogger<UsersModuleHealthCheck> logger, IEnumerable<IModuleInstaller> installers)
    {
        _logger = logger;
        _installers = installers;
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var installed = _installers.Any(i => i.GetType().Name.Contains("UsersModuleInstaller"));
        if (!installed)
        {
            _logger.LogWarning("Users module installer not found in DI");
            return Task.FromResult(HealthCheckResult.Unhealthy("Users module not registered"));
        }
        return Task.FromResult(HealthCheckResult.Healthy("Users module registered"));
    }
}

/// <summary>
/// Health check for the Exercises module
/// </summary>
public class ExercisesModuleHealthCheck : IHealthCheck
{
    private readonly ILogger<ExercisesModuleHealthCheck> _logger;
    private readonly IEnumerable<IModuleInstaller> _installers;

    public ExercisesModuleHealthCheck(ILogger<ExercisesModuleHealthCheck> logger, IEnumerable<IModuleInstaller> installers)
    {
        _logger = logger;
        _installers = installers;
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var installed = _installers.Any(i => i.GetType().Name.Contains("ExercisesModuleInstaller"));
        if (!installed)
        {
            _logger.LogWarning("Exercises module installer not found in DI");
            return Task.FromResult(HealthCheckResult.Unhealthy("Exercises module not registered"));
        }
        return Task.FromResult(HealthCheckResult.Healthy("Exercises module registered"));
    }
}

/// <summary>
/// Health check for the module registry system
/// </summary>
public class ModuleRegistryHealthCheck : IHealthCheck
{
    private readonly ILogger<ModuleRegistryHealthCheck> _logger;
    private readonly IEnumerable<IModuleInstaller> _installers;

    public ModuleRegistryHealthCheck(ILogger<ModuleRegistryHealthCheck> logger, IEnumerable<IModuleInstaller> installers)
    {
        _logger = logger;
        _installers = installers;
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var installerNames = _installers.Select(i => i.GetType().Name).OrderBy(n => n).ToList();
            var total = installerNames.Count;
            if (total == 0)
            {
                _logger.LogError("No module installers registered in DI");
                return Task.FromResult(HealthCheckResult.Unhealthy("No module installers registered"));
            }

            var data = new Dictionary<string, object>
            {
                { "total_installers", total },
                { "installers", installerNames }
            };

            _logger.LogDebug("Module installers registered: {Installers}", string.Join(", ", installerNames));
            return Task.FromResult(HealthCheckResult.Healthy("Module installers registered", data));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during module registry health check");
            return Task.FromResult(HealthCheckResult.Unhealthy("Module registry health check failed", ex));
        }
    }
} 