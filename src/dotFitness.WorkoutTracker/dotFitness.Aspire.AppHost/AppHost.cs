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
    .WaitFor(mongoDb)
    .WaitFor(postgresDb);

var frontend = builder.AddNpmApp("frontend", "../ClientApp", "dev")
    .WithReference(webApi)
    .WithHttpEndpoint()
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

builder.Build().Run();