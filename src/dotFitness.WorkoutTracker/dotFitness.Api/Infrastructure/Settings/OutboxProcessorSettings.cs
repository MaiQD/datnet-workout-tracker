namespace dotFitness.Api.Infrastructure.Settings;

/// <summary>
/// Configuration settings for the Outbox Processor Service
/// </summary>
public class OutboxProcessorSettings
{
    /// <summary>
    /// Processing interval in seconds. Default is 10 seconds.
    /// </summary>
    public int IntervalSeconds { get; set; } = 10;

    /// <summary>
    /// Maximum number of retry attempts for failed messages. Default is 3.
    /// </summary>
    public int MaxRetryAttempts { get; set; } = 3;

    /// <summary>
    /// Batch size for processing messages. Default is 50.
    /// </summary>
    public int BatchSize { get; set; } = 50;
}
