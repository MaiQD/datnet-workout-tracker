using Serilog;
using MongoDB.Driver;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using dotFitness.SharedKernel.Outbox;
using dotFitness.Modules.Users.Application.Configuration;
using dotFitness.Api.Infrastructure;
using Microsoft.Extensions.Logging;

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// Use Serilog as the logging provider
builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers();

// Add API Versioning
builder.Services.AddApiVersioning(opt =>
{
    opt.DefaultApiVersion = new ApiVersion(1, 0);
    opt.AssumeDefaultVersionWhenUnspecified = true;
});

// Add Swagger/OpenAPI services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "dotFitness API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new()
    {
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new()
    {
        {
            new()
            {
                Reference = new()
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[]{}
        }
    });
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Configure MongoDB
var mongoConnectionString = builder.Configuration.GetConnectionString("MongoDB");
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    return new MongoClient(mongoConnectionString);
});

builder.Services.AddSingleton<IMongoDatabase>(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    return client.GetDatabase("dotFitnessDb");
});

// Register base MongoDB Collections for shared types
builder.Services.AddSingleton(sp =>
{
    var database = sp.GetRequiredService<IMongoDatabase>();
    return database.GetCollection<OutboxMessage>("outboxMessages");
});

// Add FluentValidation
builder.Services.AddFluentValidationAutoValidation()
    .AddFluentValidationClientsideAdapters();

// Add health checks for modules
builder.Services.AddModuleHealthChecks();

// Set up module registry logger - convert Serilog to Microsoft.Extensions.Logging
var loggerFactory = LoggerFactory.Create(builder => builder.AddSerilog(Log.Logger));
var microsoftLogger = loggerFactory.CreateLogger("ModuleRegistry");
ModuleRegistry.SetLogger(microsoftLogger);

// Validate module configuration
var configurationValidation = ModuleConfigurationValidator.ValidateModuleConfiguration(builder.Configuration, microsoftLogger);
if (!configurationValidation.IsValid)
{
    microsoftLogger.LogWarning("Module configuration validation found issues: {ValidationResult}", configurationValidation.ToJson());
}

// Add MediatR with automatic module assembly discovery
builder.Services.AddMediatR(cfg => 
{
    // Register API assembly
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
    
    // Auto-discover and register all module assemblies
    ModuleRegistry.RegisterModuleAssemblies(cfg);
});

// Register all modules automatically
ModuleRegistry.RegisterAllModules(builder.Services, builder.Configuration);

var app = builder.Build();

// Configure MongoDB Indexes (shared + module-specific)
await ConfigureMongoDbIndexes(app.Services);

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();

// Add health check endpoints
app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var response = new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                description = e.Value.Description,
                duration = e.Value.Duration.ToString()
            })
        };
        await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response));
    }
});

app.MapHealthChecks("/health/modules", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("module"),
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var response = new
        {
            status = report.Status.ToString(),
            modules = report.Entries.Where(e => e.Value.Tags.Contains("module")).Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                description = e.Value.Description,
                duration = e.Value.Duration.ToString()
            })
        };
        await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response));
    }
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

Log.Information("dotFitness API starting up...");

app.Run();

static async Task ConfigureMongoDbIndexes(IServiceProvider services)
{
    using var scope = services.CreateScope();
    var database = scope.ServiceProvider.GetRequiredService<IMongoDatabase>();
    
    // Create indexes for shared/global collections (OutboxMessage)
    var outboxCollection = database.GetCollection<OutboxMessage>("outboxMessages");
    var outboxIndexBuilder = Builders<OutboxMessage>.IndexKeys;
    
    await outboxCollection.Indexes.CreateManyAsync(new[]
    {
        new CreateIndexModel<OutboxMessage>(outboxIndexBuilder.Ascending(x => x.IsProcessed)),
        new CreateIndexModel<OutboxMessage>(outboxIndexBuilder.Ascending(x => x.CreatedAt)),
        new CreateIndexModel<OutboxMessage>(outboxIndexBuilder.Ascending(x => x.EventType))    });    
    // Configure module-specific indexes for all modules
    await ModuleRegistry.ConfigureAllModuleIndexes(services);
    
    Log.Information("MongoDB indexes configured successfully");
}
