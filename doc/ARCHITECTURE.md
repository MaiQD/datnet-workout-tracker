# dotFitness Architecture: Modular Monolith with DDD

> Based on industry best practices from [modular-monolith-with-ddd](https://github.com/MaiQD/modular-monolith-with-ddd/blob/master/README.md)

## 🏗️ Architectural Overview

dotFitness implements a **Modular Monolith** architecture that combines the simplicity of a monolith with the scalability and maintainability of microservices. This pattern is inspired by Domain-Driven Design (DDD) principles and provides a clear path for future evolution.

## 🎯 Core Principles

### 1. **Modular Independence**
- Each module is a self-contained vertical slice
- Modules communicate through well-defined interfaces
- No direct dependencies between modules
- Shared concerns are isolated in the SharedKernel

### 2. **Domain-Driven Design**
- Business logic is organized around domain concepts
- Clear boundaries between different business capabilities
- Ubiquitous language reflected in code structure
- Domain entities encapsulate business rules

### 3. **Clean Architecture**
- Dependency inversion principle
- Business logic independent of infrastructure
- Testable and maintainable code structure
- Clear separation of concerns

#### 3.1 Single Responsibility Principle (SRP)
- Each class has exactly one reason to change.
- Split responsibilities into focused classes:
  - Command/Query objects: data-only intent
  - Handlers: application orchestration per request
  - Domain entities: business invariants and behavior
  - Repositories: persistence concerns behind interfaces
  - Mappers: DTO ↔ domain mapping (Mapperly)
  - Validators: FluentValidation rules per command/query

#### 3.2 Layered Concerns
- Domain: Entities, Value Objects, Domain Events, Repository Interfaces
- Application: Commands/Queries, Handlers, DTOs, Mappers, Validators
- Infrastructure: Repository implementations, external services, handlers’ wiring
- API: Controllers, filters/middleware, DI setup

#### 3.3 Class Separation Guidelines
- Keep handlers small; extract complex policies/services into separate application services when logic grows
- Avoid God objects: prefer cohesive domain methods over sprawling services
- No infrastructure dependencies in Domain/Application (use interfaces)
- Keep constructors minimal; prefer injecting abstractions only

#### 3.4 Module Boundaries
- No cross-module domain references; communicate via commands/queries or events
- SharedKernel only for truly cross-cutting abstractions (no domain rules)

#### 3.5 Testing Implications
- Domain is pure and easily unit-testable
- Application handlers tested with mocked repositories/services
- Infrastructure verified via integration tests

## 📁 Project Structure

```
dotFitness.WorkoutTracker/
├── dotFitness.Api/                    # 🚀 Application Entry Point
│   ├── Controllers/                   # REST API endpoints
│   ├── Infrastructure/               # Cross-cutting concerns
│   │   ├── Metrics/                  # Performance monitoring
│   │   ├── ModuleRegistry.cs         # Dynamic module discovery
│   │   ├── ModuleHealthChecks.cs     # Health monitoring
│   │   └── ModuleConfigurationValidator.cs
│   └── Program.cs                    # Application bootstrap
│
├── dotFitness.SharedKernel/          # 🔗 Shared Components
│   ├── Results/                      # Result pattern implementation
│   ├── Outbox/                       # Outbox pattern for events
│   ├── Interfaces/                   # Common interfaces
│   └── Utilities/                    # Shared utilities
│
└── Modules/                          # 📦 Business Modules
    ├── Users/                        # 👤 User Management Module
    │   ├── dotFitness.Modules.Users.Domain/
    │   ├── dotFitness.Modules.Users.Application/
    │   ├── dotFitness.Modules.Users.Infrastructure/
    │   └── dotFitness.Modules.Users.Tests/
    │
    ├── Exercises/                    # 💪 Exercise Management Module
    │   ├── dotFitness.Modules.Exercises.Domain/
    │   ├── dotFitness.Modules.Exercises.Application/
    │   ├── dotFitness.Modules.Exercises.Infrastructure/
    │   └── dotFitness.Modules.Exercises.Tests/
    │
    ├── Routines/                     # 📋 Workout Routine Module (Planned)
    │   └── [Future implementation]
    │
    └── WorkoutLogs/                  # 📊 Workout Tracking Module (Planned)
        └── [Future implementation]
```

## 🧩 Module Structure

Each module follows a consistent internal structure:

### Domain Layer (`*.Domain`)
```csharp
// Core business entities
public class User : IEntity
{
    public string Id { get; set; }
    public string GoogleId { get; set; }
    public string Email { get; set; }
    // Business logic and validation
}

// Domain events
public class UserCreatedEvent : INotification
{
    public string UserId { get; set; }
    public string Email { get; set; }
}

// Repository interfaces
public interface IUserRepository
{
    Task<User?> GetByIdAsync(string id);
    Task<User?> GetByGoogleIdAsync(string googleId);
    Task CreateAsync(User user);
    Task UpdateAsync(User user);
}
```

### Application Layer (`*.Application`)
```csharp
// Commands (write operations)
public class CreateUserCommand : IRequest<Result<UserDto>>
{
    public string GoogleId { get; set; }
    public string Email { get; set; }
    public string DisplayName { get; set; }
}

// Queries (read operations)
public class GetUserByIdQuery : IRequest<Result<UserDto>>
{
    public string UserId { get; set; }
}

// DTOs (Data Transfer Objects)
public class UserDto
{
    public string Id { get; set; }
    public string Email { get; set; }
    public string DisplayName { get; set; }
}

// Mappers (static for performance)
public static class UserMapper
{
    public static UserDto ToDto(User user) => new()
    {
        Id = user.Id,
        Email = user.Email,
        DisplayName = user.DisplayName
    };
}
```

### Infrastructure Layer (`*.Infrastructure`)
```csharp
// Command/Query handlers
public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<UserDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMediator _mediator;

    public async Task<Result<UserDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        // Business logic implementation
        var user = new User { /* ... */ };
        await _userRepository.CreateAsync(user);
        
        // Publish domain event
        await _mediator.Publish(new UserCreatedEvent { UserId = user.Id });
        
        return Result<UserDto>.Success(UserMapper.ToDto(user));
    }
}

// Repository implementations
public class UserRepository : IUserRepository
{
    private readonly IMongoCollection<User> _collection;

    public async Task<User?> GetByIdAsync(string id)
    {
        return await _collection.Find(u => u.Id == id).FirstOrDefaultAsync();
    }
    // Other repository methods...
}
```

## 🔄 Module Registration System

### Automatic Discovery
The `ModuleRegistry` automatically discovers and registers all modules:

```csharp
// Zero-configuration module registration
public static readonly string[] ModuleNames = 
{
    "Users",
    "Exercises", 
    "Routines",
    "WorkoutLogs"
};

// Automatic registration in Program.cs
ModuleRegistry.RegisterAllModules(services, configuration);
ModuleRegistry.RegisterModuleAssemblies(mediatRConfig);
```

### Dynamic Assembly Loading
- **Application Assemblies**: Loaded for CQRS handlers and DTOs
- **Infrastructure Assemblies**: Loaded for repository implementations
- **Health Checks**: Automatic health monitoring for each module
- **Metrics**: Performance tracking and monitoring

## 🎨 Design Patterns

### 1. **CQRS (Command Query Responsibility Segregation)**
- **Commands**: Intent to change state (`CreateUserCommand`)
- **Queries**: Intent to retrieve data (`GetUserByIdQuery`)
- **Handlers**: Process commands/queries (`CreateUserCommandHandler`)
- **Benefits**: Clear separation, optimized read/write paths

### 2. **Result Pattern**
```csharp
// Explicit success/failure handling
public async Task<Result<UserDto>> Handle(CreateUserCommand request)
{
    if (await _userRepository.ExistsAsync(request.Email))
        return Result<UserDto>.Failure("User already exists");
    
    var user = new User { /* ... */ };
    await _userRepository.CreateAsync(user);
    return Result<UserDto>.Success(UserMapper.ToDto(user));
}
```

### 3. **Outbox Pattern**
- Ensures reliable event publishing
- Stores events in database transaction
- Background processing for event delivery
- Prevents data inconsistency

### 4. **Static Mappers**
```csharp
// Compile-time mapping for zero runtime overhead
public static class UserMapper
{
    public static UserDto ToDto(User user) => new()
    {
        Id = user.Id,
        Email = user.Email,
        DisplayName = user.DisplayName
    };
}
```

## 🔗 Module Communication

### 1. **Synchronous Communication**
- Direct method calls within the same module
- Interface-based communication between layers
- No direct dependencies between different modules

### 2. **Asynchronous Communication (Event-Driven)**
- Domain events for cross-module communication.
- **Outbox pattern** for reliable event publishing by the producing module.
- **Inbox pattern** for idempotent processing by consuming modules:
  - Each consumer records processed `eventId`s in its `inboxMessages` to avoid duplicates.
  - Handlers are idempotent and retriable.
- Event handlers for side effects and projections (read models).

### 3. **Shared Kernel**
- Common interfaces (`IEntity`)
- Result pattern implementation
- Shared utilities and constants
- No business logic in shared components

## 🧪 Testing Strategy

### Unit Testing
```csharp
[Fact]
public async Task Handle_ValidCommand_ReturnsSuccess()
{
    // Arrange
    var command = new CreateUserCommand { /* ... */ };
    var handler = new CreateUserCommandHandler(_mockRepo, _mockMediator);

    // Act
    var result = await handler.Handle(command, CancellationToken.None);

    // Assert
    result.IsSuccess.Should().BeTrue();
    result.Value.Should().NotBeNull();
}
```

### Integration Testing
- MongoDB integration with Testcontainers
- Full module testing with real dependencies
- API endpoint testing with HTTP client

### Test Structure
```
dotFitness.Modules.Users.Tests/
├── Domain/           # Entity and business logic tests
├── Application/      # Command/Query handler tests
├── Infrastructure/   # Repository and external service tests
└── MongoDB/          # Database integration tests
```

## 📊 Monitoring & Observability

### Health Checks
- Module-specific health endpoints
- Database connectivity monitoring
- External service health checks

### Metrics
- Module registration performance
- Assembly loading times
- MediatR handler registration
- MongoDB index configuration

### Logging
- Structured logging with Serilog
- Module-specific log contexts
- Performance and error tracking

## 🚀 Deployment & Scalability

### Current State
- Single deployment unit (monolith)
- Shared database (MongoDB)
- Horizontal scaling through load balancing

### Future Evolution Path
1. **Module Extraction**: Extract modules to separate services
2. **Database Separation**: Split databases per module
3. **Event-Driven Architecture**: Implement message brokers
4. **API Gateway**: Centralized routing and authentication

## 🎯 Benefits of This Architecture

### Development Benefits
- **Clear Boundaries**: Each module has well-defined responsibilities
- **Team Autonomy**: Teams can work on different modules independently
- **Testability**: Isolated testing of business logic
- **Maintainability**: Changes are localized to specific modules

### Operational Benefits
- **Simple Deployment**: Single application to deploy and monitor
- **Shared Infrastructure**: Common database, logging, and monitoring
- **Cost Effective**: Lower operational overhead than microservices
- **Performance**: No network latency between modules

### Business Benefits
- **Rapid Development**: New features can be added quickly
- **Risk Mitigation**: Changes in one module don't affect others
- **Scalability**: Can scale horizontally or extract modules as needed
- **Technology Flexibility**: Different modules can use different technologies

## 📚 References

- [Modular Monolith with DDD](https://github.com/MaiQD/modular-monolith-with-ddd/blob/master/README.md) - Reference implementation
- [Domain-Driven Design](https://martinfowler.com/bliki/DomainDrivenDesign.html) - Core concepts
- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html) - Architectural principles
- [CQRS Pattern](https://martinfowler.com/bliki/CQRS.html) - Command Query Responsibility Segregation 