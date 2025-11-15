var builder = DistributedApplication.CreateBuilder(args);

var webApi = builder.AddProject<Projects.dotFitness_Api>("webApi");

var frontend = builder.AddNpmApp("frontend", "../ClientApp", "dev")
    .WithReference(webApi)
    .WithHttpEndpoint()
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

builder.Build().Run();