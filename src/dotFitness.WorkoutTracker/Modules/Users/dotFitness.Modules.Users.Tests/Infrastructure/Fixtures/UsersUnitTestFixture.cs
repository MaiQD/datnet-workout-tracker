using Microsoft.EntityFrameworkCore;
using dotFitness.Modules.Users.Infrastructure.Data;

namespace dotFitness.Modules.Users.Tests.Infrastructure.Fixtures;

/// <summary>
/// Fixture for Users module unit tests that use in-memory databases
/// </summary>
public class UsersUnitTestFixture : BaseUnitTestFixture
{
    public UsersUnitTestFixture() : base(ConfigureUsersOptions)
    {
    }

    private static void ConfigureUsersOptions(DbContextOptionsBuilder optionsBuilder)
    {
        // Add any Users-specific configuration here
        // For example, if we need to configure specific providers or options
    }

    /// <summary>
    /// Creates a UsersDbContext with in-memory database for unit tests
    /// </summary>
    public UsersDbContext CreateUsersDbContext()
    {
        return CreateInMemoryDbContext<UsersDbContext>();
    }
}

[CollectionDefinition("UsersUnitTests")]
public class UsersUnitTestCollectionFixture : ICollectionFixture<UsersUnitTestFixture> { }
