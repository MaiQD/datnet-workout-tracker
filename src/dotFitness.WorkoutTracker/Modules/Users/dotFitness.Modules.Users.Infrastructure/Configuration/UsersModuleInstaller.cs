using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using System.Text;
using dotFitness.ModuleContracts;
using dotFitness.Modules.Users.Domain.Entities;
using dotFitness.Modules.Users.Domain.Repositories;
using dotFitness.Modules.Users.Infrastructure.Repositories;
using dotFitness.Modules.Users.Infrastructure.Services;
using dotFitness.Modules.Users.Application.Mappers;
using dotFitness.Modules.Users.Application.Services;
using dotFitness.Modules.Users.Infrastructure.Settings;
using dotFitness.Modules.Users.Infrastructure.Data;
using dotFitness.Modules.Users.Infrastructure.HealthChecks;
using dotFitness.SharedKernel.Inbox;

namespace dotFitness.Modules.Users.Infrastructure.Configuration;

/// <summary>
/// Users module installer implementing IModuleInstaller contract
/// </summary>
public class UsersModuleInstaller : IModuleInstaller
{
    public void InstallServices(IServiceCollection services, IConfiguration configuration)
    {
        // Configure User Module Settings
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
        services.Configure<AdminSettings>(configuration.GetSection("AdminSettings"));

        // Configure JWT Authentication (since it's primarily used for user authentication)
        var jwtSettings = configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"];

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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

        services.AddAuthorization();

        // Configure PostgreSQL DbContext for Users module
        services.AddDbContext<UsersDbContext>(options =>
        {
            // Try Aspire connection first, then fallback to manual configuration
            var connectionString = configuration.GetConnectionString("dotFitnessDb-pg") 
                                   ?? configuration.GetConnectionString("PostgreSQL");
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "users");
                npgsqlOptions.EnableRetryOnFailure(maxRetryCount: 3, maxRetryDelay: TimeSpan.FromSeconds(5), errorCodesToAdd: null);
            });
            
            // Enable sensitive data logging in development
            if (configuration.GetValue<bool>("Logging:EnableSensitiveDataLogging"))
            {
                options.EnableSensitiveDataLogging();
            }
            
            // Enable detailed errors in development
            if (configuration.GetValue<bool>("Logging:EnableDetailedErrors"))
            {
                options.EnableDetailedErrors();
            }
        });

        // Register MongoDB collections specific to Users module
        services.AddSingleton(sp =>
        {
            var database = sp.GetRequiredService<IMongoDatabase>();
            return database.GetCollection<User>("users");
        });

        services.AddSingleton(sp =>
        {
            var database = sp.GetRequiredService<IMongoDatabase>();
            return database.GetCollection<UserMetric>("userMetrics");
        });

        // Register Inbox collection (shared inboxMessages)
        services.AddSingleton(sp =>
        {
            var database = sp.GetRequiredService<IMongoDatabase>();
            return database.GetCollection<InboxMessage>("inboxMessages");
        });

        // Register repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserMetricsRepository, UserMetricsRepository>();

        // Register services
        services.AddScoped<IGoogleAuthService, GoogleAuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IJwtService, JwtService>();
        
        // Register HttpClient for GoogleAuthService
        services.AddHttpClient<IGoogleAuthService, GoogleAuthService>();

        // Register MediatR handlers (auto-registered in Bootstrap) - removed
        // Register validators (auto-registered in Bootstrap) - removed

        // Register Mapperly mappers - they will be generated as implementations
        services.AddScoped<UserMapper>();
        services.AddScoped<UserMetricMapper>();

        // Register Users module health check
        services.AddHealthChecks()
            .AddCheck<UsersModuleHealthCheck>("users-module", tags: new[] { "module", "users", "live" });

        // Register Users module configuration validator
        services.AddScoped<dotFitness.SharedKernel.Configuration.IModuleConfigurationValidator, UsersConfigurationValidator>();

        // Register database migration service for auto-applying migrations
        services.AddHostedService<DatabaseMigrationService>();
    }

    public void ConfigureIndexes(IMongoDatabase database)
    {
        UsersMongoIndexConfigurator.Configure(database);
    }

    public void SeedData(IMongoDatabase database)
    {
        // TODO: Implement user seeding if needed
        // For now, users are created through the registration process
    }
}
