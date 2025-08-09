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
using dotFitness.SharedKernel.Inbox;
using dotFitness.Modules.Exercises.Domain.Entities;

namespace dotFitness.Modules.Exercises.Infrastructure.Configuration;

/// <summary>
/// Configuration class for Exercises module infrastructure services.
/// This class is now deprecated in favor of ExercisesModuleInstaller which implements IModuleInstaller.
/// </summary>
public static class ExercisesInfrastructureModule
{
    // This class is kept for backward compatibility but is no longer used.
    // All functionality has been moved to ExercisesModuleInstaller.
}
