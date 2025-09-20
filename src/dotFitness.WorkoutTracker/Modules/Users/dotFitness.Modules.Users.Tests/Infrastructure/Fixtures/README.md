# Test Fixtures for Module Testing

This directory contains test fixtures for the Users module. The fixtures are designed to be reusable and configurable across different modules.

## Architecture

### BaseUnitTestFixture
A generic base fixture that can be configured with custom options for different modules.

**Features:**
- Configurable DbContext options via constructor parameter
- In-memory database support
- Automatic warning suppression for common test scenarios
- Sensitive data logging for debugging

### UsersUnitTestFixture
Module-specific fixture that extends `BaseUnitTestFixture` with Users-specific configuration.

**Features:**
- Inherits all base functionality
- Provides `CreateUsersDbContext()` convenience method
- Can be extended with Users-specific configuration

### UsersPostgresSqlFixture
Module-specific PostgreSQL fixture for integration tests with proper schema configuration.

**Features:**
- Extends the base `PostgresSqlFixture`
- Automatically configures the "users" schema for migrations
- Provides `CreateUsersDbContext()` and `CreateFreshUsersDbContext()` convenience methods
- Uses module-specific collection definition

## Usage

### For Unit Tests (In-Memory Database)

```csharp
public class MyHandlerTests : IAsyncLifetime
{
    private readonly UsersUnitTestFixture _fixture = new();
    private UsersDbContext _context = null!;

    public async Task InitializeAsync()
    {
        _context = _fixture.CreateUsersDbContext();
        await _context.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await _context.DisposeAsync();
    }

    [Fact]
    public async Task Should_Do_Something()
    {
        // Arrange
        var user = new User { /* ... */ };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act & Assert
        // ...
    }
}
```

### For Integration Tests (PostgreSQL)

```csharp
[Collection("UsersPostgreSQL.Shared")]
public class MyIntegrationTests(UsersPostgresSqlFixture fixture) : IAsyncLifetime
{
    private readonly UsersPostgresSqlFixture _fixture = fixture;
    private UsersDbContext _context = null!;

    public async Task InitializeAsync()
    {
        await _fixture.InitializeAsync();
        _context = _fixture.CreateFreshUsersDbContext();
        await _context.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await _context.DisposeAsync();
    }

    [Fact]
    public async Task Should_Do_Something()
    {
        // Integration test logic
    }
}
```

## Creating Fixtures for Other Modules

### Step 1: Create Module-Specific Fixture

```csharp
// Modules/Exercises/dotFitness.Modules.Exercises.Tests/Infrastructure/Fixtures/ExercisesUnitTestFixture.cs
using Microsoft.EntityFrameworkCore;
using dotFitness.Modules.Exercises.Infrastructure.Data;

namespace dotFitness.Modules.Exercises.Tests.Infrastructure.Fixtures;

public class ExercisesUnitTestFixture : BaseUnitTestFixture
{
    public ExercisesUnitTestFixture() : base(ConfigureExercisesOptions)
    {
    }

    private static void ConfigureExercisesOptions(DbContextOptionsBuilder optionsBuilder)
    {
        // Add Exercises-specific configuration here
        // For example, if we need to configure specific providers or options
    }

    public ExercisesDbContext CreateExercisesDbContext()
    {
        return CreateInMemoryDbContext<ExercisesDbContext>();
    }
}

[CollectionDefinition("ExercisesUnitTests")]
public class ExercisesUnitTestCollectionFixture : ICollectionFixture<ExercisesUnitTestFixture> { }
```

### Step 2: Copy BaseUnitTestFixture

Copy the `BaseUnitTestFixture.cs` file to the new module's test project.

### Step 3: Update Test Classes

Update all test classes to use the new module-specific fixture:

```csharp
// Change from:
using dotFitness.SharedKernel.Tests.PostgreSQL;
private readonly UnitTestFixture _fixture = new();

// To:
using dotFitness.Modules.Exercises.Tests.Infrastructure.Fixtures;
private readonly ExercisesUnitTestFixture _fixture = new();
```

## Benefits

1. **Module Isolation**: Each module has its own test fixtures
2. **Reusability**: Base fixture can be reused across modules
3. **Configurability**: Easy to add module-specific configuration
4. **Maintainability**: Clear separation of concerns
5. **No Cross-Module Dependencies**: Tests don't depend on shared kernel test infrastructure

## Best Practices

1. **Use In-Memory for Unit Tests**: Fast, isolated, no external dependencies
2. **Use PostgreSQL for Integration Tests**: Real database behavior, proper isolation
3. **Configure Warnings Appropriately**: Suppress expected warnings, keep important ones
4. **Use Collections for Integration Tests**: Share expensive resources like database containers
5. **Don't Use Collections for Unit Tests**: Each test should be completely isolated
