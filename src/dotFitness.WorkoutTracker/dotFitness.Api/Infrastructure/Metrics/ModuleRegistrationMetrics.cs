namespace dotFitness.Api.Infrastructure.Metrics;

/// <summary>
/// Metrics for a specific module's registration process
/// </summary>
public class ModuleRegistrationMetrics
{
    private readonly List<TimeSpan> _registrationTimes = new();
    private readonly List<TimeSpan> _assemblyLoadTimes = new();
    private readonly List<TimeSpan> _mediatRRegistrationTimes = new();
    private readonly List<TimeSpan> _indexConfigurationTimes = new();
    private readonly Dictionary<string, AssemblyLoadingMetrics> _assemblyMetrics = new();

    public ModuleRegistrationMetrics(string moduleName)
    {
        ModuleName = moduleName;
        RegistrationAttempts = 0;
        SuccessfulRegistrations = 0;
        FailedRegistrations = 0;
        TotalHandlersRegistered = 0;
        TotalIndexesConfigured = 0;
    }

    public string ModuleName { get; }
    public int RegistrationAttempts { get; private set; }
    public int SuccessfulRegistrations { get; private set; }
    public int FailedRegistrations { get; private set; }
    public int TotalAttempts => RegistrationAttempts;
    public double SuccessRate => RegistrationAttempts > 0 ? (double)SuccessfulRegistrations / RegistrationAttempts : 0;

    public TimeSpan AverageRegistrationTime => _registrationTimes.Any() 
        ? TimeSpan.FromMilliseconds(_registrationTimes.Average(t => t.TotalMilliseconds)) 
        : TimeSpan.Zero;

    public TimeSpan TotalRegistrationTime => TimeSpan.FromMilliseconds(_registrationTimes.Sum(t => t.TotalMilliseconds));
    public TimeSpan TotalAssemblyLoadTime => TimeSpan.FromMilliseconds(_assemblyLoadTimes.Sum(t => t.TotalMilliseconds));
    public TimeSpan TotalMediatRRegistrationTime => TimeSpan.FromMilliseconds(_mediatRRegistrationTimes.Sum(t => t.TotalMilliseconds));
    public TimeSpan TotalIndexConfigurationTime => TimeSpan.FromMilliseconds(_indexConfigurationTimes.Sum(t => t.TotalMilliseconds));

    public int TotalHandlersRegistered { get; private set; }
    public int TotalIndexesConfigured { get; private set; }

    public Dictionary<string, AssemblyLoadingMetrics> AssemblyMetrics => new Dictionary<string, AssemblyLoadingMetrics>(_assemblyMetrics);

    public void RecordRegistration(bool success, TimeSpan duration)
    {
        RegistrationAttempts++;
        if (success)
        {
            SuccessfulRegistrations++;
        }
        else
        {
            FailedRegistrations++;
        }
        _registrationTimes.Add(duration);
    }

    public void RecordAssemblyLoading(string assemblyName, bool success, TimeSpan duration)
    {
        if (!_assemblyMetrics.ContainsKey(assemblyName))
        {
            _assemblyMetrics[assemblyName] = new AssemblyLoadingMetrics(assemblyName);
        }

        _assemblyMetrics[assemblyName].RecordLoading(success, duration);
        _assemblyLoadTimes.Add(duration);
    }

    public void RecordMediatRRegistration(string assemblyName, int handlerCount, TimeSpan duration)
    {
        if (!_assemblyMetrics.ContainsKey(assemblyName))
        {
            _assemblyMetrics[assemblyName] = new AssemblyLoadingMetrics(assemblyName);
        }

        _assemblyMetrics[assemblyName].RecordMediatRRegistration(handlerCount, duration);
        _mediatRRegistrationTimes.Add(duration);
        TotalHandlersRegistered += handlerCount;
    }

    public void RecordIndexConfiguration(int indexCount, TimeSpan duration)
    {
        _indexConfigurationTimes.Add(duration);
        TotalIndexesConfigured += indexCount;
    }
} 