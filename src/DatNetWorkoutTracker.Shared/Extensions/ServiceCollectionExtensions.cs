using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using DatNetWorkoutTracker.Shared.Configuration;
using DatNetWorkoutTracker.Shared.Domain;
using DatNetWorkoutTracker.Shared.Infrastructure;

namespace DatNetWorkoutTracker.Shared.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSharedServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure MongoDB
        services.Configure<MongoDbSettings>(configuration.GetSection("MongoDbSettings"));
        
        var mongoSettings = configuration.GetSection("MongoDbSettings").Get<MongoDbSettings>();
        services.AddSingleton<IMongoClient>(sp => new MongoClient(mongoSettings?.ConnectionString));
        services.AddSingleton<IMongoDatabase>(sp =>
        {
            var client = sp.GetRequiredService<IMongoClient>();
            return client.GetDatabase(mongoSettings?.DatabaseName);
        });

        // Configure Google OAuth
        services.Configure<GoogleOAuthSettings>(configuration.GetSection("GoogleOAuth"));

        return services;
    }

    public static IServiceCollection AddRepository<TEntity>(this IServiceCollection services, string collectionName)
        where TEntity : BaseEntity
    {
        services.AddScoped<IRepository<TEntity>>(sp =>
        {
            var database = sp.GetRequiredService<IMongoDatabase>();
            return new MongoRepository<TEntity>(database, collectionName);
        });

        return services;
    }
}
