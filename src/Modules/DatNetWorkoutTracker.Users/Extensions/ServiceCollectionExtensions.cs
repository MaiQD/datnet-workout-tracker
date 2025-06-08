using Microsoft.Extensions.DependencyInjection;
using DatNetWorkoutTracker.Shared.Extensions;
using DatNetWorkoutTracker.Users.Domain;
using DatNetWorkoutTracker.Users.Services;

namespace DatNetWorkoutTracker.Users.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddUsersModule(this IServiceCollection services)
    {
        // Register repositories
        services.AddRepository<User>("users");

        // Register services
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IGoogleAuthService, GoogleAuthService>();

        // Register HttpClient for Google API calls
        services.AddHttpClient<IGoogleAuthService, GoogleAuthService>();

        return services;
    }
}
