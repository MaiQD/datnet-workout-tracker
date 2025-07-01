# dotFitness Development: Step-by-Step Guide

> Based on modular monolith best practices from [modular-monolith-with-ddd](https://github.com/MaiQD/modular-monolith-with-ddd/blob/master/README.md)

This guide provides a comprehensive step-by-step approach to developing features in the dotFitness modular monolith architecture.

---

### Phase 1: Foundational Setup & Project Structure

1. **Create Root Solution Folder:** Create a directory named `dotFitness.WorkoutTracker`.
2. **Navigate to Root Folder:** Change the current working directory to `dotFitness.WorkoutTracker`.
3. **Create Solution File:** Execute `dotnet new sln -n dotFitness.WorkoutTracker`.
4. **Create Backend API Project:** Execute `dotnet new webapi -n dotFitness.Api`.
5. **Create Shared Kernel Project:** Execute `dotnet new classlib -n dotFitness.SharedKernel`.
6. **Create Users Module - Application Layer Project:** Execute `dotnet new classlib -n dotFitness.Modules.Users.Application`.
7. **Create Users Module - Domain Layer Project:** Execute `dotnet new classlib -n dotFitness.Modules.Users.Domain`.
8. **Create Users Module - Infrastructure Layer Project:** Execute `dotnet new classlib -n dotFitness.Modules.Users.Infrastructure`.
9. **Create Users Module - Tests Project:** Execute `dotnet new xunit -n dotFitness.Modules.Users.Tests`.
10. **Add All Created Projects to Solution:** Execute `dotnet sln add **/*.csproj`.
11. **Add Project References - dotFitness.Api:**
    - `dotnet add dotFitness.Api/dotFitness.Api.csproj reference dotFitness.SharedKernel/dotFitness.SharedKernel.csproj`
    - `dotnet add dotFitness.Api/dotFitness.Api.csproj reference dotFitness.Modules.Users.Application/dotFitness.Modules.Users.Application.csproj`
    - *(Repeat this for all other `.Application` modules as they are created later: Exercises, Routines, WorkoutLogs)*
    - **Note:** Following Clean Architecture principles, the API project should only reference Application layers and SharedKernel. Infrastructure dependencies are managed through the generic module registration system.
12. **Add Project References - dotFitness.SharedKernel:** (No outgoing references from SharedKernel)
13. **Add Project References - dotFitness.Modules.Users.Application:**
    - `dotnet add dotFitness.Modules.Users.Application/dotFitness.Modules.Users.Application.csproj reference dotFitness.SharedKernel/dotFitness.SharedKernel.csproj`
14. **Add Project References - dotFitness.Modules.Users.Domain:**
    - `dotnet add dotFitness.Modules.Users.Domain/dotFitness.Modules.Users.Domain.csproj reference dotFitness.SharedKernel/dotFitness.SharedKernel.csproj`
15. **Add Project References - dotFitness.Modules.Users.Infrastructure:**
    - `dotnet add dotFitness.Modules.Users.Infrastructure/dotFitness.Modules.Users.Infrastructure.csproj reference dotFitness.Modules.Users.Domain/dotFitness.Modules.Users.Domain.csproj`
    - `dotnet add dotFitness.Modules.Users.Infrastructure/dotFitness.Modules.Users.Infrastructure.csproj reference dotFitness.Modules.Users.Application/dotFitness.Modules.Users.Application.csproj`
    - `dotnet add dotFitness.Modules.Users.Infrastructure/dotFitness.Modules.Users.Infrastructure.csproj reference dotFitness.SharedKernel/dotFitness.SharedKernel.csproj`
16. **Add Project References - dotFitness.Modules.Users.Tests:**
    - `dotnet add dotFitness.Modules.Users.Tests/dotFitness.Modules.Users.Tests.csproj reference dotFitness.Modules.Users.Application/dotFitness.Modules.Users.Application.csproj`
    - `dotnet add dotFitness.Modules.Users.Tests/dotFitness.Modules.Users.Tests.csproj reference dotFitness.Modules.Users.Domain/dotFitness.Modules.Users.Domain.csproj`
    - `dotnet add dotFitness.Modules.Users.Tests/dotFitness.Modules.Users.Tests.csproj reference dotFitness.Modules.Users.Infrastructure/dotFitness.Modules.Users.Infrastructure.csproj`
    - `dotnet add dotFitness.Modules.Users.Tests/dotFitness.Modules.Users.Tests.csproj reference dotFitness.SharedKernel/dotFitness.SharedKernel.csproj`
17. **Implement `dotFitness.SharedKernel` - `Results` Folder & `Result.cs`:**
    - Create directory `dotFitness.SharedKernel/Results`.
    - Create file `dotFitness.SharedKernel/Results/Result.cs` and add `Result` and `Result<TValue>` classes (as per design document).
    - Add NuGet package `System.Diagnostics.CodeAnalysis` (for `NotNullWhen` attribute).
18. **Implement `dotFitness.SharedKernel` - `Interfaces` Folder & `IEntity.cs`:**
    - Create directory `dotFitness.SharedKernel/Interfaces`.
    - Create file `dotFitness.SharedKernel/Interfaces/IEntity.cs` and add the `IEntity` interface.
19. **Implement `dotFitness.SharedKernel` - `Outbox` Folder & `OutboxMessage.cs`:**
    - Create directory `dotFitness.SharedKernel/Outbox`.
    - Create file `dotFitness.SharedKernel/Outbox/OutboxMessage.cs` and add the `OutboxMessage` class.
    - Add NuGet package `MongoDB.Bson`.
    - Add NuGet package `System.Text.Json`.
19a. **Implement `dotFitness.SharedKernel` - `Utilities` Folder & `UnitConverter.cs`:**
    - Create directory `dotFitness.SharedKernel/Utilities`.
    - Create file `dotFitness.SharedKernel/Utilities/UnitConverter.cs` with centralized unit conversion utilities.
    - Add NuGet package `UnitsNet` for unit conversion capabilities.
    - This enables cross-module unit handling and BMI calculations.
20. **Setup Local MongoDB with Docker Compose:**
    - Create file `dotFitness.WorkoutTracker/docker-compose.yml` with the MongoDB service definition.
    - Execute `docker compose up -d` in the `dotFitness.WorkoutTracker` directory to start the MongoDB container.
21. **Configure `dotFitness.Api` - Add NuGet Packages:**
    - `dotnet add dotFitness.Api/dotFitness.Api.csproj package MongoDB.Driver`
    - `dotnet add dotFitness.Api/dotFitness.Api.csproj package Serilog.AspNetCore`
    - `dotnet add dotFitness.Api/dotFitness.Api.csproj package Serilog.Sinks.Console`
    - `dotnet add dotFitness.Api/dotFitness.Api.csproj package MediatR`
    - `dotnet add dotFitness.Api/dotFitness.Api.csproj package Microsoft.AspNetCore.Authentication.JwtBearer`
    - `dotnet add dotFitness.Api/dotFitness.Api.csproj package Microsoft.AspNetCore.Mvc.Versioning`
    - `dotnet add dotFitness.Api/dotFitness.Api.csproj package FluentValidation.AspNetCore`
    - **Note:** MediatR and authentication configurations are now handled through the generic module registration system.
22. **Configure `dotFitness.Api` - Update `appsettings.json`:**
    - Modify `dotFitness.Api/appsettings.json` and `dotFitness.Api/appsettings.Development.json` to include:
        - `ConnectionStrings:MongoDB` (e.g., `"mongodb://admin:password@localhost:27017/dotFitnessDb?authSource=admin"`).
        - `AdminSettings:AdminEmails` array (e.g., `["your.admin.email@gmail.com"]`).
23. **Configure `dotFitness.Api` - Update `Program.cs`:**
    - Set up Serilog logging.
    - Register `IMongoClient` and `IMongoDatabase` as singletons.
    - Register all `IMongoCollection<T>` (for shared entities like `OutboxMessage`) as singletons.
    - Implement `ConfigureMongoDbIndexes` function for shared collections and call it on startup.
    - Add Swagger/OpenAPI services with JWT Bearer authentication configuration.
    - Add CORS services.
    - **Implement Generic Module Registration System:**
        - Create `dotFitness.Api/Infrastructure/ModuleRegistry.cs` with automatic module discovery.
        - Use `ModuleRegistry.RegisterAllModules()` to automatically discover and register all modules.
        - Use `ModuleRegistry.RegisterModuleAssemblies()` to register MediatR from discovered assemblies.
        - Each module handles its own JWT authentication, MongoDB indexes, and dependency injection configuration.
    - Configure centralized error handling middleware.
    - Configure API Versioning services.
    - **Note:** Module-specific configurations (JWT, MongoDB collections, MediatR handlers) are now automatically handled by each module's registration system.

### Phase 2: Core User & Authentication Implementation ✅ COMPLETED

**Status:** Phase 2 has been fully implemented with Clean Architecture refactoring and generic module registration system.

1. **✅ Implement Users Module - Domain Layer (`dotFitness.Modules.Users.Domain`):**
    - ✅ Created `Entities` directory with `User.cs` and `UserMetric.cs` entities
    - ✅ Applied MongoDB.Bson.Serialization.Attributes correctly
    - ✅ Created `Repositories` directory with `IUserRepository.cs` and `IUserMetricsRepository.cs`
    - ✅ Created `Events` directory with `UserCreatedEvent.cs`

2. **✅ Implement Users Module - Infrastructure Layer (`dotFitness.Modules.Users.Infrastructure`):**
    - ✅ Created MongoDB repositories implementing domain interfaces
    - ✅ Added comprehensive NuGet packages: MediatR, FluentValidation, Microsoft.Extensions.Options, System.IdentityModel.Tokens.Jwt, Google.Apis.Auth
    - ✅ Implemented complete CQRS handlers:
        - `LoginWithGoogleCommandHandler` - Google OAuth, JWT generation, admin role assignment
        - `UpdateUserProfileCommandHandler` - Profile updates with enum handling
        - `AddUserMetricCommandHandler` - Body metrics with BMI calculation using user's unit preference
        - `GetUserByIdQueryHandler`, `GetUserProfileQueryHandler`, `GetLatestUserMetricQueryHandler`, `GetUserMetricsQueryHandler`
    - ✅ Created modular configuration system with `UsersInfrastructureModule.AddUsersModule()`
    - ✅ Separated settings classes into `/Settings/` directory

3. **✅ Implement Users Module - Application Layer (`dotFitness.Modules.Users.Application`):**
    - ✅ Created comprehensive Commands and Queries with proper CQRS implementation
    - ✅ Implemented DTOs: `LoginResponseDto`, `UserDto`, `UserMetricDto`
    - ✅ Created Riok.Mapperly-based mappers: `UserMapper` and `UserMetricMapper`
    - ✅ Implemented FluentValidation validators for all commands
    - ✅ Created `UsersModuleRegistration` for dependency injection

4. **✅ Integrate Users Module into `dotFitness.Api`:**
    - ✅ Created `AuthController.cs` with Google OAuth login endpoint
    - ✅ Created `UsersController.cs` with complete user profile and metrics management
    - ✅ Applied proper authorization with `[Authorize]` attributes
    - ✅ Implemented comprehensive error handling and validation

5. **✅ Users Module Testing Implementation:**
    - ✅ **Next Step:** Implement comprehensive unit tests for Users module using the testing framework above
    - ✅ **Next Step:** Create HTTP test files for Users API endpoints
    - ✅ **Next Step:** Verify all CRUD operations, authentication flows, and error scenarios
    - **Testing Focus Areas:**
      - User entity validation and business logic
      - UserMetric BMI calculation accuracy
      - Repository operations with MongoDB
      - Google OAuth authentication flow
      - JWT generation and validation
      - Admin role assignment logic
      - Profile update workflows
      - Metrics tracking and retrieval

**✅ Clean Architecture Refactoring Completed:**
- ✅ Updated API project to only reference Application layers + SharedKernel
- ✅ Moved JWT authentication configuration from API to Users module
- ✅ Created modular registration system maintaining proper boundaries
- ✅ Separated MongoDB index configuration by ownership

**✅ Generic Module Registration System Implemented:**
- ✅ Created `ModuleRegistry` with automatic module discovery
- ✅ Zero-configuration module addition (just add name to array)
- ✅ Automatic assembly discovery and MediatR handler registration
- ✅ Graceful handling of missing modules with proper logging
- ✅ Clean Architecture compliance maintained

1. **Implement Users Module - Domain Layer (`dotFitness.Modules.Users.Domain`):**
    - Create `Entities` directory.
    - Create `dotFitness.Modules.Users.Domain/Entities/User.cs` and define properties: `Id`, `GoogleId`, `Email`, `DisplayName`, `LoginMethod`, `Roles`, `Gender`, `DateOfBirth`, `UnitPreference`, `CreatedAt`, `UpdatedAt`. Apply `MongoDB.Bson.Serialization.Attributes` correctly.
    - Create `dotFitness.Modules.Users.Domain/Entities/UserMetric.cs` and define properties: `Id`, `UserId`, `Date`, `Weight`, `Height`, `Bmi`, `Notes`, `CreatedAt`, `UpdatedAt`. Apply `MongoDB.Bson.Serialization.Attributes`.
    - Create `Repositories` directory.
    - Create `dotFitness.Modules.Users.Domain/Repositories/IUserRepository.cs`.
    - Create `dotFitness.Modules.Users.Domain/Repositories/IUserMetricsRepository.cs`.
    - Create `Events` directory.
    - Create `dotFitness.Modules.Users.Domain/Events/UserCreatedEvent.cs`.
2. **Implement Users Module - Infrastructure Layer (`dotFitness.Modules.Users.Infrastructure`):**
    - Create `MongoDB` directory.
    - Create `dotFitness.Modules.Users.Infrastructure/MongoDB/UserRepository.cs` implementing `IUserRepository`.
    - Create `dotFitness.Modules.Users.Infrastructure/MongoDB/UserMetricsRepository.cs` implementing `IUserMetricsRepository`.
    - Add NuGet packages to `dotFitness.Modules.Users.Infrastructure.csproj`: `MediatR`, `FluentValidation`, `Microsoft.AspNetCore.Authentication.Google` (if using client-side auth flow directly in backend), `Microsoft.Extensions.Options`.
    - Create `Handlers` directory.
    - Create `dotFitness.Modules.Users.Infrastructure/Handlers/LoginWithGoogleCommandHandler.cs`:
        - Implements `IRequestHandler<LoginWithGoogleCommand, Result<LoginResponseDto>>`.
        - Uses `IUserRepository` to find/create `User`.
        - Checks `AdminEmails` config to assign “Admin” role if matched.
        - Generates JWT token for authentication.
        - **UM-005 Admin Access:** Implements logic for assigning the “Admin” role based on configured email whitelist.
    - Create `dotFitness.Modules.Users.Infrastructure/Handlers/UpdateUserProfileCommandHandler.cs`.
    - Create `dotFitness.Modules.Users.Infrastructure/Handlers/AddUserMetricCommandHandler.cs`:
        - Calculates BMI.
        - **UM-003 Body Metric Tracking:** Saves `UserMetric`.
        - **Outbox Pattern:** Publishes `UserMetricAddedEvent` (or `UserCreatedEvent` from original `CreateUserCommandHandler`) using MongoDB transactions.
    - Create `dotFitness.Modules.Users.Infrastructure/Handlers/GetUserByIdQueryHandler.cs`.
    - Create `dotFitness.Modules.Users.Infrastructure/Handlers/GetLatestUserMetricQueryHandler.cs`.
    - Create `dotFitness.Modules.Users.Infrastructure/Handlers/GetUserMetricsQueryHandler.cs`.
    - Create `dotFitness.Modules.Users.Infrastructure/Handlers/GetUserProfileQueryHandler.cs`.
3. **Implement Users Module - Application Layer (`dotFitness.Modules.Users.Application`):**
    - Create `Commands` directory.
    - Define `dotFitness.Modules.Users.Application/Commands/LoginWithGoogleCommand.cs`.
    - Define `dotFitness.Modules.Users.Application/Commands/UpdateUserProfileCommand.cs`.
    - Define `dotFitness.Modules.Users.Application/Commands/AddUserMetricCommand.cs`.
    - Create `Queries` directory.
    - Define `dotFitness.Modules.Users.Application/Queries/GetUserByIdQuery.cs`.
    - Define `dotFitness.Modules.Users.Application/Queries/GetLatestUserMetricQuery.cs`.
    - Define `dotFitness.Modules.Users.Application/Queries/GetUserMetricsQuery.cs`.
    - Define `dotFitness.Modules.Users.Application/Queries/GetUserProfileQuery.cs`.
    - Create `DTOs` directory.
    - Define `dotFitness.Modules.Users.Application/DTOs/LoginResponseDto.cs`.
    - Define `dotFitness.Modules.Users.Application/DTOs/UserDto.cs`.
    - Define `dotFitness.Modules.Users.Application/DTOs/UserProfileUpdateDto.cs`.
    - Define `dotFitness.Modules.Users.Application/DTOs/UserMetricDto.cs`.
    - Create `Mappers` directory.
    - Create `dotFitness.Modules.Users.Application/Mappers/IUserMapper.cs` (Mapperly interface for User and UserDto).
    - Create `dotFitness.Modules.Users.Application/Mappers/IUserMetricMapper.cs` (Mapperly interface for UserMetric and UserMetricDto).
    - Create `Validators` directory.
    - Create `dotFitness.Modules.Users.Application/Validators/LoginWithGoogleCommandValidator.cs`.
    - Create `dotFitness.Modules.Users.Application/Validators/AddUserMetricCommandValidator.cs`.
4. **Integrate Users Module into `dotFitness.Api`:**
    - Create `Controllers` directory.
    - Create `dotFitness.Api/Controllers/AuthController.cs`:
        - Implements `POST /api/auth/google-login` endpoint.
        - Sends `LoginWithGoogleCommand` via `IMediator`.
        - Returns `LoginResponseDto`.
    - Create `dotFitness.Api/Controllers/UsersController.cs`:
        - Implements `GET /api/users/profile` (for `GetUserProfileQuery`).
        - Implements `PUT /api/users/profile` (for `UpdateUserProfileCommand`).
        - Implements `POST /api/users/metrics` (for `AddUserMetricCommand`).
        - Implements `GET /api/users/metrics` (for `GetUserMetricsQuery`).
        - Implements `GET /api/users/metrics/latest` (for `GetLatestUserMetricQuery`).
        - Apply `[Authorize]` attribute to all actions.
        - **UM-002 Authenticated Access:** All core application features are now implicitly protected.

### Phase 3: Remaining Backend Module Development (Iterative)

## ✅ COMPLETED: Exercises Module Implementation

1. **✅ Created Exercises Module Projects:**
    - ✅ `dotFitness.Modules.Exercises.Application` - Application layer with commands, queries, DTOs
    - ✅ `dotFitness.Modules.Exercises.Domain` - Domain entities and repository interfaces
    - ✅ `dotFitness.Modules.Exercises.Infrastructure` - MongoDB repositories and MediatR handlers
    - 🔄 `dotFitness.Modules.Exercises.Tests` - **TODO: Comprehensive test coverage needed**
    - ✅ All projects added to solution with proper references

2. **✅ Implemented Exercises Module - Domain Layer:**
    - ✅ `Exercise.cs` entity with `IsGlobal`, `UserId`, muscle groups, equipment, difficulty levels
    - ✅ `MuscleGroup.cs` and `Equipment.cs` entities with global/user-specific support
    - ✅ Repository interfaces: `IExerciseRepository`, `IMuscleGroupRepository`, `IEquipmentRepository`
    - ✅ Domain events: `ExerciseCreatedEvent` for exercise creation notifications

3. **✅ Implemented Exercises Module - Infrastructure Layer:**
    - ✅ MongoDB repositories with CRUD operations, search, filtering, and user ownership validation
    - ✅ **Static Mapperly mappers** (performance optimized, no dependency injection)
    - ✅ MediatR Command Handlers: `CreateExerciseCommandHandler`, `UpdateExerciseCommandHandler`, `DeleteExerciseCommandHandler`
    - ✅ MediatR Query Handlers: `GetExerciseByIdQueryHandler`, `GetAllExercisesQueryHandler`, `GetAllMuscleGroupsQueryHandler`, `GetAllEquipmentQueryHandler`
    - ✅ MongoDB index configuration for optimal performance
    - ✅ Comprehensive error handling with Result pattern
    - ✅ **Clean Architecture compliance** - Infrastructure copied to API output without direct references

4. **✅ Implemented Exercises Module - Application Layer:**
    - ✅ Commands: `CreateExerciseCommand`, `UpdateExerciseCommand`, `DeleteExerciseCommand`
    - ✅ Queries: `GetExerciseByIdQuery`, `GetAllExercisesQuery`, `GetAllMuscleGroupsQuery`, `GetAllEquipmentQuery`
    - ✅ DTOs: `ExerciseDto`, `MuscleGroupDto`, `EquipmentDto`, `CreateExerciseRequest`, `UpdateExerciseRequest`
    - ✅ **Static Riok.Mapperly mappers** for compile-time performance
    - ✅ FluentValidation validators for all commands
    - ✅ Module registration with reflection-based Infrastructure discovery

5. **✅ Integrated Exercises Module into API:**
    - ✅ `ExercisesController.cs` with full CRUD operations, muscle groups, and equipment endpoints
    - ✅ Proper authorization with JWT Bearer tokens
    - ✅ User ownership validation and global/user-specific exercise support
    - ✅ **Automatic module discovery** through `ModuleRegistry` system
    - ✅ MongoDB collections auto-registered with dependency injection
    - ✅ Swagger/OpenAPI documentation with comprehensive endpoint coverage

**Exercises Module Status: ✅ PRODUCTION READY**
- Module successfully loads and registers at startup
- All endpoints accessible via Swagger UI at `/swagger`
- MongoDB indexes configured and optimized
- Error handling and logging implemented
- Clean Architecture principles maintained

### Module Testing Framework (Apply to Each Module)

**Note:** Apply these testing steps to each module (Users, Exercises, Routines, WorkoutLogs) after implementing the respective module. Replace `[Module]` with the actual module name.

#### Unit Testing Implementation

1. **Setup Test Project Dependencies:**
   ```bash
   # Add required NuGet packages to the test project
   dotnet add dotFitness.Modules.[Module].Tests/dotFitness.Modules.[Module].Tests.csproj package Microsoft.Extensions.Logging.Abstractions
   dotnet add dotFitness.Modules.[Module].Tests/dotFitness.Modules.[Module].Tests.csproj package Moq
   dotnet add dotFitness.Modules.[Module].Tests/dotFitness.Modules.[Module].Tests.csproj package FluentAssertions
   dotnet add dotFitness.Modules.[Module].Tests/dotFitness.Modules.[Module].Tests.csproj package Microsoft.Extensions.Options
   dotnet add dotFitness.Modules.[Module].Tests/dotFitness.Modules.[Module].Tests.csproj package Testcontainers.MongoDb
   ```

2. **Create Domain Entity Tests:**
   - Create `dotFitness.Modules.[Module].Tests/Domain/Entities/[Entity]Tests.cs`
   - Test entity creation, validation, and business logic
   - Example structure:
     ```csharp
     [Fact]
     public void Should_Create_Valid_Entity_With_Required_Properties()
     [Fact] 
     public void Should_Calculate_Derived_Properties_Correctly()
     [Theory, InlineData(...)]
     public void Should_Validate_Input_Parameters(params)
     ```

3. **Create Repository Tests:**
   - Create `dotFitness.Modules.[Module].Tests/Infrastructure/MongoDB/[Entity]RepositoryTests.cs`
   - Use Testcontainers.MongoDb for integration testing
   - Test CRUD operations, queries, and error handling
   - Example structure:
     ```csharp
     [Fact]
     public async Task Should_Create_Entity_Successfully()
     [Fact]
     public async Task Should_Retrieve_Entity_By_Id()
     [Fact]
     public async Task Should_Update_Entity_Successfully()
     [Fact]
     public async Task Should_Delete_Entity_Successfully()
     [Fact]
     public async Task Should_Return_NotFound_For_NonExistent_Entity()
     ```

4. **Create Command Handler Tests:**
   - Create `dotFitness.Modules.[Module].Tests/Infrastructure/Handlers/[Command]HandlerTests.cs`
   - Mock dependencies (repositories, logger, external services)
   - Test success scenarios, validation failures, and error cases
   - Example structure:
     ```csharp
     [Fact]
     public async Task Should_Handle_Valid_Command_Successfully()
     [Fact]
     public async Task Should_Return_ValidationError_For_Invalid_Command()
     [Fact]
     public async Task Should_Handle_Repository_Errors_Gracefully()
     ```

5. **Create Query Handler Tests:**
   - Create `dotFitness.Modules.[Module].Tests/Infrastructure/Handlers/[Query]HandlerTests.cs`
   - Test data retrieval, filtering, and error handling
   - Example structure:
     ```csharp
     [Fact]
     public async Task Should_Return_Entity_When_Found()
     [Fact]
     public async Task Should_Return_NotFound_When_Entity_DoesNot_Exist()
     [Fact]
     public async Task Should_Apply_Filters_Correctly()
     ```

6. **Create Validator Tests:**
   - Create `dotFitness.Modules.[Module].Tests/Application/Validators/[Command]ValidatorTests.cs`
   - Test all validation rules and edge cases
   - Example structure:
     ```csharp
     [Fact]
     public void Should_Pass_Validation_For_Valid_Command()
     [Theory, InlineData(...)]
     public void Should_Fail_Validation_For_Invalid_Property(params)
     [Fact]
     public void Should_Have_Validation_Error_For_Missing_Required_Field()
     ```

7. **Create Mapper Tests:**
   - Create `dotFitness.Modules.[Module].Tests/Application/Mappers/[Entity]MapperTests.cs`
   - Test entity-to-DTO and DTO-to-entity mappings
   - Example structure:
     ```csharp
     [Fact]
     public void Should_Map_Entity_To_Dto_Correctly()
     [Fact]
     public void Should_Map_Dto_To_Entity_Correctly() 
     [Fact]
     public void Should_Handle_Null_Values_In_Mapping()
     ```

8. **Run Unit Tests:**
   ```bash
   # Run all tests for the module
   dotnet test dotFitness.Modules.[Module].Tests/
   
   # Run with coverage (if coverage tools are installed)
   dotnet test dotFitness.Modules.[Module].Tests/ --collect:"XPlat Code Coverage"
   
   # Run specific test class
   dotnet test dotFitness.Modules.[Module].Tests/ --filter "ClassName=[Entity]Tests"
   ```

#### API Testing with HTTP Files

1. **Create HTTP Test Files Directory:**
   - Create `tests/api/` directory in the solution root
   - Create `tests/api/[module]/` subdirectory for each module

2. **Create Environment Configuration:**
   - Create `tests/api/http-client.env.json`:
     ```json
     {
       "dev": {
         "baseUrl": "https://localhost:7001",
         "adminEmail": "your.admin.email@gmail.com",
         "testUserEmail": "test.user@example.com"
       },
       "prod": {
         "baseUrl": "https://your-api.azurewebsites.net"
       }
     }
     ```

3. **Create Authentication HTTP File:**
   - Create `tests/api/auth/auth.http`:
     ```http
     ### Login with Google (Mock for testing)
     POST {{baseUrl}}/api/auth/google-login
     Content-Type: application/json
     
     {
       "googleTokenId": "mock_google_token_for_{{adminEmail}}"
     }
     
     > {%
       client.global.set("authToken", response.body.token);
       client.global.set("userId", response.body.user.id);
     %}
     
     ### Verify Authentication
     GET {{baseUrl}}/api/users/profile
     Authorization: Bearer {{authToken}}
     ```

4. **Create Module-Specific HTTP Files:**
   
   **For Users Module (`tests/api/users/users.http`):**
   ```http
   ### Get User Profile
   GET {{baseUrl}}/api/users/profile
   Authorization: Bearer {{authToken}}
   
   ### Update User Profile
   PUT {{baseUrl}}/api/users/profile
   Authorization: Bearer {{authToken}}
   Content-Type: application/json
   
   {
     "displayName": "Updated Test User",
     "gender": "Male",
     "dateOfBirth": "1990-01-01",
     "unitPreference": "Metric"
   }
   
   ### Add User Metric
   POST {{baseUrl}}/api/users/metrics
   Authorization: Bearer {{authToken}}
   Content-Type: application/json
   
   {
     "weight": 70.5,
     "height": 175.0,
     "date": "{{$isoTimestamp}}",
     "notes": "Test metric entry"
   }
   
   ### Get Latest User Metric
   GET {{baseUrl}}/api/users/metrics/latest
   Authorization: Bearer {{authToken}}
   
   ### Get User Metrics History
   GET {{baseUrl}}/api/users/metrics?startDate=2024-01-01&endDate={{$isoTimestamp}}
   Authorization: Bearer {{authToken}}
   ```

   **For Exercises Module (`tests/api/exercises/exercises.http`):**
   ```http
   ### Get All Exercises
   GET {{baseUrl}}/api/exercises
   Authorization: Bearer {{authToken}}
   
   ### Create Exercise
   POST {{baseUrl}}/api/exercises
   Authorization: Bearer {{authToken}}
   Content-Type: application/json
   
   {
     "name": "Test Exercise",
     "description": "A test exercise for API testing",
     "muscleGroups": ["Chest", "Triceps"],
     "equipment": ["Barbell"],
     "instructions": ["Step 1", "Step 2"],
     "difficulty": "Intermediate",
     "videoUrl": "https://example.com/video"
   }
   
   > {%
     client.global.set("exerciseId", response.body.id);
   %}
   
   ### Get Exercise by ID
   GET {{baseUrl}}/api/exercises/{{exerciseId}}
   Authorization: Bearer {{authToken}}
   
   ### Update Exercise
   PUT {{baseUrl}}/api/exercises/{{exerciseId}}
   Authorization: Bearer {{authToken}}
   Content-Type: application/json
   
   {
     "name": "Updated Test Exercise",
     "description": "Updated description",
     "muscleGroups": ["Chest", "Shoulders"],
     "equipment": ["Dumbbell"],
     "instructions": ["Updated Step 1", "Updated Step 2"],
     "difficulty": "Advanced",
     "videoUrl": "https://example.com/updated-video"
   }
   
   ### Delete Exercise
   DELETE {{baseUrl}}/api/exercises/{{exerciseId}}
   Authorization: Bearer {{authToken}}
   ```

   **For Routines Module (`tests/api/routines/routines.http`):**
   ```http
   ### Get All Routines
   GET {{baseUrl}}/api/routines
   Authorization: Bearer {{authToken}}
   
   ### Create Routine
   POST {{baseUrl}}/api/routines
   Authorization: Bearer {{authToken}}
   Content-Type: application/json
   
   {
     "name": "Test Workout Routine",
     "description": "A test routine for API testing",
     "exercises": [
       {
         "exerciseId": "{{exerciseId}}",
         "sets": 3,
         "reps": 10,
         "weight": 50.0,
         "restTime": 60,
         "notes": "Focus on form"
       }
     ],
     "tags": ["Strength", "Upper Body"]
   }
   
   > {%
     client.global.set("routineId", response.body.id);
   %}
   
   ### Get Routine Suggestions
   GET {{baseUrl}}/api/routines/suggest?muscleGroups=Chest&difficulty=Intermediate
   Authorization: Bearer {{authToken}}
   
   ### Get Predefined Templates
   GET {{baseUrl}}/api/routines/templates
   Authorization: Bearer {{authToken}}
   ```

   **For WorkoutLogs Module (`tests/api/workoutlogs/workoutlogs.http`):**
   ```http
   ### Log Workout
   POST {{baseUrl}}/api/workoutlogs
   Authorization: Bearer {{authToken}}
   Content-Type: application/json
   
   {
     "routineId": "{{routineId}}",
     "startTime": "{{$isoTimestamp}}",
     "endTime": "{{$isoTimestamp}}",
     "exercisesPerformed": [
       {
         "exerciseId": "{{exerciseId}}",
         "setsPerformed": [
           {
             "reps": 10,
             "weight": 50.0,
             "restTime": 60,
             "completed": true
           },
           {
             "reps": 8,
             "weight": 52.5,
             "restTime": 60,
             "completed": true
           }
         ]
       }
     ],
     "notes": "Great workout session!"
   }
   
   > {%
     client.global.set("workoutLogId", response.body.id);
   %}
   
   ### Get Workout History
   GET {{baseUrl}}/api/workoutlogs/history?startDate=2024-01-01&endDate={{$isoTimestamp}}
   Authorization: Bearer {{authToken}}
   
   ### Get Workout Statistics
   GET {{baseUrl}}/api/workoutlogs/statistics
   Authorization: Bearer {{authToken}}
   
   ### Get Personal Bests
   GET {{baseUrl}}/api/workoutlogs/personal-bests
   Authorization: Bearer {{authToken}}
   
   ### Get Exercise Progress
   GET {{baseUrl}}/api/workoutlogs/exercise-progress/{{exerciseId}}
   Authorization: Bearer {{authToken}}
   ```

5. **Create Test Scenarios Documentation:**
   - Create `tests/api/README.md` with test execution instructions:
     ```markdown
     # API Testing Guide
     
     ## Prerequisites
     - VS Code with REST Client extension installed
     - Application running locally or deployed
     - Valid Google OAuth setup for authentication
     
     ## Test Execution Order
     1. Run `auth/auth.http` to authenticate and get token
     2. Run module-specific tests in dependency order:
        - Users (foundational)
        - Exercises (required for routines)
        - Routines (required for workout logs)
        - WorkoutLogs (depends on routines and exercises)
     
     ## Environment Configuration
     - Update `http-client.env.json` with your environment URLs
     - Set appropriate email addresses for testing
     
     ## Expected Results
     - All requests should return appropriate HTTP status codes
     - Response bodies should match expected schemas
     - Authentication should be required for protected endpoints
     ```

6. **Run API Tests:**
   - Open HTTP files in VS Code with REST Client extension
   - Execute requests in order (authentication first)
   - Verify responses and status codes
   - Test error scenarios (invalid data, unauthorized access)
   - Document any issues found

#### Comprehensive Testing Checklist

For each module, ensure the following testing aspects are covered:

**Domain Layer Testing:**
- [ ] Entity creation and validation
- [ ] Business rule enforcement
- [ ] Value object behavior
- [ ] Domain event creation

**Application Layer Testing:**
- [ ] Command validation
- [ ] Query parameter validation
- [ ] DTO mapping accuracy
- [ ] Service orchestration

**Infrastructure Layer Testing:**
- [ ] Repository CRUD operations
- [ ] Database connectivity
- [ ] Error handling and logging
- [ ] External service integration

**API Layer Testing:**
- [ ] Endpoint accessibility
- [ ] Request/response serialization
- [ ] Authentication and authorization
- [ ] Error response formatting
- [ ] API documentation accuracy

**Integration Testing:**
- [ ] End-to-end workflow testing
- [ ] Cross-module communication
- [ ] Database transaction handling
- [ ] Outbox pattern functionality

**Performance Testing:**
- [ ] Response time benchmarks
- [ ] Concurrent request handling
- [ ] Database query optimization
- [ ] Memory usage monitoring
6. **Implement Routines Module (Repeat Steps 28-32, replacing "Exercises" with "Routines"):**
    - **`dotFitness.Modules.Routines` Projects:** Create and reference.
    - **Domain:** Define `Routine.cs` (including nested `Exercises` property).
    - **Infrastructure:** Implement `IRoutineRepository.cs`, `RoutineRepository.cs`. Implement `CreateRoutineCommandHandler`, `UpdateRoutineCommandHandler`, `DeleteRoutineCommandHandler`. Implement `GetRoutineByIdQueryHandler`, `GetAllRoutinesQueryHandler`, `SuggestRoutineQueryHandler`, `GetPredefinedRoutineTemplatesQueryHandler`.
    - **Application:** Define `Commands`, `Queries`, `DTOs` (`RoutineDto`, `RoutineExerciseDto`), Mappers, and FluentValidation validators.
    - **API:** Create `dotFitness.Api/Controllers/RoutinesController.cs` (for CRUD, suggestions, templates). Register `IMongoCollection<Routine>`.
    - **Testing:** Apply the Module Testing Framework above for comprehensive unit and API testing.
7. **Implement Workout Logs Module (Repeat Steps 28-32, replacing "Exercises" with "WorkoutLogs"):**
    - **`dotFitness.Modules.WorkoutLogs` Projects:** Create and reference.
    - **Domain:** Define `WorkoutLog.cs` (including nested `ExercisesPerformed`, `SetsPerformed`). Define `WorkoutCompletedEvent.cs`.
    - **Infrastructure:** Implement `IWorkoutLogRepository.cs`, `WorkoutLogRepository.cs`. Implement `LogWorkoutCommandHandler`.
        - **WT-003 Set Logging (Backend):** `LogWorkoutCommandHandler` will save `WorkoutLog` and **initiate Outbox Pattern** by inserting `WorkoutCompletedEvent` into `outboxMessages` within a MongoDB transaction.
    - **Application:** Define `Commands` (`LogWorkoutCommand`), `Queries` (`GetWorkoutLogByIdQuery`, `GetWorkoutHistoryQuery`, `GetWorkoutStatisticsQuery`, `GetPersonalBestsQuery`, `GetExerciseProgressQuery`), `DTOs`, Mappers, Validators.
    - **API:** Create `dotFitness.Api/Controllers/WorkoutLogsController.cs` (for logging, history, stats, personal bests). Register `IMongoCollection<WorkoutLog>`.
	- **Testing:** Apply the Module Testing Framework above, with special focus on Outbox Pattern testing and workout statistics calculations.

### Phase 4: Backend Cross-Cutting Enhancements

1. **Implement Outbox Processor:**
    - Create `dotFitness.Api/Services/OutboxProcessorService.cs` (or similar location) implementing `IHostedService`.
    - Logic: Poll `outboxMessages` collection, deserialize events, publish events via `IMediator` (or external message broker client). Mark messages as processed.
    - Register `OutboxProcessorService` in `dotFitness.Api/Program.cs` using `builder.Services.AddHostedService<OutboxProcessorService>()`.
2. **Integrate Centralized Error Handling:**
    - In `dotFitness.Api/Program.cs`, add exception handling middleware/filters to catch exceptions globally and return consistent `ProblemDetails` based JSON error responses. Ensure `FluentValidation` errors are correctly serialized.
3. **Finalize API Versioning:**
    - In `dotFitness.Api/Program.cs`, ensure API Versioning is fully configured.
    - Apply `[ApiVersion("1.0")]` attribute to all controllers and use versioning routing conventions.

### Phase 5: Frontend Development (Vue.js)

1. **Create Vue.js Project:**
    - Navigate to `dotFitness.WorkoutTracker/` directory.
    - Execute `npm create vue@latest dotFitness.WebUI`. Select default options.
    - `cd dotFitness.WebUI`.
    - `npm install`.
2. **Configure Vue Router:**
    - `npm install vue-router`.
    - Create `dotFitness.WebUI/src/router/index.js` and define routes (e.g., `/login`, `/dashboard`, `/exercises`, `/routines`, `/workout`).
    - Implement global navigation guard (`router.beforeEach`) to check for JWT and redirect to `/login` if not authenticated.
3. **Configure Tailwind CSS:**
    - `npm install -D tailwindcss postcss autoprefixer`.
    - `npx tailwindcss init -p`.
    - Configure `dotFitness.WebUI/tailwind.config.js` (`darkMode: 'class'`, `content` to scan Vue files).
    - Add Tailwind directives to `dotFitness.WebUI/src/assets/main.css` (or `style.css`).
4. **Configure Chart.js:**
    - `npm install chart.js vue-chart-3`.
    - Import and register `vue-chart-3` components in relevant Vue components.
5. **Implement Theming (URS NFR-U-005):**
    - **42.1. Pinia Store for Theme:** Create `dotFitness.WebUI/src/stores/theme.js` with `currentTheme` state, `toggleTheme` action, and persistence to `localStorage`.
    - **42.2. Theme Toggle Component:** Create `dotFitness.WebUI/src/components/ThemeToggle.vue` and place it in the `Navbar.vue`.
    - **42.3. Apply Theme Styling:** Use Tailwind `dark:` variants extensively.
    - **42.4. Adapt Chart.js:** Dynamically set Chart.js chart colors based on `currentTheme` and ensure reactive re-rendering.
6. **Implement Core Layout and Navigation:**
    - Create `dotFitness.WebUI/src/App.vue` (main layout).
    - Create `dotFitness.WebUI/src/components/Navbar.vue` (top bar).
    - Create `dotFitness.WebUI/src/components/Sidebar.vue` (left navigation).
    - Create `dotFitness.WebUI/src/components/BottomNav.vue` (mobile navigation).
    - Apply responsive Tailwind CSS for layout adaptation.
7. **Implement Authentication Flow (URS UM-001, UM-002):**
    - **44.1. Welcome/Login Screen:** (`dotFitness.WebUI/src/views/WelcomeView.vue`). Design with logo, tagline, “Sign in with Google” button.
    - **44.2. Google OAuth Integration:** Use a Vue Google Auth library (e.g., `@vue-google-oauth2/google-oauth2-vue`) or implement a custom flow.
    - **44.3. Backend JWT Exchange:** Call `POST /api/auth/google-login` endpoint from frontend, sending Google auth code.
    - **44.4. JWT Handling:** Store received JWT in a Pinia store (`src/stores/auth.js`) and/or `localStorage`.
    - **44.5. Axios Interceptors:** Configure `axios` to attach JWT to `Authorization: Bearer` header for all subsequent API requests.
    - **44.6. Protected Routes:** Ensure Vue Router guards redirect unauthenticated users.
8. **Implement Dashboard (URS PA-001):**
    - **`dotFitness.WebUI/src/views/DashboardView.vue`:**
    - Fetch user profile, today’s routine, motivational stats (`Workout Streak`, `Total Workouts`) from backend.
    - Create components: `TodaysWorkoutCard.vue`, `MotivationalMetricsCard.vue`, `ProgressSummaryCard.vue`, `QuickActionsGrid.vue`.
9. **Implement Exercise Management (URS EM-001 to EM-004, EM-005):**
    - **`dotFitness.WebUI/src/views/ExercisesView.vue`:** For listing/search.
    - **`dotFitness.WebUI/src/views/ExerciseForm.vue`:** For create/edit.
    - Components: `ExerciseList.vue`, `ExerciseCard.vue`, `ExerciseFilter.vue`, `ExerciseFormFields.vue` (with `MultiSelectDropdown.vue` for muscle groups/equipment), `VideoEmbedPreview.vue`.
    - Implement API calls for `GET /api/exercises`, `POST`, `PUT`, `DELETE /api/exercises/{id}`.
    - Implement dynamic population of multi-selects for muscle groups/equipment from backend (`GET /api/musclegroups`, `GET /api/equipment`). Allow user to add custom tags if not found in dropdown (then backend creates user-specific tag).
10. **Implement Routine Management (URS RM-001 to RM-006, RM-007, RM-008):**
    - **`dotFitness.WebUI/src/views/RoutinesView.vue`:** For listing routines.
    - **`dotFitness.WebUI/src/views/RoutineBuilderView.vue`:** For create/edit.
    - Components: `RoutineList.vue`, `RoutineCard.vue`, `RoutineBuilderForm.vue` (with drag-and-drop), `ExercisePickerModal.vue`, `RoutineExerciseItem.vue` (for inputs: sets, reps, rest time with unit picker).
    - Implement API calls for `GET /api/routines`, `POST`, `PUT`, `DELETE /api/routines/{id}`.
    - Implement calls for `GET /api/routines/suggest` and `GET /api/routines/templates`.
11. **Implement Workout Session Screen (URS WT-001 to WT-006):**
    - **`dotFitness.WebUI/src/views/WorkoutSessionView.vue`:**
    - Components: `CurrentExerciseDisplay.vue` (with `VideoPlayer.vue`), `SetLogger.vue` (inputs for reps/weight), `RestTimer.vue` (countdown timer).
    - Logic: Track current exercise, log sets, manage rest timer, navigate.
    - Implement API call `POST /api/workoutlogs` to submit completed workout data.
    - Implement `src/views/WorkoutSummaryView.vue` for post-workout.
12. **Implement Progress & Body Metrics Views (URS PA-003, PA-004, PA-005, PA-006):**
    - **`dotFitness.WebUI/src/views/ProgressView.vue`:** Main hub for progress visualization.
    - **`dotFitness.WebUI/src/views/BodyMetricsView.vue`:** (within ProgressView or separate) for input and historical data.
    - Components: `WeightChart.vue`, `BmiChart.vue`, `ExerciseProgressChart.vue` (using Chart.js), `BodyMetricForm.vue` (for input), `PersonalBestsList.vue`, `AchievementBadges.vue`.
    - Implement API calls (`GET /api/users/metrics`, `GET /api/stats/exercises/{exerciseId}/progress`, `GET /api/stats/personalbests`).
    - Implement data preparation for Chart.js.

### Phase 6: Infrastructure as Code (Terraform)

1. **Setup Azure Service Principal:** Execute `az login` and `az ad sp create-for-rbac --role="Contributor" --scopes="/subscriptions/<YOUR_SUBSCRIPTION_ID>" --name "TerraformServicePrincipal"`. Store outputs.
2. **Setup MongoDB Atlas Project & API Keys:** Manually create an Atlas Project and generate Programmatic API Keys (Public and Private). Note Project ID.
3. **Create `infra/` Directory:** In `dotFitness.WorkoutTracker/`, create `infra/`.
4. **Create `versions.tf`:** `infra/versions.tf` (define Terraform/provider versions, e.g., `azurerm ~>3.0`, `mongodbatlas ~>1.14`).
5. **Create `main.tf`:** `infra/main.tf` (define all Azure resources: Resource Group, App Service Plan (F1), Linux Web App, Static Site, Application Insights; and MongoDB Atlas resources: Project, Cluster (M0), Database User, Project IP Access List).
6. **Create `variables.tf`:** `infra/variables.tf` (define variables for app name, location, environment, initial admin email, all MongoDB Atlas credentials, marking sensitive variables).
7. **Create `outputs.tf`:** `infra/outputs.tf` (export `api_url`, `frontend_url`).
8. **(Optional) Create `backend.tf`:** `infra/backend.tf` (if using Azure Storage for remote Terraform state).

### Phase 7: CI/CD Pipeline Setup (GitHub Actions)

1. **Initialize GitHub Repository:**
    - Navigate to `dotFitness.WorkoutTracker`.
    - `git init`.
    - Create `.gitignore` (include `bin/`, `obj/`, `publish/`, `node_modules/`, `dist/`, `.tfstate*`, `.tfvars`, `.env`).
    - `git add .`
    - `git commit -m "Initial commit of dotFitness project structure and infra"`
    - Create a new GitHub repository and push your local repository to it.
2. **Configure GitHub Secrets:**
    - In your GitHub repository settings, go to `Secrets` -> `Actions`.
    - Add all necessary secrets: `AZURE_CLIENT_ID`, `AZURE_TENANT_ID`, `AZURE_SUBSCRIPTION_ID`, `MONGODB_ATLAS_PUBLIC_KEY`, `MONGODB_ATLAS_PRIVATE_KEY`, `MONGODB_ATLAS_PROJECT_ID`, `MONGODB_ATLAS_DB_USERNAME`, `MONGODB_ATLAS_DB_PASSWORD`, `INITIAL_ADMIN_EMAIL`, `GOOGLE_CLIENT_ID`, `GOOGLE_CLIENT_SECRET`, `AZURE_STATIC_WEB_APPS_API_TOKEN`.
3. **Create GitHub Actions Workflows Directory:**
    - In `dotFitness.WorkoutTracker/`, create `.github/workflows/`.
4. **Create `infra-deploy.yml` (Terraform Pipeline):**
    - Create `dotFitness.WorkoutTracker/.github/workflows/infra-deploy.yml`.
    - Define workflow name, trigger (`push` to `main`, `paths: 'infra/**'`), jobs.
    - Steps: Checkout, Setup Terraform, Azure Login (using Service Principal secrets), Configure MongoDB Atlas Provider secrets (from GitHub Secrets), `terraform init`, `terraform plan`, `terraform apply` (conditional on `main` branch push).
5. **Create `backend-ci-cd.yml` (.NET API Pipeline):**
    - Create `dotFitness.WorkoutTracker/.github/workflows/backend-ci-cd.yml`.
    - Define workflow name, trigger (`push`/`pull_request` to `main`, targeting backend paths).
    - Steps: Checkout, Setup .NET (`8.0.x`), `dotnet restore`, `dotnet build`, `dotnet test`, `dotnet publish`, Azure Login, `azure/webapps-deploy@v2` (for API deployment, passing `app-settings` from GitHub Secrets).
6. **Create `frontend-ci-cd.yml` (Vue.js Pipeline):**
    - Create `dotFitness.WorkoutTracker/.github/workflows/frontend-ci-cd.yml`.
    - Define workflow name, trigger (`push`/`pull_request` to `main`, targeting frontend path).
    - Steps: Checkout, Setup Node.js (`20`), `npm install` (in `dotFitness.WebUI`), `npm run build` (in `dotFitness.WebUI`, passing `VITE_APP_API_BASE_URL` from deployed API URL as an environment variable), `azure/static-web-apps-deploy@v1`.

### Phase 8: Final Testing & Deployment

1. **Initial Terraform Apply:**
    - Navigate to `dotFitness.WorkoutTracker/infra`.
    - Execute `terraform init`.
    - Execute `terraform plan` (review changes).
    - Execute `terraform apply -auto-approve` (this will provision your cloud resources for the first time).
    - Note the outputs (API URL, Frontend URL).
2. **Push Code to Trigger CI/CD:**
    - Ensure your `backend-ci-cd.yml` and `frontend-ci-cd.yml` are correctly configured with the actual deployed URLs (e.g., `VITE_APP_API_BASE_URL`).
    - Commit and push all your application code (`git add .`, `git commit -m "Implement initial app features"`, `git push origin main`).
    - Monitor GitHub Actions for successful CI/CD runs and deployments.
3. **End-to-End Testing (Manual):**
    - Access the deployed frontend URL.
    - Perform manual tests for all functional requirements (URS 2.1-2.6) on the deployed Azure environment.
    - Verify authentication, exercise creation, routine building, workout logging, progress visualization, theme switching.
    - Test API endpoints using Postman/Insomnia against the deployed backend URL.
4. **User Acceptance Testing (UAT):**
    - Onboard your “small subset of people” to test the application in a real-world scenario.
    - Collect feedback on usability, missing features, and bugs.
5. **Monitor & Iterate:**
    - Continuously monitor application performance and logs using Azure Application Insights.
    - Systematically gather user feedback.
    - Plan and prioritize future development sprints based on feedback and business needs.

---

This comprehensive list provides a granular, actionable plan for developing **dotFitness**.