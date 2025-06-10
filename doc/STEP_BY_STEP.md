## dotFitness: Full Step-by-Step Development Actions for AI Agent

This document provides a comprehensive, sequential list of actions required to develop the `dotFitness` application from initial setup through deployment, based on all agreed-upon architectural, design, and implementation details.

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
    - `dotnet add dotFitness.Api/dotFitness.Api.csproj reference dotFitness.Modules.Users.Application/dotFitness.Modules.Users.Application.csproj`
    - *(Repeat this for all other `.Application` modules as they are created later: Exercises, Routines, WorkoutLogs)*
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
20. **Setup Local MongoDB with Docker Compose:**
    - Create file `dotFitness.WorkoutTracker/docker-compose.yml` with the MongoDB service definition.
    - Execute `docker compose up -d` in the `dotFitness.WorkoutTracker` directory to start the MongoDB container.
21. **Configure `dotFitness.Api` - Add NuGet Packages:**
    - `dotnet add dotFitness.Api/dotFitness.Api.csproj package MongoDB.Driver`
    - `dotnet add dotFitness.Api/dotFitness.Api.csproj package Serilog.AspNetCore`
    - `dotnet add dotFitness.Api/dotFitness.Api.csproj package Serilog.Sinks.Console`
    - `dotnet add dotFitness.Api/dotFitness.Api.csproj package MediatR`
    - `dotnet add dotFitness.Api/dotFitness.Api.csproj package MediatR.Extensions.Microsoft.DependencyInjection`
    - `dotnet add dotFitness.Api/dotFitness.Api.csproj package Microsoft.AspNetCore.Authentication.JwtBearer`
    - `dotnet add dotFitness.Api/dotFitness.Api.csproj package Microsoft.AspNetCore.Mvc.Versioning`
    - `dotnet add dotFitness.Api/dotFitness.Api.csproj package FluentValidation.AspNetCore`
    - `dotnet add dotFitness.Api/dotFitness.Api.csproj package Mapperly.Diagnostics` (This is a diagnostic package for Mapperly, the core functionality is built-in as a source generator with `Mapperly.Generator` which is not a direct package add).
22. **Configure `dotFitness.Api` - Update `appsettings.json`:**
    - Modify `dotFitness.Api/appsettings.json` and `dotFitness.Api/appsettings.Development.json` to include:
        - `ConnectionStrings:MongoDB` (e.g., `"mongodb://admin:password@localhost:27017/dotFitnessDb?authSource=admin"`).
        - `AdminSettings:AdminEmails` array (e.g., `["your.admin.email@gmail.com"]`).
23. **Configure `dotFitness.Api` - Update `Program.cs`:**
    - Set up Serilog logging.
    - Register `IMongoClient` and `IMongoDatabase` as singletons.
    - Register all `IMongoCollection<T>` (for all planned entities: `User`, `UserMetric`, `Exercise`, `MuscleGroup`, `Equipment`, `Routine`, `WorkoutLog`, `OutboxMessage`) as singletons.
    - Implement `ConfigureMongoDbIndexes` function and call it to create indexes on startup.
    - Add Swagger/OpenAPI services.
    - Add CORS services.
    - Configure JWT Authentication and Authorization services.
    - Register MediatR to scan all `Application` assemblies.
    - Configure centralized error handling middleware.
    - Configure API Versioning services.

### Phase 2: Core User & Authentication Implementation

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

1. **Create Exercises Module Projects:**
    - `dotnet new classlib -n dotFitness.Modules.Exercises.Application`
    - `dotnet new classlib -n dotFitness.Modules.Exercises.Domain`
    - `dotnet new classlib -n dotFitness.Modules.Exercises.Infrastructure`
    - `dotnet new xunit -n dotFitness.Modules.Exercises.Tests`
    - Add all new projects to `dotFitness.WorkoutTracker.sln`.
    - Add project references as per the design document.
2. **Implement Exercises Module - Domain Layer (`dotFitness.Modules.Exercises.Domain`):**
    - Define `Exercise.cs` entity.
    - Define `MuscleGroup.cs` entity (with `IsGlobal`, `UserId`).
    - Define `Equipment.cs` entity (with `IsGlobal`, `UserId`).
    - Define `IExerciseRepository.cs`, `IMuscleGroupRepository.cs`, `IEquipmentRepository.cs` interfaces.
3. **Implement Exercises Module - Infrastructure Layer (`dotFitness.Modules.Exercises.Infrastructure`):**
    - Implement `ExerciseRepository.cs`, `MuscleGroupRepository.cs`, `EquipmentRepository.cs`.
    - Implement Command Handlers for `CreateExerciseCommand`, `UpdateExerciseCommand`, `DeleteExerciseCommand`.
    - Implement Command Handlers for `CreateMuscleGroupCommand`, `UpdateMuscleGroupCommand`, `DeleteMuscleGroupCommand` (and similar for Equipment).
    - Implement Query Handlers for `GetExerciseByIdQuery`, `GetAllExercisesQuery`, `GetAllMuscleGroupsQuery`, `GetAllEquipmentQuery`.
4. **Implement Exercises Module - Application Layer (`dotFitness.Modules.Exercises.Application`):**
    - Define `Commands`, `Queries`, `DTOs` (`ExerciseDto`, `CreateExerciseRequest`, `MuscleGroupDto`, `EquipmentDto`).
    - Define Mapperly interfaces (`IExerciseMapper`, `IMuscleGroupMapper`, `IEquipmentMapper`).
    - Implement FluentValidation validators for Commands.
5. **Integrate Exercises Module into `dotFitness.Api`:**
    - Create `dotFitness.Api/Controllers/ExercisesController.cs` for user-facing exercise management (CRUD).
    - Create `dotFitness.Api/Controllers/AdminMuscleGroupsController.cs` and `dotFitness.Api/Controllers/AdminEquipmentController.cs` for global management.
    - Apply `[Authorize]` and `[Authorize(Roles="Admin")]` appropriately.
    - Register `IMongoCollection<Exercise>`, `IMongoCollection<MuscleGroup>`, `IMongoCollection<Equipment>` in `Program.cs`.
6. **Implement Routines Module (Repeat Steps 28-32, replacing “Exercises” with “Routines”):**
    - **`dotFitness.Modules.Routines` Projects:** Create and reference.
    - **Domain:** Define `Routine.cs` (including nested `Exercises` property).
    - **Infrastructure:** Implement `IRoutineRepository.cs`, `RoutineRepository.cs`. Implement `CreateRoutineCommandHandler`, `UpdateRoutineCommandHandler`, `DeleteRoutineCommandHandler`. Implement `GetRoutineByIdQueryHandler`, `GetAllRoutinesQueryHandler`, `SuggestRoutineQueryHandler`, `GetPredefinedRoutineTemplatesQueryHandler`.
    - **Application:** Define `Commands`, `Queries`, `DTOs` (`RoutineDto`, `RoutineExerciseDto`), Mappers, and FluentValidation validators.
    - **API:** Create `dotFitness.Api/Controllers/RoutinesController.cs` (for CRUD, suggestions, templates). Register `IMongoCollection<Routine>`.
7. **Implement Workout Logs Module (Repeat Steps 28-32, replacing “Exercises” with “WorkoutLogs”):**
    - **`dotFitness.Modules.WorkoutLogs` Projects:** Create and reference.
    - **Domain:** Define `WorkoutLog.cs` (including nested `ExercisesPerformed`, `SetsPerformed`). Define `WorkoutCompletedEvent.cs`.
    - **Infrastructure:** Implement `IWorkoutLogRepository.cs`, `WorkoutLogRepository.cs`. Implement `LogWorkoutCommandHandler`.
        - **WT-003 Set Logging (Backend):** `LogWorkoutCommandHandler` will save `WorkoutLog` and **initiate Outbox Pattern** by inserting `WorkoutCompletedEvent` into `outboxMessages` within a MongoDB transaction.
    - **Application:** Define `Commands` (`LogWorkoutCommand`), `Queries` (`GetWorkoutLogByIdQuery`, `GetWorkoutHistoryQuery`, `GetWorkoutStatisticsQuery`, `GetPersonalBestsQuery`, `GetExerciseProgressQuery`), `DTOs`, Mappers, Validators.
    - **API:** Create `dotFitness.Api/Controllers/WorkoutLogsController.cs` (for logging, history, stats, personal bests). Register `IMongoCollection<WorkoutLog>`.

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