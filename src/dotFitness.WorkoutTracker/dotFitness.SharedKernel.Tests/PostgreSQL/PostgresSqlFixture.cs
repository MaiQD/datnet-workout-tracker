using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace dotFitness.SharedKernel.Tests.PostgreSQL;

public class PostgresSqlFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgresContainer;
    private readonly SemaphoreSlim _initializationSemaphore = new(1, 1);
    private bool _isInitialized = false;
    private bool _isDisposed = false;
    
    public string ConnectionString { get; private set; } = string.Empty;

    public PostgresSqlFixture()
    {
        _postgresContainer = new PostgreSqlBuilder()
            .WithImage("postgres:16-alpine")
            .WithDatabase($"testdb_{Guid.NewGuid():N}")
            .WithUsername("testuser")
            .WithPassword("testpass")
            .WithPortBinding(0, true)
            .WithName($"test-postgres-{Guid.NewGuid():N}")
            .Build();
    }

    public async Task InitializeAsync()
    {
        if (_isDisposed)
            throw new ObjectDisposedException(nameof(PostgresSqlFixture));
            
        if (_isInitialized)
            return;

        await _initializationSemaphore.WaitAsync();
        try
        {
            if (_isInitialized)
                return;

            await _postgresContainer.StartAsync();
            ConnectionString = _postgresContainer.GetConnectionString();
            _isInitialized = true;
        }
        finally
        {
            _initializationSemaphore.Release();
        }
    }

    public async Task DisposeAsync()
    {
        // Don't dispose the static instance - let it live for the entire test run
        // This allows parallel tests to share the same database
    }

    /// <summary>
    /// Creates a DbContext with the test database connection
    /// </summary>
    public TContext CreateDbContext<TContext>(string? schema = null) where TContext : DbContext
    {
        var options = new DbContextOptionsBuilder<TContext>()
            .UseNpgsql(ConnectionString, options =>
            {
                if (!string.IsNullOrEmpty(schema))
                {
                    options.MigrationsHistoryTable("__EFMigrationsHistory", schema);
                }
                options.EnableRetryOnFailure(maxRetryCount: 3, maxRetryDelay: TimeSpan.FromSeconds(5), errorCodesToAdd: null);
            })
            .Options;

        return (TContext)Activator.CreateInstance(typeof(TContext), options)!;
    }

    /// <summary>
    /// Creates a DbContext with in-memory database for unit tests
    /// </summary>
    public TContext CreateInMemoryDbContext<TContext>() where TContext : DbContext
    {
        var options = new DbContextOptionsBuilder<TContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .ConfigureWarnings(warnings => warnings
                .Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning)
                .Throw(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.QueryPossibleUnintendedUseOfEqualsWarning))
            .EnableSensitiveDataLogging()
            .Options;

        return (TContext)Activator.CreateInstance(typeof(TContext), options)!;
    }

    /// <summary>
    /// Creates a fresh DbContext with a unique database name for test isolation
    /// </summary>
    public TContext CreateFreshDbContext<TContext>(string? schema = null) where TContext : DbContext
    {
        var uniqueDbName = $"testdb_{Guid.NewGuid():N}";
        var uniqueConnectionString = ConnectionString.Replace($"testdb_{ConnectionString.Split('_')[1].Split(';')[0]}", uniqueDbName);
        
        var options = new DbContextOptionsBuilder<TContext>()
            .UseNpgsql(uniqueConnectionString, options =>
            {
                if (!string.IsNullOrEmpty(schema))
                {
                    options.MigrationsHistoryTable("__EFMigrationsHistory", schema);
                }
                options.EnableRetryOnFailure(maxRetryCount: 3, maxRetryDelay: TimeSpan.FromSeconds(5), errorCodesToAdd: null);
            })
            .Options;

        return (TContext)Activator.CreateInstance(typeof(TContext), options)!;
    }
}

[CollectionDefinition("PostgresSQL.Shared")]
public class PostgresSqlSharedCollectionFixture : ICollectionFixture<PostgresSqlFixture> { }

