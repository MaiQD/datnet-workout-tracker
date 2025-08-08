This is a .NET 8 workout tracker application built with Clean Architecture, CQRS (MediatR), and MongoDB (UTC timestamps).

Architecture & modules
- Modular monolith: modules (Users, Exercises, Routines, WorkoutLogs) each have Domain, Application, Infrastructure, and dedicated Tests.
- Clean Architecture: Domain/Application MUST NOT depend on Infrastructure. Use interfaces and DI.
- Single Responsibility: one handler/command/query/validator per file.

Testing
- xUnit + FluentAssertions + Moq. Repository integration tests use Testcontainers.MongoDb.
- Test method names: Should_[ExpectedBehavior]_[UnderCondition]. Use Arrange-Act-Assert.
- NEVER mock Mapperly mappers; use real generated mappers.
- Avoid DateTime.UtcNow in tests; use fixed UTC timestamps.
- Always include all mock parameters (e.g., CancellationToken) for signature matching.
- All operations return Result<T> (no exceptions for expected business errors).

MongoDB
- Entities implement IEntity and use UTC CreatedAt/UpdatedAt.
- Obtain IMongoCollection<T> via DI as singletons (driver is thread-safe).
- Maintain indexes per module:
  - Users: unique Email; GoogleId; Roles; metrics {UserId, Date}.
  - Exercises: scoped unique {UserId, IsGlobal, Name}; MuscleGroups; Equipment; Tags.
  - MuscleGroups: Name (unique per scope); BodyRegion; ParentId.
  - Equipment: scoped unique {Name, IsGlobal, UserId}.
  - Inbox: unique {Consumer, EventId}; {Consumer, Status, OccurredOn}.
  - Outbox: IsProcessed; CreatedAt; EventType.
- Seeders must be idempotent upserts (global muscle groups/equipment seeded in Exercises module).

Auth & Authorization
- JWT-based auth; roles: Admin, PT, User.
- Use policies: AdminOnly, PTOnly, UserOnly, SelfOrAdmin, OwnerUserOrAdmin (PTAssignedToUserOrAdmin when applicable).

Eventing
- Current design: Outbox on producers, Inbox per module on consumers for idempotency. No external broker unless requested.
- Events are DTO payloads and include eventId, eventType, occurredOn, correlationId, traceId.

Conventions
- Keep handlers small; extract complex logic into application services or domain methods.
- No direct cross-module data access; only via interfaces or events.
- Update docs (URS.md, UI_DESIGN.md, TECHNICAL.md, ARCHITECTURE.md) when adding features.
- Branch naming: feature/* with focused commits (what/why).