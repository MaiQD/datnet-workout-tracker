using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using MediatR;
using FluentValidation;
using dotFitness.Modules.Exercises.Domain.Entities;
using dotFitness.Modules.Exercises.Domain.Repositories;
using dotFitness.Modules.Exercises.Infrastructure.Repositories;
using dotFitness.Modules.Exercises.Infrastructure.Handlers;
using dotFitness.Modules.Exercises.Application.Mappers;
using dotFitness.Modules.Exercises.Application.Commands;
using dotFitness.Modules.Exercises.Application.Queries;
using dotFitness.Modules.Exercises.Application.Validators;
using dotFitness.Modules.Exercises.Application.DTOs;
using dotFitness.SharedKernel.Results;
using dotFitness.SharedKernel.Inbox;

namespace dotFitness.Modules.Exercises.Infrastructure.Configuration;

/// <summary>
/// Configuration class for Exercises module infrastructure services
/// </summary>
public static class ExercisesInfrastructureModule
{
    /// <summary>
    /// Adds all Exercises module services and configuration to the dependency injection container
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration to bind settings from</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddExercisesModule(this IServiceCollection services, IConfiguration configuration)
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

        // Register repositories
        services.AddScoped<IExerciseRepository, ExerciseRepository>();
        services.AddScoped<IMuscleGroupRepository, MuscleGroupRepository>();
        services.AddScoped<IEquipmentRepository, EquipmentRepository>();

        // Register Inbox collection (shared inboxMessages)
        services.AddSingleton(sp =>
        {
            var database = sp.GetRequiredService<IMongoDatabase>();
            return database.GetCollection<InboxMessage>("inboxMessages");
        });

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

        return services;
    }

    /// <summary>
    /// Configures MongoDB indexes for Exercises module entities
    /// </summary>
    /// <param name="services">The service provider</param>
    /// <returns>Task representing the async operation</returns>
    public static async Task ConfigureExercisesModuleIndexes(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var database = scope.ServiceProvider.GetRequiredService<IMongoDatabase>();
        
        // Create indexes for Exercise collection
        var exerciseCollection = database.GetCollection<Exercise>("exercises");
        var exerciseIndexBuilder = Builders<Exercise>.IndexKeys;
        
        await exerciseCollection.Indexes.CreateManyAsync(new[]
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
        
        await muscleGroupCollection.Indexes.CreateManyAsync(new[]
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
        
        await equipmentCollection.Indexes.CreateManyAsync(new[]
        {
            new CreateIndexModel<Equipment>(equipmentIndexBuilder.Ascending(x => x.Name)),
            new CreateIndexModel<Equipment>(equipmentIndexBuilder.Ascending(x => x.UserId)),
            new CreateIndexModel<Equipment>(equipmentIndexBuilder.Ascending(x => x.IsGlobal)),
            new CreateIndexModel<Equipment>(equipmentIndexBuilder.Ascending(x => x.CreatedAt)),
            // Scoped unique: (name, isGlobal, userId)
            new CreateIndexModel<Equipment>(
                equipmentIndexBuilder.Ascending(x => x.Name).Ascending(x => x.IsGlobal).Ascending(x => x.UserId),
                new CreateIndexOptions { Unique = true })
        });

        // Create indexes for Inbox collection (Exercises consumers)
        var inboxCollection = database.GetCollection<InboxMessage>("inboxMessages");
        var inboxIndexBuilder = Builders<InboxMessage>.IndexKeys;
        await inboxCollection.Indexes.CreateManyAsync(new[]
        {
            new CreateIndexModel<InboxMessage>(
                inboxIndexBuilder.Ascending(x => x.Consumer).Ascending(x => x.EventId),
                new CreateIndexOptions { Unique = true }),
            new CreateIndexModel<InboxMessage>(inboxIndexBuilder.Ascending(x => x.Consumer).Ascending(x => x.Status).Ascending(x => x.OccurredOn))
        });
    }

    /// <summary>
    /// Seeds global muscle groups based on Appendix A
    /// </summary>
    public static async Task SeedExercisesModuleData(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var database = scope.ServiceProvider.GetRequiredService<IMongoDatabase>();
        var muscleGroupCollection = database.GetCollection<MuscleGroup>("muscleGroups");
        var equipmentCollection = database.GetCollection<Equipment>("equipment");

        async Task UpsertAsync(string name, BodyRegion region, string? parentId = null, params string[] aliases)
        {
            var filter = Builders<MuscleGroup>.Filter.And(
                Builders<MuscleGroup>.Filter.Eq(x => x.Name, name),
                Builders<MuscleGroup>.Filter.Eq(x => x.IsGlobal, true));

            var update = Builders<MuscleGroup>.Update
                .SetOnInsert(x => x.IsGlobal, true)
                .Set(x => x.BodyRegion, region)
                .Set(x => x.ParentId, parentId)
                .Set(x => x.Aliases, aliases?.ToList())
                .Set(x => x.UpdatedAt, DateTime.UtcNow)
                .SetOnInsert(x => x.CreatedAt, DateTime.UtcNow);

            await muscleGroupCollection.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = true });
        }

        async Task UpsertEquipmentAsync(string name, string? category = null, string? description = null)
        {
            var filter = Builders<Equipment>.Filter.And(
                Builders<Equipment>.Filter.Eq(x => x.Name, name),
                Builders<Equipment>.Filter.Eq(x => x.IsGlobal, true));

            var update = Builders<Equipment>.Update
                .SetOnInsert(x => x.IsGlobal, true)
                .Set(x => x.Category, category)
                .Set(x => x.Description, description)
                .Set(x => x.UpdatedAt, DateTime.UtcNow)
                .SetOnInsert(x => x.CreatedAt, DateTime.UtcNow);

            await equipmentCollection.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = true });
        }

        // Seed Upper
        await UpsertAsync("Chest", BodyRegion.Upper);
        await UpsertAsync("Back", BodyRegion.Upper);
        var back = await muscleGroupCollection.Find(x => x.Name == "Back" && x.IsGlobal).FirstOrDefaultAsync();
        var backId = back?.Id;
        await UpsertAsync("Lats", BodyRegion.Upper, backId, "Latissimus Dorsi");
        await UpsertAsync("Traps", BodyRegion.Upper, backId, "Trapezius");
        await UpsertAsync("Rhomboids", BodyRegion.Upper, backId);
        await UpsertAsync("Erector Spinae (Thoracic)", BodyRegion.Upper, backId);
        await UpsertAsync("Shoulders", BodyRegion.Upper);
        var shoulders = await muscleGroupCollection.Find(x => x.Name == "Shoulders" && x.IsGlobal).FirstOrDefaultAsync();
        var shouldersId = shoulders?.Id;
        await UpsertAsync("Anterior Deltoid", BodyRegion.Upper, shouldersId);
        await UpsertAsync("Lateral Deltoid", BodyRegion.Upper, shouldersId);
        await UpsertAsync("Posterior Deltoid", BodyRegion.Upper, shouldersId);
        await UpsertAsync("Arms", BodyRegion.Upper);
        var arms = await muscleGroupCollection.Find(x => x.Name == "Arms" && x.IsGlobal).FirstOrDefaultAsync();
        var armsId = arms?.Id;
        await UpsertAsync("Biceps", BodyRegion.Upper, armsId, "Biceps Brachii", "Brachialis");
        await UpsertAsync("Triceps", BodyRegion.Upper, armsId, "Triceps Brachii");
        await UpsertAsync("Forearms", BodyRegion.Upper, armsId, "Forearm Flexors", "Forearm Extensors");
        await UpsertAsync("Neck", BodyRegion.Upper, null, "Sternocleidomastoid", "Upper Traps");

        // Seed Core
        await UpsertAsync("Abs", BodyRegion.Core, null, "Rectus Abdominis");
        await UpsertAsync("Obliques", BodyRegion.Core, null, "External Obliques", "Internal Obliques");
        await UpsertAsync("Transverse Abdominis", BodyRegion.Core);
        await UpsertAsync("Lower Back", BodyRegion.Core, null, "Lumbar Erector Spinae", "Quadratus Lumborum");
        await UpsertAsync("Hip Flexors", BodyRegion.Core, null, "Iliopsoas");

        // Seed Lower
        await UpsertAsync("Glutes", BodyRegion.Lower, null, "Glute Maximus", "Glute Medius", "Glute Minimus");
        await UpsertAsync("Quadriceps", BodyRegion.Lower, null, "Rectus Femoris", "Vastus Lateralis", "Vastus Medialis", "Vastus Intermedius");
        await UpsertAsync("Hamstrings", BodyRegion.Lower, null, "Biceps Femoris", "Semitendinosus", "Semimembranosus");
        await UpsertAsync("Calves", BodyRegion.Lower, null, "Gastrocnemius", "Soleus");
        await UpsertAsync("Hip Abductors", BodyRegion.Lower, null, "Tensor Fasciae Latae", "Glute Medius", "Glute Minimus");
        await UpsertAsync("Hip Adductors", BodyRegion.Lower, null, "Adductor Group");
        await UpsertAsync("Tibialis Anterior", BodyRegion.Lower);

        // Seed Full Body
        await UpsertAsync("Full Body", BodyRegion.FullBody);

        // Seed Equipment (Global)
        await UpsertEquipmentAsync("Bodyweight", "Bodyweight", "Exercises requiring no equipment");
        await UpsertEquipmentAsync("Dumbbells", "Weights");
        await UpsertEquipmentAsync("Barbell", "Weights");
        await UpsertEquipmentAsync("Kettlebell", "Weights");
        await UpsertEquipmentAsync("Resistance Band", "Resistance");
        await UpsertEquipmentAsync("Pull-up Bar", "Bodyweight");
        await UpsertEquipmentAsync("Bench", "Weights");
        await UpsertEquipmentAsync("Yoga Mat", "Accessories");
        await UpsertEquipmentAsync("Jump Rope", "Cardio");
    }
}
