using System.Reflection;
using dotFitness.Common.Configuration;
using dotFitness.Common.Events;
using dotFitness.Common.Inbox;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using dotFitness.ModuleContracts;
using dotFitness.Modules.Exercises.Application.Commands;
using dotFitness.Modules.Exercises.Domain.Entities;
using dotFitness.Modules.Exercises.Domain.Repositories;
using dotFitness.Modules.Exercises.Infrastructure.Repositories;
using dotFitness.Modules.Exercises.Infrastructure.HealthChecks;
using dotFitness.Modules.Exercises.Infrastructure.EventHandlers;
using dotFitness.Modules.Exercises.Infrastructure.Services;

namespace dotFitness.Modules.Exercises.Infrastructure.Configuration;

/// <summary>
/// Exercises module installer implementing IModuleInstaller contract
/// </summary>
public class ExercisesModuleInstaller : IModuleInstaller
{
    public static string ModuleName = "exercises";

    public void InstallServices(IServiceCollection services, IConfiguration configuration, List<Assembly> assemblies)
    {
        assemblies.Add(typeof(ExercisesModuleInstaller).Assembly);
        assemblies.Add(typeof(CreateExerciseCommandHandler).Assembly); // Application assembly for handlers and validators

        services.AddSingleton<IMongoClient>(sp =>
        {
            var connectionString = configuration.GetConnectionString("ExercisesModuleConnectionString")
                                   ?? throw new InvalidOperationException("MongoDB connection string not found. Configure one of: ExercisesModuleConnectionString, dotFitnessDb-mongo, or MongoDB");
            return new MongoClient(connectionString);
        });

        services.AddSingleton<IMongoDatabase>(sp =>
        {
            var client = sp.GetRequiredService<IMongoClient>();
            var connectionString = configuration.GetConnectionString("ExercisesModuleConnectionString")
                                   ?? throw new InvalidOperationException("MongoDB connection string not found");
            
            var mongoUrl = new MongoUrl(connectionString);
            var dbName = mongoUrl.DatabaseName ?? "dotFitness";
            return client.GetDatabase(dbName);
        });

        // Register MongoDB collections specific to Exercises module
        services.AddSingleton(sp =>
        {
            var database = sp.GetRequiredService<IMongoDatabase>();
            return database.GetCollection<Exercise>("exercises");
        });

        services.AddSingleton(sp =>
        {
            var database = sp.GetRequiredService<IMongoDatabase>();
            return database.GetCollection<MuscleGroup>("muscleGroups");
        });

        services.AddSingleton(sp =>
        {
            var database = sp.GetRequiredService<IMongoDatabase>();
            return database.GetCollection<Equipment>("equipment");
        });

        // Register Inbox collection (shared inboxMessages)
        services.AddSingleton(sp =>
        {
            var database = sp.GetRequiredService<IMongoDatabase>();
            return database.GetCollection<InboxMessage>("inboxMessages");
        });

        // Register user preferences projection collection
        services.AddSingleton(sp =>
        {
            var database = sp.GetRequiredService<IMongoDatabase>();
            return database.GetCollection<UserPreferencesProjection>("userPreferencesProjections");
        });

        // Register repositories
        services.AddScoped<IExerciseRepository, ExerciseRepository>();
        services.AddScoped<IMuscleGroupRepository, MuscleGroupRepository>();
        services.AddScoped<IEquipmentRepository, EquipmentRepository>();
        services.AddScoped<IUserPreferencesProjectionRepository, UserPreferencesProjectionRepository>();

        // Register event handlers for cross-module communication
        services.AddScoped<IEventHandler<UserProfileUpdatedEvent>, UserProfileUpdatedEventHandler>();

        // Register Exercises module health check
        services.AddHealthChecks()
            .AddCheck<ExercisesModuleHealthCheck>("exercises-module", tags: ["module", "exercises", "live"]);

        // Register Exercises module configuration validator
        services.AddScoped<IModuleConfigurationValidator, ExercisesConfigurationValidator>();

        // Register MongoDB initialization service for auto-configuring indexes and seeding data
        services.AddHostedService<MongoDbInitializationService>();
    }
}