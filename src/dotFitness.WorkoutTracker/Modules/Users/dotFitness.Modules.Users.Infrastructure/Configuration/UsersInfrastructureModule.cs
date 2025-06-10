using Microsoft.Extensions.DependencyInjection;
using MediatR;
using FluentValidation;
using dotFitness.Modules.Users.Domain.Repositories;
using dotFitness.Modules.Users.Infrastructure.Repositories;
using dotFitness.Modules.Users.Infrastructure.Handlers;
using dotFitness.Modules.Users.Application.Mappers;
using dotFitness.Modules.Users.Application.Commands;
using dotFitness.Modules.Users.Application.Queries;
using dotFitness.Modules.Users.Application.Validators;
using dotFitness.Modules.Users.Application.DTOs;
using dotFitness.SharedKernel.Results;

namespace dotFitness.Modules.Users.Infrastructure.Configuration;

/// <summary>
/// Configuration class for Users module infrastructure services
/// </summary>
public static class UsersInfrastructureModule
{
    /// <summary>
    /// Adds Users infrastructure services to the dependency injection container
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddUsersInfrastructure(this IServiceCollection services)
    {
        // Register repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserMetricsRepository, UserMetricsRepository>();

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

        return services;
    }
}
