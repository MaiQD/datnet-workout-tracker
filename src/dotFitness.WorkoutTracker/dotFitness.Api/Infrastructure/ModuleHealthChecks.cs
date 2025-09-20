using Microsoft.Extensions.Diagnostics.HealthChecks;
using dotFitness.ModuleContracts;

namespace dotFitness.Api.Infrastructure;

/// <summary>
/// Health checks for global/shared components (modules register their own health checks)
/// </summary>
public static class ModuleHealthChecks
{
    /// <summary>
    /// Adds health checks for global/shared components (modules handle their own)
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddGlobalHealthChecks(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddCheck<ModuleRegistryHealthCheck>("module-registry", tags: ["global", "registry", "live"]);

        return services;
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