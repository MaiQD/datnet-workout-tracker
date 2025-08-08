using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using MediatR;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using dotFitness.Modules.Users.Domain.Entities;
using dotFitness.Modules.Users.Domain.Repositories;
using dotFitness.Modules.Users.Infrastructure.Repositories;
using dotFitness.Modules.Users.Infrastructure.Handlers;
using dotFitness.Modules.Users.Infrastructure.Settings;
using dotFitness.Modules.Users.Application.Mappers;
using dotFitness.Modules.Users.Application.Commands;
using dotFitness.Modules.Users.Application.Queries;
using dotFitness.Modules.Users.Application.Validators;
using dotFitness.Modules.Users.Application.DTOs;
using dotFitness.Modules.Users.Application.Abstractions;
using dotFitness.SharedKernel.Results;
using dotFitness.SharedKernel.Inbox;

namespace dotFitness.Modules.Users.Infrastructure.Installers;

/// <summary>
/// Implementation of IUsersModuleInstaller that registers all Users module services
/// </summary>
public class UsersModuleInstaller : IUsersModuleInstaller
{
    public IServiceCollection Install(IServiceCollection services, IConfiguration configuration)
    {
        // Configure User Module Settings
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
        services.Configure<AdminSettings>(configuration.GetSection("AdminSettings"));

        // Configure JWT Authentication (since it's primarily used for user authentication)
        var jwtSettings = configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"];

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!)),
                    ClockSkew = TimeSpan.Zero
                };
            });

        services.AddAuthorization();

        // Register MongoDB collections specific to Users module
        services.AddSingleton(sp =>
        {
            var database = sp.GetRequiredService<IMongoDatabase>();
            return database.GetCollection<User>("users");
        });

        services.AddSingleton(sp =>
        {
            var database = sp.GetRequiredService<IMongoDatabase>();
            return database.GetCollection<UserMetric>("userMetrics");
        });

        // Register repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserMetricsRepository, UserMetricsRepository>();

        // Register Inbox collection (shared inboxMessages)
        services.AddSingleton(sp =>
        {
            var database = sp.GetRequiredService<IMongoDatabase>();
            return database.GetCollection<InboxMessage>("inboxMessages");
        });

        // Register MediatR handlers
        services.AddScoped<IRequestHandler<LoginWithGoogleCommand, Result<LoginResponseDto>>, LoginWithGoogleCommandHandler>();
        services.AddScoped<IRequestHandler<UpdateUserProfileCommand, Result<UserDto>>, UpdateUserProfileCommandHandler>();
        services.AddScoped<IRequestHandler<AddUserMetricCommand, Result<UserMetricDto>>, AddUserMetricCommandHandler>();
        services.AddScoped<IRequestHandler<GetUserByIdQuery, Result<UserDto>>, GetUserByIdQueryHandler>();
        services.AddScoped<IRequestHandler<GetUserProfileQuery, Result<UserDto>>, GetUserProfileQueryHandler>();
        services.AddScoped<IRequestHandler<GetLatestUserMetricQuery, Result<UserMetricDto>>, GetLatestUserMetricQueryHandler>();
        services.AddScoped<IRequestHandler<GetUserMetricsQuery, Result<IEnumerable<UserMetricDto>>>, GetUserMetricsQueryHandler>();

        // Register validators
        services.AddScoped<IValidator<LoginWithGoogleCommand>, LoginWithGoogleCommandValidator>();
        services.AddScoped<IValidator<UpdateUserProfileCommand>, UpdateUserProfileCommandValidator>();
        services.AddScoped<IValidator<AddUserMetricCommand>, AddUserMetricCommandValidator>();

        // Register Mapperly mappers - they will be generated as implementations
        services.AddScoped<UserMapper>();
        services.AddScoped<UserMetricMapper>();

        // Register the installer itself so it can be resolved
        services.AddSingleton<IUsersModuleInstaller, UsersModuleInstaller>();

        return services;
    }

    public async Task ConfigureIndexes(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var database = scope.ServiceProvider.GetRequiredService<IMongoDatabase>();
        
        // Create indexes for User collection
        var userCollection = database.GetCollection<User>("users");
        var userIndexBuilder = Builders<User>.IndexKeys;
        
        await userCollection.Indexes.CreateManyAsync(new[]
        {
            new CreateIndexModel<User>(userIndexBuilder.Ascending(x => x.Email), new CreateIndexOptions { Unique = true }),
            new CreateIndexModel<User>(userIndexBuilder.Ascending(x => x.GoogleId)),
            new CreateIndexModel<User>(userIndexBuilder.Ascending(x => x.CreatedAt)),
            new CreateIndexModel<User>(userIndexBuilder.Ascending(x => x.Roles))
        });

        // Create indexes for UserMetric collection
        var userMetricCollection = database.GetCollection<UserMetric>("userMetrics");
        var userMetricIndexBuilder = Builders<UserMetric>.IndexKeys;
        
        await userMetricCollection.Indexes.CreateManyAsync(new[]
        {
            new CreateIndexModel<UserMetric>(userMetricIndexBuilder.Ascending(x => x.UserId)),
            new CreateIndexModel<UserMetric>(userMetricIndexBuilder.Ascending(x => x.Date)),
            new CreateIndexModel<UserMetric>(userMetricIndexBuilder.Ascending(x => x.UserId).Descending(x => x.Date)),
            new CreateIndexModel<UserMetric>(userMetricIndexBuilder.Ascending(x => x.UserId).Ascending(x => x.Date), new CreateIndexOptions { Unique = true })
        });

        // Create indexes for Inbox collection (Users consumers)
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
}
