using Microsoft.Extensions.DependencyInjection;
using DatNetWorkoutTracker.Shared.Extensions;
using DatNetWorkoutTracker.Exercises.Domain;
using DatNetWorkoutTracker.Exercises.Services;

namespace DatNetWorkoutTracker.Exercises.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddExercisesModule(this IServiceCollection services)
    {
        // Register repositories
        services.AddRepository<Exercise>("exercises");

        // Register services
        services.AddScoped<IExerciseService, ExerciseService>();

        return services;
    }
}
