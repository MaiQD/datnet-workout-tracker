using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using dotFitness.ModuleContracts;
using dotFitness.Modules.Users.Infrastructure.Configuration;
using dotFitness.Modules.Exercises.Infrastructure.Configuration;
using dotFitness.SharedKernel.Outbox;
using FluentValidation;

namespace dotFitness.Bootstrap;

public static class ModuleRegistry
{
    public static void RegisterAllModules(this IServiceCollection services, IConfiguration configuration, ILogger logger)
    {
        var installers = new List<IModuleInstaller>
        {
            new UsersModuleInstaller(),
            new ExercisesModuleInstaller()
        };

        services.AddSingleton<IEnumerable<IModuleInstaller>>(installers);

        foreach (var installer in installers)
        {
            installer.InstallServices(services, configuration);
        }

        // Shared infra
        services.AddSingleton<IMongoClient>(sp =>
        {
            var conn = configuration.GetConnectionString("dotFitnessDb-mongo");
            return new MongoClient(conn);
        });

        services.AddSingleton<IMongoDatabase>(sp =>
        {
            var client = sp.GetRequiredService<IMongoClient>();
            var connectionString = configuration.GetConnectionString("dotFitnessDb-mongo");
            var mongoUrl = new MongoUrl(connectionString);
            var dbName = mongoUrl.DatabaseName ?? "dotFitness";
            return client.GetDatabase(dbName);
        });
        
        // Register base MongoDB Collections for shared types
        services.AddSingleton(sp =>
        {
            var database = sp.GetRequiredService<IMongoDatabase>();
            return database.GetCollection<OutboxMessage>("outboxMessages");
        });
        
        // MediatR handlers
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblyContaining<UsersModuleInstaller>();
            cfg.RegisterServicesFromAssemblyContaining<ExercisesModuleInstaller>();
        });

        // FluentValidation validators
        services.AddValidatorsFromAssemblyContaining<UsersModuleInstaller>();
        services.AddValidatorsFromAssemblyContaining<ExercisesModuleInstaller>();
    }

    public static void ConfigureAllModuleIndexes(this IServiceProvider provider, ILogger logger)
    {
        var database = provider.GetRequiredService<IMongoDatabase>();
        var installers = provider.GetRequiredService<IEnumerable<IModuleInstaller>>();
        foreach (var i in installers)
        {
            i.ConfigureIndexes(database);
        }
    }

    public static void SeedAllModuleData(this IServiceProvider provider, ILogger logger)
    {
        var database = provider.GetRequiredService<IMongoDatabase>();
        var installers = provider.GetRequiredService<IEnumerable<IModuleInstaller>>();
        foreach (var i in installers)
        {
            i.SeedData(database);
        }
    }
}
