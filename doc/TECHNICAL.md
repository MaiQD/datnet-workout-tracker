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
    - An administrative role will be implemented to manage and populate the global lists of **Muscle Groups** and **Equipment**.
    - Users can select from global lists, and a mechanism will be in place for users to add *their own* private muscle groups or equipment if not in the global list.

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

The Visual Studio solution (`dotFitness.sln`) will contain the following projects:

`dotFitness.sln
├── dotFitness.Api/                           <-- Main ASP.NET Core Web API entry point
│   ├── Program.cs
│   └── appsettings.json
│   └── ... global middleware, auth setup, etc.
│
├── dotFitness.SharedKernel/                  <-- Core shared components
│   ├── Results/ (Result pattern implementation)
│   ├── Outbox/ (OutboxMessage model)
│   ├── Interfaces/ (e.g., IEntity)
│   └── ... common utilities
│
├── dotFitness.Modules.Users.Application/     <-- User module: Public API (Commands, Queries, DTOs, Mappers)
├── dotFitness.Modules.Users.Domain/          <-- User module: Core business logic, entities (User, UserMetric)
├── dotFitness.Modules.Users.Infrastructure/  <-- User module: Implementation details (MongoDB repos, Handlers)
├── dotFitness.Modules.Users.Tests/           <-- User module: Unit tests
│
├── dotFitness.Modules.Exercises.Application/ <-- Similar structure for Exercises module
├── dotFitness.Modules.Exercises.Domain/
├── dotFitness.Modules.Exercises.Infrastructure/
├── dotFitness.Modules.Exercises.Tests/
│
├── dotFitness.Modules.Routines.Application/  <-- Similar structure for Routines module
├── dotFitness.Modules.Routines.Domain/
├── dotFitness.Modules.Routines.Infrastructure/
├── dotFitness.Modules.Routines.Tests/
│
├── dotFitness.Modules.WorkoutLogs.Application/ <-- Similar structure for WorkoutLogs module
├── dotFitness.Modules.WorkoutLogs.Domain/
├── dotFitness.Modules.WorkoutLogs.Infrastructure/
├── dotFitness.Modules.WorkoutLogs.Tests/
│
└── dotFitness.WebUI/                         <-- Vue.js Frontend Application`

**Project Dependencies:**

- `dotFitness.Api` references all `.Application` projects.
- `.Infrastructure` projects reference their corresponding `.Domain` and `.Application` projects, and `dotFitness.SharedKernel`.
- `.Domain` projects reference `dotFitness.SharedKernel`.
- `.Application` projects reference `dotFitness.SharedKernel`.
- `.Tests` projects reference the `Application`, `Domain`, `Infrastructure` projects they are testing, and `dotFitness.SharedKernel`.

### 3.3. Core Technical Stack

- **Backend Framework:** ASP.NET Core Web API (latest LTS version, e.g., .NET 8).
- **Frontend Framework:** Vue.js (latest stable version, e.g., Vue 3) with Vite for build tooling.
- **Database:** MongoDB.
- **Styling:** Tailwind CSS (utility-first CSS framework).
- **Charting:** Chart.js (JavaScript charting library with Vue.js wrappers).

## 4. Core Technical Patterns & Libraries

### 4.1. CQRS (Command Query Responsibility Segregation)

- **Purpose:** To separate read (Query) and write (Command) operations, optimizing each for its specific responsibility.
- **Implementation:** Leverages **MediatR** as an in-process message dispatcher.
    - Commands encapsulate intent to change state (e.g., `CreateExerciseCommand`).
    - Queries encapsulate intent to retrieve data (e.g., `GetUserRoutinesQuery`).
    - Dedicated handlers (`IRequestHandler`) process each Command/Query.
- **Benefits:** Clearer separation of concerns, independent optimization of read/write paths, improved testability.

### 4.2. Outbox Pattern

- **Purpose:** To ensure atomicity and reliability when performing a database operation and publishing a domain event (e.g., updating a `WorkoutLog` and publishing a `WorkoutCompletedEvent`). Prevents data inconsistency due to "dual-writes."
- **Implementation:**
    - An `outboxMessages` MongoDB collection stores events as part of the same database transaction as the main data change.
    - A background `IHostedService` (Outbox Processor) polls `outboxMessages`, publishes events to a message broker (or directly to consumers internally), and marks them as processed.
- **Benefits:** Guarantees eventual consistency, robustness against application crashes.

### 4.3. Result Pattern

- **Purpose:** To explicitly handle and communicate outcomes (success or failure) of operations without relying solely on throwing exceptions for expected business rule violations or "not found" scenarios.
- **Implementation:** `Result` and `Result<TValue>` objects defined in `dotFitness.SharedKernel`.
    - Methods (especially Command/Query handlers) return `Result<T>` indicating success with a value, or failure with an error message.
- **Benefits:** Explicit error handling, predictable API for consumers, improved readability, reduced exception overhead.

### 4.4. Mapperly

- **Purpose:** High-performance, compile-time code generation for mapping between DTOs (Data Transfer Objects) and domain models.
- **Implementation:** Using Mapperly's source generator capabilities within each module's `Application` project for mapping interfaces.
- **Benefits:** Eliminates runtime reflection overhead (faster), type-safe mapping, reduced boilerplate code.

### 4.5. FluentValidation

- **Purpose:** Provides a fluent and expressive API for defining robust validation rules for input DTOs and Commands.
- **Implementation:** Validator classes (e.g., `CreateExerciseCommandValidator`) are created for commands/queries, integrated with MediatR's pipeline behaviors.
- **Benefits:** Decouples validation logic from business logic, improved testability of validation rules, clear error messages.

### 4.6. Centralized Error Handling

- **Purpose:** To provide consistent and structured error responses from the API, preventing sensitive information leakage and improving client-side error handling.
- **Implementation:** ASP.NET Core's built-in Exception Handling Middleware or custom middleware to catch unhandled exceptions globally and map them to appropriate HTTP status codes and JSON error payloads. This also integrates with the `Result` pattern, mapping `Result.Failure` to HTTP responses.
- **Benefits:** Uniform error format, better user experience, simplified debugging.

### 4.7. API Versioning

- **Purpose:** To allow for controlled evolution of the API by supporting multiple versions simultaneously, preventing breaking changes for existing clients.
- **Implementation:** Using `Microsoft.AspNetCore.Mvc.Versioning` package to version API endpoints (e.g., via URL segments like `/v1/exercises`).
- **Benefits:** Smoother API updates, client compatibility, easier maintenance of long-lived APIs.

### 4.8. Structured Logging with Serilog

- **Purpose:** To provide rich, queryable logs for monitoring application behavior, diagnosing issues, and auditing.
- **Implementation:** Configuring Serilog in the `dotFitness.Api` project, using its structured logging capabilities and various sinks (e.g., Console, File, Azure Application Insights).
- **Benefits:** Easier log analysis, improved observability, faster debugging in production.

### 4.9. Testing (xUnit.net, Moq, FluentAssertions)

- **Purpose:** To ensure the correctness and reliability of the application's logic through automated testing.
- **Implementation:**
    - **xUnit.net:** The primary unit testing framework. Dedicated test projects for each module (`dotFitness.Modules.X.Tests`).
    - **Moq:** A mocking library used to create mock objects for dependencies, isolating the code under test (e.g., mocking `IMongoCollection<T>`, `IMediator`).
    - **FluentAssertions:** An assertion library that provides a highly readable and fluent syntax for asserting test outcomes.
- **Benefits:** High code quality, regression prevention, faster development cycles.

## 5. Database Schema (MongoDB Collections)

All collections will implicitly include `_id` (ObjectId string), `createdAt` (UTC DateTime), and `updatedAt` (UTC DateTime) fields as standard.

- **`users`**
    - `googleId` (String): Unique ID from Google.
    - `email` (String): User's email from Google.
    - `displayName` (String): User's name from Google.
    - `profilePicture` (String): URL to Google profile picture.
    - `loginMethod` (String): "google".
    - `roles` (Array of Strings): e.g., ["User", "Admin"].
    - `gender` (String, optional).
    - `dateOfBirth` (Date, optional).
    - `unitPreference` (String): "metric" or "imperial".
- **`userMetrics`**
    - `userId` (ObjectId): Reference to `users` collection.
    - `date` (Date): Date/time measurement was taken.
    - `weight` (Number).
    - `height` (Number, optional).
    - `bmi` (Number, calculated, optional).
    - `notes` (String, optional).
- **`exercises`**
    - `userId` (ObjectId): Reference to `users` collection (creator).
    - `name` (String).
    - `description` (String, optional).
    - `muscleGroupIds` (Array of ObjectId): References to `muscleGroups`.
    - `equipmentIds` (Array of ObjectId): References to `equipment`.
    - `videoLink` (String).
- **`muscleGroups`**
    - `name` (String, unique).
    - `isGlobal` (Boolean): `true` for admin-defined, `false` for user-defined.
    - `userId` (ObjectId, nullable): If `isGlobal` is `false`.
- **`equipment`**
    - `name` (String, unique).
    - `isGlobal` (Boolean): `true` for admin-defined, `false` for user-defined.
    - `userId` (ObjectId, nullable): If `isGlobal` is `false`.
- **`routines`**
    - `userId` (ObjectId): Reference to `users` collection.
    - `name` (String).
    - `description` (String, optional).
    - `exercises` (Array of Objects):
        - `exerciseId` (ObjectId): Reference to `exercises`.
        - `sets` (Number).
        - `reps` (String).
        - `restTimeSeconds` (Number).
        - `notes` (String, optional).
- **`workoutLogs`**
    - `userId` (ObjectId): Reference to `users` collection.
    - `routineId` (ObjectId, nullable): Reference to `routines` collection (if followed a routine).
    - `date` (Date): Date/time workout performed.
    - `notes` (String, optional).
    - `exercisesPerformed` (Array of Objects):
        - `exerciseId` (ObjectId): Reference to `exercises`.
        - `setsPerformed` (Array of Objects):
            - `setNumber` (Number).
            - `reps` (Number).
            - `weight` (Number).
            - `unit` (String): "kg" or "lbs".
            - `notes` (String, optional).
        - `exerciseNotes` (String, optional).
- **`outboxMessages`**
    - `occurredOn` (Date).
    - `type` (String): Full event type name.
    - `data` (String): Serialized event payload (JSON).
    - `processedDate` (Date, nullable).
    - `error` (String, nullable).
    - `retries` (Number).
    - `correlationId` (String, optional).
    - `traceId` (String, optional).

## 6. Authentication and Authorization

- **Authentication:** Handled exclusively via **Google OAuth 2.0**. Upon successful Google authentication, the backend issues a **JWT (JSON Web Token)** containing user identity and roles.
- **Authorization:**
    - **Role-Based Access Control (RBAC):** Users will have roles (e.g., "User", "Admin").
    - **Policy-Based Authorization:** ASP.NET Core policies will enforce access rules (e.g., `[Authorize(Roles = "Admin")]` for admin functionalities; custom policies for data ownership like "users can only manage their own exercises").
- **Admin Initiation:** The first admin account will be designated via a **Configuration/Environment Variable Whitelist**. Upon first login, if a user's Google email matches a whitelisted email, they will automatically be assigned the "Admin" role.

## 7. Development Environment Setup (Mac)

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

## 8. Deployment Strategy (Zero-Cost Azure Focus)

The deployment strategy is entirely focused on leveraging **free tiers** of cloud services for a highly cost-effective solution, managed consistently via **Infrastructure as Code (IaC)**.

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

## 9. CI/CD Pipeline (GitHub Actions)

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
        - **Trigger:** Push to `main` branch (or Pull Request) affecting files within the `YourAppName.WebUI/` directory.
        - **Stages:**
            - `npm install` (frontend dependencies).
            - `npm run build` (builds Vue.js application into static files).
            - `azure/static-web-apps-deploy@v1` (deploys static files to Azure Static Web Apps).
        - **Secrets:** `AZURE_STATIC_WEB_APPS_API_TOKEN` (generated by Azure Static Web Apps), `VITE_APP_API_BASE_URL` (environment variable to point to the deployed API URL).

This comprehensive technical document outlines the entire journey for building **dotFitness**, from its conceptual features to its robust deployment and CI/CD strategy.