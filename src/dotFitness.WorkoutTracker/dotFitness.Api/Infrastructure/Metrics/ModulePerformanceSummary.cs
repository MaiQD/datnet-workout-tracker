namespace dotFitness.Api.Infrastructure.Metrics;

/// <summary>
/// Overall performance summary for all modules
/// </summary>
public class ModulePerformanceSummary
{
    public int TotalModules { get; set; }
    public int SuccessfulModules { get; set; }
    public int FailedModules { get; set; }
    public double SuccessRate => TotalModules > 0 ? (double)SuccessfulModules / TotalModules : 0;
    public TimeSpan AverageRegistrationTime { get; set; }
    public TimeSpan TotalRegistrationTime { get; set; }
    public TimeSpan TotalAssemblyLoadTime { get; set; }
    public TimeSpan TotalMediatRRegistrationTime { get; set; }
    public TimeSpan TotalIndexConfigurationTime { get; set; }
    public int TotalHandlersRegistered { get; set; }
    public int TotalIndexesConfigured { get; set; }
    public TimeSpan TotalModuleSetupTime => TotalRegistrationTime + TotalAssemblyLoadTime + TotalMediatRRegistrationTime + TotalIndexConfigurationTime;
} 