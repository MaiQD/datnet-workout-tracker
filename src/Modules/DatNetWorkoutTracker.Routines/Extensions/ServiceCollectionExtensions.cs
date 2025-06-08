using Microsoft.Extensions.DependencyInjection;
using DatNetWorkoutTracker.Shared.Extensions;
using DatNetWorkoutTracker.Routines.Domain;
using DatNetWorkoutTracker.Routines.Services;

namespace DatNetWorkoutTracker.Routines.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRoutinesModule(this IServiceCollection services)
    {
        // Register repositories
        services.AddRepository<Routine>("routines");

        // Register services
        services.AddScoped<IRoutineService, RoutineService>();

        return services;
    }
}
