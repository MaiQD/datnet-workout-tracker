using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace dotFitness.SharedKernel.Tests.PostgreSQL;

public class PostgreSqlFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgresContainer;
    
    public string ConnectionString { get; private set; } = string.Empty;

    public PostgreSqlFixture()
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
        await _postgresContainer.StartAsync();
        ConnectionString = _postgresContainer.GetConnectionString();
    }

    public async Task DisposeAsync()
    {
        await _postgresContainer.DisposeAsync();
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
