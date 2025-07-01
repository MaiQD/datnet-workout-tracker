# Adding a New Module to dotFitness

> Based on modular monolith best practices from [modular-monolith-with-ddd](https://github.com/MaiQD/modular-monolith-with-ddd/blob/master/README.md)

This guide walks you through creating a new module in the dotFitness modular monolith architecture. Each module follows Clean Architecture principles with Domain-Driven Design (DDD) patterns.

## 🎯 Module Architecture Overview

Each module is a **vertical slice** that contains:
- **Domain Layer**: Business entities, domain events, and repository interfaces
- **Application Layer**: Commands, queries, DTOs, and mappers
- **Infrastructure Layer**: Command/query handlers and repository implementations
- **Tests**: Comprehensive test coverage for all layers

## 📁 Module Structure Template

```
dotFitness.Modules.{ModuleName}/
├── dotFitness.Modules.{ModuleName}.Domain/
│   ├── Entities/
│   ├── Events/
│   └── Repositories/
├── dotFitness.Modules.{ModuleName}.Application/
│   ├── Commands/
│   ├── Queries/
│   ├── DTOs/
│   ├── Mappers/
│   ├── Validators/
│   └── Configuration/
├── dotFitness.Modules.{ModuleName}.Infrastructure/
│   ├── Handlers/
│   ├── Repositories/
│   ├── Configuration/
│   └── Settings/
└── dotFitness.Modules.{ModuleName}.Tests/
    ├── Domain/
    ├── Application/
    ├── Infrastructure/
    └── MongoDB/
```

## 🚀 Step-by-Step Module Creation

### Step 1: Create Module Projects

Create the four core projects for your module:

```bash
# Create Domain project
dotnet new classlib -n dotFitness.Modules.{ModuleName}.Domain
dotnet new classlib -n dotFitness.Modules.{ModuleName}.Application
dotnet new classlib -n dotFitness.Modules.{ModuleName}.Infrastructure
dotnet new xunit -n dotFitness.Modules.{ModuleName}.Tests
```

### Step 2: Set Up Project References

```bash
# Domain project references
dotnet add dotFitness.Modules.{ModuleName}.Domain/dotFitness.Modules.{ModuleName}.Domain.csproj reference dotFitness.SharedKernel/dotFitness.SharedKernel.csproj

# Application project references
dotnet add dotFitness.Modules.{ModuleName}.Application/dotFitness.Modules.{ModuleName}.Application.csproj reference dotFitness.Modules.{ModuleName}.Domain/dotFitness.Modules.{ModuleName}.Domain.csproj
dotnet add dotFitness.Modules.{ModuleName}.Application/dotFitness.Modules.{ModuleName}.Application.csproj reference dotFitness.SharedKernel/dotFitness.SharedKernel.csproj

# Infrastructure project references
dotnet add dotFitness.Modules.{ModuleName}.Infrastructure/dotFitness.Modules.{ModuleName}.Infrastructure.csproj reference dotFitness.Modules.{ModuleName}.Domain/dotFitness.Modules.{ModuleName}.Domain.csproj
dotnet add dotFitness.Modules.{ModuleName}.Infrastructure/dotFitness.Modules.{ModuleName}.Infrastructure.csproj reference dotFitness.Modules.{ModuleName}.Application/dotFitness.Modules.{ModuleName}.Application.csproj
dotnet add dotFitness.Modules.{ModuleName}.Infrastructure/dotFitness.Modules.{ModuleName}.Infrastructure.csproj reference dotFitness.SharedKernel/dotFitness.SharedKernel.csproj

# Tests project references
dotnet add dotFitness.Modules.{ModuleName}.Tests/dotFitness.Modules.{ModuleName}.Tests.csproj reference dotFitness.Modules.{ModuleName}.Domain/dotFitness.Modules.{ModuleName}.Domain.csproj
dotnet add dotFitness.Modules.{ModuleName}.Tests/dotFitness.Modules.{ModuleName}.Tests.csproj reference dotFitness.Modules.{ModuleName}.Application/dotFitness.Modules.{ModuleName}.Application.csproj
dotnet add dotFitness.Modules.{ModuleName}.Tests/dotFitness.Modules.{ModuleName}.Tests.csproj reference dotFitness.Modules.{ModuleName}.Infrastructure/dotFitness.Modules.{ModuleName}.Infrastructure.csproj
dotnet add dotFitness.Modules.{ModuleName}.Tests/dotFitness.Modules.{ModuleName}.Tests.csproj reference dotFitness.SharedKernel/dotFitness.SharedKernel.csproj
```

### Step 3: Add Required NuGet Packages

#### Domain Project
```xml
<ItemGroup>
    <PackageReference Include="MediatR" Version="12.2.0" />
</ItemGroup>
```

#### Application Project
```xml
<ItemGroup>
    <PackageReference Include="MediatR" Version="12.2.0" />
    <PackageReference Include="FluentValidation" Version="11.8.1" />
    <PackageReference Include="FluentValidation.DependencyInjectionExtensions" Version="11.8.1" />
</ItemGroup>
```

#### Infrastructure Project
```xml
<ItemGroup>
    <PackageReference Include="MediatR" Version="12.2.0" />
    <PackageReference Include="MongoDB.Driver" Version="2.23.1" />
    <PackageReference Include="Microsoft.Extensions.Options" Version="8.0.0" />
    <PackageReference Include="Microsoft.Extensions.Configuration" Version="8.0.0" />
</ItemGroup>
```

#### Tests Project
```xml
<ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.8.0" />
    <PackageReference Include="xunit" Version="2.6.6" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.5.6" />
    <PackageReference Include="FluentAssertions" Version="6.12.0" />
    <PackageReference Include="Moq" Version="4.20.70" />
    <PackageReference Include="Testcontainers.MongoDb" Version="3.7.0" />
</ItemGroup>
```

### Step 4: Create Domain Layer

#### 1. Create Domain Entity
```csharp
// dotFitness.Modules.{ModuleName}.Domain/Entities/{EntityName}.cs
using dotFitness.SharedKernel.Interfaces;

namespace dotFitness.Modules.{ModuleName}.Domain.Entities;

public class {EntityName} : IEntity
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Business logic methods
    public void UpdateName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new ArgumentException("Name cannot be empty");
        
        Name = newName;
        UpdatedAt = DateTime.UtcNow;
    }
}
```

#### 2. Create Domain Events
```csharp
// dotFitness.Modules.{ModuleName}.Domain/Events/{EntityName}CreatedEvent.cs
using MediatR;

namespace dotFitness.Modules.{ModuleName}.Domain.Events;

public class {EntityName}CreatedEvent : INotification
{
    public string {EntityName}Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
```

#### 3. Create Repository Interface
```csharp
// dotFitness.Modules.{ModuleName}.Domain/Repositories/I{EntityName}Repository.cs
namespace dotFitness.Modules.{ModuleName}.Domain.Repositories;

public interface I{EntityName}Repository
{
    Task<{EntityName}?> GetByIdAsync(string id);
    Task<IEnumerable<{EntityName}>> GetAllAsync();
    Task<{EntityName}?> GetByNameAsync(string name);
    Task CreateAsync({EntityName} {entityName});
    Task UpdateAsync({EntityName} {entityName});
    Task DeleteAsync(string id);
    Task<bool> ExistsAsync(string id);
}
```

### Step 5: Create Application Layer

#### 1. Create Commands
```csharp
// dotFitness.Modules.{ModuleName}.Application/Commands/Create{EntityName}Command.cs
using MediatR;
using dotFitness.SharedKernel.Results;
using dotFitness.Modules.{ModuleName}.Application.DTOs;

namespace dotFitness.Modules.{ModuleName}.Application.Commands;

public class Create{EntityName}Command : IRequest<Result<{EntityName}Dto>>
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
```

#### 2. Create Queries
```csharp
// dotFitness.Modules.{ModuleName}.Application/Queries/Get{EntityName}ByIdQuery.cs
using MediatR;
using dotFitness.SharedKernel.Results;
using dotFitness.Modules.{ModuleName}.Application.DTOs;

namespace dotFitness.Modules.{ModuleName}.Application.Queries;

public class Get{EntityName}ByIdQuery : IRequest<Result<{EntityName}Dto>>
{
    public string Id { get; set; } = string.Empty;
}
```

#### 3. Create DTOs
```csharp
// dotFitness.Modules.{ModuleName}.Application/DTOs/{EntityName}Dto.cs
namespace dotFitness.Modules.{ModuleName}.Application.DTOs;

public class {EntityName}Dto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

#### 4. Create Static Mappers
```csharp
// dotFitness.Modules.{ModuleName}.Application/Mappers/{EntityName}Mapper.cs
using dotFitness.Modules.{ModuleName}.Domain.Entities;
using dotFitness.Modules.{ModuleName}.Application.DTOs;

namespace dotFitness.Modules.{ModuleName}.Application.Mappers;

public static class {EntityName}Mapper
{
    public static {EntityName}Dto ToDto({EntityName} {entityName}) => new()
    {
        Id = {entityName}.Id,
        Name = {entityName}.Name,
        Description = {entityName}.Description,
        CreatedAt = {entityName}.CreatedAt,
        UpdatedAt = {entityName}.UpdatedAt
    };

    public static {EntityName} ToEntity({EntityName}Dto dto) => new()
    {
        Id = dto.Id,
        Name = dto.Name,
        Description = dto.Description,
        CreatedAt = dto.CreatedAt,
        UpdatedAt = dto.UpdatedAt
    };
}
```

#### 5. Create Validators
```csharp
// dotFitness.Modules.{ModuleName}.Application/Validators/Create{EntityName}CommandValidator.cs
using FluentValidation;
using dotFitness.Modules.{ModuleName}.Application.Commands;

namespace dotFitness.Modules.{ModuleName}.Application.Validators;

public class Create{EntityName}CommandValidator : AbstractValidator<Create{EntityName}Command>
{
    public Create{EntityName}CommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");
    }
}
```

#### 6. Create Module Registration
```csharp
// dotFitness.Modules.{ModuleName}.Application/Configuration/{ModuleName}ModuleRegistration.cs
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using dotFitness.Modules.{ModuleName}.Application.Validators;

namespace dotFitness.Modules.{ModuleName}.Application.Configuration;

public static class {ModuleName}ModuleRegistration
{
    public static IServiceCollection Add{ModuleName}Module(this IServiceCollection services, IConfiguration configuration)
    {
        // Register validators
        services.AddValidatorsFromAssemblyContaining<Create{EntityName}CommandValidator>();

        // Register module-specific services
        services.Configure<{ModuleName}Settings>(configuration.GetSection("{ModuleName}"));

        return services;
    }
}
```

### Step 6: Create Infrastructure Layer

#### 1. Create Command Handlers
```csharp
// dotFitness.Modules.{ModuleName}.Infrastructure/Handlers/Create{EntityName}CommandHandler.cs
using MediatR;
using dotFitness.SharedKernel.Results;
using dotFitness.Modules.{ModuleName}.Domain.Entities;
using dotFitness.Modules.{ModuleName}.Domain.Repositories;
using dotFitness.Modules.{ModuleName}.Domain.Events;
using dotFitness.Modules.{ModuleName}.Application.Commands;
using dotFitness.Modules.{ModuleName}.Application.DTOs;
using dotFitness.Modules.{ModuleName}.Application.Mappers;

namespace dotFitness.Modules.{ModuleName}.Infrastructure.Handlers;

public class Create{EntityName}CommandHandler : IRequestHandler<Create{EntityName}Command, Result<{EntityName}Dto>>
{
    private readonly I{EntityName}Repository _{entityName}Repository;
    private readonly IMediator _mediator;

    public Create{EntityName}CommandHandler(I{EntityName}Repository {entityName}Repository, IMediator mediator)
    {
        _{entityName}Repository = {entityName}Repository;
        _mediator = mediator;
    }

    public async Task<Result<{EntityName}Dto>> Handle(Create{EntityName}Command request, CancellationToken cancellationToken)
    {
        try
        {
            // Check if entity already exists
            var existing = await _{entityName}Repository.GetByNameAsync(request.Name);
            if (existing != null)
            {
                return Result<{EntityName}Dto>.Failure($"A {entityName} with name '{request.Name}' already exists");
            }

            // Create new entity
            var {entityName} = new {EntityName}
            {
                Id = Guid.NewGuid().ToString(),
                Name = request.Name,
                Description = request.Description,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _{entityName}Repository.CreateAsync({entityName});

            // Publish domain event
            await _mediator.Publish(new {EntityName}CreatedEvent
            {
                {EntityName}Id = {entityName}.Id,
                Name = {entityName}.Name,
                CreatedAt = {entityName}.CreatedAt
            }, cancellationToken);

            return Result<{EntityName}Dto>.Success({EntityName}Mapper.ToDto({entityName}));
        }
        catch (Exception ex)
        {
            return Result<{EntityName}Dto>.Failure($"Failed to create {entityName}: {ex.Message}");
        }
    }
}
```

#### 2. Create Query Handlers
```csharp
// dotFitness.Modules.{ModuleName}.Infrastructure/Handlers/Get{EntityName}ByIdQueryHandler.cs
using MediatR;
using dotFitness.SharedKernel.Results;
using dotFitness.Modules.{ModuleName}.Domain.Repositories;
using dotFitness.Modules.{ModuleName}.Application.Queries;
using dotFitness.Modules.{ModuleName}.Application.DTOs;
using dotFitness.Modules.{ModuleName}.Application.Mappers;

namespace dotFitness.Modules.{ModuleName}.Infrastructure.Handlers;

public class Get{EntityName}ByIdQueryHandler : IRequestHandler<Get{EntityName}ByIdQuery, Result<{EntityName}Dto>>
{
    private readonly I{EntityName}Repository _{entityName}Repository;

    public Get{EntityName}ByIdQueryHandler(I{EntityName}Repository {entityName}Repository)
    {
        _{entityName}Repository = {entityName}Repository;
    }

    public async Task<Result<{EntityName}Dto>> Handle(Get{EntityName}ByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var {entityName} = await _{entityName}Repository.GetByIdAsync(request.Id);
            if ({entityName} == null)
            {
                return Result<{EntityName}Dto>.Failure($"{EntityName} with ID '{request.Id}' not found");
            }

            return Result<{EntityName}Dto>.Success({EntityName}Mapper.ToDto({entityName}));
        }
        catch (Exception ex)
        {
            return Result<{EntityName}Dto>.Failure($"Failed to retrieve {entityName}: {ex.Message}");
        }
    }
}
```

#### 3. Create Repository Implementation
```csharp
// dotFitness.Modules.{ModuleName}.Infrastructure/Repositories/{EntityName}Repository.cs
using MongoDB.Driver;
using dotFitness.Modules.{ModuleName}.Domain.Entities;
using dotFitness.Modules.{ModuleName}.Domain.Repositories;

namespace dotFitness.Modules.{ModuleName}.Infrastructure.Repositories;

public class {EntityName}Repository : I{EntityName}Repository
{
    private readonly IMongoCollection<{EntityName}> _collection;

    public {EntityName}Repository(IMongoDatabase database)
    {
        _collection = database.GetCollection<{EntityName}>("{entityNames}");
    }

    public async Task<{EntityName}?> GetByIdAsync(string id)
    {
        return await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<{EntityName}>> GetAllAsync()
    {
        return await _collection.Find(_ => true).ToListAsync();
    }

    public async Task<{EntityName}?> GetByNameAsync(string name)
    {
        return await _collection.Find(x => x.Name == name).FirstOrDefaultAsync();
    }

    public async Task CreateAsync({EntityName} {entityName})
    {
        await _collection.InsertOneAsync({entityName});
    }

    public async Task UpdateAsync({EntityName} {entityName})
    {
        await _collection.ReplaceOneAsync(x => x.Id == {entityName}.Id, {entityName});
    }

    public async Task DeleteAsync(string id)
    {
        await _collection.DeleteOneAsync(x => x.Id == id);
    }

    public async Task<bool> ExistsAsync(string id)
    {
        return await _collection.Find(x => x.Id == id).AnyAsync();
    }
}
```

#### 4. Create Infrastructure Module Registration
```csharp
// dotFitness.Modules.{ModuleName}.Infrastructure/Configuration/{ModuleName}InfrastructureModule.cs
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using dotFitness.Modules.{ModuleName}.Domain.Repositories;
using dotFitness.Modules.{ModuleName}.Infrastructure.Repositories;

namespace dotFitness.Modules.{ModuleName}.Infrastructure.Configuration;

public static class {ModuleName}InfrastructureModule
{
    public static IServiceCollection Add{ModuleName}Infrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Register repositories
        services.AddScoped<I{EntityName}Repository, {EntityName}Repository>();

        return services;
    }
}
```

### Step 7: Create Tests

#### 1. Domain Tests
```csharp
// dotFitness.Modules.{ModuleName}.Tests/Domain/Entities/{EntityName}Tests.cs
using FluentAssertions;
using dotFitness.Modules.{ModuleName}.Domain.Entities;

namespace dotFitness.Modules.{ModuleName}.Tests.Domain.Entities;

public class {EntityName}Tests
{
    [Fact]
    public void UpdateName_ValidName_UpdatesSuccessfully()
    {
        // Arrange
        var {entityName} = new {EntityName}
        {
            Id = "1",
            Name = "Old Name",
            Description = "Test Description"
        };

        // Act
        {entityName}.UpdateName("New Name");

        // Assert
        {entityName}.Name.Should().Be("New Name");
        {entityName}.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void UpdateName_EmptyName_ThrowsArgumentException()
    {
        // Arrange
        var {entityName} = new {EntityName} { Id = "1", Name = "Test" };

        // Act & Assert
        var action = () => {entityName}.UpdateName("");
        action.Should().Throw<ArgumentException>().WithMessage("Name cannot be empty");
    }
}
```

#### 2. Application Tests
```csharp
// dotFitness.Modules.{ModuleName}.Tests/Application/Validators/Create{EntityName}CommandValidatorTests.cs
using FluentAssertions;
using FluentValidation.TestHelper;
using dotFitness.Modules.{ModuleName}.Application.Commands;
using dotFitness.Modules.{ModuleName}.Application.Validators;

namespace dotFitness.Modules.{ModuleName}.Tests.Application.Validators;

public class Create{EntityName}CommandValidatorTests
{
    private readonly Create{EntityName}CommandValidator _validator;

    public Create{EntityName}CommandValidatorTests()
    {
        _validator = new Create{EntityName}CommandValidator();
    }

    [Fact]
    public void Validate_ValidCommand_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var command = new Create{EntityName}Command
        {
            Name = "Test {EntityName}",
            Description = "Test Description"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EmptyName_ShouldHaveValidationError()
    {
        // Arrange
        var command = new Create{EntityName}Command
        {
            Name = "",
            Description = "Test Description"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }
}
```

### Step 8: Register Module in Main Application

#### 1. Add Module to ModuleRegistry
```csharp
// In dotFitness.Api/Infrastructure/ModuleRegistry.cs
public static readonly string[] ModuleNames = 
{
    "Users",
    "Exercises", 
    "{ModuleName}",  // Add your new module here
    "Routines",
    "WorkoutLogs"
};
```

#### 2. Add API Controller
```csharp
// dotFitness.Api/Controllers/{ModuleName}Controller.cs
using Microsoft.AspNetCore.Mvc;
using MediatR;
using dotFitness.Modules.{ModuleName}.Application.Commands;
using dotFitness.Modules.{ModuleName}.Application.Queries;

namespace dotFitness.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class {ModuleName}Controller : ControllerBase
{
    private readonly IMediator _mediator;

    public {ModuleName}Controller(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(Create{EntityName}Command command)
    {
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var query = new Get{EntityName}ByIdQuery { Id = id };
        var result = await _mediator.Send(query);
        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
    }
}
```

### Step 9: Configure MongoDB Indexes

```csharp
// dotFitness.Modules.{ModuleName}.Infrastructure/Configuration/{ModuleName}IndexConfiguration.cs
using MongoDB.Driver;

namespace dotFitness.Modules.{ModuleName}.Infrastructure.Configuration;

public static class {ModuleName}IndexConfiguration
{
    public static async Task ConfigureIndexes(IMongoDatabase database)
    {
        var collection = database.GetCollection<{EntityName}>("{entityNames}");

        // Create indexes
        var indexKeysDefinition = Builders<{EntityName}>.IndexKeys.Ascending(x => x.Name);
        var indexOptions = new CreateIndexOptions { Unique = true };
        var indexModel = new CreateIndexModel<{EntityName}>(indexKeysDefinition, indexOptions);

        await collection.Indexes.CreateOneAsync(indexModel);
    }
}
```

## 🎯 Best Practices

### 1. **Naming Conventions**
- Use PascalCase for class names and properties
- Use camelCase for variables and parameters
- Use descriptive names that reflect the domain language

### 2. **Error Handling**
- Use the Result pattern for explicit error handling
- Don't throw exceptions for business rule violations
- Provide meaningful error messages

### 3. **Validation**
- Validate at the application layer with FluentValidation
- Keep domain entities focused on business logic
- Validate input early in the request pipeline

### 4. **Testing**
- Write tests for all layers (Domain, Application, Infrastructure)
- Use meaningful test names that describe the scenario
- Test both success and failure cases

### 5. **Performance**
- Use static mappers for zero runtime overhead
- Implement proper MongoDB indexes
- Use async/await consistently

## 🔧 Module Configuration

### Environment Variables
```json
{
  "{ModuleName}": {
    "DatabaseName": "dotFitness",
    "CollectionName": "{entityNames}"
  }
}
```

### Health Checks
The module will automatically register health checks through the ModuleRegistry system.

### Metrics
Module registration and performance metrics are automatically tracked through the ModuleMetrics system.

## 📚 References

- [Modular Monolith with DDD](https://github.com/MaiQD/modular-monolith-with-ddd/blob/master/README.md) - Reference implementation
- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html) - Architectural principles
- [Domain-Driven Design](https://martinfowler.com/bliki/DomainDrivenDesign.html) - DDD concepts
- [CQRS Pattern](https://martinfowler.com/bliki/CQRS.html) - Command Query Responsibility Segregation
