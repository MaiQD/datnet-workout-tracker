using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace dotFitness.SharedKernel.Tests.PostgreSQL;

public class PostgreSqlFixture : IAsyncLifetime
{
    private static readonly Lazy<PostgreSqlFixture> _instance = new(() => new PostgreSqlFixture());
    private readonly PostgreSqlContainer _postgresContainer;
    private readonly SemaphoreSlim _initializationSemaphore = new(1, 1);
    private bool _isInitialized = false;
    private bool _isDisposed = false;
    
    public string ConnectionString { get; private set; } = string.Empty;

    public static PostgreSqlFixture Instance => _instance.Value;

    private PostgreSqlFixture()
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
            throw new ObjectDisposedException(nameof(PostgreSqlFixture));
            
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
    public TContext CreateDbContext<TContext>() where TContext : DbContext
    {
        var options = new DbContextOptionsBuilder<TContext>()
            .UseNpgsql(ConnectionString)
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
            .Options;

        return (TContext)Activator.CreateInstance(typeof(TContext), options)!;
    }
}
