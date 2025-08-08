using Serilog;
using dotFitness.Api.Infrastructure;
using dotFitness.Api.Infrastructure.Extensions;
using dotFitness.Api.Infrastructure.Settings;

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// Use Serilog as the logging provider
builder.Host.UseSerilog();

// Configure application settings
builder.Services.Configure<GoogleOAuthSettings>(builder.Configuration.GetSection("GoogleOAuth"));

// Add core API services
builder.Services.AddCoreApiServices();

// Add Swagger with OAuth2 support
builder.Services.AddSwaggerWithOAuth();

// Add CORS policy
builder.Services.AddCorsPolicy();

// Add MongoDB services
var mongoConnectionString = builder.Configuration.GetConnectionString("MongoDB") 
    ?? throw new InvalidOperationException("MongoDB connection string is not configured");
builder.Services.AddMongoDbServices(mongoConnectionString);

// Set up module registry logger
var loggerFactory = LoggerFactory.Create(builder => builder.AddSerilog(Log.Logger));
var microsoftLogger = loggerFactory.CreateLogger("ModuleRegistry");

// Add module services
builder.Services.AddModuleServices(builder.Configuration, microsoftLogger);

var app = builder.Build();

// Configure MongoDB indexes
await MongoDbIndexConfigurator.ConfigureIndexesAsync(app.Services);
// Seed MongoDB data
await MongoDbSeeder.ConfigureSeedsAsync(app.Services);

// Configure the application pipeline
app.ConfigureSwaggerUi()
   .ConfigureCoreMiddleware()
   .ConfigureHealthChecks()
   .ConfigureEndpoints();

Log.Information("dotFitness API starting up...");

app.Run();
