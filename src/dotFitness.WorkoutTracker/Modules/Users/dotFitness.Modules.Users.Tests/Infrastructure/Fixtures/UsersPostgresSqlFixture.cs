using dotFitness.SharedKernel.Tests.PostgreSQL;
using dotFitness.Modules.Users.Infrastructure.Data;

namespace dotFitness.Modules.Users.Tests.Infrastructure.Fixtures;

/// <summary>
/// PostgreSQL fixture for Users module integration tests
/// </summary>
public class UsersPostgresSqlFixture : PostgresSqlFixture
{
    private const string UsersSchema = "users";

    /// <summary>
    /// Creates a UsersDbContext with the test database connection
    /// </summary>
    public UsersDbContext CreateUsersDbContext()
    {
        return CreateDbContext<UsersDbContext>(UsersSchema);
    }

    /// <summary>
    /// Creates a fresh UsersDbContext with a unique database name for test isolation
    /// </summary>
    public UsersDbContext CreateFreshUsersDbContext()
    {
        return CreateFreshDbContext<UsersDbContext>(UsersSchema);
    }
}

[CollectionDefinition("UsersPostgreSQL.Shared")]
public class UsersPostgresSqlSharedCollectionFixture : ICollectionFixture<UsersPostgresSqlFixture> { }
