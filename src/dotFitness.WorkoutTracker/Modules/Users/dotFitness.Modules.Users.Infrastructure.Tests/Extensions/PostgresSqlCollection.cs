using dotFitness.Common.Tests.PostgreSQL;
using Xunit;

namespace dotFitness.Modules.Users.Infrastructure.Tests.Extensions;

[CollectionDefinition("PostgresSQL.Shared")]

public class PostgresSqlCollection : ICollectionFixture<PostgresSqlFixture>
{
    
}