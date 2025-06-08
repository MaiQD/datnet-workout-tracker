using Microsoft.Extensions.DependencyInjection;
using DatNetWorkoutTracker.Analytics.Services;

namespace DatNetWorkoutTracker.Analytics.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAnalyticsModule(this IServiceCollection services)
    {
        // Register services
        services.AddScoped<IAnalyticsService, AnalyticsService>();

        return services;
    }
}
