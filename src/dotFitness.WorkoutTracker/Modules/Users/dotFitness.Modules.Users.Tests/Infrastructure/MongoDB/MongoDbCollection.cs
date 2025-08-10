using dotFitness.SharedKernel.Tests.MongoDB;

namespace dotFitness.Modules.Users.Tests.Infrastructure.MongoDB;

[CollectionDefinition("MongoDB")]
public class MongoDbCollection : ICollectionFixture<MongoDbFixture> { }
