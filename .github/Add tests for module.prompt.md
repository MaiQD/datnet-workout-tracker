# Add Comprehensive Tests for dotFitness Module - GitHub Copilot Instructions

## Overview
This prompt guides GitHub Copilot to create comprehensive test suites for dotFitness modules following established testing patterns, ensuring reliability, maintainability, and consistent quality.

## When to Use This Prompt
Use this prompt when you need to add complete test coverage for an existing dotFitness module or enhance existing tests to meet quality standards.

## Testing Architecture Context
- **Framework**: xUnit with FluentAssertions and Moq
- **Integration Testing**: Testcontainers.MongoDb for repository tests
- **API Testing**: HTTP files with REST Client extension
- **Patterns**: AAA (Arrange-Act-Assert), Test Data Builders, Result Pattern testing
- **Coverage Targets**: Domain 100%, Application 95%, Infrastructure 85%, API 80%

## Test Project Structure

Create the following comprehensive test structure for the module:

```
dotFitness.Modules.[ModuleName].Tests/
├── Domain/
│   └── Entities/
│       ├── [Entity]Tests.cs           # Comprehensive entity tests
│       └── [Entity]SimpleTests.cs     # Basic validation tests
├── Application/
│   ├── Validators/
│   │   ├── Create[Entity]CommandValidatorTests.cs
│   │   ├── Update[Entity]CommandValidatorTests.cs
│   │   └── Delete[Entity]CommandValidatorTests.cs
│   └── Mappers/
│       └── [Entity]MapperTests.cs     # Real mapper instance tests
├── Infrastructure/
│   ├── Handlers/
│   │   ├── Create[Entity]CommandHandlerTests.cs
│   │   ├── Update[Entity]CommandHandlerTests.cs
│   │   ├── Delete[Entity]CommandHandlerTests.cs
│   │   ├── Get[Entity]ByIdQueryHandlerTests.cs
│   │   └── GetAll[Entities]QueryHandlerTests.cs
│   └── MongoDB/
│       ├── [Entity]RepositoryTests.cs # Integration tests with Testcontainers
│       └── MongoDbFixture.cs          # Shared test fixture
└── TestUtilities/
    ├── TestDataBuilder.cs             # Test data creation utilities
    ├── TestConstants.cs               # Shared test constants
    └── TestExtensions.cs              # Helper extension methods
```

## Required Test Dependencies

Add these NuGet packages to the test project:

```xml
<PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.8.0" />
<PackageReference Include="xunit" Version="2.5.3" />
<PackageReference Include="xunit.runner.visualstudio" Version="2.5.3" />
<PackageReference Include="FluentAssertions" Version="7.0.0" />
<PackageReference Include="Moq" Version="4.20.72" />
<PackageReference Include="Microsoft.Extensions.Logging.Abstractions" Version="9.0.5" />
<PackageReference Include="Microsoft.Extensions.Options" Version="9.0.5" />
<PackageReference Include="Testcontainers.MongoDb" Version="4.1.0" />
<PackageReference Include="MongoDB.Driver" Version="3.4.0" />
<PackageReference Include="MediatR" Version="12.5.0" />
<PackageReference Include="FluentValidation" Version="12.0.0" />
<PackageReference Include="FluentValidation.TestHelper" Version="12.0.0" />
```

## Domain Entity Tests Implementation

### Entity Tests Structure
```csharp
public class [Entity]Tests
{
    [Fact]
    public void Should_Create_Valid_[Entity]_With_Required_Properties()
    {
        // Arrange - Use manually defined UTC DateTime to avoid flaky tests
        var testDateTime = new DateTime(2024, 1, 15, 10, 30, 0, DateTimeKind.Utc);
        
        // Act
        var entity = new [Entity]
        {
            // Set required properties with test data
            Name = "Test [Entity]",
            // Other required properties
        };

        // Assert
        entity.Id.Should().NotBeNullOrEmpty();
        entity.Name.Should().Be("Test [Entity]");
        entity.CreatedAt.Kind.Should().Be(DateTimeKind.Utc);
        entity.UpdatedAt.Kind.Should().Be(DateTimeKind.Utc);
        entity.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Should_Calculate_Derived_Properties_Correctly()
    {
        // Test business logic calculations
        // Example: BMI calculation, status derivation, etc.
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void Should_Validate_Required_Properties(string invalidValue)
    {
        // Test validation rules for required properties
    }

    [Fact]
    public void Should_Update_ModifiedAt_When_Property_Changes()
    {
        // Arrange
        var testDateTime = new DateTime(2024, 1, 15, 10, 30, 0, DateTimeKind.Utc);
        var entity = TestDataBuilder.CreateValid[Entity]();
        var originalUpdatedAt = entity.UpdatedAt;

        // Act
        entity.Name = "Updated Name";

        // Assert
        entity.UpdatedAt.Should().BeOnOrAfter(originalUpdatedAt);
    }
}
```

### Business Logic Method Tests
```csharp
[Fact]
public void Should_Execute_Business_Method_Successfully()
{
    // Test entity business methods
    // Example: Add/Remove operations, status changes, calculations
}

[Fact]
public void Should_Enforce_Business_Rules()
{
    // Test business rule enforcement
    // Example: Prevent duplicate additions, validate state transitions
}
```

## Repository Integration Tests Implementation

### MongoDB Test Fixture
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
        _database.GetCollection<[Entity]>("[entityCollection]");
        _loggerMock = new Mock<ILogger<[Entity]Repository>>();
        _repository = new [Entity]Repository(_database, _loggerMock.Object);
    }

    public async Task DisposeAsync()
    {
        await fixture.CleanupDatabaseAsync();
    }

    [Fact]
    public async Task Should_Create_[Entity]_Successfully()
    {
        // Arrange
        var entity = TestDataBuilder.CreateValid[Entity]();

        // Act
        var result = await _repository.CreateAsync(entity, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Id.Should().NotBeNullOrEmpty();
        result.Value.CreatedAt.Kind.Should().Be(DateTimeKind.Utc);
    }

    [Fact]
    public async Task Should_Retrieve_[Entity]_By_Id()
    {
        // Arrange
        var entity = TestDataBuilder.CreateValid[Entity]();
        await _repository.CreateAsync(entity, CancellationToken.None);

        // Act
        var result = await _repository.GetByIdAsync(entity.Id, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Id.Should().Be(entity.Id);
    }

    [Fact]
    public async Task Should_Update_[Entity]_Successfully()
    {
        // Test update operations
    }

    [Fact]
    public async Task Should_Delete_[Entity]_Successfully()
    {
        // Test delete operations
    }

    [Fact]
    public async Task Should_Return_NotFound_For_NonExistent_[Entity]()
    {
        // Arrange
        var nonExistentId = "507f1f77bcf86cd799439011";

        // Act
        var result = await _repository.GetByIdAsync(nonExistentId, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("not found");
    }
}
```

## Command Handler Tests Implementation

### Handler Test Structure
```csharp
public class Create[Entity]CommandHandlerTests
{
    private readonly Mock<I[Entity]Repository> _repositoryMock;
    private readonly Mock<ILogger<Create[Entity]CommandHandler>> _loggerMock;
    private readonly Mock<IOptions<[ModuleName]Settings>> _settingsMock;
    private readonly Create[Entity]CommandHandler _handler;

    public Create[Entity]CommandHandlerTests()
    {
        _repositoryMock = new Mock<I[Entity]Repository>();
        _loggerMock = new Mock<ILogger<Create[Entity]CommandHandler>>();
        _settingsMock = new Mock<IOptions<[ModuleName]Settings>>();
        
        // Setup settings mock
        _settingsMock.Setup(x => x.Value)
            .Returns(new [ModuleName]Settings 
            { 
                // Initialize settings
            });

        _handler = new Create[Entity]CommandHandler(
            _repositoryMock.Object,
            _loggerMock.Object,
            _settingsMock.Object
        );
    }

    [Fact]
    public async Task Should_Handle_Valid_Command_Successfully()
    {
        // Arrange
        var command = new Create[Entity]Command(
            Name: "Test [Entity]",
            // Other required properties
        );

        var entity = TestDataBuilder.CreateValid[Entity]();
        _repositoryMock.Setup(x => x.CreateAsync(It.IsAny<[Entity]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(entity));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        
        _repositoryMock.Verify(x => x.CreateAsync(
            It.Is<[Entity]>(e => e.Name == command.Name), 
            It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Fact]
    public async Task Should_Return_ValidationError_For_Invalid_Command()
    {
        // Test validation failure scenarios
    }

    [Fact]
    public async Task Should_Handle_Repository_Errors_Gracefully()
    {
        // Arrange
        var command = new Create[Entity]Command(
            Name: "Test [Entity]"
        );

        _repositoryMock.Setup(x => x.CreateAsync(It.IsAny<[Entity]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure("Database error"));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Database error");
    }
}
```

## Query Handler Tests Implementation

```csharp
public class Get[Entity]ByIdQueryHandlerTests
{
    private readonly Mock<I[Entity]Repository> _repositoryMock;
    private readonly Mock<ILogger<Get[Entity]ByIdQueryHandler>> _loggerMock;
    private readonly Get[Entity]ByIdQueryHandler _handler;

    public Get[Entity]ByIdQueryHandlerTests()
    {
        _repositoryMock = new Mock<I[Entity]Repository>();
        _loggerMock = new Mock<ILogger<Get[Entity]ByIdQueryHandler>>();
        
        _handler = new Get[Entity]ByIdQueryHandler(
            _repositoryMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task Should_Return_[Entity]_When_Found()
    {
        // Arrange
        var entityId = "507f1f77bcf86cd799439011";
        var entity = TestDataBuilder.CreateValid[Entity]();
        
        _repositoryMock.Setup(x => x.GetByIdAsync(entityId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(entity));

        var query = new Get[Entity]ByIdQuery(entityId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Id.Should().Be(entity.Id);
    }

    [Fact]
    public async Task Should_Return_NotFound_When_[Entity]_DoesNot_Exist()
    {
        // Test not found scenarios
    }
}
```

## Validator Tests Implementation

```csharp
public class Create[Entity]CommandValidatorTests
{
    private readonly Create[Entity]CommandValidator _validator;

    public Create[Entity]CommandValidatorTests()
    {
        _validator = new Create[Entity]CommandValidator();
    }

    [Fact]
    public void Should_Pass_Validation_For_Valid_Command()
    {
        // Arrange
        var command = new Create[Entity]Command(
            Name: "Valid [Entity] Name",
            // Other valid properties
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void Should_Fail_Validation_For_Invalid_Name(string invalidName)
    {
        // Arrange
        var command = new Create[Entity]Command(
            Name: invalidName
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Should_Have_Validation_Error_For_Name_Too_Long()
    {
        // Test length validation
    }
}
```

## Mapper Tests Implementation

```csharp
public class [Entity]MapperTests
{
    private readonly [Entity]Mapper _mapper;

    public [Entity]MapperTests()
    {
        // NEVER mock the mapper - use real mapper instance
        _mapper = new [Entity]Mapper();
    }

    [Fact]
    public void Should_Map_Entity_To_Dto_Correctly()
    {
        // Arrange
        var entity = TestDataBuilder.CreateValid[Entity]();

        // Act
        var dto = _mapper.ToDto(entity);

        // Assert
        dto.Should().NotBeNull();
        dto.Id.Should().Be(entity.Id);
        dto.Name.Should().Be(entity.Name);
        dto.CreatedAt.Should().Be(entity.CreatedAt);
    }

    [Fact]
    public void Should_Map_Dto_To_Entity_Correctly()
    {
        // Test DTO → entity mapping using real mapper instance
    }

    [Fact]
    public void Should_Handle_Null_Values_In_Mapping()
    {
        // Test null handling in mappings using real mapper instance
    }
}
```

## Test Data Builder Implementation

```csharp
public static class TestDataBuilder
{
    // Use manually defined UTC DateTime values to avoid flaky tests
    public static readonly DateTime TestDateTime = new DateTime(2024, 1, 15, 10, 30, 0, DateTimeKind.Utc);
    public static readonly DateTime TestDateTimeUpdated = new DateTime(2024, 1, 15, 11, 0, 0, DateTimeKind.Utc);
    
    public static [Entity] CreateValid[Entity](
        string name = "Test [Entity]",
        string? customProperty = null)
    {
        return new [Entity]
        {
            Name = name,
            // Set other properties with test values
            CreatedAt = TestDateTime,
            UpdatedAt = TestDateTime
        };
    }
    
    public static Create[Entity]Command CreateValid[Entity]Command(
        string name = "Test [Entity]")
    {
        return new Create[Entity]Command(
            Name: name
            // Other command properties
        );
    }

    public static [Entity]Dto CreateValid[Entity]Dto(
        string name = "Test [Entity]")
    {
        return new [Entity]Dto
        {
            Id = "507f1f77bcf86cd799439011",
            Name = name,
            CreatedAt = TestDateTime,
            UpdatedAt = TestDateTime
        };
    }
}
```

## Test Constants Implementation

```csharp
public static class TestConstants
{
    public const string ValidName = "Test [Entity]";
    public const string ValidId = "507f1f77bcf86cd799439011";
    
    // Predefined UTC DateTime values for consistent testing
    public static readonly DateTime BaseTestDateTime = new DateTime(2024, 1, 15, 10, 30, 0, DateTimeKind.Utc);
    public static readonly DateTime UpdatedTestDateTime = new DateTime(2024, 1, 15, 11, 0, 0, DateTimeKind.Utc);
    public static readonly DateTime FutureTestDateTime = new DateTime(2024, 2, 15, 10, 30, 0, DateTimeKind.Utc);
}
```

## API Testing with HTTP Files

Create comprehensive HTTP test files in `tests/api/[modulename]/[modulename].http`:

```http
### Get All [Entities]
GET {{baseUrl}}/api/v1/[entities]
Authorization: Bearer {{authToken}}

### Create [Entity]
POST {{baseUrl}}/api/v1/[entities]
Authorization: Bearer {{authToken}}
Content-Type: application/json

{
  "name": "Test [Entity]",
  "description": "A test [entity] for API testing"
}

> {%
  client.global.set("[entity]Id", response.body.id);
%}

### Get [Entity] by ID
GET {{baseUrl}}/api/v1/[entities]/{{[entity]Id}}
Authorization: Bearer {{authToken}}

### Update [Entity]
PUT {{baseUrl}}/api/v1/[entities]/{{[entity]Id}}
Authorization: Bearer {{authToken}}
Content-Type: application/json

{
  "name": "Updated Test [Entity]",
  "description": "Updated description"
}

### Delete [Entity]
DELETE {{baseUrl}}/api/v1/[entities]/{{[entity]Id}}
Authorization: Bearer {{authToken}}

### Test Error Scenarios
POST {{baseUrl}}/api/v1/[entities]
Authorization: Bearer {{authToken}}
Content-Type: application/json

{
  "name": "",
  "description": null
}
```

## Critical Testing Rules

### 🚫 NEVER Mock Mappers
- **Rule**: Always use real mapper instances in tests
- **Reason**: Mappers contain business logic that needs to be tested
- **Implementation**: Instantiate actual mapper classes, never create Mock objects

### 🕐 ALWAYS Use UTC for MongoDB
- **Rule**: Use manually defined UTC DateTime values in tests
- **Reason**: Prevents flaky tests caused by timing variations
- **Implementation**: Define test DateTime constants, avoid DateTime.UtcNow in tests

### 🎯 ALWAYS Include Default Parameters in Mocks
- **Rule**: Include all parameters when setting up or verifying mocks
- **Reason**: Ensures proper method signature matching
- **Implementation**: Use It.IsAny<CancellationToken>() for default parameters

## Test Execution Commands

```bash
# Run all tests for the module
dotnet test dotFitness.Modules.[ModuleName].Tests/

# Run with coverage
dotnet test dotFitness.Modules.[ModuleName].Tests/ --collect:"XPlat Code Coverage"

# Run specific test class
dotnet test --filter "ClassName=[Entity]Tests"

# Run all domain tests
dotnet test --filter "FullyQualifiedName~Domain"

# Run all integration tests
dotnet test --filter "FullyQualifiedName~MongoDB"
```

## Validation Checklist

After implementing tests, verify:

- [ ] All test projects compile and run successfully
- [ ] Domain layer tests achieve 100% coverage
- [ ] Application layer tests achieve 95% coverage  
- [ ] Infrastructure layer tests achieve 85% coverage
- [ ] All tests follow AAA pattern consistently
- [ ] Test names clearly describe behavior and conditions
- [ ] UTC DateTime values are used consistently
- [ ] Real mapper instances are used (no mocks)
- [ ] Default parameters are included in all mock setups
- [ ] Integration tests use Testcontainers properly
- [ ] HTTP tests cover all API endpoints
- [ ] Error scenarios are thoroughly tested
- [ ] Test data builders provide consistent test data
- [ ] All tests are isolated and don't affect each other

This comprehensive testing approach ensures high-quality, maintainable test suites that provide confidence in the dotFitness application's reliability and correctness.
