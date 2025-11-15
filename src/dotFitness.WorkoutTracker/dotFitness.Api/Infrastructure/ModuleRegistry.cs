using System.Reflection;
using dotFitness.Common.Outbox;
using dotFitness.ModuleContracts;
using dotFitness.Modules.Exercises.Infrastructure.Configuration;
using dotFitness.Modules.Users.Infrastructure.Configuration;
using FluentValidation;
using MongoDB.Driver;

namespace dotFitness.Api.Infrastructure;

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
        var assemblies = new List<Assembly>();
        foreach (var installer in installers)
        {
            installer.InstallServices(services, configuration, assemblies);
        }

        // Shared infra
        // services.AddSingleton<IMongoClient>(sp =>
        // {
        //     var conn = configuration.GetConnectionString("dotFitnessDb-mongo");
        //     return new MongoClient(conn);
        // });
        //
        // services.AddSingleton<IMongoDatabase>(sp =>
        // {
        //     var client = sp.GetRequiredService<IMongoClient>();
        //     var connectionString = configuration.GetConnectionString("dotFitnessDb-mongo");
        //     var mongoUrl = new MongoUrl(connectionString);
        //     var dbName = mongoUrl.DatabaseName ?? "dotFitness";
        //     return client.GetDatabase(dbName);
        // });
        
        // Register base MongoDB Collections for shared types
        // services.AddSingleton(sp =>
        // {
        //     var database = sp.GetRequiredService<IMongoDatabase>();
        //     return database.GetCollection<OutboxMessage>("outboxMessages");
        // });
        
        // MediatR handlers
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(assemblies.ToArray());
        });

        // FluentValidation 
        services.AddValidatorsFromAssemblies(assemblies.ToArray());
    }

    // public static void ConfigureAllModuleIndexes(this IServiceProvider provider, ILogger logger)
    // {
    //     var database = provider.GetRequiredService<IMongoDatabase>();
    //     var installers = provider.GetRequiredService<IEnumerable<IModuleInstaller>>();
    //     foreach (var i in installers)
    //     {
    //         i.ConfigureIndexes(database);
    //     }
    // }
    //
    // public static void SeedAllModuleData(this IServiceProvider provider, ILogger logger)
    // {
    //     var database = provider.GetRequiredService<IMongoDatabase>();
    //     var installers = provider.GetRequiredService<IEnumerable<IModuleInstaller>>();
    //     foreach (var i in installers)
    //     {
    //         i.SeedData(database);
    //     }
    // }
}
