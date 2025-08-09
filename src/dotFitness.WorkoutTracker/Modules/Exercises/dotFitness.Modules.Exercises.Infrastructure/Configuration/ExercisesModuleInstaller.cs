using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using FluentValidation;
using MediatR;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using dotFitness.ModuleContracts;
using dotFitness.SharedKernel.Results;
using dotFitness.Modules.Exercises.Domain.Entities;
using dotFitness.Modules.Exercises.Domain.Repositories;
using dotFitness.Modules.Exercises.Infrastructure.Repositories;
using dotFitness.Modules.Exercises.Application.Commands;
using dotFitness.Modules.Exercises.Application.Queries;
using dotFitness.Modules.Exercises.Application.DTOs;
using dotFitness.Modules.Exercises.Infrastructure.Handlers;
using dotFitness.Modules.Exercises.Application.Validators;
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

        // Register MediatR command handlers
        services.AddScoped<IRequestHandler<CreateExerciseCommand, Result<ExerciseDto>>, CreateExerciseCommandHandler>();
        services.AddScoped<IRequestHandler<UpdateExerciseCommand, Result<ExerciseDto>>, UpdateExerciseCommandHandler>();
        services.AddScoped<IRequestHandler<DeleteExerciseCommand, Result>, DeleteExerciseCommandHandler>();

        // Register MediatR query handlers
        services.AddScoped<IRequestHandler<GetExerciseByIdQuery, Result<ExerciseDto?>>, GetExerciseByIdQueryHandler>();
        services.AddScoped<IRequestHandler<GetAllExercisesQuery, Result<IEnumerable<ExerciseDto>>>, GetAllExercisesQueryHandler>();
        services.AddScoped<IRequestHandler<GetAllMuscleGroupsQuery, Result<IEnumerable<MuscleGroupDto>>>, GetAllMuscleGroupsQueryHandler>();
        services.AddScoped<IRequestHandler<GetAllEquipmentQuery, Result<IEnumerable<EquipmentDto>>>, GetAllEquipmentQueryHandler>();
        services.AddScoped<IRequestHandler<GetSmartExerciseSuggestionsQuery, Result<IEnumerable<ExerciseDto>>>, GetSmartExerciseSuggestionsQueryHandler>();

        // Register validators
        services.AddScoped<IValidator<CreateExerciseCommand>, CreateExerciseCommandValidator>();
        services.AddScoped<IValidator<UpdateExerciseCommand>, UpdateExerciseCommandValidator>();
        services.AddScoped<IValidator<DeleteExerciseCommand>, DeleteExerciseCommandValidator>();
    }

    public void ConfigureIndexes(IMongoDatabase database)
    {
        // Create indexes for Exercise collection
        var exerciseCollection = database.GetCollection<Exercise>("exercises");
        var exerciseIndexBuilder = Builders<Exercise>.IndexKeys;
        
        exerciseCollection.Indexes.CreateMany(new[]
        {
            new CreateIndexModel<Exercise>(exerciseIndexBuilder.Ascending(x => x.Name)),
            new CreateIndexModel<Exercise>(exerciseIndexBuilder.Ascending(x => x.UserId)),
            new CreateIndexModel<Exercise>(exerciseIndexBuilder.Ascending(x => x.IsGlobal)),
            new CreateIndexModel<Exercise>(exerciseIndexBuilder.Ascending(x => x.MuscleGroups)),
            new CreateIndexModel<Exercise>(exerciseIndexBuilder.Ascending(x => x.Equipment)),
            new CreateIndexModel<Exercise>(exerciseIndexBuilder.Ascending(x => x.Difficulty)),
            new CreateIndexModel<Exercise>(exerciseIndexBuilder.Ascending(x => x.Tags)),
            new CreateIndexModel<Exercise>(exerciseIndexBuilder.Ascending(x => x.CreatedAt)),
            new CreateIndexModel<Exercise>(exerciseIndexBuilder.Text(x => x.Name).Text(x => x.Description)),
            // Scoped unique: (userId, isGlobal, name)
            new CreateIndexModel<Exercise>(
                exerciseIndexBuilder.Ascending(x => x.UserId).Ascending(x => x.IsGlobal).Ascending(x => x.Name),
                new CreateIndexOptions { Unique = true })
        });

        // Create indexes for MuscleGroup collection
        var muscleGroupCollection = database.GetCollection<MuscleGroup>("muscleGroups");
        var muscleGroupIndexBuilder = Builders<MuscleGroup>.IndexKeys;
        
        muscleGroupCollection.Indexes.CreateMany(new[]
        {
            new CreateIndexModel<MuscleGroup>(muscleGroupIndexBuilder.Ascending(x => x.Name)),
            new CreateIndexModel<MuscleGroup>(muscleGroupIndexBuilder.Ascending(x => x.UserId)),
            new CreateIndexModel<MuscleGroup>(muscleGroupIndexBuilder.Ascending(x => x.IsGlobal)),
            new CreateIndexModel<MuscleGroup>(muscleGroupIndexBuilder.Ascending(x => x.BodyRegion)),
            new CreateIndexModel<MuscleGroup>(muscleGroupIndexBuilder.Ascending(x => x.ParentId)),
            new CreateIndexModel<MuscleGroup>(muscleGroupIndexBuilder.Ascending(x => x.CreatedAt))
        });

        // Create indexes for Equipment collection
        var equipmentCollection = database.GetCollection<Equipment>("equipment");
        var equipmentIndexBuilder = Builders<Equipment>.IndexKeys;
        
        equipmentCollection.Indexes.CreateMany(new[]
        {
            new CreateIndexModel<Equipment>(equipmentIndexBuilder.Ascending(x => x.Name)),
            new CreateIndexModel<Equipment>(equipmentIndexBuilder.Ascending(x => x.UserId)),
            new CreateIndexModel<Equipment>(equipmentIndexBuilder.Ascending(x => x.IsGlobal)),
            new CreateIndexModel<Equipment>(equipmentIndexBuilder.Ascending(x => x.Category)),
            new CreateIndexModel<Equipment>(equipmentIndexBuilder.Ascending(x => x.CreatedAt))
        });

        // Create indexes for UserPreferencesProjection collection
        var userPreferencesCollection = database.GetCollection<UserPreferencesProjection>("userPreferencesProjections");
        var userPreferencesIndexBuilder = Builders<UserPreferencesProjection>.IndexKeys;
        
        userPreferencesCollection.Indexes.CreateMany(new[]
        {
            new CreateIndexModel<UserPreferencesProjection>(userPreferencesIndexBuilder.Ascending(x => x.UserId), new CreateIndexOptions { Unique = true }),
            new CreateIndexModel<UserPreferencesProjection>(userPreferencesIndexBuilder.Ascending(x => x.UpdatedAt))
        });

        // Create indexes for Inbox collection
        var inboxCollection = database.GetCollection<InboxMessage>("inboxMessages");
        var inboxIndexBuilder = Builders<InboxMessage>.IndexKeys;
        
        inboxCollection.Indexes.CreateMany(new[]
        {
            new CreateIndexModel<InboxMessage>(inboxIndexBuilder.Ascending(x => x.Consumer).Ascending(x => x.EventId), new CreateIndexOptions { Unique = true }),
            new CreateIndexModel<InboxMessage>(inboxIndexBuilder.Ascending(x => x.Consumer).Ascending(x => x.Status).Ascending(x => x.OccurredOn))
        });
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
