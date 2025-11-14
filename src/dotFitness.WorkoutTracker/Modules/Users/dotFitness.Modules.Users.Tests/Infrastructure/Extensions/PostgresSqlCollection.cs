using dotFitness.Common.Tests.PostgreSQL;

namespace dotFitness.Modules.Users.Tests.Infrastructure.Extensions;

[CollectionDefinition("PostgresSQL.Shared")]

public class PostgresSqlCollection : ICollectionFixture<PostgresSqlFixture>
{
    
}