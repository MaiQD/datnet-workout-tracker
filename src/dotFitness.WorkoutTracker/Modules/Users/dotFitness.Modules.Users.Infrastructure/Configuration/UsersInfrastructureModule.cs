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
using dotFitness.SharedKernel.Results;
using dotFitness.SharedKernel.Inbox;

namespace dotFitness.Modules.Users.Infrastructure.Configuration;

/// <summary>
/// Configuration class for Users module infrastructure services.
/// This class is now deprecated in favor of UsersModuleInstaller which implements IModuleInstaller.
/// </summary>
public static class UsersInfrastructureModule
{
    // This class is kept for backward compatibility but is no longer used.
    // All functionality has been moved to UsersModuleInstaller.
}
