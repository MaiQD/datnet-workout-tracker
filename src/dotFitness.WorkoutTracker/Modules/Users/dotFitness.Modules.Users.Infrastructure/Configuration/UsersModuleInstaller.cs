using dotFitness.Common.Authorization;
using dotFitness.Common.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;
using dotFitness.ModuleContracts;
using dotFitness.Modules.Users.Domain.Entities;
using dotFitness.Modules.Users.Infrastructure.Services;
using dotFitness.Modules.Users.Application.Services;
using dotFitness.Modules.Users.Infrastructure.Settings;
using dotFitness.Modules.Users.Infrastructure.Data;
using dotFitness.Modules.Users.Infrastructure.HealthChecks;

namespace dotFitness.Modules.Users.Infrastructure.Configuration;

/// <summary>
/// Users module installer implementing IModuleInstaller contract
/// </summary>
public class UsersModuleInstaller : IModuleInstaller
{
    public void InstallServices(IServiceCollection services, IConfiguration configuration)
    {
        // Configure User Module Settings
        services.Configure<AdminSettings>(configuration.GetSection("AdminSettings"));

        // Add Identity
        services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
        {
            options.SignIn.RequireConfirmedAccount = false;
            options.User.RequireUniqueEmail = true;
            options.Password.RequireDigit = false; // OAuth users don't set passwords
            options.Password.RequiredLength = 6;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
        })
        .AddEntityFrameworkStores<UsersDbContext>()
        .AddDefaultTokenProviders()
        .AddApiEndpoints(); // Adds /register, /login, /refresh endpoints

        // Keep JWT bearer authentication for API token validation
        services.AddAuthentication()
            .AddBearerToken(IdentityConstants.BearerScheme);

        // Configure authorization policies using constants
        services.AddAuthorizationBuilder()
            .AddPolicy(AuthorizationPolicies.AdminOnly, policy => 
                policy.RequireRole(Roles.Admin))
            .AddPolicy(AuthorizationPolicies.PtOnly, policy => 
                policy.RequireRole(Roles.Pt))
            .AddPolicy(AuthorizationPolicies.UserOnly, policy => 
                policy.RequireRole(Roles.User));

        // Configure PostgreSQL DbContext for Users module
        services.AddDbContext<UsersDbContext>(options =>
        {
            // Try Aspire connection first, then fallback to manual configuration
            var connectionString = configuration.GetConnectionString("dotFitnessDb-pg") 
                                   ?? configuration.GetConnectionString("PostgreSQL");
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "users");
                
                // Configure retry strategy - can be disabled for explicit transaction control
                var enableRetryStrategy = configuration.GetValue<bool>("Database:EnableRetryStrategy", defaultValue: true);
                if (enableRetryStrategy)
                {
                    npgsqlOptions.EnableRetryOnFailure(maxRetryCount: 3, maxRetryDelay: TimeSpan.FromSeconds(5), errorCodesToAdd: null);
                }
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

        // Register services
        services.AddScoped<IGoogleAuthService, GoogleAuthService>();
        services.AddScoped<IUserService, UserService>();
        
        // Register HttpClient for GoogleAuthService
        services.AddHttpClient<IGoogleAuthService, GoogleAuthService>();

        // Register Users module health check
        services.AddHealthChecks()
            .AddCheck<UsersModuleHealthCheck>("users-module", tags: ["module", "users", "live"]);

        // Register Users module configuration validator
        services.AddScoped<IModuleConfigurationValidator, UsersConfigurationValidator>();

        // Register database migration service for auto-applying migrations
        services.AddHostedService<DatabaseMigrationService>();
    }

    public void ConfigureIndexes(IMongoDatabase database)
    {
        // Users module uses PostgreSQL - no MongoDB indexes to configure
        // This method is required by IModuleInstaller interface but not used for Users module
    }

    public void SeedData(IMongoDatabase database)
    {
        // Users module uses PostgreSQL - no MongoDB data to seed
        // This method is required by IModuleInstaller interface but not used for Users module
    }
}
