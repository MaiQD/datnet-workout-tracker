# dotFitness: A Customizable Home Workout Tracker

## 1. Introduction

**dotFitness** is a web-based productivity application designed to empower individuals to stay active and achieve their fitness goals from the comfort of their homes. It provides robust tools for customizing workout experiences, tracking progress, and maintaining motivation with minimal external equipment.

**Target User:** Individuals seeking to maintain an active lifestyle at home, ranging from beginners to those with some fitness experience, who value structured workouts and clear progress visualization.

**Core Value Proposition:** To offer a highly customizable, easy-to-use, and motivating platform for planning, executing, and tracking home-based workouts, providing visual feedback on progress to drive consistency.

## 2. Application Features (Functional Requirements)

**dotFitness** will offer the following key features:

- **User Authentication:**
    - Secure user login via **Google OAuth 2.0** for a seamless onboarding experience.
    - Initial admin account assignment via a configured whitelist of Google emails.
    - First-time onboarding flagged and surfaced on the dashboard.
- **User Profile Management:**
    - Ability to track and update user information: weight, height, and unit preference (metric/imperial).
    - **Progress Visualization:** Historical graphs of weight and BMI changes over time.
- **Custom Exercise Management:**
    - Users can create, view, edit, and delete their own custom exercises.
    - Each exercise will include: Name, Description, **Muscle Groups** (selectable from pre-defined/user-defined list), **Equipment** (selectable from pre-defined/user-defined list), and a **Video Link** for demonstration.
    - Emphasis on bodyweight and limited-equipment exercises suitable for home.
- **Custom Routine Creation & Management:**
    - Users can build personalized workout routines by combining their custom exercises.
    - Each exercise within a routine specifies: desired Sets, Reps (flexible string input), and **Rest Time** (in seconds) between sets.
    - Ability to reorder exercises within a routine.
    - **Smart Routine Suggestions:** Potentially generate quick workouts based on duration, target muscle groups, and available equipment.
    - **Pre-built Home Workout Templates:** Offer curated routines for various home workout scenarios.
- **Workout Logging:**
    - Users can start a routine or an ad-hoc workout session.
    - During a session, users can log actual sets, reps, and weight for each exercise.
    - Integrated **rest timer** that automatically starts after logging a set.
    - Ability to add session-specific notes.
- **Progress Visualization Dashboard:**
    - **Workout Streak** tracking.
    - **Total Workouts Completed** count.
    - **Trend Graphs:** Visualizing progress for specific exercises (e.g., max reps for push-ups over time).
    - **Personal Bests** tracking and display.
    - Summaries of total workout duration or muscle group focus.
- **Motivation & Engagement:**
    - Workout reminders/notifications.
    - Achievements or badges for reaching milestones (e.g., "First 7-day streak").
    - Post-workout summaries with key achievements.
- **Admin Role for Master Data:**
 - **Smart Exercise Suggestions:**
     - Query endpoint returns a list of suggested exercises for the current user, ranked by fit to preferences (focus muscles, available equipment). Initial heuristic is simple scoring; future iterations may consider history and progression.

    - An administrative role will be implemented to manage and populate the global lists of **Muscle Groups** and **Equipment**.
    - Users can select from global lists, and a mechanism will be in place for users to add *their own* private muscle groups or equipment if not in the global list.
 - **Billing & Premium (Future):**
     - Premium plans with Stripe (Checkout, Customer Portal), plan gating via policies, and subscription webhooks.

## 3. Architectural Design

**dotFitness** will be built as a **Modular Monolith** using **ASP.NET Core** for the backend API and **Vue.js** for the frontend user interface, leveraging **MongoDB** as the database.

### 3.1. Architectural Pattern: Modular Monolith

- **Description:** The application is structured into loosely coupled, highly cohesive modules (vertical slices), each owning its domain, application logic, and infrastructure concerns. All modules are deployed as a single, unified ASP.NET Core application.
- **Benefits:**
    - **Clear Separation of Concerns:** Each module focuses on a specific business capability (Users, Exercises, Routines, Workout Logs).
    - **Improved Maintainability:** Changes in one module are less likely to affect others directly.
    - **Scalability:** Allows for independent development and potential future migration to microservices if needed.
    - **Simplified Deployment:** Deployed as a single unit, reducing operational overhead compared to distributed systems.

### 3.2. Project Structure

The Visual Studio solution (`dotFitness.WorkoutTracker.sln`) contains the following projects:

```
dotFitness.WorkoutTracker/
├── dotFitness.Api/                           <-- Web API entry point (controllers, middleware)
├── dotFitness.Bootstrap/                     <-- Composition root (register installers, MediatR, validators)
├── dotFitness.ModuleContracts/               <-- Contracts for installers (IModuleInstaller)
├── dotFitness.SharedKernel/                  <-- Shared components (Results, Outbox, Interfaces, Utilities)
├── Modules/
│   ├── Users/
│   │   ├── dotFitness.Modules.Users.Domain/
│   │   ├── dotFitness.Modules.Users.Application/
│   │   └── dotFitness.Modules.Users.Infrastructure/
│   └── Exercises/ (same pattern)
└── ... Tests per module
```

**Project Dependencies:**
- `dotFitness.Api` → references `dotFitness.Bootstrap` and module `.Application` projects
- `dotFitness.Bootstrap` → references module `.Infrastructure` projects and `dotFitness.ModuleContracts`
- Module `.Infrastructure` → references its `.Application`, `.Domain`, `dotFitness.SharedKernel`, and `dotFitness.ModuleContracts`
- Module `.Application` → references `.Domain` and `dotFitness.SharedKernel`
- Module `.Domain` → references `dotFitness.SharedKernel`

### 3.3. Core Technical Stack

- **Backend Framework:** ASP.NET Core Web API (.NET 8)
- **Frontend Framework:** Vue 3 (Vite)
- **Database:** Hybrid MongoDB + PostgreSQL (see Database Architecture section)
- **Validation:** FluentValidation
- **Mapping:** Mapperly (source generator)
- **Messaging:** MediatR (in-process CQRS)

### 3.4. Module Composition (Bootstrap + Contracts)

- Each module exposes an installer implementing `dotFitness.ModuleContracts.IModuleInstaller` with:
  - `InstallServices(IServiceCollection, IConfiguration)`
  - `ConfigureIndexes(IMongoDatabase)`
  - `SeedData(IMongoDatabase)`
- `dotFitness.Bootstrap.ModuleRegistry`:
  - Registers all installers (explicit list; no reflection)
  - Registers shared Mongo (`IMongoClient`, `IMongoDatabase`)
  - Auto-registers MediatR handlers: `RegisterServicesFromAssemblyContaining<ModuleInstaller>`
  - Auto-registers FluentValidation validators: `AddValidatorsFromAssemblyContaining<ModuleInstaller>`
- API remains “pure”: controllers/middleware only; it calls Bootstrap to compose the app.

## 4. Database Architecture Decision

### 4.1. Architecture Decision: Hybrid MongoDB + PostgreSQL

After comprehensive analysis of the dotFitness project requirements, we have chosen a **hybrid database architecture** that leverages both MongoDB and PostgreSQL, with each database optimized for specific module requirements.

#### 4.1.1. Decision-Making Process

**Initial Analysis:**
- **Current State**: Single MongoDB database for all modules
- **Requirements**: Zero-cost deployment, optimal performance, data integrity
- **Constraints**: Modular monolith architecture, clean module boundaries

**Options Evaluated:**
1. **Full PostgreSQL Migration**: 20-30 days effort, high risk
2. **Keep MongoDB Only**: Suboptimal for user management and relational data
3. **Hybrid Approach**: 3-4 days effort, optimal per use case, zero additional cost

**Decision Criteria:**
- **ACID Compliance**: Critical for user authentication and data integrity
- **Schema Flexibility**: Required for exercise data and workout logs
- **Query Performance**: Optimized for each module's data patterns
- **Migration Effort**: Minimal disruption to existing system
- **Cost Impact**: Maintains zero-cost deployment strategy

#### 4.1.2. Module-to-Database Mapping

**PostgreSQL Modules (Relational Data):**
```
👤 Users Module
├── User profiles (ACID compliance critical)
├── Authentication data (strong consistency required)
├── User metrics (time-series, relational queries)
└── Role management (referential integrity)

📋 Routines Module (Future)
├── Workout routines (structured relationships)
├── Exercise sequences (ordered, relational)
└── Template management (hierarchical data)
```

**MongoDB Modules (Document Data):**
```
💪 Exercises Module
├── Exercise definitions (flexible schema)
├── Muscle groups (hierarchical, flexible)
├── Equipment catalog (semi-structured)
└── Search functionality (text search, aggregations)

📊 WorkoutLogs Module (Future)
├── Workout sessions (semi-structured)
├── Performance data (time-series, flexible)
├── Progress tracking (document-based)
└── Analytics (aggregation-friendly)
```

#### 4.1.3. ACID Properties Analysis

**High ACID Requirements (PostgreSQL):**
- **Atomicity**: User registration, routine creation, password changes
- **Consistency**: Foreign key constraints, business rule enforcement
- **Isolation**: Concurrent user operations, role assignments
- **Durability**: Authentication data, user profiles, critical business data

**Low ACID Requirements (MongoDB):**
- **Single Document Operations**: Exercise creation, workout logging
- **Eventual Consistency**: Exercise catalog updates, search indexing
- **Flexible Schema**: Exercise data evolution, workout log variations
- **Read-Heavy Operations**: Search, analytics, recommendations

#### 4.1.4. Technical Implementation

**Database Configuration:**
```csharp
// dotFitness.Bootstrap/ModuleRegistry.cs
public static void RegisterAllModules(this IServiceCollection services, IConfiguration configuration)
{
    // PostgreSQL for Users and Routines modules
    services.AddDbContext<UsersDbContext>(options =>
        options.UseNpgsql(configuration.GetConnectionString("PostgreSQL")));
    
    // MongoDB for Exercises and WorkoutLogs modules
    services.AddSingleton<IMongoClient>(sp => 
        new MongoClient(configuration.GetConnectionString("MongoDB")));
    services.AddSingleton<IMongoDatabase>(sp => 
        sp.GetRequiredService<IMongoClient>().GetDatabase("dotFitness"));
}
```

**Module-Specific Database Access:**
- **Users Module**: Entity Framework Core with PostgreSQL
- **Exercises Module**: MongoDB Driver (existing implementation)
- **Future Modules**: Database choice based on data characteristics

#### 4.1.5. Zero-Cost Deployment Compatibility

**Free Tier Resources:**
- **MongoDB Atlas M0**: 512MB storage, 100 connections
- **Supabase PostgreSQL**: 0.5GB storage, 100 connections
- **Total Additional Cost**: $0/month

**Storage Capacity Analysis:**
- **MongoDB**: ~170,000 exercises (perfect for exercise catalog)
- **PostgreSQL**: ~250,000 users (perfect for user management)

#### 4.1.6. Migration Strategy

**Phase 1: Setup PostgreSQL (1 day)**
- Create Neon PostgreSQL database
- Update connection strings and configuration
- Add Entity Framework Core packages

**Phase 2: Migrate Users Module (3-4 days)**
- Create UsersDbContext with EF Core
- Convert User entities from BSON to EF Core attributes
- Update UserRepository to use DbContext
- Maintain existing API contracts (no breaking changes)

**Phase 3: Future Modules (As Needed)**
- Routines Module → PostgreSQL (relational data)
- WorkoutLogs Module → MongoDB (flexible, time-series data)

#### 4.1.7. Benefits and Trade-offs

**Benefits:**
- **Optimal Performance**: Each database optimized for its use case
- **Zero Additional Cost**: Both databases have generous free tiers
- **Minimal Migration Effort**: Only migrate 1 module, not 2+
- **Future Flexibility**: Easy to scale each database independently
- **Risk Mitigation**: Gradual migration, not big bang

**Trade-offs:**
- **Operational Complexity**: Two database systems to monitor
- **Development Overhead**: Two ORMs/Drivers to maintain
- **Testing Complexity**: Two different test database setups

**Risk Mitigation:**
- **Module Isolation**: Each module already independent
- **Event-Driven Communication**: Cross-module via events, not direct DB access
- **Repository Pattern**: Database abstraction already in place
- **Comprehensive Testing**: Each module has its own test suite

## 5. Core Technical Patterns & Libraries

### 5.1. CQRS (Command Query Responsibility Segregation)

- **Purpose:** To separate read (Query) and write (Command) operations, optimizing each for its specific responsibility.
- **Implementation:** Leverages **MediatR** as an in-process message dispatcher.
    - Commands encapsulate intent to change state (e.g., `CreateExerciseCommand`).
    - Queries encapsulate intent to retrieve data (e.g., `GetUserRoutinesQuery`).
    - Dedicated handlers (`IRequestHandler`) process each Command/Query.
- **Benefits:** Clearer separation of concerns, independent optimization of read/write paths, improved testability.

### 5.2. Outbox Pattern

- **Purpose:** To ensure atomicity and reliability when performing a database operation and publishing a domain event (e.g., updating a `WorkoutLog` and publishing a `WorkoutCompletedEvent`). Prevents data inconsistency due to "dual-writes."
- **Implementation:**
    - An `outboxMessages` MongoDB collection stores events as part of the same database transaction as the main data change.
    - A background `IHostedService` (Outbox Processor) polls `outboxMessages`, publishes events to a message broker (or directly to consumers internally), and marks them as processed.
- **Benefits:** Guarantees eventual consistency, robustness against application crashes.

### 5.3. Result Pattern

- **Purpose:** To explicitly handle and communicate outcomes (success or failure) of operations without relying solely on throwing exceptions for expected business rule violations or "not found" scenarios.
- **Implementation:** `Result` and `Result<TValue>` objects defined in `dotFitness.SharedKernel`.
    - Methods (especially Command/Query handlers) return `Result<T>` indicating success with a value, or failure with an error message.
- **Benefits:** Explicit error handling, predictable API for consumers, improved readability, reduced exception overhead.

### 5.4. Mapperly

- **Purpose:** High-performance, compile-time code generation for mapping between DTOs (Data Transfer Objects) and domain models.
- **Implementation:** Using Mapperly's source generator capabilities within each module's `Application` project for mapping interfaces.
- **Benefits:** Eliminates runtime reflection overhead (faster), type-safe mapping, reduced boilerplate code.

### 5.5. FluentValidation

- **Purpose:** Provides a fluent and expressive API for defining robust validation rules for input DTOs and Commands.
- **Implementation:** Validator classes (e.g., `CreateExerciseCommandValidator`) are created for commands/queries, integrated with MediatR's pipeline behaviors.
- **Benefits:** Decouples validation logic from business logic, improved testability of validation rules, clear error messages.

### 5.6. Centralized Error Handling

- **Purpose:** To provide consistent and structured error responses from the API, preventing sensitive information leakage and improving client-side error handling.
- **Implementation:** ASP.NET Core's built-in Exception Handling Middleware or custom middleware to catch unhandled exceptions globally and map them to appropriate HTTP status codes and JSON error payloads. This also integrates with the `Result` pattern, mapping `Result.Failure` to HTTP responses.
- **Benefits:** Uniform error format, better user experience, simplified debugging.

### 5.7. API Versioning

- **Purpose:** To allow for controlled evolution of the API by supporting multiple versions simultaneously, preventing breaking changes for existing clients.
- **Implementation:** Using `Microsoft.AspNetCore.Mvc.Versioning` package to version API endpoints (e.g., via URL segments like `/v1/exercises`).
- **Benefits:** Smoother API updates, client compatibility, easier maintenance of long-lived APIs.

### 5.8. Structured Logging with Serilog

- **Purpose:** To provide rich, queryable logs for monitoring application behavior, diagnosing issues, and auditing.
- **Implementation:** Configuring Serilog in the `dotFitness.Api` project, using its structured logging capabilities and various sinks (e.g., Console, File, Azure Application Insights).
- **Benefits:** Easier log analysis, improved observability, faster debugging in production.

### 5.9. Testing (xUnit.net, Moq, FluentAssertions)

- **Purpose:** To ensure the correctness and reliability of the application's logic through automated testing.
- **Implementation:**
    - **xUnit.net:** The primary unit testing framework. Dedicated test projects for each module (`dotFitness.Modules.X.Tests`).
    - **Moq:** A mocking library used to create mock objects for dependencies, isolating the code under test (e.g., mocking `IMongoCollection<T>`, `IMediator`).
    - **FluentAssertions:** An assertion library that provides a highly readable and fluent syntax for asserting test outcomes.
- **Benefits:** High code quality, regression prevention, faster development cycles.

## 6. Database Schema (Hybrid MongoDB + PostgreSQL)

### 6.1. PostgreSQL Schema (Users Module)

**Users Table:**
```sql
CREATE TABLE users (
  id SERIAL PRIMARY KEY,
  google_id VARCHAR(255) UNIQUE,
  email VARCHAR(255) UNIQUE NOT NULL,
  display_name VARCHAR(255) NOT NULL,
  profile_picture VARCHAR(500),
  login_method VARCHAR(50) DEFAULT 'google',
  roles TEXT[] DEFAULT ARRAY['User'],
  gender VARCHAR(20),
  date_of_birth DATE,
  unit_preference VARCHAR(10) DEFAULT 'metric',
  is_onboarded BOOLEAN DEFAULT FALSE,
  onboarding_completed_at TIMESTAMP,
  available_equipment_ids INTEGER[],
  focus_muscle_group_ids INTEGER[],
  created_at TIMESTAMP DEFAULT NOW(),
  updated_at TIMESTAMP DEFAULT NOW()
);
```

**User Metrics Table:**
```sql
CREATE TABLE user_metrics (
  id SERIAL PRIMARY KEY,
  user_id INTEGER REFERENCES users(id) ON DELETE CASCADE,
  date DATE NOT NULL,
  weight DECIMAL(5,2),
  height DECIMAL(5,2),
  bmi DECIMAL(4,1),
  notes TEXT,
  created_at TIMESTAMP DEFAULT NOW(),
  UNIQUE(user_id, date)
);
```

**Indexes:**
```sql
CREATE INDEX idx_users_email ON users(email);
CREATE INDEX idx_users_google_id ON users(google_id);
CREATE INDEX idx_users_roles ON users USING GIN(roles);
CREATE INDEX idx_user_metrics_user_date ON user_metrics(user_id, date DESC);
```

### 6.2. MongoDB Schema (Exercises Module)

**Exercises Collection:**
```javascript
{
  _id: ObjectId,
  name: String, // [Required] [Scoped Unique: (userId, isGlobal, name)]
  description: String,
  muscleGroups: [String], // [Indexed]
  equipment: [String], // [Indexed]
  instructions: [String],
  difficulty: String, // "Beginner"|"Intermediate"|"Advanced"|"Expert"
  videoUrl: String,
  imageUrl: String,
  isGlobal: Boolean,
  userId: ObjectId, // null for global exercises
  tags: [String], // [Indexed]
  createdAt: Date,
  updatedAt: Date
}
```

**Muscle Groups Collection:**
```javascript
{
  _id: ObjectId,
  name: String, // [Unique per scope]
  bodyRegion: String, // "Upper"|"Lower"|"Core"|"FullBody"
  parentId: ObjectId, // [Indexed] for hierarchy
  aliases: [String],
  isGlobal: Boolean,
  userId: ObjectId, // null for global
  createdAt: Date
}
```

**Equipment Collection:**
```javascript
{
  _id: ObjectId,
  name: String, // [Unique per scope]
  category: String,
  isGlobal: Boolean,
  userId: ObjectId, // null for global
  createdAt: Date
}
```

**Indexes:**
```javascript
// Exercises
{ userId: 1, isGlobal: 1, name: 1 } // Unique compound index
{ muscleGroups: 1 }
{ equipment: 1 }
{ tags: 1 }
{ difficulty: 1 }
{ name: "text", description: "text" } // Text search

// Muscle Groups
{ name: 1, isGlobal: 1, userId: 1 } // Unique compound index
{ bodyRegion: 1 }
{ parentId: 1 }

// Equipment
{ name: 1, isGlobal: 1, userId: 1 } // Unique compound index
{ category: 1 }
```

### 6.3. Future Modules Schema

**Routines Module (PostgreSQL):**

```sql
CREATE TABLE routines (
  id SERIAL PRIMARY KEY,
  user_id INTEGER REFERENCES users(id) ON DELETE CASCADE,
  name VARCHAR(255) NOT NULL,
  description TEXT,
  created_at TIMESTAMP DEFAULT NOW(),
  updated_at TIMESTAMP DEFAULT NOW()
);

CREATE TABLE routine_exercises (
  id SERIAL PRIMARY KEY,
  routine_id INTEGER REFERENCES routines(id) ON DELETE CASCADE,
  exercise_id INTEGER, -- References exercises in MongoDB
  order_index INTEGER NOT NULL,
  sets INTEGER,
  reps VARCHAR(50),
  rest_seconds INTEGER,
  notes TEXT,
  UNIQUE(routine_id, order_index)
);
```

**WorkoutLogs Module (MongoDB):**
```javascript
{
  _id: ObjectId,
  userId: ObjectId,
  routineId: ObjectId, // Optional reference
  date: ISODate,
  duration: Number, // seconds
  exercises: [{
    exerciseId: ObjectId,
    exerciseName: String, // Denormalized
    sets: [{
      setNumber: Number,
      reps: Number,
      weight: Number,
      restTime: Number,
      notes: String
    }],
    totalVolume: Number,
    personalRecord: Boolean
  }],
  notes: String,
  tags: [String],
  weather: Object, // Optional context
  mood: String // Optional tracking
}
```

### 6.4. Seed Data Strategy

**PostgreSQL (Users Module):**
- No seed data required for users (user-generated content)
- User metrics seeded through user interactions

**MongoDB (Exercises Module):**
- Global muscle groups seeded at startup
- Global equipment catalog seeded at startup
- User-specific data created through user interactions

## 7. Authentication and Authorization

- **Authentication:** Handled exclusively via **Google OAuth 2.0**. Upon successful Google authentication, the backend issues a **JWT (JSON Web Token)** containing user identity and roles.
- **Authorization:**
    - **Role-Based Access Control (RBAC):** Users will have roles (e.g., "User", "Admin").
    - **Policy-Based Authorization:** ASP.NET Core policies will enforce access rules (e.g., `[Authorize(Roles = "Admin")]` for admin functionalities; custom policies for data ownership like "users can only manage their own exercises").
- **Admin Initiation:** The first admin account will be designated via a **Configuration/Environment Variable Whitelist**. Upon first login, if a user's Google email matches a whitelisted email, they will automatically be assigned the "Admin" role.

## 8. Onboarding Flow (Technical)

- **Triggering Condition:** `User.isOnboarded == false` on client; backend exposes this via `GetUserProfile`.
- **Data Capture:**
  - Update profile: display name, unit preference (existing `UpdateUserProfileCommand`).
  - Create baseline metric: weight (required), height (optional) via `AddUserMetricCommand` with current date.
  - Save preferences: equipment and focus muscles via a new `UpdateUserPreferencesCommand` (or extend profile update) that persists `availableEquipmentIds` and `focusMuscleGroupIds`.
- **Completion Flag:** Add `CompleteOnboardingCommand` (or reuse profile update + metrics creation) that sets `User.isOnboarded = true` and `onboardingCompletedAt = DateTime.UtcNow`.
- **Validation:** Use FluentValidation for onboarding DTOs (weight required if completing).
- **Mapping:** Use Mapperly to map onboarding DTOs to `UpdateUserProfileCommand` and `AddUserMetricCommand` inputs.
- **Security:** Endpoints protected by `SelfOrAdmin` policy.

## 9. Exercise Import API (CSV)

- **Endpoint (user library):** `POST /api/v1/exercises/import`
  - Auth: `User` or `Admin`.
  - Body: `multipart/form-data` with `file`, options: `{ overwriteOnNameMatch: bool, defaultDifficulty: string }`.
  - Behavior: Creates user-owned exercises; if `overwriteOnNameMatch` is true, updates matching by (name, userId).

- **Endpoint (global, Admin):** `POST /api/v1/exercises/import/global`
  - Auth: `AdminOnly`.
  - Behavior: Creates global exercises; if overwrite, match by name and `isGlobal=true`.

- **CSV Columns (expected):**
  - `name` (required)
  - `description`
  - `muscleGroups` (comma-separated; match by seeded names/aliases, create user-private if not found)
  - `equipment` (comma-separated; create user-private if not found)
  - `instructions` (pipe-separated steps)
  - `difficulty` (Beginner|Intermediate|Advanced|Expert)
  - `videoUrl`
  - `imageUrl`
  - `tags` (comma-separated)

- **Response:**
```json
{
  "total": 120,
  "created": 100,
  "updated": 10,
  "failed": 10,
  "errors": [
    { "row": 12, "reason": "name missing" },
    { "row": 45, "reason": "invalid difficulty" }
  ]
}
```

- **Implementation Notes:**
  - Stream parse CSV to avoid large memory; validate per row with FluentValidation.
  - Use Mapperly to map parsed rows to `CreateExerciseCommand`/`UpdateExerciseCommand`.
  - Normalize/trim case for name matching; support `aliases` mapping for muscle groups.
  - Wrap in a MediatR command `ImportExercisesFromCsvCommand` with a handler in Exercises.Infrastructure.

## 10. Clean Architecture & SOLID (Enforcement)

- Each handler, command, and query SHALL reside in its own file. Do not co-locate multiple handlers in one file.
- Domain/Application code SHALL not depend on Infrastructure types. Use interfaces and inject abstractions.
- Large handlers SHOULD extract orchestration/services to maintain SRP.
- Domain/Application do not depend on Infrastructure; API/Bootstrap are outer layers.
- Installers live in Infrastructure; database index configuration is separated into per-module configurators.

### Service Interface Placement (Critical)
- **Service interfaces MUST be defined in the Application layer** as application contracts
- **Service implementations MUST be in the Infrastructure layer** as concrete implementations
- This ensures proper dependency flow: Application → Infrastructure (not the reverse)
- Handlers depend on Application layer interfaces, not Infrastructure layer contracts
- Violates Clean Architecture if service interfaces are placed in Infrastructure layer

## 11. Development Environment Setup (Mac)

- **Operating System:** macOS
- **Package Manager:** Homebrew
- **Version Control:** Git
- **IDEs/Editors:**
    - **Backend:** Visual Studio Code (with C# extension), Visual Studio for Mac, or JetBrains Rider.
    - **Frontend:** Visual Studio Code (with Volar, Tailwind CSS IntelliSense, ESLint, Prettier).
- **API Testing:** Postman or Insomnia.
- **Node.js Management:** NVM (Node Version Manager) for flexible Node.js version switching.
- **Local Database:** **MongoDB running in a Docker Compose container.**
    - `docker-compose.yml` will define the MongoDB service, mapping port `27017` to the host.
    - ASP.NET Core API will connect to `mongodb://admin:password@localhost:27017/WorkoutTrackerDb?authSource=admin`.
- **MongoDB GUI:** MongoDB Compass for inspecting local and Atlas databases.

## 12. Deployment Strategy (Zero-Cost Azure Focus)

The deployment strategy is entirely focused on leveraging **free tiers** of cloud services for a highly cost-effective solution, managed consistently via **Infrastructure as Code (IaC)**.

<svg aria-roledescription="flowchart-v2" role="graphics-document document" viewBox="-7.5 -8 773.953125 524.0132446289062" style="max-width: 773.953125px;" xmlns="http://www.w3.org/2000/svg" width="100%" id="mermaid-svg-1758215222110-r4hqq1kao"><style>#mermaid-svg-1758215222110-r4hqq1kao{font-family:"trebuchet ms",verdana,arial,sans-serif;font-size:16px;fill:rgba(204, 204, 204, 0.87);}#mermaid-svg-1758215222110-r4hqq1kao .error-icon{fill:#bf616a;}#mermaid-svg-1758215222110-r4hqq1kao .error-text{fill:#bf616a;stroke:#bf616a;}#mermaid-svg-1758215222110-r4hqq1kao .edge-thickness-normal{stroke-width:2px;}#mermaid-svg-1758215222110-r4hqq1kao .edge-thickness-thick{stroke-width:3.5px;}#mermaid-svg-1758215222110-r4hqq1kao .edge-pattern-solid{stroke-dasharray:0;}#mermaid-svg-1758215222110-r4hqq1kao .edge-pattern-dashed{stroke-dasharray:3;}#mermaid-svg-1758215222110-r4hqq1kao .edge-pattern-dotted{stroke-dasharray:2;}#mermaid-svg-1758215222110-r4hqq1kao .marker{fill:rgba(204, 204, 204, 0.87);stroke:rgba(204, 204, 204, 0.87);}#mermaid-svg-1758215222110-r4hqq1kao .marker.cross{stroke:rgba(204, 204, 204, 0.87);}#mermaid-svg-1758215222110-r4hqq1kao svg{font-family:"trebuchet ms",verdana,arial,sans-serif;font-size:16px;}#mermaid-svg-1758215222110-r4hqq1kao .label{font-family:"trebuchet ms",verdana,arial,sans-serif;color:rgba(204, 204, 204, 0.87);}#mermaid-svg-1758215222110-r4hqq1kao .cluster-label text{fill:#f3f3f3;}#mermaid-svg-1758215222110-r4hqq1kao .cluster-label span,#mermaid-svg-1758215222110-r4hqq1kao p{color:#f3f3f3;}#mermaid-svg-1758215222110-r4hqq1kao .label text,#mermaid-svg-1758215222110-r4hqq1kao span,#mermaid-svg-1758215222110-r4hqq1kao p{fill:rgba(204, 204, 204, 0.87);color:rgba(204, 204, 204, 0.87);}#mermaid-svg-1758215222110-r4hqq1kao .node rect,#mermaid-svg-1758215222110-r4hqq1kao .node circle,#mermaid-svg-1758215222110-r4hqq1kao .node ellipse,#mermaid-svg-1758215222110-r4hqq1kao .node polygon,#mermaid-svg-1758215222110-r4hqq1kao .node path{fill:#191919;stroke:#242424;stroke-width:1px;}#mermaid-svg-1758215222110-r4hqq1kao .flowchart-label text{text-anchor:middle;}#mermaid-svg-1758215222110-r4hqq1kao .node .label{text-align:center;}#mermaid-svg-1758215222110-r4hqq1kao .node.clickable{cursor:pointer;}#mermaid-svg-1758215222110-r4hqq1kao .arrowheadPath{fill:#e6e6e6;}#mermaid-svg-1758215222110-r4hqq1kao .edgePath .path{stroke:rgba(204, 204, 204, 0.87);stroke-width:2.0px;}#mermaid-svg-1758215222110-r4hqq1kao .flowchart-link{stroke:rgba(204, 204, 204, 0.87);fill:none;}#mermaid-svg-1758215222110-r4hqq1kao .edgeLabel{background-color:#19191999;text-align:center;}#mermaid-svg-1758215222110-r4hqq1kao .edgeLabel rect{opacity:0.5;background-color:#19191999;fill:#19191999;}#mermaid-svg-1758215222110-r4hqq1kao .labelBkg{background-color:rgba(25, 25, 25, 0.5);}#mermaid-svg-1758215222110-r4hqq1kao .cluster rect{fill:rgba(64, 64, 64, 0.47);stroke:#30373a;stroke-width:1px;}#mermaid-svg-1758215222110-r4hqq1kao .cluster text{fill:#f3f3f3;}#mermaid-svg-1758215222110-r4hqq1kao .cluster span,#mermaid-svg-1758215222110-r4hqq1kao p{color:#f3f3f3;}#mermaid-svg-1758215222110-r4hqq1kao div.mermaidTooltip{position:absolute;text-align:center;max-width:200px;padding:2px;font-family:"trebuchet ms",verdana,arial,sans-serif;font-size:12px;background:#88c0d0;border:1px solid #30373a;border-radius:2px;pointer-events:none;z-index:100;}#mermaid-svg-1758215222110-r4hqq1kao .flowchartTitleText{text-anchor:middle;font-size:18px;fill:rgba(204, 204, 204, 0.87);}#mermaid-svg-1758215222110-r4hqq1kao :root{--mermaid-font-family:"trebuchet ms",verdana,arial,sans-serif;}</style><g><marker orient="auto" markerHeight="12" markerWidth="12" markerUnits="userSpaceOnUse" refY="5" refX="6" viewBox="0 0 10 10" class="marker flowchart" id="mermaid-svg-1758215222110-r4hqq1kao_flowchart-pointEnd"><path style="stroke-width: 1; stroke-dasharray: 1, 0;" class="arrowMarkerPath" d="M 0 0 L 10 5 L 0 10 z"/></marker><marker orient="auto" markerHeight="12" markerWidth="12" markerUnits="userSpaceOnUse" refY="5" refX="4.5" viewBox="0 0 10 10" class="marker flowchart" id="mermaid-svg-1758215222110-r4hqq1kao_flowchart-pointStart"><path style="stroke-width: 1; stroke-dasharray: 1, 0;" class="arrowMarkerPath" d="M 0 5 L 10 10 L 10 0 z"/></marker><marker orient="auto" markerHeight="11" markerWidth="11" markerUnits="userSpaceOnUse" refY="5" refX="11" viewBox="0 0 10 10" class="marker flowchart" id="mermaid-svg-1758215222110-r4hqq1kao_flowchart-circleEnd"><circle style="stroke-width: 1; stroke-dasharray: 1, 0;" class="arrowMarkerPath" r="5" cy="5" cx="5"/></marker><marker orient="auto" markerHeight="11" markerWidth="11" markerUnits="userSpaceOnUse" refY="5" refX="-1" viewBox="0 0 10 10" class="marker flowchart" id="mermaid-svg-1758215222110-r4hqq1kao_flowchart-circleStart"><circle style="stroke-width: 1; stroke-dasharray: 1, 0;" class="arrowMarkerPath" r="5" cy="5" cx="5"/></marker><marker orient="auto" markerHeight="11" markerWidth="11" markerUnits="userSpaceOnUse" refY="5.2" refX="12" viewBox="0 0 11 11" class="marker cross flowchart" id="mermaid-svg-1758215222110-r4hqq1kao_flowchart-crossEnd"><path style="stroke-width: 2; stroke-dasharray: 1, 0;" class="arrowMarkerPath" d="M 1,1 l 9,9 M 10,1 l -9,9"/></marker><marker orient="auto" markerHeight="11" markerWidth="11" markerUnits="userSpaceOnUse" refY="5.2" refX="-1" viewBox="0 0 11 11" class="marker cross flowchart" id="mermaid-svg-1758215222110-r4hqq1kao_flowchart-crossStart"><path style="stroke-width: 2; stroke-dasharray: 1, 0;" class="arrowMarkerPath" d="M 1,1 l 9,9 M 10,1 l -9,9"/></marker><g class="root"><g class="clusters"/><g class="edgePaths"/><g class="edgeLabels"/><g class="nodes"><g transform="translate(-7.5, -8)" class="root"><g class="clusters"><g id="subGraph3" class="cluster default flowchart-label"><rect height="508.013240814209" width="757.953125" y="8" x="8" ry="0" rx="0" style=""/><g transform="translate(277.5625, 8)" class="cluster-label"><foreignObject height="19" width="218.828125"><div style="display: inline-block; white-space: nowrap;" xmlns="http://www.w3.org/1999/xhtml"><span class="nodeLabel">Zero-Cost Cloud Infrastructure</span></div></foreignObject></g></g><g id="subGraph0" class="cluster default flowchart-label"><rect height="234.50085830688477" width="464.90625" y="53.51238250732422" x="33" ry="0" rx="0" style=""/><g transform="translate(204.4375, 53.51238250732422)" class="cluster-label"><foreignObject height="19" width="122.03125"><div style="display: inline-block; white-space: nowrap;" xmlns="http://www.w3.org/1999/xhtml"><span class="nodeLabel">Azure (Free Tier)</span></div></foreignObject></g></g><g id="subGraph1" class="cluster default flowchart-label"><rect height="305.00171661376953" width="193.046875" y="28" x="547.90625" ry="0" rx="0" style=""/><g transform="translate(567.7890625, 28)" class="cluster-label"><foreignObject height="19" width="153.28125"><div style="display: inline-block; white-space: nowrap;" xmlns="http://www.w3.org/1999/xhtml"><span class="nodeLabel">Databases (Free Tier)</span></div></foreignObject></g></g><g id="subGraph2" class="cluster default flowchart-label"><rect height="188" width="224.859375" y="308.013240814209" x="33" ry="0" rx="0" style=""/><g transform="translate(59.7421875, 308.013240814209)" class="cluster-label"><foreignObject height="19" width="171.375"><div style="display: inline-block; white-space: nowrap;" xmlns="http://www.w3.org/1999/xhtml"><span class="nodeLabel">External Services (Free)</span></div></foreignObject></g></g></g><g class="edgePaths"><path marker-end="url(#mermaid-svg-1758215222110-r4hqq1kao_flowchart-pointEnd)" style="fill:none;" class="edge-thickness-normal edge-pattern-solid flowchart-link LS-API LE-MongoDB" id="L-API-MongoDB-0" d="M408.941,191.007L423.769,177.424C438.596,163.842,468.251,136.677,487.245,123.095C506.24,109.512,514.573,109.512,522.906,109.512C531.24,109.512,539.573,109.512,547.023,109.512C554.473,109.512,561.04,109.512,564.323,109.512L567.606,109.512"/><path marker-end="url(#mermaid-svg-1758215222110-r4hqq1kao_flowchart-pointEnd)" style="fill:none;" class="edge-thickness-normal edge-pattern-solid flowchart-link LS-API LE-PostgreSQL" id="L-API-PostgreSQL-0" d="M431.92,225.007L442.917,229.508C453.915,234.009,475.911,243.011,491.075,247.512C506.24,252.013,514.573,252.013,522.906,252.013C531.24,252.013,539.573,252.013,547.673,252.013C555.772,252.013,563.639,252.013,567.572,252.013L571.505,252.013"/><path marker-end="url(#mermaid-svg-1758215222110-r4hqq1kao_flowchart-pointEnd)" style="fill:none;" class="edge-thickness-normal edge-pattern-solid flowchart-link LS-Frontend LE-API" id="L-Frontend-API-0" d="M232.859,105.512L237.026,105.512C241.193,105.512,249.526,105.512,257.859,105.512C266.193,105.512,274.526,105.512,293.002,119.152C311.477,132.792,340.095,160.071,354.404,173.71L368.712,187.35"/><path marker-end="url(#mermaid-svg-1758215222110-r4hqq1kao_flowchart-pointEnd)" style="fill:none;" class="edge-thickness-normal edge-pattern-solid flowchart-link LS-Monitor LE-API" id="L-Monitor-API-0" d="M222.813,212.763L228.654,212.763C234.495,212.763,246.177,212.763,256.185,212.763C266.193,212.763,274.526,212.763,281.977,212.618C289.428,212.472,295.996,212.182,299.28,212.036L302.565,211.891"/><path marker-end="url(#mermaid-svg-1758215222110-r4hqq1kao_flowchart-pointEnd)" style="fill:none;" class="edge-thickness-normal edge-pattern-solid flowchart-link LS-GitHub LE-API" id="L-GitHub-API-0" d="M229.172,360.013L233.953,360.013C238.734,360.013,248.297,360.013,257.245,343.68C266.193,327.347,274.526,294.68,290.183,272.575C305.84,250.471,328.82,238.928,340.311,233.157L351.801,227.385"/></g><g class="edgeLabels"><g class="edgeLabel"><g transform="translate(0, 0)" class="label"><foreignObject height="0" width="0"><div style="display: inline-block; white-space: nowrap;" xmlns="http://www.w3.org/1999/xhtml"><span class="edgeLabel"></span></div></foreignObject></g></g><g class="edgeLabel"><g transform="translate(0, 0)" class="label"><foreignObject height="0" width="0"><div style="display: inline-block; white-space: nowrap;" xmlns="http://www.w3.org/1999/xhtml"><span class="edgeLabel"></span></div></foreignObject></g></g><g class="edgeLabel"><g transform="translate(0, 0)" class="label"><foreignObject height="0" width="0"><div style="display: inline-block; white-space: nowrap;" xmlns="http://www.w3.org/1999/xhtml"><span class="edgeLabel"></span></div></foreignObject></g></g><g class="edgeLabel"><g transform="translate(0, 0)" class="label"><foreignObject height="0" width="0"><div style="display: inline-block; white-space: nowrap;" xmlns="http://www.w3.org/1999/xhtml"><span class="edgeLabel"></span></div></foreignObject></g></g><g class="edgeLabel"><g transform="translate(0, 0)" class="label"><foreignObject height="0" width="0"><div style="display: inline-block; white-space: nowrap;" xmlns="http://www.w3.org/1999/xhtml"><span class="edgeLabel"></span></div></foreignObject></g></g></g><g class="nodes"><g transform="translate(390.3828125, 208.0066204071045)" id="flowchart-API-169" class="node default default flowchart-label"><rect height="34" width="165.046875" y="-17" x="-82.5234375" ry="0" rx="0" style="" class="basic label-container"/><g transform="translate(-75.0234375, -9.5)" style="" class="label"><rect/><foreignObject height="19" width="150.046875"><div style="display: inline-block; white-space: nowrap;" xmlns="http://www.w3.org/1999/xhtml"><span class="nodeLabel">Azure App Service F1</span></div></foreignObject></g></g><g transform="translate(644.4296875, 109.51238250732422)" id="flowchart-MongoDB-172" class="node default default flowchart-label"><path transform="translate(-71.5234375,-46.51238705916642)" d="M 0,13.341591372777614 a 71.5234375,13.341591372777614 0,0,0 143.046875 0 a 71.5234375,13.341591372777614 0,0,0 -143.046875 0 l 0,66.34159137277761 a 71.5234375,13.341591372777614 0,0,0 143.046875 0 l 0,-66.34159137277761" style=""/><g transform="translate(-64.0234375, -19)" style="" class="label"><rect/><foreignObject height="38" width="128.046875"><div style="display: inline-block; white-space: nowrap;" xmlns="http://www.w3.org/1999/xhtml"><span class="nodeLabel">MongoDB Atlas M0<br />512MB</span></div></foreignObject></g></g><g transform="translate(644.4296875, 252.01324081420898)" id="flowchart-PostgreSQL-173" class="node default default flowchart-label"><path transform="translate(-67.625,-45.98847262247839)" d="M 0,12.992315081652258 a 67.625,12.992315081652258 0,0,0 135.25 0 a 67.625,12.992315081652258 0,0,0 -135.25 0 l 0,65.99231508165227 a 67.625,12.992315081652258 0,0,0 135.25 0 l 0,-65.99231508165227" style=""/><g transform="translate(-60.125, -19)" style="" class="label"><rect/><foreignObject height="38" width="120.25"><div style="display: inline-block; white-space: nowrap;" xmlns="http://www.w3.org/1999/xhtml"><span class="nodeLabel">Supabase PostgreSQL<br />0.5GB</span></div></foreignObject></g></g><g transform="translate(145.4296875, 105.51238250732422)" id="flowchart-Frontend-170" class="node default default flowchart-label"><rect height="34" width="174.859375" y="-17" x="-87.4296875" ry="0" rx="0" style="" class="basic label-container"/><g transform="translate(-79.9296875, -9.5)" style="" class="label"><rect/><foreignObject height="19" width="159.859375"><div style="display: inline-block; white-space: nowrap;" xmlns="http://www.w3.org/1999/xhtml"><span class="nodeLabel">Azure Static Web Apps</span></div></foreignObject></g></g><g transform="translate(145.4296875, 212.7628116607666)" id="flowchart-Monitor-171" class="node default default flowchart-label"><rect height="34" width="154.765625" y="-17" x="-77.3828125" ry="0" rx="0" style="" class="basic label-container"/><g transform="translate(-69.8828125, -9.5)" style="" class="label"><rect/><foreignObject height="19" width="139.765625"><div style="display: inline-block; white-space: nowrap;" xmlns="http://www.w3.org/1999/xhtml"><span class="nodeLabel">Application Insights</span></div></foreignObject></g></g><g transform="translate(145.4296875, 360.013240814209)" id="flowchart-GitHub-175" class="node default default flowchart-label"><rect height="34" width="167.484375" y="-17" x="-83.7421875" ry="0" rx="0" style="" class="basic label-container"/><g transform="translate(-76.2421875, -9.5)" style="" class="label"><rect/><foreignObject height="19" width="152.484375"><div style="display: inline-block; white-space: nowrap;" xmlns="http://www.w3.org/1999/xhtml"><span class="nodeLabel">GitHub Actions CI/CD</span></div></foreignObject></g></g><g transform="translate(145.4296875, 444.013240814209)" id="flowchart-Google-174" class="node default default flowchart-label"><rect height="34" width="113.34375" y="-17" x="-56.671875" ry="0" rx="0" style="" class="basic label-container"/><g transform="translate(-49.171875, -9.5)" style="" class="label"><rect/><foreignObject height="19" width="98.34375"><div style="display: inline-block; white-space: nowrap;" xmlns="http://www.w3.org/1999/xhtml"><span class="nodeLabel">Google OAuth</span></div></foreignObject></g></g></g></g></g></g></g></svg>
- **Infrastructure as Code (IaC): Terraform**
    - **Purpose:** To define, provision, and manage all cloud resources declaratively, ensuring consistency, reproducibility, and version control.
    - **Components Managed:** Azure Resource Group, Azure App Service Plan (F1 Free Tier), Azure App Service (for API), Azure Static Web App (for Frontend), Azure Application Insights, MongoDB Atlas Project/Cluster/Database User.
    - **Secrets Handling:** Environment variables for Terraform credentials; Azure App Service Application Settings for application secrets (MongoDB connection string, Google OAuth secrets, admin email whitelist).
- **Backend Hosting:** **Azure App Service (Free Tier - F1)**
    - Provides a managed platform for the ASP.NET Core API.
    - **Zero cost**, suitable for development and low-traffic usage.
- **Database Hosting:** **MongoDB Atlas (M0 Shared Cluster - Free Tier)**
    - Official managed MongoDB service.
    - **Zero cost**, suitable for development and small data volumes.
- **Frontend Hosting:** **Azure Static Web Apps (Free Tier)**
    - Designed for static web applications like Vue.js.
    - **Zero cost**, includes custom domains, free SSL, and integrated CI/CD.
- **Monitoring:** **Azure Application Insights (Consumption Pricing - Free Grant)**
    - Application performance monitoring and logging.
    - Very likely to stay within free usage limits for a small user base.
- **Authentication Provider:** **Google Cloud Project (Free Usage Tiers)**
    - Google OAuth services are free for standard usage volumes.

## 13. CI/CD Pipeline (GitHub Actions)

A robust CI/CD pipeline using **GitHub Actions** will automate the entire software delivery process, ensuring consistency, reliability, and faster iterations.

- **Repository Structure:** A single monorepo is recommended (`dotFitness.WorkoutTracker/`) containing `infra/`, all `.NET` projects, and `dotFitness.WebUI/`.
- **Separate Workflows:** Dedicated GitHub Actions workflows will be created for different concerns:
    1. **`infra-deploy.yml` (Infrastructure Pipeline):**
        - **Trigger:** Push to `main` branch affecting files within the `infra/` directory.
        - **Stages:** `terraform init`, `terraform plan`, `terraform apply`.
        - **Secrets:** Azure Service Principal credentials, MongoDB Atlas Programmatic API keys, MongoDB database user credentials, initial admin email (all stored as GitHub Secrets).
    2. **`backend-ci-cd.yml` (Backend Application Pipeline):**
        - **Trigger:** Push to `main` branch (or Pull Request) affecting `.NET` project files (`.cs`, `.csproj`).
        - **Stages:**
            - `dotnet restore` (dependencies).
            - `dotnet build` (compilation).
            - `dotnet test` (runs xUnit.net unit tests).
            - `dotnet publish` (prepares for deployment).
            - `azure/webapps-deploy@v2` (deploys to Azure App Service).
        - **Secrets:** Azure Service Principal credentials.
    3. **`frontend-ci-cd.yml` (Frontend Application Pipeline):**
        - **Trigger:** Push to `main`