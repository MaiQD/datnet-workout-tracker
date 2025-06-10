using Serilog;
using MongoDB.Driver;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using dotFitness.SharedKernel.Outbox;
using dotFitness.Modules.Users.Domain.Entities;

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

// Register MongoDB Collections
builder.Services.AddSingleton(sp =>
{
    var database = sp.GetRequiredService<IMongoDatabase>();
    return database.GetCollection<OutboxMessage>("outboxMessages");
});

builder.Services.AddSingleton(sp =>
{
    var database = sp.GetRequiredService<IMongoDatabase>();
    return database.GetCollection<User>("users");
});

builder.Services.AddSingleton(sp =>
{
    var database = sp.GetRequiredService<IMongoDatabase>();
    return database.GetCollection<UserMetric>("userMetrics");
});

// Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"];

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// Add FluentValidation
builder.Services.AddFluentValidationAutoValidation()
    .AddFluentValidationClientsideAdapters();

// Add MediatR (will be configured to scan assemblies when modules are added)
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

var app = builder.Build();

// Configure MongoDB Indexes
await ConfigureMongoDbIndexes(app.Services);

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

Log.Information("dotFitness API starting up...");

app.Run();

static async Task ConfigureMongoDbIndexes(IServiceProvider services)
{
    using var scope = services.CreateScope();
    var database = scope.ServiceProvider.GetRequiredService<IMongoDatabase>();
    
    // Create indexes for OutboxMessage collection
    var outboxCollection = database.GetCollection<OutboxMessage>("outboxMessages");
    var outboxIndexBuilder = Builders<OutboxMessage>.IndexKeys;
    
    await outboxCollection.Indexes.CreateManyAsync(new[]
    {
        new CreateIndexModel<OutboxMessage>(outboxIndexBuilder.Ascending(x => x.IsProcessed)),
        new CreateIndexModel<OutboxMessage>(outboxIndexBuilder.Ascending(x => x.CreatedAt)),
        new CreateIndexModel<OutboxMessage>(outboxIndexBuilder.Ascending(x => x.EventType))
    });

    // Create indexes for User collection
    var userCollection = database.GetCollection<User>("users");
    var userIndexBuilder = Builders<User>.IndexKeys;
    
    await userCollection.Indexes.CreateManyAsync(new[]
    {
        new CreateIndexModel<User>(userIndexBuilder.Ascending(x => x.Email), new CreateIndexOptions { Unique = true }),
        new CreateIndexModel<User>(userIndexBuilder.Ascending(x => x.GoogleId)),
        new CreateIndexModel<User>(userIndexBuilder.Ascending(x => x.CreatedAt)),
        new CreateIndexModel<User>(userIndexBuilder.Ascending(x => x.Roles))
    });

    // Create indexes for UserMetric collection
    var userMetricCollection = database.GetCollection<UserMetric>("userMetrics");
    var userMetricIndexBuilder = Builders<UserMetric>.IndexKeys;
      await userMetricCollection.Indexes.CreateManyAsync(new[]
    {
        new CreateIndexModel<UserMetric>(userMetricIndexBuilder.Ascending(x => x.UserId)),
        new CreateIndexModel<UserMetric>(userMetricIndexBuilder.Ascending(x => x.Date)),
        new CreateIndexModel<UserMetric>(userMetricIndexBuilder.Ascending(x => x.UserId).Descending(x => x.Date)),
        new CreateIndexModel<UserMetric>(userMetricIndexBuilder.Ascending(x => x.UserId).Ascending(x => x.Date), new CreateIndexOptions { Unique = true })
    });
    
    Log.Information("MongoDB indexes configured successfully");
}
