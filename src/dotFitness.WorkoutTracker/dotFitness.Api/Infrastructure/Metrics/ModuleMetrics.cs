using Microsoft.Extensions.Logging;

namespace dotFitness.Api.Infrastructure.Metrics;

/// <summary>
/// Metrics collection for module registration and performance monitoring
/// </summary>
public static class ModuleMetrics
{
    private static readonly Dictionary<string, ModuleRegistrationMetrics> _moduleMetrics = new();
    private static readonly object _lockObject = new();

    /// <summary>
    /// Records module registration metrics
    /// </summary>
    /// <param name="moduleName">Name of the module</param>
    /// <param name="success">Whether registration was successful</param>
    /// <param name="duration">Time taken for registration</param>
    /// <param name="logger">Logger for recording metrics</param>
    public static void RecordModuleRegistration(string moduleName, bool success, TimeSpan duration, ILogger logger)
    {
        lock (_lockObject)
        {
            if (!_moduleMetrics.ContainsKey(moduleName))
            {
                _moduleMetrics[moduleName] = new ModuleRegistrationMetrics(moduleName);
            }

            var metrics = _moduleMetrics[moduleName];
            metrics.RecordRegistration(success, duration);

            logger.LogInformation("Module registration metrics - {ModuleName}: Success={Success}, Duration={Duration}ms, TotalAttempts={TotalAttempts}, SuccessRate={SuccessRate:P1}",
                moduleName, success, duration.TotalMilliseconds, metrics.TotalAttempts, metrics.SuccessRate);
        }
    }

    /// <summary>
    /// Records assembly loading metrics
    /// </summary>
    /// <param name="assemblyName">Name of the assembly</param>
    /// <param name="success">Whether loading was successful</param>
    /// <param name="duration">Time taken for loading</param>
    /// <param name="logger">Logger for recording metrics</param>
    public static void RecordAssemblyLoading(string assemblyName, bool success, TimeSpan duration, ILogger logger)
    {
        lock (_lockObject)
        {
            var moduleName = ExtractModuleNameFromAssembly(assemblyName);
            if (!_moduleMetrics.ContainsKey(moduleName))
            {
                _moduleMetrics[moduleName] = new ModuleRegistrationMetrics(moduleName);
            }

            var metrics = _moduleMetrics[moduleName];
            metrics.RecordAssemblyLoading(assemblyName, success, duration);

            logger.LogDebug("Assembly loading metrics - {AssemblyName}: Success={Success}, Duration={Duration}ms",
                assemblyName, success, duration.TotalMilliseconds);
        }
    }

    /// <summary>
    /// Records MediatR registration metrics
    /// </summary>
    /// <param name="assemblyName">Name of the assembly</param>
    /// <param name="handlerCount">Number of handlers registered</param>
    /// <param name="duration">Time taken for registration</param>
    /// <param name="logger">Logger for recording metrics</param>
    public static void RecordMediatRRegistration(string assemblyName, int handlerCount, TimeSpan duration, ILogger logger)
    {
        lock (_lockObject)
        {
            var moduleName = ExtractModuleNameFromAssembly(assemblyName);
            if (!_moduleMetrics.ContainsKey(moduleName))
            {
                _moduleMetrics[moduleName] = new ModuleRegistrationMetrics(moduleName);
            }

            var metrics = _moduleMetrics[moduleName];
            metrics.RecordMediatRRegistration(assemblyName, handlerCount, duration);

            logger.LogInformation("MediatR registration metrics - {AssemblyName}: Handlers={HandlerCount}, Duration={Duration}ms",
                assemblyName, handlerCount, duration.TotalMilliseconds);
        }
    }

    /// <summary>
    /// Records MongoDB index configuration metrics
    /// </summary>
    /// <param name="moduleName">Name of the module</param>
    /// <param name="indexCount">Number of indexes configured</param>
    /// <param name="duration">Time taken for configuration</param>
    /// <param name="logger">Logger for recording metrics</param>
    public static void RecordIndexConfiguration(string moduleName, int indexCount, TimeSpan duration, ILogger logger)
    {
        lock (_lockObject)
        {
            if (!_moduleMetrics.ContainsKey(moduleName))
            {
                _moduleMetrics[moduleName] = new ModuleRegistrationMetrics(moduleName);
            }

            var metrics = _moduleMetrics[moduleName];
            metrics.RecordIndexConfiguration(indexCount, duration);

            logger.LogInformation("Index configuration metrics - {ModuleName}: Indexes={IndexCount}, Duration={Duration}ms",
                moduleName, indexCount, duration.TotalMilliseconds);
        }
    }

    /// <summary>
    /// Gets comprehensive metrics for all modules
    /// </summary>
    /// <returns>Dictionary of module metrics</returns>
    public static Dictionary<string, ModuleRegistrationMetrics> GetAllMetrics()
    {
        lock (_lockObject)
        {
            return new Dictionary<string, ModuleRegistrationMetrics>(_moduleMetrics);
        }
    }

    /// <summary>
    /// Gets metrics for a specific module
    /// </summary>
    /// <param name="moduleName">Name of the module</param>
    /// <returns>Module metrics or null if not found</returns>
    public static ModuleRegistrationMetrics? GetModuleMetrics(string moduleName)
    {
        lock (_lockObject)
        {
            return _moduleMetrics.TryGetValue(moduleName, out var metrics) ? metrics : null;
        }
    }

    /// <summary>
    /// Gets overall registration performance summary
    /// </summary>
    /// <returns>Performance summary</returns>
    public static ModulePerformanceSummary GetPerformanceSummary()
    {
        lock (_lockObject)
        {
            var summary = new ModulePerformanceSummary();
            var allMetrics = _moduleMetrics.Values.ToList();

            if (!allMetrics.Any())
            {
                return summary;
            }

            summary.TotalModules = allMetrics.Count;
            summary.SuccessfulModules = allMetrics.Count(m => m.SuccessRate > 0);
            summary.FailedModules = allMetrics.Count(m => m.SuccessRate == 0);
            summary.AverageRegistrationTime = TimeSpan.FromMilliseconds(allMetrics.Average(m => m.AverageRegistrationTime.TotalMilliseconds));
            summary.TotalRegistrationTime = TimeSpan.FromMilliseconds(allMetrics.Sum(m => m.TotalRegistrationTime.TotalMilliseconds));
            summary.TotalAssemblyLoadTime = TimeSpan.FromMilliseconds(allMetrics.Sum(m => m.TotalAssemblyLoadTime.TotalMilliseconds));
            summary.TotalMediatRRegistrationTime = TimeSpan.FromMilliseconds(allMetrics.Sum(m => m.TotalMediatRRegistrationTime.TotalMilliseconds));
            summary.TotalIndexConfigurationTime = TimeSpan.FromMilliseconds(allMetrics.Sum(m => m.TotalIndexConfigurationTime.TotalMilliseconds));
            summary.TotalHandlersRegistered = allMetrics.Sum(m => m.TotalHandlersRegistered);
            summary.TotalIndexesConfigured = allMetrics.Sum(m => m.TotalIndexesConfigured);

            return summary;
        }
    }

    /// <summary>
    /// Clears all metrics (useful for testing)
    /// </summary>
    public static void ClearMetrics()
    {
        lock (_lockObject)
        {
            _moduleMetrics.Clear();
        }
    }

    /// <summary>
    /// Extracts module name from assembly name
    /// </summary>
    /// <param name="assemblyName">Full assembly name</param>
    /// <returns>Module name</returns>
    private static string ExtractModuleNameFromAssembly(string assemblyName)
    {
        // Extract module name from assembly name like "dotFitness.Modules.Users.Application"
        var parts = assemblyName.Split('.');
        if (parts.Length >= 3 && parts[0] == "dotFitness" && parts[1] == "Modules")
        {
            return parts[2];
        }
        return "Unknown";
    }
} 