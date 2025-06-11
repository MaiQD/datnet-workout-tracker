# Add New Module to dotFitness - GitHub Copilot Instructions

## Overview
This prompt guides GitHub Copilot to create a new module in the dotFitness workout tracker application following the established Clean Architecture and modular monolith patterns.

## When to Use This Prompt
Use this prompt when you need to add a completely new module to the dotFitness application (e.g., Exercises, Routines, WorkoutLogs, or any new business domain).

## Project Architecture Context
- **Pattern**: Modular Monolith with Clean Architecture
- **Structure**: Domain → Application → Infrastructure → API
- **Database**: MongoDB with UTC timestamps
- **Patterns**: CQRS with MediatR, Result Pattern, Outbox Pattern
- **Testing**: xUnit, FluentAssertions, Moq, Testcontainers

## Module Creation Instructions

When creating a new module, follow these steps exactly:

### Quick Setup (All Commands)

For rapid module creation, run this complete setup script (replace `[ModuleName]` with your actual module name):

```bash
#!/bin/bash
# Complete module setup script - replace [ModuleName] with actual module name
MODULE_NAME="[ModuleName]"

# Step 1: Create all module projects
dotnet new classlib -n dotFitness.Modules.${MODULE_NAME}.Domain -o Modules/${MODULE_NAME}/dotFitness.Modules.${MODULE_NAME}.Domain && \
dotnet new classlib -n dotFitness.Modules.${MODULE_NAME}.Application -o Modules/${MODULE_NAME}/dotFitness.Modules.${MODULE_NAME}.Application && \
dotnet new classlib -n dotFitness.Modules.${MODULE_NAME}.Infrastructure -o Modules/${MODULE_NAME}/dotFitness.Modules.${MODULE_NAME}.Infrastructure && \
dotnet new xunit -n dotFitness.Modules.${MODULE_NAME}.Tests -o Modules/${MODULE_NAME}/dotFitness.Modules.${MODULE_NAME}.Tests

# Step 2: Add all projects to solution
dotnet sln add \
  Modules/${MODULE_NAME}/dotFitness.Modules.${MODULE_NAME}.Domain/dotFitness.Modules.${MODULE_NAME}.Domain.csproj \
  Modules/${MODULE_NAME}/dotFitness.Modules.${MODULE_NAME}.Application/dotFitness.Modules.${MODULE_NAME}.Application.csproj \
  Modules/${MODULE_NAME}/dotFitness.Modules.${MODULE_NAME}.Infrastructure/dotFitness.Modules.${MODULE_NAME}.Infrastructure.csproj \
  Modules/${MODULE_NAME}/dotFitness.Modules.${MODULE_NAME}.Tests/dotFitness.Modules.${MODULE_NAME}.Tests.csproj

# Step 3: Configure all project references
dotnet add dotFitness.Api/dotFitness.Api.csproj reference Modules/${MODULE_NAME}/dotFitness.Modules.${MODULE_NAME}.Application/dotFitness.Modules.${MODULE_NAME}.Application.csproj && \
dotnet add Modules/${MODULE_NAME}/dotFitness.Modules.${MODULE_NAME}.Application/dotFitness.Modules.${MODULE_NAME}.Application.csproj reference dotFitness.SharedKernel/dotFitness.SharedKernel.csproj && \
dotnet add Modules/${MODULE_NAME}/dotFitness.Modules.${MODULE_NAME}.Domain/dotFitness.Modules.${MODULE_NAME}.Domain.csproj reference dotFitness.SharedKernel/dotFitness.SharedKernel.csproj && \
dotnet add Modules/${MODULE_NAME}/dotFitness.Modules.${MODULE_NAME}.Infrastructure/dotFitness.Modules.${MODULE_NAME}.Infrastructure.csproj reference Modules/${MODULE_NAME}/dotFitness.Modules.${MODULE_NAME}.Domain/dotFitness.Modules.${MODULE_NAME}.Domain.csproj && \
dotnet add Modules/${MODULE_NAME}/dotFitness.Modules.${MODULE_NAME}.Infrastructure/dotFitness.Modules.${MODULE_NAME}.Infrastructure.csproj reference Modules/${MODULE_NAME}/dotFitness.Modules.${MODULE_NAME}.Application/dotFitness.Modules.${MODULE_NAME}.Application.csproj && \
dotnet add Modules/${MODULE_NAME}/dotFitness.Modules.${MODULE_NAME}.Infrastructure/dotFitness.Modules.${MODULE_NAME}.Infrastructure.csproj reference dotFitness.SharedKernel/dotFitness.SharedKernel.csproj && \
dotnet add Modules/${MODULE_NAME}/dotFitness.Modules.${MODULE_NAME}.Tests/dotFitness.Modules.${MODULE_NAME}.Tests.csproj reference Modules/${MODULE_NAME}/dotFitness.Modules.${MODULE_NAME}.Application/dotFitness.Modules.${MODULE_NAME}.Application.csproj && \
dotnet add Modules/${MODULE_NAME}/dotFitness.Modules.${MODULE_NAME}.Tests/dotFitness.Modules.${MODULE_NAME}.Tests.csproj reference Modules/${MODULE_NAME}/dotFitness.Modules.${MODULE_NAME}.Domain/dotFitness.Modules.${MODULE_NAME}.Domain.csproj && \
dotnet add Modules/${MODULE_NAME}/dotFitness.Modules.${MODULE_NAME}.Tests/dotFitness.Modules.${MODULE_NAME}.Tests.csproj reference Modules/${MODULE_NAME}/dotFitness.Modules.${MODULE_NAME}.Infrastructure/dotFitness.Modules.${MODULE_NAME}.Infrastructure.csproj && \
dotnet add Modules/${MODULE_NAME}/dotFitness.Modules.${MODULE_NAME}.Tests/dotFitness.Modules.${MODULE_NAME}.Tests.csproj reference dotFitness.SharedKernel/dotFitness.SharedKernel.csproj

echo "Module ${MODULE_NAME} projects created and configured successfully!"
```

### Step-by-Step Instructions (Alternative)

If you prefer to run commands step by step:

### Step 1: Create Module Projects

Create the following projects for the new module (replace `[ModuleName]` with the actual module name):

```bash
# Create module projects in organized folder structure
dotnet new classlib -n dotFitness.Modules.[ModuleName].Domain -o Modules/[ModuleName]/dotFitness.Modules.[ModuleName].Domain && \
dotnet new classlib -n dotFitness.Modules.[ModuleName].Application -o Modules/[ModuleName]/dotFitness.Modules.[ModuleName].Application && \
dotnet new classlib -n dotFitness.Modules.[ModuleName].Infrastructure -o Modules/[ModuleName]/dotFitness.Modules.[ModuleName].Infrastructure && \
dotnet new xunit -n dotFitness.Modules.[ModuleName].Tests -o Modules/[ModuleName]/dotFitness.Modules.[ModuleName].Tests

# Add all projects to solution
dotnet sln add \
  Modules/[ModuleName]/dotFitness.Modules.[ModuleName].Domain/dotFitness.Modules.[ModuleName].Domain.csproj \
  Modules/[ModuleName]/dotFitness.Modules.[ModuleName].Application/dotFitness.Modules.[ModuleName].Application.csproj \
  Modules/[ModuleName]/dotFitness.Modules.[ModuleName].Infrastructure/dotFitness.Modules.[ModuleName].Infrastructure.csproj \
  Modules/[ModuleName]/dotFitness.Modules.[ModuleName].Tests/dotFitness.Modules.[ModuleName].Tests.csproj
```

### Step 2: Configure Project References

Set up proper Clean Architecture dependencies:

```bash
# API references Application layer only
dotnet add dotFitness.Api/dotFitness.Api.csproj reference Modules/[ModuleName]/dotFitness.Modules.[ModuleName].Application/dotFitness.Modules.[ModuleName].Application.csproj

# Application layer references
dotnet add Modules/[ModuleName]/dotFitness.Modules.[ModuleName].Application/dotFitness.Modules.[ModuleName].Application.csproj reference dotFitness.SharedKernel/dotFitness.SharedKernel.csproj

# Domain layer references  
dotnet add Modules/[ModuleName]/dotFitness.Modules.[ModuleName].Domain/dotFitness.Modules.[ModuleName].Domain.csproj reference dotFitness.SharedKernel/dotFitness.SharedKernel.csproj

# Infrastructure layer references
dotnet add Modules/[ModuleName]/dotFitness.Modules.[ModuleName].Infrastructure/dotFitness.Modules.[ModuleName].Infrastructure.csproj reference Modules/[ModuleName]/dotFitness.Modules.[ModuleName].Domain/dotFitness.Modules.[ModuleName].Domain.csproj && \
dotnet add Modules/[ModuleName]/dotFitness.Modules.[ModuleName].Infrastructure/dotFitness.Modules.[ModuleName].Infrastructure.csproj reference Modules/[ModuleName]/dotFitness.Modules.[ModuleName].Application/dotFitness.Modules.[ModuleName].Application.csproj && \
dotnet add Modules/[ModuleName]/dotFitness.Modules.[ModuleName].Infrastructure/dotFitness.Modules.[ModuleName].Infrastructure.csproj reference dotFitness.SharedKernel/dotFitness.SharedKernel.csproj

# Tests references
dotnet add Modules/[ModuleName]/dotFitness.Modules.[ModuleName].Tests/dotFitness.Modules.[ModuleName].Tests.csproj reference Modules/[ModuleName]/dotFitness.Modules.[ModuleName].Application/dotFitness.Modules.[ModuleName].Application.csproj && \
dotnet add Modules/[ModuleName]/dotFitness.Modules.[ModuleName].Tests/dotFitness.Modules.[ModuleName].Tests.csproj reference Modules/[ModuleName]/dotFitness.Modules.[ModuleName].Domain/dotFitness.Modules.[ModuleName].Domain.csproj && \
dotnet add Modules/[ModuleName]/dotFitness.Modules.[ModuleName].Tests/dotFitness.Modules.[ModuleName].Tests.csproj reference Modules/[ModuleName]/dotFitness.Modules.[ModuleName].Infrastructure/dotFitness.Modules.[ModuleName].Infrastructure.csproj && \
dotnet add Modules/[ModuleName]/dotFitness.Modules.[ModuleName].Tests/dotFitness.Modules.[ModuleName].Tests.csproj reference dotFitness.SharedKernel/dotFitness.SharedKernel.csproj
```

### Step 3: Domain Layer Implementation

Create the following structure in `Modules/[ModuleName]/dotFitness.Modules.[ModuleName].Domain`:

```
Modules/[ModuleName]/dotFitness.Modules.[ModuleName].Domain/
├── Entities/
│   └── [MainEntity].cs          # Primary domain entity
├── Repositories/
│   └── I[MainEntity]Repository.cs  # Repository interface
└── Events/
    └── [Entity]CreatedEvent.cs  # Domain events
```

#### Domain Entity Requirements:
- Implement `IEntity` interface from SharedKernel
- Use MongoDB.Bson attributes for serialization
- Set `CreatedAt` and `UpdatedAt` to `DateTime.UtcNow` in constructor
- Include auto-generated `Id` property
- Implement business logic methods within the entity

#### Repository Interface Requirements:
- Return `Result<T>` or `Result` from SharedKernel
- Include `CancellationToken` parameters with default values
- Follow async/await patterns
- Include CRUD operations: `CreateAsync`, `GetByIdAsync`, `UpdateAsync`, `DeleteAsync`

### Step 4: Application Layer Implementation

Create the following structure in `Modules/[ModuleName]/dotFitness.Modules.[ModuleName].Application`:

```
Modules/[ModuleName]/dotFitness.Modules.[ModuleName].Application/
├── Commands/
│   ├── Create[Entity]Command.cs
│   ├── Update[Entity]Command.cs
│   └── Delete[Entity]Command.cs
├── Queries/
│   ├── Get[Entity]ByIdQuery.cs
│   └── GetAll[Entities]Query.cs
├── DTOs/
│   ├── [Entity]Dto.cs
│   └── [Entity]Request.cs
├── Mappers/
│   └── [Entity]Mapper.cs        # Riok.Mapperly mapper class
├── Validators/
│   ├── Create[Entity]CommandValidator.cs
│   └── Update[Entity]CommandValidator.cs
└── Configuration/
    └── [ModuleName]ModuleRegistration.cs
```

#### CQRS Requirements:
- Commands/Queries implement `IRequest<Result<T>>` from MediatR
- Use record types for immutable Commands/Queries
- Include comprehensive validation attributes
- DTOs should be simple data transfer objects without business logic

#### Mapper Requirements:
- Use Riok.Mapperly for compile-time mapping
- Create mapper class with `[Mapper]` attribute and `partial` keyword
- Include bidirectional mapping methods
- Handle null values appropriately
- Make the class static if no instance state is needed

#### Validation Requirements:
- Use FluentValidation for all Commands
- Include business rule validation
- Validate required fields, formats, and ranges
- Return descriptive error messages

### Step 5: Infrastructure Layer Implementation

Create the following structure in `Modules/[ModuleName]/dotFitness.Modules.[ModuleName].Infrastructure`:

```
Modules/[ModuleName]/dotFitness.Modules.[ModuleName].Infrastructure/
├── Repositories/
│   └── [Entity]Repository.cs    # MongoDB implementation
├── Handlers/
│   ├── Create[Entity]CommandHandler.cs
│   ├── Update[Entity]CommandHandler.cs
│   ├── Delete[Entity]CommandHandler.cs
│   ├── Get[Entity]ByIdQueryHandler.cs
│   └── GetAll[Entities]QueryHandler.cs
├── Settings/
│   └── [ModuleName]Settings.cs
└── Configuration/
    └── [ModuleName]InfrastructureModule.cs
```

#### Repository Implementation Requirements:
- Implement domain repository interface
- Use MongoDB.Driver for database operations
- Include proper error handling with logging
- Use transactions for complex operations
- Return Result<T> pattern for all operations

#### Handler Requirements:
- Implement `IRequestHandler<TRequest, TResponse>` from MediatR
- Include comprehensive logging
- Use dependency injection for repositories and services
- Handle exceptions gracefully with Result pattern
- Include performance monitoring

#### Module Registration Requirements:
- Create extension method `Add[ModuleName]Module(this IServiceCollection services)`
- Register all dependencies (repositories, handlers, validators, mappers)
- Configure MongoDB collections with proper indexing
- Set up any module-specific settings

### Step 6: API Integration

Create controller in `dotFitness.Api/Controllers/[ModuleName]Controller.cs`:

#### Controller Requirements:
- Use `[ApiController]` and `[Route("api/v1/[controller]")]` attributes
- Include `[Authorize]` for protected endpoints
- Use MediatR for command/query dispatching
- Return appropriate HTTP status codes
- Include comprehensive API documentation with XML comments
- Handle Result<T> responses properly

### Step 7: Update Module Registry

Add the new module to `dotFitness.Api/Infrastructure/ModuleRegistry.cs`:

```csharp
private static readonly string[] ModuleNames = 
{
    "Users",
    "[ModuleName]"  // Add your new module here
};
```

### Step 8: Required NuGet Packages

#### Domain Project:
```xml
<PackageReference Include="MongoDB.Bson" Version="3.4.0" />
```

#### Application Project:
```xml
<PackageReference Include="MediatR.Contracts" Version="2.0.1" />
<PackageReference Include="FluentValidation" Version="12.0.0" />
<PackageReference Include="Riok.Mapperly" Version="4.1.0" />
```

#### Infrastructure Project:
```xml
<PackageReference Include="MongoDB.Driver" Version="3.4.0" />
<PackageReference Include="MediatR" Version="12.5.0" />
<PackageReference Include="FluentValidation" Version="12.0.0" />
<PackageReference Include="Microsoft.Extensions.Logging.Abstractions" Version="9.0.5" />
<PackageReference Include="Microsoft.Extensions.Options" Version="9.0.5" />
<PackageReference Include="Microsoft.Extensions.DependencyInjection.Abstractions" Version="9.0.5" />
```

## Critical Implementation Rules

1. **UTC Timestamps**: Always use `DateTime.UtcNow` for MongoDB operations
2. **Result Pattern**: All operations must return `Result<T>` or `Result`
3. **Async/Await**: All database operations must be asynchronous
4. **Clean Architecture**: Maintain proper dependency directions
5. **CQRS**: Separate read and write operations
6. **Error Handling**: Use Result pattern instead of exceptions for business errors
7. **Logging**: Include comprehensive logging in all handlers
8. **Testing**: Each layer must be thoroughly tested

## Validation Checklist

After creating the module, verify:

- [ ] All projects compile successfully
- [ ] Project references follow Clean Architecture principles
- [ ] Domain entities implement IEntity interface
- [ ] All operations return Result<T> pattern
- [ ] MongoDB collections are properly configured
- [ ] Module is registered in ModuleRegistry
- [ ] API endpoints are accessible and documented
- [ ] All dependencies are properly injected
- [ ] UTC timestamps are used consistently
- [ ] Error handling follows established patterns

## Example Usage

To create an "Exercises" module, replace all instances of `[ModuleName]` with "Exercises" and `[Entity]` with "Exercise" in the above instructions.

The module should handle:
- Exercise creation, editing, and deletion
- Exercise categorization by muscle groups and equipment
- Custom user exercises vs. global exercises
- Exercise search and filtering capabilities

This prompt ensures consistent, high-quality module implementation following the established dotFitness architecture patterns.
