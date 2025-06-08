# DatNet Workout Tracker - Technical Documentation

## Table of Contents

1. [Project Overview](#project-overview)
2. [Architecture](#architecture)
3. [Technology Stack](#technology-stack)
4. [Module Breakdown](#module-breakdown)
5. [Database Design](#database-design)
6. [Authentication & Security](#authentication--security)
7. [API Specifications](#api-specifications)
8. [Development Setup](#development-setup)
9. [Deployment](#deployment)
10. [Configuration](#configuration)
11. [Testing Strategy](#testing-strategy)
12. [Performance Considerations](#performance-considerations)

---

## Project Overview

**DatNet Workout Tracker** is a comprehensive fitness tracking application built with .NET 8 and Blazor Server. The application follows a **Modular Monolith** architecture pattern, providing users with the ability to track workouts, manage exercise routines, monitor progress through analytics, and maintain their fitness journey data.

### Key Features

- **Workout Management**: Create, track, and manage workout sessions
- **Exercise Library**: Comprehensive exercise database with categories and instructions
- **Routine Builder**: Design and save custom workout routines
- **Progress Analytics**: Visual charts and statistics for fitness progress tracking
- **Calendar Integration**: Schedule and view workout sessions on a calendar
- **User Authentication**: Secure Google OAuth integration
- **Responsive UI**: Modern, mobile-friendly interface using Syncfusion Blazor components

---

## Architecture

### Modular Monolith Pattern

The application is structured as a modular monolith with clear separation of concerns across different business domains:

```
┌─────────────────────────────────────────────────────────┐
│                    WebApp (Host)                        │
│  ┌─────────────────────────────────────────────────────┐│
│  │              Blazor Server UI                       ││
│  │  ┌─────────┬─────────┬─────────┬─────────┬────────┐ ││
│  │  │  Pages  │ Shared  │ Layouts │ Auth    │ Config │ ││
│  │  └─────────┴─────────┴─────────┴─────────┴────────┘ ││
│  └─────────────────────────────────────────────────────┘│
├─────────────────────────────────────────────────────────┤
│                    Module Layer                         │
│  ┌─────────┬─────────┬─────────┬─────────┬───────────┐  │
│  │  Users  │Exercises│Workouts │Routines │Analytics  │  │
│  │ Module  │ Module  │ Module  │ Module  │  Module   │  │
│  └─────────┴─────────┴─────────┴─────────┴───────────┘  │
├─────────────────────────────────────────────────────────┤
│                   Shared Layer                          │
│  ┌─────────────────────────────────────────────────────┐│
│  │     Infrastructure & Common Services                ││
│  │  ┌─────────────┬─────────────┬─────────────────────┐││
│  │  │ Repository  │ Base Entity │ Service Extensions  │││
│  │  │   Pattern   │             │                     │││
│  │  └─────────────┴─────────────┴─────────────────────┘││
│  └─────────────────────────────────────────────────────┘│
├─────────────────────────────────────────────────────────┤
│                 Data Layer                              │
│  ┌─────────────────────────────────────────────────────┐│
│  │                MongoDB Atlas                        ││
│  └─────────────────────────────────────────────────────┘│
└─────────────────────────────────────────────────────────┘
```

### Design Principles

- **Single Responsibility**: Each module handles a specific business domain
- **Dependency Inversion**: Modules depend on abstractions, not concrete implementations
- **Clean Architecture**: Clear separation between business logic and infrastructure
- **SOLID Principles**: Followed throughout the codebase design

---

## Technology Stack

### Core Technologies

| Component | Technology | Version | Purpose |
|-----------|------------|---------|---------|
| **Framework** | .NET | 8.0 | Main application framework |
| **UI Framework** | Blazor Server | 8.0 | Server-side web UI |
| **Database** | MongoDB Atlas | Latest | Document database |
| **Authentication** | Google OAuth 2.0 | - | User authentication |
| **UI Components** | Syncfusion Blazor | 27.1.48 | Professional UI components |
| **Containerization** | Docker | - | Development environment |

### Key NuGet Packages

```xml
<!-- Authentication & Authorization -->
<PackageReference Include="Microsoft.AspNetCore.Authentication.Google" Version="8.0.8" />

<!-- Database -->
<PackageReference Include="MongoDB.Driver" Version="2.29.0" />

<!-- UI Components -->
<PackageReference Include="Syncfusion.Blazor.Themes" Version="27.1.48" />
<PackageReference Include="Syncfusion.Blazor.Charts" Version="27.1.48" />
<PackageReference Include="Syncfusion.Blazor.Calendars" Version="27.1.48" />
<PackageReference Include="Syncfusion.Blazor.Grid" Version="27.1.48" />
<PackageReference Include="Syncfusion.Blazor.Buttons" Version="27.1.48" />
```

---

## Module Breakdown

### 1. Users Module (`DatNetWorkoutTracker.Users`)

**Responsibility**: User management and profile handling

**Key Components**:
- `User` domain entity with profile information
- `IUserService` and `UserService` for user operations
- User profile management and settings

**Domain Model**:
```csharp
public class User : BaseEntity
{
    public string Email { get; set; }
    public string Name { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
}
```

### 2. Exercises Module (`DatNetWorkoutTracker.Exercises`)

**Responsibility**: Exercise library and exercise data management

**Key Components**:
- `Exercise` domain entity with detailed exercise information
- `IExerciseService` and `ExerciseService` for exercise operations
- Exercise categorization and search functionality

**Domain Model**:
```csharp
public class Exercise : BaseEntity
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public string Category { get; set; }
    public string? Instructions { get; set; }
    public List<string> MuscleGroups { get; set; }
    public string? ImageUrl { get; set; }
    public DifficultyLevel Difficulty { get; set; }
}
```

### 3. Workouts Module (`DatNetWorkoutTracker.Workouts`)

**Responsibility**: Individual workout session management

**Key Components**:
- `Workout` and `WorkoutExercise` domain entities
- `IWorkoutService` and `WorkoutService` for workout operations
- Workout tracking and history management

**Domain Model**:
```csharp
public class Workout : BaseEntity
{
    public string UserId { get; set; }
    public string Name { get; set; }
    public DateTime Date { get; set; }
    public List<WorkoutExercise> Exercises { get; set; }
    public TimeSpan Duration { get; set; }
    public string? Notes { get; set; }
}

public class WorkoutExercise
{
    public string ExerciseId { get; set; }
    public string ExerciseName { get; set; }
    public List<WorkoutSet> Sets { get; set; }
}
```

### 4. Routines Module (`DatNetWorkoutTracker.Routines`)

**Responsibility**: Workout routine templates and scheduling

**Key Components**:
- `Routine` and `RoutineExercise` domain entities
- `IRoutineService` and `RoutineService` for routine operations
- Template management for recurring workouts

**Domain Model**:
```csharp
public class Routine : BaseEntity
{
    public string UserId { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public List<RoutineExercise> Exercises { get; set; }
    public List<DayOfWeek> ScheduledDays { get; set; }
    public bool IsActive { get; set; }
}
```

### 5. Analytics Module (`DatNetWorkoutTracker.Analytics`)

**Responsibility**: Progress tracking and fitness statistics

**Key Components**:
- `ProgressMetric` and analytics models
- `IAnalyticsService` and `AnalyticsService` for data analysis
- Chart data preparation and statistical calculations

**Models**:
```csharp
public class ProgressMetric
{
    public string UserId { get; set; }
    public string MetricType { get; set; }
    public double Value { get; set; }
    public DateTime RecordedAt { get; set; }
    public string? ExerciseId { get; set; }
}
```

---

## Database Design

### MongoDB Collections

The application uses MongoDB with the following collection structure:

#### Users Collection
```json
{
  "_id": "ObjectId",
  "Email": "user@example.com",
  "Name": "John Doe",
  "ProfilePictureUrl": "https://...",
  "CreatedAt": "2024-01-01T00:00:00Z",
  "LastLoginAt": "2024-01-01T00:00:00Z"
}
```

#### Exercises Collection
```json
{
  "_id": "ObjectId",
  "Name": "Push-up",
  "Description": "A bodyweight exercise...",
  "Category": "Strength",
  "Instructions": "Step by step instructions...",
  "MuscleGroups": ["Chest", "Shoulders", "Triceps"],
  "ImageUrl": "https://...",
  "Difficulty": "Beginner"
}
```

#### Workouts Collection
```json
{
  "_id": "ObjectId",
  "UserId": "ObjectId",
  "Name": "Morning Workout",
  "Date": "2024-01-01T00:00:00Z",
  "Exercises": [
    {
      "ExerciseId": "ObjectId",
      "ExerciseName": "Push-up",
      "Sets": [
        {
          "SetNumber": 1,
          "Reps": 15,
          "Weight": 0,
          "RestTime": "00:01:00"
        }
      ]
    }
  ],
  "Duration": "00:45:00",
  "Notes": "Great workout session"
}
```

### Repository Pattern Implementation

The application uses a generic repository pattern with MongoDB:

```csharp
public class MongoRepository<T> : IRepository<T> where T : BaseEntity
{
    private readonly IMongoCollection<T> _collection;
    
    public async Task<T> GetByIdAsync(string id)
    public async Task<IEnumerable<T>> GetAllAsync()
    public async Task<T> AddAsync(T entity)
    public async Task UpdateAsync(T entity)
    public async Task DeleteAsync(string id)
    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
}
```

---

## Authentication & Security

### Google OAuth Integration

The application uses Google OAuth 2.0 for user authentication:

**Configuration** (appsettings.json):
```json
{
  "Authentication": {
    "Google": {
      "ClientId": "your-google-client-id",
      "ClientSecret": "your-google-client-secret"
    }
  }
}
```

**Authentication Flow**:
1. User clicks "Login with Google"
2. Redirected to Google OAuth consent screen
3. Google returns authorization code
4. Application exchanges code for access token
5. User profile information retrieved from Google API
6. User session established with cookie authentication

**Security Configuration**:
```csharp
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
.AddCookie()
.AddGoogle(options =>
{
    options.ClientId = configuration["Authentication:Google:ClientId"];
    options.ClientSecret = configuration["Authentication:Google:ClientSecret"];
});
```

### Authorization

- **User-based Authorization**: Each user can only access their own data
- **Session Management**: Cookie-based authentication with secure settings
- **Data Isolation**: User ID filtering at service layer level

---

## API Specifications

### Authentication Endpoints

#### POST /auth/login
- **Purpose**: Initiate Google OAuth login
- **Response**: Redirect to Google OAuth

#### POST /auth/logout
- **Purpose**: Clear user session
- **Response**: Redirect to login page

### Internal Service APIs

The application uses service-to-service communication within the modular architecture:

#### User Service
```csharp
Task<User> GetUserByIdAsync(string userId);
Task<User> GetUserByEmailAsync(string email);
Task<User> CreateUserAsync(User user);
Task UpdateUserAsync(User user);
```

#### Exercise Service
```csharp
Task<IEnumerable<Exercise>> GetAllExercisesAsync();
Task<Exercise> GetExerciseByIdAsync(string exerciseId);
Task<IEnumerable<Exercise>> GetExercisesByCategoryAsync(string category);
Task<Exercise> CreateExerciseAsync(Exercise exercise);
```

#### Workout Service
```csharp
Task<IEnumerable<Workout>> GetUserWorkoutsAsync(string userId);
Task<Workout> GetWorkoutByIdAsync(string workoutId);
Task<Workout> CreateWorkoutAsync(Workout workout);
Task UpdateWorkoutAsync(Workout workout);
Task DeleteWorkoutAsync(string workoutId);
```

#### Routine Service
```csharp
Task<IEnumerable<Routine>> GetUserRoutinesAsync(string userId);
Task<Routine> GetRoutineByIdAsync(string routineId);
Task<Routine> CreateRoutineAsync(Routine routine);
Task UpdateRoutineAsync(Routine routine);
```

#### Analytics Service
```csharp
Task<IEnumerable<ProgressMetric>> GetUserProgressMetricsAsync(string userId);
Task<AnalyticsSummary> GetWorkoutSummaryAsync(string userId, DateTime fromDate, DateTime toDate);
Task RecordProgressMetricAsync(ProgressMetric metric);
```

---

## Development Setup

### Prerequisites

- **.NET 8 SDK** or later
- **Docker & Docker Compose** (for local MongoDB)
- **IDE**: Visual Studio 2022, VS Code, or JetBrains Rider
- **Git** for version control

### Local Development Setup

1. **Clone the Repository**
   ```bash
   git clone <repository-url>
   cd datnet-workout-tracker
   ```

2. **Start MongoDB with Docker Compose**
   ```bash
   docker-compose up -d
   ```

3. **Configure Application Settings**
   
   Create `appsettings.Development.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "mongodb://localhost:27017/datnet-workout-tracker"
     },
     "Authentication": {
       "Google": {
         "ClientId": "your-development-client-id",
         "ClientSecret": "your-development-client-secret"
       }
     }
   }
   ```

4. **Restore Dependencies**
   ```bash
   dotnet restore
   ```

5. **Run the Application**
   ```bash
   dotnet run --project src/DatNetWorkoutTracker.WebApp
   ```

6. **Access the Application**
   - Navigate to `https://localhost:5001` or `http://localhost:5000`

### Google OAuth Setup

1. **Create Google Cloud Project**
   - Go to [Google Cloud Console](https://console.cloud.google.com/)
   - Create a new project or select existing one

2. **Enable Google+ API**
   - Navigate to APIs & Services → Library
   - Search and enable "Google+ API"

3. **Create OAuth 2.0 Credentials**
   - Go to APIs & Services → Credentials
   - Create OAuth 2.0 Client ID
   - Set authorized redirect URIs:
     - `https://localhost:5001/signin-google` (Development)
     - `https://yourdomain.com/signin-google` (Production)

4. **Configure Application**
   - Update `appsettings.json` with ClientId and ClientSecret

---

## Deployment

### Docker Deployment

The application includes Docker support for containerized deployment:

**Dockerfile** (to be created):
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/DatNetWorkoutTracker.WebApp/DatNetWorkoutTracker.WebApp.csproj", "src/DatNetWorkoutTracker.WebApp/"]
COPY ["src/DatNetWorkoutTracker.Shared/DatNetWorkoutTracker.Shared.csproj", "src/DatNetWorkoutTracker.Shared/"]
COPY ["src/Modules/DatNetWorkoutTracker.Users/DatNetWorkoutTracker.Users.csproj", "src/Modules/DatNetWorkoutTracker.Users/"]
COPY ["src/Modules/DatNetWorkoutTracker.Exercises/DatNetWorkoutTracker.Exercises.csproj", "src/Modules/DatNetWorkoutTracker.Exercises/"]
COPY ["src/Modules/DatNetWorkoutTracker.Workouts/DatNetWorkoutTracker.Workouts.csproj", "src/Modules/DatNetWorkoutTracker.Workouts/"]
COPY ["src/Modules/DatNetWorkoutTracker.Routines/DatNetWorkoutTracker.Routines.csproj", "src/Modules/DatNetWorkoutTracker.Routines/"]
COPY ["src/Modules/DatNetWorkoutTracker.Analytics/DatNetWorkoutTracker.Analytics.csproj", "src/Modules/DatNetWorkoutTracker.Analytics/"]

RUN dotnet restore "src/DatNetWorkoutTracker.WebApp/DatNetWorkoutTracker.WebApp.csproj"
COPY . .
WORKDIR "/src/src/DatNetWorkoutTracker.WebApp"
RUN dotnet build "DatNetWorkoutTracker.WebApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DatNetWorkoutTracker.WebApp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DatNetWorkoutTracker.WebApp.dll"]
```

### Production Deployment

**Environment Configuration**:
- Use MongoDB Atlas for production database
- Configure production Google OAuth credentials
- Set up HTTPS certificates
- Configure environment-specific settings

**Recommended Hosting Platforms**:
- **Azure App Service** (recommended for .NET applications)
- **AWS Elastic Beanstalk**
- **Google Cloud Run**
- **Self-hosted with Docker**

---

## Configuration

### Application Settings Structure

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "mongodb://localhost:27017/datnet-workout-tracker"
  },
  "Authentication": {
    "Google": {
      "ClientId": "your-google-client-id",
      "ClientSecret": "your-google-client-secret"
    }
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

### Environment-Specific Configuration

- **Development**: `appsettings.Development.json`
- **Staging**: `appsettings.Staging.json`
- **Production**: `appsettings.Production.json`

### Docker Compose Configuration

```yaml
version: '3.8'
services:
  mongo:
    image: mongo:latest
    container_name: datnet-mongo
    restart: always
    ports:
      - "27017:27017"
    volumes:
      - ./mongo-data:/data/db
      - ./mongo-init.js:/docker-entrypoint-initdb.d/mongo-init.js:ro
    environment:
      MONGO_INITDB_DATABASE: datnet-workout-tracker
```

---

## Testing Strategy

### Recommended Testing Approach

1. **Unit Tests**
   - Service layer testing
   - Domain model validation
   - Repository pattern testing

2. **Integration Tests**
   - MongoDB integration
   - Authentication flow testing
   - Module interaction testing

3. **End-to-End Tests**
   - User workflow testing
   - UI component testing with Blazor testing framework

### Testing Framework Recommendations

```xml
<PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.8.0" />
<PackageReference Include="xunit" Version="2.4.2" />
<PackageReference Include="xunit.runner.visualstudio" Version="2.4.5" />
<PackageReference Include="Moq" Version="4.20.69" />
<PackageReference Include="Microsoft.AspNetCore.Mvc.Testing" Version="8.0.0" />
```

---

## Performance Considerations

### Database Optimization

1. **MongoDB Indexing**
   ```javascript
   // Users collection
   db.users.createIndex({ "Email": 1 }, { unique: true })
   
   // Workouts collection
   db.workouts.createIndex({ "UserId": 1, "Date": -1 })
   
   // Exercises collection
   db.exercises.createIndex({ "Category": 1, "MuscleGroups": 1 })
   ```

2. **Query Optimization**
   - Use projection to limit returned fields
   - Implement pagination for large datasets
   - Use aggregation pipeline for complex queries

### Blazor Server Optimization

1. **SignalR Configuration**
   - Configure appropriate connection limits
   - Implement connection monitoring
   - Handle connection failures gracefully

2. **Component Optimization**
   - Use `@key` directives for list rendering
   - Implement `ShouldRender()` for expensive components
   - Use Blazor virtualization for large datasets

3. **State Management**
   - Minimize state in components
   - Use cascading parameters efficiently
   - Implement proper disposal patterns

### Caching Strategy

1. **Application-Level Caching**
   - Cache exercise data (relatively static)
   - Cache user profile information
   - Implement distributed caching for scalability

2. **Database Query Caching**
   - Cache frequently accessed routines
   - Cache analytics aggregations

---

## Monitoring & Logging

### Recommended Logging Configuration

```json
{
  "Serilog": {
    "Using": ["Serilog.Sinks.Console", "Serilog.Sinks.File"],
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    },
    "WriteTo": [
      { "Name": "Console" },
      {
        "Name": "File",
        "Args": {
          "path": "./logs/datnet-workout-tracker-.txt",
          "rollingInterval": "Day"
        }
      }
    ]
  }
}
```

### Monitoring Recommendations

1. **Application Performance Monitoring (APM)**
   - Application Insights (Azure)
   - New Relic
   - Datadog

2. **Health Checks**
   - Database connectivity
   - Authentication service availability
   - Memory and CPU usage monitoring

---

## Future Enhancements

### Planned Features

1. **Mobile Application**
   - Xamarin or .NET MAUI mobile app
   - Offline workout tracking
   - Synchronization with web application

2. **Social Features**
   - Workout sharing
   - Friend connections
   - Community challenges

3. **Advanced Analytics**
   - Machine learning progress predictions
   - Personalized workout recommendations
   - Injury prevention insights

4. **Integration Capabilities**
   - Fitness tracker integration (Fitbit, Apple Health)
   - Nutrition tracking
   - Third-party exercise APIs

### Technical Improvements

1. **Microservices Migration**
   - Convert modules to independent microservices
   - Implement API Gateway
   - Add service mesh for communication

2. **Real-time Features**
   - Live workout sessions
   - Real-time progress updates
   - Push notifications

3. **Performance Enhancements**
   - Implement CQRS pattern
   - Add event sourcing for audit trails
   - Implement horizontal scaling strategies

---

## Contributing

### Development Guidelines

1. **Code Standards**
   - Follow C# coding conventions
   - Use consistent naming patterns
   - Implement proper error handling

2. **Git Workflow**
   - Feature branch development
   - Pull request reviews
   - Automated testing before merge

3. **Documentation**
   - Update technical documentation
   - Maintain API documentation
   - Include inline code comments

### Project Structure Guidelines

- Keep modules independent and loosely coupled
- Follow SOLID principles in all implementations
- Maintain clear separation between domain and infrastructure
- Use dependency injection consistently across all modules

---

## Contact & Support

For technical questions or contributions, please refer to the project repository and follow the established contribution guidelines.

---

*This technical documentation is a living document and should be updated as the application evolves and new features are added.*
