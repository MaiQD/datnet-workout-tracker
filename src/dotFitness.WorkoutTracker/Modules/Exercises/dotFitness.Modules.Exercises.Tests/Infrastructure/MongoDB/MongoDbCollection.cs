using Xunit;
using dotFitness.SharedKernel.Tests.MongoDB;

namespace dotFitness.Modules.Exercises.Tests.Infrastructure.MongoDB;

[CollectionDefinition("MongoDB")]
public class MongoDbCollection : ICollectionFixture<MongoDbFixture> { }
