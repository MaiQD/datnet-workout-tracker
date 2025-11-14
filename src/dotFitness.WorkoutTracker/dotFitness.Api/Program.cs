using Serilog;
using FastEndpoints;
using dotFitness.Api.Infrastructure;
using dotFitness.Api.Infrastructure.Configuration;
using dotFitness.Api.Infrastructure.Extensions;
using dotFitness.Api.Infrastructure.Settings;
using dotFitness.Aspire.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);
// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

// Use Serilog as the logging provider
builder.Host.UseSerilog();

// Configure application settings
builder.Services.Configure<GoogleOAuthSettings>(builder.Configuration.GetSection("GoogleOAuth"));
builder.Services.Configure<CorsSettings>(builder.Configuration.GetSection("CorsSettings"));
builder.Services.Configure<OutboxProcessorSettings>(builder.Configuration.GetSection("OutboxProcessor"));

// Add core API services
builder.Services.AddCoreApiServices();

// Add Swagger with OAuth2 support
builder.Services.AddSwaggerWithOAuth();

// Add CORS policy
builder.Services.AddCorsPolicy();

// Set up module registry logger
var loggerFactory = LoggerFactory.Create(builder => builder.AddSerilog(Log.Logger));
var microsoftLogger = loggerFactory.CreateLogger("ModuleRegistry");

// Add module services
builder.Services.AddModuleServices(builder.Configuration, microsoftLogger);
builder.Services.AddApiAuthorization(); // Add API-level authorization policies

// Add FastEndpoints
builder.Services.AddFastEndpoints();

builder.AddServiceDefaults();

var app = builder.Build();

// Configure MongoDB indexes
await MongoDbIndexConfigurator.ConfigureIndexesAsync(app.Services);
// Seed MongoDB data
await MongoDbSeeder.ConfigureSeedsAsync(app.Services);

// Map Identity API endpoints (includes /login, /refresh, /register, etc.)
app.MapGroup("/api/v1/auth")
    .MapIdentityApi<dotFitness.Modules.Users.Domain.Entities.ApplicationUser>();

// Configure the application pipeline
app.UseGlobalErrorHandler()
   .ConfigureSwaggerUi()
   .ConfigureCoreMiddleware()
   .ConfigureHealthChecks()
   .ConfigureEndpoints()
   .MapDefaultEndpoints();

Log.Information("dotFitness API starting up...");

app.Run();

// Make Program class accessible for testing
namespace dotFitness.Api
{
    public partial class Program
    {
    }
}
