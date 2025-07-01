namespace dotFitness.Api.Infrastructure.Metrics;

/// <summary>
/// Metrics for assembly loading and MediatR registration
/// </summary>
public class AssemblyLoadingMetrics
{
    private readonly List<TimeSpan> _loadingTimes = new();
    private readonly List<TimeSpan> _mediatRRegistrationTimes = new();

    public AssemblyLoadingMetrics(string assemblyName)
    {
        AssemblyName = assemblyName;
        LoadingAttempts = 0;
        SuccessfulLoadings = 0;
        FailedLoadings = 0;
        TotalHandlersRegistered = 0;
    }

    public string AssemblyName { get; }
    public int LoadingAttempts { get; private set; }
    public int SuccessfulLoadings { get; private set; }
    public int FailedLoadings { get; private set; }
    public double LoadingSuccessRate => LoadingAttempts > 0 ? (double)SuccessfulLoadings / LoadingAttempts : 0;

    public TimeSpan AverageLoadingTime => _loadingTimes.Any() 
        ? TimeSpan.FromMilliseconds(_loadingTimes.Average(t => t.TotalMilliseconds)) 
        : TimeSpan.Zero;

    public TimeSpan AverageMediatRRegistrationTime => _mediatRRegistrationTimes.Any() 
        ? TimeSpan.FromMilliseconds(_mediatRRegistrationTimes.Average(t => t.TotalMilliseconds)) 
        : TimeSpan.Zero;

    public int TotalHandlersRegistered { get; private set; }

    public void RecordLoading(bool success, TimeSpan duration)
    {
        LoadingAttempts++;
        if (success)
        {
            SuccessfulLoadings++;
        }
        else
        {
            FailedLoadings++;
        }
        _loadingTimes.Add(duration);
    }

    public void RecordMediatRRegistration(int handlerCount, TimeSpan duration)
    {
        _mediatRRegistrationTimes.Add(duration);
        TotalHandlersRegistered += handlerCount;
    }
} 