# GitHub Copilot Instructions for dotFitness Testing

## Overview
This document provides comprehensive testing guidelines for the dotFitness workout tracker application, following established patterns and best practices observed in the codebase.

When generating code, please follow these user provided coding instructions. You can ignore an instruction if it contradicts a system message.

## Instructions
This document serves as the definitive guide for maintaining consistency and quality in the dotFitness testing suite. When GitHub Copilot generates test code for this project, it must strictly adhere to these patterns and practices to ensure reliability, maintainability, and consistency across all test implementations.

## Project Architecture
- **Clean Architecture**: Domain → Application → Infrastructure → API
- **Modular Design**: Each module (Users, Exercises, Routines, WorkoutLogs) has its own test project
- **CQRS Pattern**: Commands for writes, Queries for reads
- **MongoDB**: Primary database with Testcontainers for integration tests
- **Result Pattern**: All operations return `Result<T>` or `Result` from SharedKernel

## Testing Framework Stack
- **Unit Testing**: xUnit, FluentAssertions, Moq
- **Integration Testing**: Testcontainers.MongoDb for repository tests
- **API Testing**: HTTP files with REST Client extension
- **Validation Testing**: FluentValidation.TestHelper

## Test File Organization Pattern

### Domain Layer Tests
```
dotFitness.Modules.[Module].Tests/
├── Domain/
│   └── Entities/
│       ├── [Entity]Tests.cs
│       └── [Entity]SimpleTests.cs
```

### Application Layer Tests
```
dotFitness.Modules.[Module].Tests/
├── Application/
│   ├── Validators/
│   │   └── [Command]ValidatorTests.cs
│   └── Mappers/
│       └── [Entity]MapperTests.cs
```

### Infrastructure Layer Tests
```
dotFitness.Modules.[Module].Tests/
├── Infrastructure/
│   ├── Handlers/
│   │   ├── [Command]HandlerTests.cs
│   │   └── [Query]HandlerTests.cs
│   └── MongoDB/
│       └── [Entity]RepositoryTests.cs
```

## Test Naming Conventions

When creating test files and methods, always follow these specific naming patterns to maintain consistency across the entire test suite:

### Test Method Names
- **Mandatory Format**: `Should_[ExpectedBehavior]_[UnderCondition]`
- **Purpose**: This naming convention immediately communicates what the test validates and under which circumstances
- **Examples with explanations**:
  - `Should_Create_Valid_User_With_Required_Properties()` - Tests that a user can be created successfully when all required properties are provided
  - `Should_Return_ValidationError_For_Invalid_Email()` - Tests that the system properly validates email format and returns appropriate errors
  - `Should_Handle_Repository_Errors_Gracefully()` - Tests that repository failures are caught and handled without crashing the application
  - `Should_Calculate_BMI_Correctly_When_Weight_And_Height_Provided()` - Tests business logic calculations with specific input conditions

### Test Class Names
- **Mandatory Format**: `[ClassUnderTest]Tests`
- **Purpose**: Clearly identifies which class is being tested and groups all related tests together
- **Examples with explanations**: 
  - `UserTests` - Contains all tests for the User domain entity
  - `LoginWithGoogleCommandHandlerTests` - Contains all tests for the specific command handler
  - `UserRepositoryTests` - Contains all tests for user repository operations

## Test Structure Pattern (AAA)

Every test method must strictly follow the Arrange-Act-Assert pattern. This pattern ensures tests are readable, maintainable, and follow a logical flow that any developer can understand:

```csharp
[Fact]
public async Task Should_Create_User_Successfully()
{
    // Arrange - Set up all test data, mocks, and preconditions
    // This section prepares everything needed for the test
    var user = new User
    {
        Email = "test@example.com",
        DisplayName = "Test User"
    };

    // Act - Execute the specific action being tested
    // This should be a single operation that we're validating
    var result = await _repository.CreateAsync(user);

    // Assert - Verify that the action produced the expected results
    // Use multiple assertions to thoroughly validate the outcome
    result.IsSuccess.Should().BeTrue();
    result.Value.Should().NotBeNull();
    result.Value.Id.Should().NotBeNullOrEmpty();
}
```

**Critical Guidelines for AAA Pattern:**
- **Arrange section**: Must set up all test data with predictable, manually defined values (especially DateTime values)
- **Act section**: Should contain only the single operation being tested - avoid multiple actions in one test
- **Assert section**: Must thoroughly validate all expected outcomes using FluentAssertions for readability

## Required Test Dependencies

### Standard Test Packages
```xml
<PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.8.0" />
<PackageReference Include="xunit" Version="2.5.3" />
<PackageReference Include="xunit.runner.visualstudio" Version="2.5.3" />
<PackageReference Include="FluentAssertions" Version="7.0.0" />
<PackageReference Include="Moq" Version="4.20.72" />
<PackageReference Include="Microsoft.Extensions.Logging.Abstractions" Version="9.0.5" />
<PackageReference Include="Microsoft.Extensions.Options" Version="9.0.5" />
```

### MongoDB Integration Tests
```xml
<PackageReference Include="Testcontainers.MongoDb" Version="4.1.0" />
<PackageReference Include="MongoDB.Driver" Version="3.4.0" />
```

### CQRS/MediatR Tests
```xml
<PackageReference Include="MediatR" Version="12.5.0" />
<PackageReference Include="FluentValidation" Version="12.0.0" />
```

## Domain Entity Tests

### Test Entity Creation and Validation
```csharp
[Fact]
public void Should_Create_Valid_Entity_With_Required_Properties()
{
    // Test default values, auto-generated IDs, timestamps
}

[Fact]
public void Should_Calculate_Derived_Properties_Correctly()
{
    // Test business logic calculations (e.g., BMI calculation)
}

[Theory]
[InlineData("invalid-email")]
[InlineData("")]
[InlineData(null)]
public void Should_Validate_Input_Parameters(string email)
{
    // Test validation rules
}
```

### Test Business Logic Methods
```csharp
[Fact]
public void Should_Add_Role_Successfully()
{
    // Test entity methods that modify state
}

[Fact]
public void Should_Not_Add_Duplicate_Role()
{
    // Test business rule enforcement
}
```

## Repository Tests (Integration)

### MongoDB Test Fixture Pattern
```csharp
[Collection("MongoDB")]
public class [Entity]RepositoryTests(MongoDbFixture fixture) : IAsyncLifetime
{
    private IMongoDatabase _database = null!;
    private [Entity]Repository _repository = null!;
    private Mock<ILogger<[Entity]Repository>> _loggerMock = null!;

    public async Task InitializeAsync()
    {
        _database = fixture.CreateFreshDatabase();
        _database.GetCollection<[Entity]>("[collection_name]");
        _loggerMock = new Mock<ILogger<[Entity]Repository>>();
        _repository = new [Entity]Repository(_database, _loggerMock.Object);
    }

    public async Task DisposeAsync()
    {
        await fixture.CleanupDatabaseAsync();
    }
}
```

### CRUD Operation Tests
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

## Command Handler Tests

### Mock Setup Pattern
```csharp
public class [Command]HandlerTests
{
    private readonly Mock<I[Entity]Repository> _repositoryMock;
    private readonly Mock<ILogger<[Command]Handler>> _loggerMock;
    private readonly Mock<IOptions<[Settings]>> _settingsMock;
    private readonly [Command]Handler _handler;

    public [Command]HandlerTests()
    {
        _repositoryMock = new Mock<I[Entity]Repository>();
        _loggerMock = new Mock<ILogger<[Command]Handler>>();
        _settingsMock = new Mock<IOptions<[Settings]>>();
        
        _handler = new [Command]Handler(
            _repositoryMock.Object,
            _loggerMock.Object,
            _settingsMock.Object
        );
    }
}
```

### Handler Test Cases
```csharp
[Fact]
public async Task Should_Handle_Valid_Command_Successfully()
{
    // Test successful execution path
}

[Fact]
public async Task Should_Return_ValidationError_For_Invalid_Command()
{
    // Test validation failure scenarios
}

[Fact]
public async Task Should_Handle_Repository_Errors_Gracefully()
{
    // Test error handling and logging
}
```

## Query Handler Tests

### Query Test Pattern
```csharp
[Fact]
public async Task Should_Return_Entity_When_Found()
{
    // Test successful data retrieval
}

[Fact]
public async Task Should_Return_NotFound_When_Entity_DoesNot_Exist()
{
    // Test not found scenarios
}

[Fact]
public async Task Should_Apply_Filters_Correctly()
{
    // Test query filtering and sorting
}
```

## Validator Tests

### FluentValidation Testing
```csharp
[Fact]
public void Should_Pass_Validation_For_Valid_Command()
{
    // Arrange
    var command = new [Command](...);

    // Act
    var result = _validator.TestValidate(command);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
}

[Theory]
[InlineData("")]
[InlineData(null)]
[InlineData("invalid-value")]
public void Should_Fail_Validation_For_Invalid_Property(string invalidValue)
{
    // Test validation rule violations
}
```

## Mapper Tests

### Entity-DTO Mapping Tests
```csharp
[Fact]
public void Should_Map_Entity_To_Dto_Correctly()
{
    // Test entity → DTO mapping using real mapper instance
    // NEVER mock the mapper - use actual mapper to test real behavior
}

[Fact]
public void Should_Map_Dto_To_Entity_Correctly()
{
    // Test DTO → entity mapping using real mapper instance
    // NEVER mock the mapper - use actual mapper to test real behavior
}

[Fact]
public void Should_Handle_Null_Values_In_Mapping()
{
    // Test null handling in mappings using real mapper instance
}
```

### Important: Never Mock Mappers
- **Critical Rule**: Always use real mapper instances in tests - NEVER create mock objects for mappers
- **Reasoning**: Mappers (especially Riok.Mapperly generated ones) contain actual business logic that needs to be tested
- **Why mocking is wrong**: Mocking mappers defeats the purpose of testing the actual mapping behavior and can hide mapping configuration errors
- **Correct approach**: Instantiate the real mapper class and test its actual behavior to ensure mapping logic works correctly

## API Testing with HTTP Files

### Directory Structure
```
tests/api/
├── http-client.env.json
├── auth/
│   └── auth.http
├── users/
│   └── users.http
├── exercises/
│   └── exercises.http
└── README.md
```

### Environment Configuration
```json
{
  "dev": {
    "baseUrl": "https://localhost:7001",
    "adminEmail": "admin@dotfitness.com",
    "testUserEmail": "test.user@example.com"
  },
  "prod": {
    "baseUrl": "https://your-api.azurewebsites.net"
  }
}
```

### HTTP Test Pattern
```http
### Test Description
POST {{baseUrl}}/api/v1/endpoint
Authorization: Bearer {{authToken}}
Content-Type: application/json

{
  "property": "value"
}

> {%
  client.global.set("variableName", response.body.property);
%}
```

## Test Execution Order

### Unit Tests
1. Domain entity tests
2. Validator tests
3. Mapper tests
4. Repository tests (integration)
5. Handler tests (command/query)

### API Tests
1. Authentication (auth.http)
2. Users module (foundational)
3. Exercises module
4. Routines module (depends on exercises)
5. WorkoutLogs module (depends on routines and exercises)

## Assertions and Best Practices

### FluentAssertions Usage
```csharp
// Preferred assertions with manually defined test DateTime values
result.Should().NotBeNull();
result.IsSuccess.Should().BeTrue();
result.Value.Should().BeEquivalentTo(expected);

// Use predefined test DateTime instead of DateTime.UtcNow for exact comparisons
var testDateTime = new DateTime(2024, 1, 15, 10, 30, 0, DateTimeKind.Utc);
user.CreatedAt.Should().Be(testDateTime);
user.UpdatedAt.Should().Be(testDateTime);

// Only use BeCloseTo for scenarios where exact timing cannot be controlled
user.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
```

### Mock Verification
```csharp
// Verify method calls
_repositoryMock.Verify(x => x.CreateAsync(It.IsAny<User>()), Times.Once);

// Verify logging
_loggerMock.Verify(
    x => x.Log(
        LogLevel.Information,
        It.IsAny<EventId>(),
        It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("User created")),
        It.IsAny<Exception>(),
        It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
    Times.Once);
```

### Mock Setup with Default Parameters
```csharp
// When mocking methods with default parameters, include ALL parameters
// ❌ WRONG - Avoiding default parameters
_repositoryMock.Setup(x => x.FindAsync(It.IsAny<string>()))
    .ReturnsAsync(Result.Success(user));

// ✅ CORRECT - Include default parameters explicitly
_repositoryMock.Setup(x => x.FindAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
    .ReturnsAsync(Result.Success(user));

// For methods with multiple default parameters
_serviceMock.Setup(x => x.ProcessAsync(
    It.IsAny<string>(), 
    It.IsAny<bool>(), 
    It.IsAny<int>(), 
    It.IsAny<CancellationToken>()))
    .ReturnsAsync(Result.Success());
```

## Test Data Management

### Test Data Builder Pattern
```csharp
public static class TestDataBuilder
{
    // Use manually defined UTC DateTime values to avoid flaky tests
    public static readonly DateTime TestDateTime = new DateTime(2024, 1, 15, 10, 30, 0, DateTimeKind.Utc);
    public static readonly DateTime TestDateTimeUpdated = new DateTime(2024, 1, 15, 11, 0, 0, DateTimeKind.Utc);
    
    public static User CreateValidUser(string email = "test@example.com") =>
        new User
        {
            Email = email,
            DisplayName = "Test User",
            GoogleId = "google123",
            CreatedAt = TestDateTime,
            UpdatedAt = TestDateTime
        };
        
    public static UserMetric CreateValidUserMetric(string userId = "user123") =>
        new UserMetric
        {
            UserId = userId,
            Date = TestDateTime,
            Weight = 70.5,
            Height = 175.0,
            CreatedAt = TestDateTime,
            UpdatedAt = TestDateTime
        };
}
```

### Test Constants
```csharp
public static class TestConstants
{
    public const string ValidEmail = "test@example.com";
    public const string AdminEmail = "admin@dotfitness.com";
    public const string ValidGoogleId = "google123";
    
    // Predefined UTC DateTime values for consistent testing
    public static readonly DateTime BaseTestDateTime = new DateTime(2024, 1, 15, 10, 30, 0, DateTimeKind.Utc);
    public static readonly DateTime UpdatedTestDateTime = new DateTime(2024, 1, 15, 11, 0, 0, DateTimeKind.Utc);
    public static readonly DateTime FutureTestDateTime = new DateTime(2024, 2, 15, 10, 30, 0, DateTimeKind.Utc);
}
```

## Performance Considerations

### Test Isolation
- Use `MongoDbFixture` for shared container instance
- Create fresh database per test class
- Clean up data after each test

### Async Testing
```csharp
[Fact]
public async Task Should_Handle_Async_Operations()
{
    // Always use async/await for async operations
    var result = await _handler.Handle(command, CancellationToken.None);
    result.Should().NotBeNull();
}
```

### MongoDB UTC Timestamp Requirements
```csharp
[Fact]
public async Task Should_Create_Entity_With_UTC_Timestamps()
{
    // Arrange - Use manually defined UTC DateTime to avoid flaky tests
    var testDateTime = new DateTime(2024, 1, 15, 10, 30, 0, DateTimeKind.Utc);
    var entity = new Entity
    {
        // Use manually defined UTC DateTime values in tests
        CreatedAt = testDateTime,
        UpdatedAt = testDateTime
    };

    // Act
    var result = await _repository.CreateAsync(entity);

    // Assert
    result.IsSuccess.Should().BeTrue();
    result.Value.CreatedAt.Kind.Should().Be(DateTimeKind.Utc);
    result.Value.UpdatedAt.Kind.Should().Be(DateTimeKind.Utc);
    result.Value.CreatedAt.Should().Be(testDateTime);
    result.Value.UpdatedAt.Should().Be(testDateTime);
}
```

### Important: Always Use UTC for MongoDB
- **Critical Database Requirement**: All DateTime values stored in MongoDB must be in UTC format to prevent timezone-related issues
- **Test Data Strategy**: Use manually defined UTC DateTime values in tests to avoid flaky tests caused by timing variations
- **Why avoid DateTime.UtcNow in tests**: Using `DateTime.UtcNow` can cause timing-related test failures due to microsecond differences between test execution
- **Assertion Strategy**: Test assertions should verify exact UTC timestamps using predefined test values
- **Entity Design**: Entity constructors should default to UTC timestamps to ensure consistency across the application

## Error Testing

### Exception Handling Tests
```csharp
[Fact]
public async Task Should_Handle_Database_Connection_Errors()
{
    // Arrange
    _repositoryMock.Setup(x => x.CreateAsync(It.IsAny<User>()))
        .ThrowsAsync(new MongoException("Connection failed"));

    // Act & Assert
    var result = await _handler.Handle(command, CancellationToken.None);
    result.IsFailure.Should().BeTrue();
    result.Error.Should().Contain("Connection failed");
}
```

## Coverage Targets

- **Domain Layer**: 100% coverage (business logic critical)
- **Application Layer**: 95% coverage (validation and mapping)
- **Infrastructure Layer**: 85% coverage (repository and handlers)
- **API Layer**: 80% coverage (controller actions)

## Test Execution Commands

```bash
# Run all tests
dotnet test

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test class
dotnet test --filter "ClassName=UserTests"

# Run tests for specific module
dotnet test dotFitness.Modules.Users.Tests/
```

## Key Guidelines for AI Code Generation

When GitHub Copilot generates test code for this project, it must follow these essential principles to maintain code quality and consistency:

1. **Always follow the established patterns** shown in existing test files - Examine the current test implementations and replicate their structure, naming, and organization
2. **Use descriptive test names** that clearly indicate what is being tested - Test method names should be self-documenting and explain both the expected behavior and the conditions under which it occurs
3. **Include comprehensive test coverage** for happy path, edge cases, and error scenarios - Every test class should cover successful operations, boundary conditions, invalid inputs, and failure scenarios
4. **Mock external dependencies** appropriately using Moq - Only mock interfaces and external services, never mock the classes being tested or mappers
5. **Use FluentAssertions** for readable and maintainable assertions - Prefer FluentAssertions over basic Assert methods for better error messages and readability
6. **Implement proper test isolation** with fresh database instances - Each test should run independently without affecting other tests
7. **Test business logic thoroughly** in domain entities - Domain layer tests should focus on business rules, calculations, and entity behavior
8. **Validate error handling** and logging in all layers - Ensure that exceptions are properly caught, logged, and converted to appropriate error responses
9. **Maintain consistent test structure** across all modules - All modules should follow the same testing patterns and organization for maintainability
10. **Include performance considerations** in integration tests - Repository and handler tests should validate that operations complete within reasonable timeframes
11. **NEVER mock mappers** - Always use real mapper instances to test actual mapping behavior and catch configuration errors
12. **Always specify UTC for MongoDB operations** - Use manually defined UTC DateTime values in tests to avoid flaky tests and ensure consistent data handling
13. **Never avoid default parameters in mocks** - Always include all parameters when setting up or verifying mocks to ensure proper method signature matching

This document serves as the definitive guide for maintaining consistency and quality in the dotFitness testing suite.

## Critical Testing Rules

### 🚫 NEVER Mock Mappers
```csharp
// ❌ WRONG - Don't do this
var mapperMock = new Mock<IUserMapper>();
mapperMock.Setup(x => x.ToDto(It.IsAny<User>())).Returns(new UserDto());

// ✅ CORRECT - Use real mapper instance
var mapper = new UserMapper(); // Real Riok.Mapperly generated mapper
var dto = mapper.ToDto(user);
```

### 🕐 ALWAYS Use UTC for MongoDB
```csharp
// ❌ WRONG - Local time
var user = new User 
{ 
    CreatedAt = DateTime.Now,  // Local time zone
    UpdatedAt = DateTime.Now 
};

// ❌ WRONG - DateTime.UtcNow in tests (causes flaky tests)
var user = new User
{
    CreatedAt = DateTime.UtcNow,  // Can cause timing-related test failures
    UpdatedAt = DateTime.UtcNow
};

// ✅ CORRECT - Manually defined UTC time in tests
var testDateTime = new DateTime(2024, 1, 15, 10, 30, 0, DateTimeKind.Utc);
var user = new User 
{ 
    CreatedAt = testDateTime,  // Predictable, consistent test data
    UpdatedAt = testDateTime 
};

// ✅ CORRECT - Test UTC verification with exact values
user.CreatedAt.Kind.Should().Be(DateTimeKind.Utc);
user.CreatedAt.Should().Be(testDateTime);
user.UpdatedAt.Should().Be(testDateTime);
```

These rules are non-negotiable and must be followed in all test implementations.

## Mock Setup Best Practices

### 🎯 ALWAYS Include Default Parameters
```csharp
// ❌ WRONG - Avoiding default parameters
_repositoryMock.Setup(x => x.FindAsync(It.IsAny<string>()))
    .ReturnsAsync(Result.Success(user));

// ✅ CORRECT - Include default parameters explicitly
_repositoryMock.Setup(x => x.FindAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
    .ReturnsAsync(Result.Success(user));

// For methods with multiple default parameters
_serviceMock.Setup(x => x.ProcessAsync(
    It.IsAny<string>(), 
    It.IsAny<bool>(), 
    It.IsAny<int>(), 
    It.IsAny<CancellationToken>()))
    .ReturnsAsync(Result.Success());
```

### Why This Matters
- **Method Signature Matching**: Moq needs exact parameter matching for proper setup and verification
- **Test Reliability**: Avoiding default parameters can cause setup failures and unpredictable test behavior
- **Future-Proofing**: Explicit parameters prevent issues when method signatures change or evolve
- **Clear Intent**: Shows exactly which parameters the test is concerned with and makes test expectations explicit
