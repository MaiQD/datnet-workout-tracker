using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace dotFitness.Modules.Users.Tests.Infrastructure.Fixtures;

/// <summary>
/// Base fixture for unit tests that use in-memory databases
/// Can be configured with custom options for different modules
/// </summary>
public class BaseUnitTestFixture : IAsyncLifetime
{
    private readonly Action<DbContextOptionsBuilder>? _configureOptions;

    public BaseUnitTestFixture(Action<DbContextOptionsBuilder>? configureOptions = null)
    {
        _configureOptions = configureOptions;
    }

    public Task InitializeAsync() => Task.CompletedTask;
    public Task DisposeAsync() => Task.CompletedTask;

    /// <summary>
    /// Creates a DbContext with in-memory database for unit tests
    /// </summary>
    public TContext CreateInMemoryDbContext<TContext>() where TContext : DbContext
    {
        var optionsBuilder = new DbContextOptionsBuilder<TContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .ConfigureWarnings(warnings => warnings
                .Ignore(InMemoryEventId.TransactionIgnoredWarning)
                .Ignore(RelationalEventId.QueryPossibleUnintendedUseOfEqualsWarning))
            .EnableSensitiveDataLogging();

        // Apply custom configuration if provided
        _configureOptions?.Invoke(optionsBuilder);

        return (TContext)Activator.CreateInstance(typeof(TContext), optionsBuilder.Options)!;
    }
}

/// <summary>
/// Configuration options for unit test fixtures
/// </summary>
public class UnitTestFixtureOptions
{
    public bool EnableSensitiveDataLogging { get; set; } = true;
    public bool IgnoreTransactionWarnings { get; set; } = true;
    public bool IgnoreQueryWarnings { get; set; } = true;
    public string? DatabaseName { get; set; }
    public Action<DbContextOptionsBuilder>? CustomConfiguration { get; set; }
}
