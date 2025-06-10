# Adding New Modules to dotFitness Modular Monolith

This document provides step-by-step instructions for adding new modules to the dotFitness workout tracker application, following Clean Architecture and Modular Monolith principles.

## Table of Contents

1. [Overview](#overview)
2. [Module Structure](#module-structure)
3. [Generic Module Discovery System](#generic-module-discovery-system)
4. [Step-by-Step Instructions](#step-by-step-instructions)
5. [Code Templates](#code-templates)
6. [Integration Steps](#integration-steps)
7. [Testing Your Module](#testing-your-module)
8. [Best Practices](#best-practices)

## Overview

The dotFitness application follows a **Modular Monolith** architecture where each module is self-contained with its own:
- **Domain Layer**: Entities, value objects, domain events, and repository interfaces
- **Application Layer**: Commands, queries, DTOs, validators, and mappers
- **Infrastructure Layer**: Repository implementations, handlers, and external integrations
- **Tests**: Unit and integration tests

Each module is automatically discovered and registered through the advanced [`ModuleRegistry`](../src/dotFitness.WorkoutTracker/dotFitness.Api/Infrastructure/ModuleRegistry.cs) system.

## Generic Module Discovery System

The dotFitness application features a sophisticated **automatic module discovery and registration system** that:

### ✅ **Key Features**
- **Zero Configuration**: Add module name to array, everything else is automatic
- **Clean Architecture Compliance**: API only references Application layers
- **Graceful Degradation**: Missing modules don't break the system
- **Comprehensive Logging**: Full visibility into registration process
- **MediatR Integration**: Automatic handler discovery and registration
- **MongoDB Integration**: Automatic index configuration per module
- **Scalable Pattern**: Supports unlimited modules without code changes

### 🔧 **How It Works**
1. **Module Discovery**: Scans [`ModuleRegistry.ModuleNames`](../src/dotFitness.WorkoutTracker/dotFitness.Api/Infrastructure/ModuleRegistry.cs) array
2. **Assembly Loading**: Loads both Application and Infrastructure assemblies via reflection
3. **Service Registration**: Invokes each module's registration method automatically
4. **MediatR Registration**: Registers all commands, queries, and handlers
5. **Index Configuration**: Configures MongoDB indexes for module entities
6. **Error Handling**: Logs warnings for missing modules but continues operation

### 📋 **Registration Logs**
When working correctly, you'll see logs like:
```bash
[INF] Loaded Users Application assembly for MediatR
[INF] Loaded Users Infrastructure assembly for MediatR  
[INF] Registered MediatR services from assembly: dotFitness.Modules.Users.Application
[INF] Registered MediatR services from assembly: dotFitness.Modules.Users.Infrastructure
[INF] Successfully registered module: Users
[INF] Successfully configured indexes for module: Users
[INF] MediatR registration completed for X module assemblies
```

## Module Structure

Every module follows this consistent structure:

```
Modules/
└── {ModuleName}/
    ├── dotFitness.Modules.{ModuleName}.Domain/
    │   ├── Entities/
    │   ├── Events/
    │   └── Repositories/
    ├── dotFitness.Modules.{ModuleName}.Application/
    │   ├── Commands/
    │   ├── Queries/
    │   ├── DTOs/
    │   ├── Validators/
    │   ├── Mappers/
    │   └── Configuration/
    ├── dotFitness.Modules.{ModuleName}.Infrastructure/
    │   ├── Repositories/
    │   ├── Handlers/
    │   ├── Settings/
    │   └── Configuration/
    └── dotFitness.Modules.{ModuleName}.Tests/
```

## Step-by-Step Instructions

### Step 1: Update Module Registry

The **only configuration change needed** is adding your module name to the discovery array:

```csharp
// File: dotFitness.Api/Infrastructure/ModuleRegistry.cs
public static readonly string[] ModuleNames = 
{
    "Users",           // ✅ Already implemented
    "Exercises",       // ⏳ Coming next  
    "Routines",        // ⏳ Planned
    "WorkoutLogs",     // ⏳ Planned
    "YourNewModule"    // ← Add your module name here
};
```

That's it! The system will automatically:
- ✅ Discover your module assemblies
- ✅ Register MediatR handlers  
- ✅ Configure MongoDB indexes
- ✅ Handle missing modules gracefully

### Step 2: Create Project Structure

Create the four main projects for your module:

```bash
# Navigate to the Modules directory
cd src/dotFitness.WorkoutTracker/Modules

# Create module directory
mkdir YourNewModule
cd YourNewModule

# Create the four core projects
dotnet new classlib -n dotFitness.Modules.YourNewModule.Domain -f net8.0
dotnet new classlib -n dotFitness.Modules.YourNewModule.Application -f net8.0
dotnet new classlib -n dotFitness.Modules.YourNewModule.Infrastructure -f net8.0
dotnet new xunit -n dotFitness.Modules.YourNewModule.Tests -f net8.0
```

### Step 3: Add Project References

Add the projects to the solution and set up references:

```bash
# Add projects to solution
cd ../../
dotnet sln add Modules/YourNewModule/dotFitness.Modules.YourNewModule.Domain/dotFitness.Modules.YourNewModule.Domain.csproj
dotnet sln add Modules/YourNewModule/dotFitness.Modules.YourNewModule.Application/dotFitness.Modules.YourNewModule.Application.csproj
dotnet sln add Modules/YourNewModule/dotFitness.Modules.YourNewModule.Infrastructure/dotFitness.Modules.YourNewModule.Infrastructure.csproj
dotnet sln add Modules/YourNewModule/dotFitness.Modules.YourNewModule.Tests/dotFitness.Modules.YourNewModule.Tests.csproj

# Set up project references - Domain layer
cd Modules/YourNewModule/dotFitness.Modules.YourNewModule.Domain
dotnet add reference ../../../dotFitness.SharedKernel/dotFitness.SharedKernel.csproj

# Set up project references - Application layer
cd ../dotFitness.Modules.YourNewModule.Application
dotnet add reference ../../../dotFitness.SharedKernel/dotFitness.SharedKernel.csproj
dotnet add reference ../dotFitness.Modules.YourNewModule.Domain/dotFitness.Modules.YourNewModule.Domain.csproj

# Set up project references - Infrastructure layer
cd ../dotFitness.Modules.YourNewModule.Infrastructure
dotnet add reference ../../../dotFitness.SharedKernel/dotFitness.SharedKernel.csproj
dotnet add reference ../dotFitness.Modules.YourNewModule.Domain/dotFitness.Modules.YourNewModule.Domain.csproj
dotnet add reference ../dotFitness.Modules.YourNewModule.Application/dotFitness.Modules.YourNewModule.Application.csproj

# Set up project references - Tests
cd ../dotFitness.Modules.YourNewModule.Tests
dotnet add reference ../../../dotFitness.SharedKernel/dotFitness.SharedKernel.csproj
dotnet add reference ../dotFitness.Modules.YourNewModule.Domain/dotFitness.Modules.YourNewModule.Domain.csproj
dotnet add reference ../dotFitness.Modules.YourNewModule.Application/dotFitness.Modules.YourNewModule.Application.csproj
dotnet add reference ../dotFitness.Modules.YourNewModule.Infrastructure/dotFitness.Modules.YourNewModule.Infrastructure.csproj
```

### Step 4: Add API Project Reference

Add reference from the API project to your module's Application layer:

```bash
cd ../../../dotFitness.Api
dotnet add reference ../Modules/YourNewModule/dotFitness.Modules.YourNewModule.Application/dotFitness.Modules.YourNewModule.Application.csproj
```

**Important**: The API project should **only** reference the Application layer to maintain Clean Architecture principles.

### Step 5: Install Required NuGet Packages

Add necessary packages to each project:

```bash
# Application Layer Packages
cd ../Modules/YourNewModule/dotFitness.Modules.YourNewModule.Application
dotnet add package MediatR --version 12.5.0
dotnet add package FluentValidation --version 12.0.0
dotnet add package Riok.Mapperly --version 4.2.1
dotnet add package Microsoft.Extensions.Configuration.Abstractions --version 8.0.0
dotnet add package Microsoft.Extensions.DependencyInjection.Abstractions --version 8.0.2

# Infrastructure Layer Packages
cd ../dotFitness.Modules.YourNewModule.Infrastructure
dotnet add package MediatR --version 12.5.0
dotnet add package FluentValidation --version 12.0.0
dotnet add package MongoDB.Driver --version 3.4.0
dotnet add package Microsoft.Extensions.Options --version 8.0.2
dotnet add package Microsoft.Extensions.Configuration.Abstractions --version 8.0.0
dotnet add package Microsoft.Extensions.DependencyInjection.Abstractions --version 8.0.2

# Tests Project Packages
cd ../dotFitness.Modules.YourNewModule.Tests
dotnet add package Moq --version 4.20.72
dotnet add package FluentAssertions --version 7.0.0
```

## Code Templates

### Domain Entity Template

```csharp
// File: Entities/YourEntity.cs
using dotFitness.SharedKernel.Interfaces;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace dotFitness.Modules.YourNewModule.Domain.Entities;

public class YourEntity : IEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    // Add your domain properties here
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
```

### Repository Interface Template

```csharp
// File: Repositories/IYourEntityRepository.cs
using dotFitness.Modules.YourNewModule.Domain.Entities;
using dotFitness.SharedKernel.Results;

namespace dotFitness.Modules.YourNewModule.Domain.Repositories;

public interface IYourEntityRepository
{
    Task<Result<YourEntity>> GetByIdAsync(string id);
    Task<Result<IEnumerable<YourEntity>>> GetAllAsync();
    Task<Result<YourEntity>> CreateAsync(YourEntity entity);
    Task<Result<YourEntity>> UpdateAsync(YourEntity entity);
    Task<Result<bool>> DeleteAsync(string id);
}
```

### Command Template

```csharp
// File: Commands/CreateYourEntityCommand.cs
using MediatR;
using dotFitness.Modules.YourNewModule.Application.DTOs;
using dotFitness.SharedKernel.Results;

namespace dotFitness.Modules.YourNewModule.Application.Commands;

public record CreateYourEntityCommand(
    string Name,
    string Description
) : IRequest<Result<YourEntityDto>>;
```

### Query Template

```csharp
// File: Queries/GetYourEntityByIdQuery.cs
using MediatR;
using dotFitness.Modules.YourNewModule.Application.DTOs;
using dotFitness.SharedKernel.Results;

namespace dotFitness.Modules.YourNewModule.Application.Queries;

public record GetYourEntityByIdQuery(string Id) : IRequest<Result<YourEntityDto>>;
```

### DTO Template

```csharp
// File: DTOs/YourEntityDto.cs
namespace dotFitness.Modules.YourNewModule.Application.DTOs;

public record YourEntityDto(
    string Id,
    string Name,
    string Description,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);
```

### Validator Template

```csharp
// File: Validators/CreateYourEntityCommandValidator.cs
using FluentValidation;
using dotFitness.Modules.YourNewModule.Application.Commands;

namespace dotFitness.Modules.YourNewModule.Application.Validators;

public class CreateYourEntityCommandValidator : AbstractValidator<CreateYourEntityCommand>
{
    public CreateYourEntityCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required")
            .MaximumLength(100)
            .WithMessage("Name cannot exceed 100 characters");

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .WithMessage("Description cannot exceed 500 characters");
    }
}
```

### Mapper Template

```csharp
// File: Mappers/IYourEntityMapper.cs
using Riok.Mapperly.Abstractions;
using dotFitness.Modules.YourNewModule.Domain.Entities;
using dotFitness.Modules.YourNewModule.Application.DTOs;

namespace dotFitness.Modules.YourNewModule.Application.Mappers;

[Mapper]
public partial class YourEntityMapper
{
    public partial YourEntityDto ToDto(YourEntity entity);
    public partial YourEntity ToEntity(YourEntityDto dto);
}
```

### Repository Implementation Template

```csharp
// File: Repositories/YourEntityRepository.cs
using MongoDB.Driver;
using Microsoft.Extensions.Logging;
using dotFitness.Modules.YourNewModule.Domain.Entities;
using dotFitness.Modules.YourNewModule.Domain.Repositories;
using dotFitness.SharedKernel.Results;

namespace dotFitness.Modules.YourNewModule.Infrastructure.Repositories;

public class YourEntityRepository : IYourEntityRepository
{
    private readonly IMongoCollection<YourEntity> _collection;
    private readonly ILogger<YourEntityRepository> _logger;

    public YourEntityRepository(
        IMongoCollection<YourEntity> collection,
        ILogger<YourEntityRepository> logger)
    {
        _collection = collection;
        _logger = logger;
    }

    public async Task<Result<YourEntity>> GetByIdAsync(string id)
    {
        try
        {
            var entity = await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();
            
            return entity != null 
                ? Result.Success(entity)
                : Result.Failure<YourEntity>("Entity not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting entity by id {Id}", id);
            return Result.Failure<YourEntity>("Failed to retrieve entity");
        }
    }

    public async Task<Result<IEnumerable<YourEntity>>> GetAllAsync()
    {
        try
        {
            var entities = await _collection.Find(_ => true).ToListAsync();
            return Result.Success<IEnumerable<YourEntity>>(entities);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all entities");
            return Result.Failure<IEnumerable<YourEntity>>("Failed to retrieve entities");
        }
    }

    public async Task<Result<YourEntity>> CreateAsync(YourEntity entity)
    {
        try
        {
            await _collection.InsertOneAsync(entity);
            return Result.Success(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating entity");
            return Result.Failure<YourEntity>("Failed to create entity");
        }
    }

    public async Task<Result<YourEntity>> UpdateAsync(YourEntity entity)
    {
        try
        {
            entity.UpdatedAt = DateTime.UtcNow;
            var result = await _collection.ReplaceOneAsync(x => x.Id == entity.Id, entity);
            
            return result.ModifiedCount > 0
                ? Result.Success(entity)
                : Result.Failure<YourEntity>("Entity not found or not modified");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating entity with id {Id}", entity.Id);
            return Result.Failure<YourEntity>("Failed to update entity");
        }
    }

    public async Task<Result<bool>> DeleteAsync(string id)
    {
        try
        {
            var result = await _collection.DeleteOneAsync(x => x.Id == id);
            
            return result.DeletedCount > 0
                ? Result.Success(true)
                : Result.Failure<bool>("Entity not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting entity with id {Id}", id);
            return Result.Failure<bool>("Failed to delete entity");
        }
    }
}
```

### Command Handler Template

```csharp
// File: Handlers/CreateYourEntityCommandHandler.cs
using MediatR;
using Microsoft.Extensions.Logging;
using dotFitness.Modules.YourNewModule.Application.Commands;
using dotFitness.Modules.YourNewModule.Application.DTOs;
using dotFitness.Modules.YourNewModule.Application.Mappers;
using dotFitness.Modules.YourNewModule.Domain.Entities;
using dotFitness.Modules.YourNewModule.Domain.Repositories;
using dotFitness.SharedKernel.Results;

namespace dotFitness.Modules.YourNewModule.Infrastructure.Handlers;

public class CreateYourEntityCommandHandler : IRequestHandler<CreateYourEntityCommand, Result<YourEntityDto>>
{
    private readonly IYourEntityRepository _repository;
    private readonly YourEntityMapper _mapper;
    private readonly ILogger<CreateYourEntityCommandHandler> _logger;

    public CreateYourEntityCommandHandler(
        IYourEntityRepository repository,
        YourEntityMapper mapper,
        ILogger<CreateYourEntityCommandHandler> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<YourEntityDto>> Handle(CreateYourEntityCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var entity = new YourEntity
            {
                Name = request.Name,
                Description = request.Description
            };

            var createResult = await _repository.CreateAsync(entity);
            
            if (createResult.IsFailure)
            {
                _logger.LogWarning("Failed to create entity: {Error}", createResult.Error);
                return Result.Failure<YourEntityDto>(createResult.Error);
            }

            var dto = _mapper.ToDto(createResult.Value);
            _logger.LogInformation("Successfully created entity with id {Id}", dto.Id);
            
            return Result.Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling CreateYourEntityCommand");
            return Result.Failure<YourEntityDto>("An error occurred while creating the entity");
        }
    }
}
```

### Module Configuration Template

```csharp
// File: Configuration/YourNewModuleRegistration.cs
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace dotFitness.Modules.YourNewModule.Application.Configuration;

public static class YourNewModuleRegistration
{
    public static IServiceCollection AddYourNewModuleModule(this IServiceCollection services, IConfiguration configuration)
    {
        // Use reflection to find and invoke the Infrastructure layer's registration
        var infrastructureAssembly = System.Reflection.Assembly.Load("dotFitness.Modules.YourNewModule.Infrastructure");
        var moduleType = infrastructureAssembly.GetType("dotFitness.Modules.YourNewModule.Infrastructure.Configuration.YourNewModuleInfrastructureModule");
        var addModuleMethod = moduleType?.GetMethod("AddYourNewModuleModule", 
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
        
        if (addModuleMethod != null)
        {
            addModuleMethod.Invoke(null, new object[] { services, configuration });
        }
        else
        {
            throw new InvalidOperationException("Could not find AddYourNewModuleModule method in YourNewModule Infrastructure layer");
        }

        return services;
    }

    public static async Task ConfigureYourNewModuleModuleIndexes(IServiceProvider services)
    {
        // Use reflection to find and invoke the Infrastructure layer's index configuration
        var infrastructureAssembly = System.Reflection.Assembly.Load("dotFitness.Modules.YourNewModule.Infrastructure");
        var moduleType = infrastructureAssembly.GetType("dotFitness.Modules.YourNewModule.Infrastructure.Configuration.YourNewModuleInfrastructureModule");
        var configureIndexesMethod = moduleType?.GetMethod("ConfigureYourNewModuleModuleIndexes", 
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
        
        if (configureIndexesMethod != null)
        {
            var task = (Task?)configureIndexesMethod.Invoke(null, new object[] { services });
            if (task != null)
            {
                await task;
            }
        }
        else
        {
            throw new InvalidOperationException("Could not find ConfigureYourNewModuleModuleIndexes method in YourNewModule Infrastructure layer");
        }
    }
}
```

### Infrastructure Module Template

```csharp
// File: Configuration/YourNewModuleInfrastructureModule.cs
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using MediatR;
using FluentValidation;
using dotFitness.Modules.YourNewModule.Domain.Entities;
using dotFitness.Modules.YourNewModule.Domain.Repositories;
using dotFitness.Modules.YourNewModule.Infrastructure.Repositories;
using dotFitness.Modules.YourNewModule.Infrastructure.Handlers;
using dotFitness.Modules.YourNewModule.Application.Mappers;
using dotFitness.Modules.YourNewModule.Application.Commands;
using dotFitness.Modules.YourNewModule.Application.Queries;
using dotFitness.Modules.YourNewModule.Application.Validators;
using dotFitness.Modules.YourNewModule.Application.DTOs;
using dotFitness.SharedKernel.Results;

namespace dotFitness.Modules.YourNewModule.Infrastructure.Configuration;

public static class YourNewModuleInfrastructureModule
{
    public static IServiceCollection AddYourNewModuleModule(this IServiceCollection services, IConfiguration configuration)
    {
        // Register MongoDB collections
        services.AddSingleton(sp =>
        {
            var database = sp.GetRequiredService<IMongoDatabase>();
            return database.GetCollection<YourEntity>("yourEntities");
        });

        // Register repositories
        services.AddScoped<IYourEntityRepository, YourEntityRepository>();

        // Register MediatR handlers
        services.AddScoped<IRequestHandler<CreateYourEntityCommand, Result<YourEntityDto>>, CreateYourEntityCommandHandler>();
        services.AddScoped<IRequestHandler<GetYourEntityByIdQuery, Result<YourEntityDto>>, GetYourEntityByIdQueryHandler>();

        // Register validators
        services.AddScoped<IValidator<CreateYourEntityCommand>, CreateYourEntityCommandValidator>();

        // Register mappers
        services.AddScoped<YourEntityMapper>();

        return services;
    }

    public static async Task ConfigureYourNewModuleModuleIndexes(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var database = scope.ServiceProvider.GetRequiredService<IMongoDatabase>();
        
        // Create indexes for YourEntity collection
        var collection = database.GetCollection<YourEntity>("yourEntities");
        var indexBuilder = Builders<YourEntity>.IndexKeys;
        
        await collection.Indexes.CreateManyAsync(new[]
        {
            new CreateIndexModel<YourEntity>(indexBuilder.Ascending(x => x.Name)),
            new CreateIndexModel<YourEntity>(indexBuilder.Ascending(x => x.CreatedAt)),
            new CreateIndexModel<YourEntity>(indexBuilder.Text(x => x.Description))
        });
    }
}
```

### API Controller Template

```csharp
// File: dotFitness.Api/Controllers/YourNewModuleController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using dotFitness.Modules.YourNewModule.Application.Commands;
using dotFitness.Modules.YourNewModule.Application.Queries;

namespace dotFitness.Api.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Authorize]
public class YourNewModuleController : ControllerBase
{
    private readonly IMediator _mediator;

    public YourNewModuleController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var query = new GetYourEntityByIdQuery(id);
        var result = await _mediator.Send(query);

        return result.IsSuccess 
            ? Ok(result.Value) 
            : NotFound(result.Error);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateYourEntityCommand command)
    {
        var result = await _mediator.Send(command);

        return result.IsSuccess 
            ? CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value)
            : BadRequest(result.Error);
    }

    // Add more endpoints as needed...
}
```

## Integration Steps

### Step 6: Build and Test Integration

```bash
# Build the solution to ensure everything compiles
cd src/dotFitness.WorkoutTracker
dotnet build

# Run tests to ensure integration works
dotnet test

# Run the API to test module discovery
dotnet run --project dotFitness.Api
```

The ModuleRegistry will automatically:
1. ✅ Discover your new module by name
2. ✅ Register MediatR assemblies (Application + Infrastructure)
3. ✅ Register module services via reflection
4. ✅ Configure MongoDB indexes
5. ✅ Log the registration process

## Testing Your Module

### Step 7: Test API Endpoints

```bash
# Test your new endpoints using the API
curl -X GET "https://localhost:7001/api/v1/YourNewModule/{id}" \
  -H "Authorization: Bearer {jwt-token}"

curl -X POST "https://localhost:7001/api/v1/YourNewModule" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer {jwt-token}" \
  -d '{
    "name": "Test Entity",
    "description": "Test Description"
  }'
```

### Check Swagger Documentation

Navigate to `https://localhost:7001/swagger` to see your new endpoints automatically documented.

## Best Practices

### ✅ Do's
- **Follow naming conventions**: Use consistent naming patterns matching existing modules
- **Use the Result pattern**: Always return `Result<T>` from handlers and repositories
- **Implement proper logging**: Add structured logging with correlation IDs
- **Write validators**: Use FluentValidation for all commands
- **Use Mapperly**: Leverage compile-time mapping for performance
- **Follow Clean Architecture**: Maintain strict layer boundaries
- **Add unit tests**: Test handlers, validators, and repositories

### ❌ Don'ts
- **Don't reference Infrastructure from API**: API should only reference Application layer
- **Don't skip validation**: Always validate input commands
- **Don't ignore errors**: Properly handle and log all exceptions
- **Don't hard-code strings**: Use constants and configuration
- **Don't bypass the Result pattern**: Always use `Result<T>` for error handling

## Module Discovery Logs

When your module is successfully integrated, you should see logs like:

```
[Information] Loaded YourNewModule Application assembly for MediatR
[Information] Loaded YourNewModule Infrastructure assembly for MediatR  
[Information] Registered MediatR services from assembly: dotFitness.Modules.YourNewModule.Application
[Information] Registered MediatR services from assembly: dotFitness.Modules.YourNewModule.Infrastructure
[Information] Successfully registered module: YourNewModule
[Information] Successfully configured indexes for module: YourNewModule
[Information] MediatR registration completed for X module assemblies
```

## Troubleshooting

### Common Issues

1. **Module not discovered**: Check that module name is added to `ModuleRegistry.ModuleNames`
2. **Build errors**: Ensure all project references are correctly set up
3. **Runtime errors**: Check that all required packages are installed
4. **Handler not found**: Verify MediatR registration and handler implementation
5. **Authorization errors**: Ensure controllers have proper `[Authorize]` attributes

### Debug Commands

```bash
# Check if assemblies can be loaded
dotnet run --project dotFitness.Api --verbosity detailed

# Run with development environment for detailed logs
ASPNETCORE_ENVIRONMENT=Development dotnet run --project dotFitness.Api
```

---

## Summary

Following this guide ensures that your new module:
- ✅ Follows Clean Architecture principles
- ✅ Is automatically discovered and registered
- ✅ Integrates seamlessly with existing infrastructure
- ✅ Maintains proper separation of concerns
- ✅ Supports scalable development patterns

Your new module will be fully integrated into the dotFitness Modular Monolith without requiring any changes to the main API project configuration! 🎉
