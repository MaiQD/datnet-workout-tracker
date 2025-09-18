var builder = DistributedApplication.CreateBuilder(args);
var postgres = builder.AddPostgres("postgres")
    .WithPgAdmin();
var postgresDb = postgres.AddDatabase("dotFitnessDb-pg");

var mongo = builder.AddMongoDB("mongo")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithMongoExpress();
var mongoDb = mongo.AddDatabase("dotFitnessDb-mongo");
var webApi = builder.AddProject<Projects.dotFitness_Api>("webApi")
    .WithReference(postgresDb)
    .WithReference(mongoDb)
    .WaitFor(mongoDb);

builder.Build().Run();
