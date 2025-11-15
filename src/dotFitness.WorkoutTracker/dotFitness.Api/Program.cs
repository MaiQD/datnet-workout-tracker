using dotFitness.Api.Extensions;
using Serilog;
using FastEndpoints;
using dotFitness.Api.Infrastructure.Configuration;
using dotFitness.Api.Infrastructure.Extensions;
using dotFitness.Api.Infrastructure.Middleware;
using dotFitness.Aspire.ServiceDefaults;
using dotFitness.Common.Application.Settings;

var builder = WebApplication.CreateBuilder(args);
// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

// Use Serilog as the logging provider
builder.Host.UseSerilog();
builder.Services.AddExceptionHandler<GlobalErrorHandler>();
builder.Services.AddProblemDetails();

// Configure application settings
builder.Configuration.AddModuleConfiguration(["users", "exercises"]);
builder.Services.Configure<GoogleOAuthSettings>(builder.Configuration.GetSection(GoogleOAuthSettings.GoogleOAuthSettingsSection));
builder.Services.Configure<CorsSettings>(builder.Configuration.GetSection(CorsSettings.CorsSettingsSection));
builder.Services.Configure<OutboxProcessorSettings>(builder.Configuration.GetSection(OutboxProcessorSettings.OutboxProcessorSettingsSection));

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
// await MongoDbIndexConfigurator.ConfigureIndexesAsync(app.Services);
// // Seed MongoDB data
// await MongoDbSeeder.ConfigureSeedsAsync(app.Services);

// Map Identity API endpoints (includes /login, /refresh, /register, etc.)
app.MapGroup("/api/v1/auth")
    .MapIdentityApi<dotFitness.Modules.Users.Domain.Entities.ApplicationUser>();

// Configure the application pipeline
app.ConfigureSwaggerUi()
   .ConfigureCoreMiddleware()
   .ConfigureHealthChecks()
   .ConfigureEndpoints()
   .MapDefaultEndpoints()
   .UseExceptionHandler();

Log.Information("dotFitness API starting up...");

app.Run();
