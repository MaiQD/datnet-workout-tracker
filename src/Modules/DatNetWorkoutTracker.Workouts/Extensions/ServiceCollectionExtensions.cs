using Microsoft.Extensions.DependencyInjection;
using DatNetWorkoutTracker.Shared.Extensions;
using DatNetWorkoutTracker.Workouts.Domain;
using DatNetWorkoutTracker.Workouts.Services;

namespace DatNetWorkoutTracker.Workouts.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWorkoutsModule(this IServiceCollection services)
    {
        // Register repositories
        services.AddRepository<Workout>("workouts");

        // Register services
        services.AddScoped<IWorkoutService, WorkoutService>();

        return services;
    }
}
