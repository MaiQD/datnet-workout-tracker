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

        // Register MediatR command handlers
        services.AddScoped<IRequestHandler<CreateExerciseCommand, Result<ExerciseDto>>, CreateExerciseCommandHandler>();
        services.AddScoped<IRequestHandler<UpdateExerciseCommand, Result<ExerciseDto>>, UpdateExerciseCommandHandler>();
        services.AddScoped<IRequestHandler<DeleteExerciseCommand, Result>, DeleteExerciseCommandHandler>();

        // Register MediatR query handlers
        services.AddScoped<IRequestHandler<GetExerciseByIdQuery, Result<ExerciseDto?>>, GetExerciseByIdQueryHandler>();
        services.AddScoped<IRequestHandler<GetAllExercisesQuery, Result<IEnumerable<ExerciseDto>>>, GetAllExercisesQueryHandler>();
        services.AddScoped<IRequestHandler<GetAllMuscleGroupsQuery, Result<IEnumerable<MuscleGroupDto>>>, GetAllMuscleGroupsQueryHandler>();
        services.AddScoped<IRequestHandler<GetAllEquipmentQuery, Result<IEnumerable<EquipmentDto>>>, GetAllEquipmentQueryHandler>();

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
            new CreateIndexModel<Exercise>(exerciseIndexBuilder.Text(x => x.Name).Text(x => x.Description))
        });

        // Create indexes for MuscleGroup collection
        var muscleGroupCollection = database.GetCollection<MuscleGroup>("muscleGroups");
        var muscleGroupIndexBuilder = Builders<MuscleGroup>.IndexKeys;
        
        await muscleGroupCollection.Indexes.CreateManyAsync(new[]
        {
            new CreateIndexModel<MuscleGroup>(muscleGroupIndexBuilder.Ascending(x => x.Name)),
            new CreateIndexModel<MuscleGroup>(muscleGroupIndexBuilder.Ascending(x => x.UserId)),
            new CreateIndexModel<MuscleGroup>(muscleGroupIndexBuilder.Ascending(x => x.IsGlobal)),
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
            new CreateIndexModel<Equipment>(equipmentIndexBuilder.Ascending(x => x.CreatedAt))
        });
    }
}
