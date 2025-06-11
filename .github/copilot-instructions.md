This is a .NET 8 workout tracker application built with Clean Architecture, CQRS pattern using MediatR, and MongoDB database with UTC timestamps.

The project follows a modular monolith pattern where each module (Users, Exercises, Routines, WorkoutLogs) has its own Domain, Application, Infrastructure layers and comprehensive test project.

Use xUnit, FluentAssertions, and Moq for testing. Repository integration tests use Testcontainers.MongoDb with fresh database instances per test class.

All test methods must follow the naming convention: Should_[ExpectedBehavior]_[UnderCondition] and use the Arrange-Act-Assert pattern.

NEVER mock mappers - always use real Riok.Mapperly mapper instances in tests to validate actual mapping behavior.

Always use manually defined UTC DateTime values in tests instead of DateTime.UtcNow to avoid flaky timing-related test failures.

When setting up mocks, include ALL method parameters including default parameters like CancellationToken to ensure proper method signature matching.

All operations return Result<T> pattern from SharedKernel instead of throwing exceptions for business errors.

For MongoDB operations, entities must implement IEntity interface and use UTC timestamps in CreatedAt/UpdatedAt properties.

Use comprehensive logging in all handlers and validate error handling with Result pattern throughout all layers.