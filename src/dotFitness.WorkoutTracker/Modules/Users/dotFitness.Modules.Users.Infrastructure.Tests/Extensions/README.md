# TestDataExtensions

This extension provides utility methods for generating unique test data in database tests.

## Usage

```csharp
using dotFitness.Modules.Users.Tests.Infrastructure.Extensions;

public class MyTestClass
{
    [Fact]
    public async Task MyTest()
    {
        // Generate unique email
        var email = this.GenerateUniqueEmail();
        
        // Generate unique date (fixed base date to avoid flaky tests)
        var date = this.GenerateUniqueDate();
        
        // Generate unique user ID for non-existent user scenarios
        var userId = this.GenerateUniqueUserId();
        
        // Clear test data if needed (optional)
        this.ClearTestData();
    }
}
```

## Benefits

- **Unique Emails**: Prevents email conflicts in parallel tests
- **Fixed Date Base**: Uses 2024-01-01 as base to avoid UTC timezone flakiness
- **Unique User IDs**: Generates non-existent user IDs for error scenarios
- **Thread-Safe**: Uses static collections to track used values across tests
- **Easy to Use**: Simple extension methods on any test class

## Key Features

- **Non-Flaky Dates**: Uses fixed base date instead of `DateTime.UtcNow` to prevent timezone-related test failures
- **Collision Prevention**: Tracks used values to ensure uniqueness
- **Parallel Safe**: Works correctly with parallel test execution
- **Memory Efficient**: Uses HashSet for O(1) lookup performance
