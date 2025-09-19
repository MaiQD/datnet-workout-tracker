using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using dotFitness.ModuleContracts;
using dotFitness.Modules.Exercises.Domain.Entities;
using dotFitness.Modules.Exercises.Domain.Repositories;
using dotFitness.Modules.Exercises.Infrastructure.Repositories;
using dotFitness.Modules.Exercises.Infrastructure.HealthChecks;
using dotFitness.SharedKernel.Inbox;

namespace dotFitness.Modules.Exercises.Infrastructure.Configuration;

/// <summary>
/// Exercises module installer implementing IModuleInstaller contract
/// </summary>
public class ExercisesModuleInstaller : IModuleInstaller
{
    public void InstallServices(IServiceCollection services, IConfiguration configuration)
    {
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

        // Register MediatR handlers (auto-registered in Bootstrap) - removed

        // Register validators (auto-registered in Bootstrap) - removed

        // Register Exercises module health check
        services.AddHealthChecks()
            .AddCheck<ExercisesModuleHealthCheck>("exercises-module", tags: new[] { "module", "exercises", "live" });

        // Register Exercises module configuration validator
        services.AddScoped<dotFitness.SharedKernel.Configuration.IModuleConfigurationValidator, ExercisesConfigurationValidator>();
    }

    public void ConfigureIndexes(IMongoDatabase database)
    {
        ExercisesMongoIndexConfigurator.Configure(database);
    }

    public void SeedData(IMongoDatabase database)
    {
        var muscleGroupCollection = database.GetCollection<MuscleGroup>("muscleGroups");
        var equipmentCollection = database.GetCollection<Equipment>("equipment");

        // Check if data already exists
        if (muscleGroupCollection.CountDocuments(FilterDefinition<MuscleGroup>.Empty) > 0)
        {
            return; // Data already seeded
        }

        // Seed global muscle groups
        var globalMuscleGroups = new List<MuscleGroup>
        {
            // Upper Body
            new MuscleGroup { Name = "Chest", Description = "Pectoral muscles", BodyRegion = BodyRegion.Upper, IsGlobal = true, UserId = null, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new MuscleGroup { Name = "Back", Description = "Back muscles including lats, traps, and rhomboids", BodyRegion = BodyRegion.Upper, IsGlobal = true, UserId = null, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new MuscleGroup { Name = "Shoulders", Description = "Deltoid muscles", BodyRegion = BodyRegion.Upper, IsGlobal = true, UserId = null, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new MuscleGroup { Name = "Biceps", Description = "Biceps brachii", BodyRegion = BodyRegion.Upper, IsGlobal = true, UserId = null, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new MuscleGroup { Name = "Triceps", Description = "Triceps brachii", BodyRegion = BodyRegion.Upper, IsGlobal = true, UserId = null, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new MuscleGroup { Name = "Forearms", Description = "Forearm muscles", BodyRegion = BodyRegion.Upper, IsGlobal = true, UserId = null, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },

            // Lower Body
            new MuscleGroup { Name = "Quadriceps", Description = "Front thigh muscles", BodyRegion = BodyRegion.Lower, IsGlobal = true, UserId = null, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new MuscleGroup { Name = "Hamstrings", Description = "Back thigh muscles", BodyRegion = BodyRegion.Lower, IsGlobal = true, UserId = null, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new MuscleGroup { Name = "Glutes", Description = "Buttock muscles", BodyRegion = BodyRegion.Lower, IsGlobal = true, UserId = null, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new MuscleGroup { Name = "Calves", Description = "Calf muscles", BodyRegion = BodyRegion.Lower, IsGlobal = true, UserId = null, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },

            // Core
            new MuscleGroup { Name = "Abs", Description = "Abdominal muscles", BodyRegion = BodyRegion.Core, IsGlobal = true, UserId = null, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new MuscleGroup { Name = "Obliques", Description = "Side abdominal muscles", BodyRegion = BodyRegion.Core, IsGlobal = true, UserId = null, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new MuscleGroup { Name = "Lower Back", Description = "Lower back muscles", BodyRegion = BodyRegion.Core, IsGlobal = true, UserId = null, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        };

        // Seed global equipment
        var globalEquipment = new List<Equipment>
        {
            new Equipment { Name = "Barbell", Description = "Standard Olympic barbell", Category = "FreeWeights", IsGlobal = true, UserId = null, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Equipment { Name = "Dumbbells", Description = "Adjustable or fixed weight dumbbells", Category = "FreeWeights", IsGlobal = true, UserId = null, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Equipment { Name = "Pull-up Bar", Description = "Bar for pull-ups and chin-ups", Category = "Bodyweight", IsGlobal = true, UserId = null, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Equipment { Name = "Resistance Bands", Description = "Elastic resistance bands", Category = "Resistance", IsGlobal = true, UserId = null, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Equipment { Name = "Yoga Mat", Description = "Exercise mat for floor work", Category = "Bodyweight", IsGlobal = true, UserId = null, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        };

        // Insert the data
        muscleGroupCollection.InsertMany(globalMuscleGroups);
        equipmentCollection.InsertMany(globalEquipment);
    }
}
