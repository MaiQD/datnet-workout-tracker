# dotFitness: Modular Monolith Workout Tracker

> A modern, scalable workout tracking application built with .NET 8, following Domain-Driven Design principles and Clean Architecture patterns.

[![.NET](https://img.shields.io/badge/.NET-8.0-blue.svg)](https://dotnet.microsoft.com/download/dotnet/8.0)
[![Architecture](https://img.shields.io/badge/Architecture-Modular%20Monolith-green.svg)](https://github.com/MaiQD/modular-monolith-with-ddd)
[![License](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

## 🎯 Overview

dotFitness is a **modular monolith** workout tracking application that combines the simplicity of a monolith with the scalability and maintainability of microservices. Built with .NET 8, MongoDB, and Clean Architecture principles, it provides a robust foundation for fitness tracking and workout management.

### Key Features

- **🏗️ Modular Architecture**: Self-contained modules with clear boundaries
- **🔐 Authentication**: Google OAuth integration with JWT tokens
- **💪 Exercise Management**: Create, manage, and track custom exercises
- **📊 Progress Tracking**: Monitor fitness metrics and progress over time
- **🎯 Clean Architecture**: Domain-Driven Design with proper layer separation
- **⚡ Performance**: Optimized with static mappers and MongoDB indexes
- **🧪 Comprehensive Testing**: Unit, integration, and API testing
- **📈 Monitoring**: Health checks, metrics, and observability

## 🏗️ Architecture

dotFitness implements a **Modular Monolith** architecture inspired by the [modular-monolith-with-ddd](https://github.com/MaiQD/modular-monolith-with-ddd) pattern:

```
dotFitness.WorkoutTracker/
├── dotFitness.Api/                    # 🚀 API Entry Point
├── dotFitness.SharedKernel/          # 🔗 Shared Components
└── Modules/                          # 📦 Business Modules
    ├── Users/                        # 👤 User Management
    ├── Exercises/                    # 💪 Exercise Management
    ├── Routines/                     # 📋 Workout Routines (Planned)
    └── WorkoutLogs/                  # 📊 Workout Tracking (Planned)
```

### Module Structure

Each module follows Clean Architecture with four layers:

```
dotFitness.Modules.{ModuleName}/
├── dotFitness.Modules.{ModuleName}.Domain/      # Business logic & entities
├── dotFitness.Modules.{ModuleName}.Application/ # Use cases & DTOs
├── dotFitness.Modules.{ModuleName}.Infrastructure/ # External concerns
└── dotFitness.Modules.{ModuleName}.Tests/       # Comprehensive tests
```

### Design Patterns

- **CQRS**: Command Query Responsibility Segregation with MediatR
- **Result Pattern**: Explicit error handling without exceptions
- **Static Mappers**: Compile-time mapping for zero runtime overhead
- **Outbox Pattern**: Reliable event publishing
- **Repository Pattern**: Data access abstraction

## 🚀 Quick Start

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [MongoDB](https://www.mongodb.com/try/download/community) (or Docker)
- [Git](https://git-scm.com/)

### Installation

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd datnet-workout-tracker
   ```

2. **Navigate to the solution**
   ```bash
   cd src/dotFitness.WorkoutTracker
   ```

3. **Start MongoDB** (using Docker)
   ```bash
   docker-compose up -d
   ```

4. **Restore dependencies**
   ```bash
   dotnet restore
   ```

5. **Build the solution**
   ```bash
   dotnet build
   ```

6. **Run the application**
   ```bash
   dotnet run --project dotFitness.Api
   ```

7. **Access the API**
   - API: https://localhost:7001
   - Swagger Documentation: https://localhost:7001 (served at root)
   - Health Checks: https://localhost:7001/health
   - **Google OAuth**: Use the Authorize button in Swagger UI to authenticate

### Configuration

Update `dotFitness.Api/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "MongoDB": "mongodb://admin:password@localhost:27017/dotFitnessDb?authSource=admin"
  },
  "AdminSettings": {
    "AdminEmails": ["your.admin.email@gmail.com"]
  },
  "JwtSettings": {
    "SecretKey": "your-secret-key-here",
    "Issuer": "dotFitness",
    "Audience": "dotFitness",
    "ExpirationHours": 24
  },
  "GoogleOAuth": {
    "ClientId": "your-google-client-id",
    "ClientSecret": "your-google-client-secret",
    "RedirectUri": "https://localhost:7001/swagger/oauth2-redirect.html"
  }
}
```

**Note**: For Google OAuth integration, follow the [Google OAuth Setup Guide](doc/GOOGLE_OAUTH_SWAGGER_SETUP.md) to configure your Google Cloud Console credentials.
```

## 📚 Documentation

- **[Architecture Guide](doc/ARCHITECTURE.md)**: Detailed architecture overview
- **[Adding New Modules](doc/ADD_NEW_MODULE.md)**: Step-by-step module creation
- **[Development Guide](doc/STEP_BY_STEP.md)**: Complete development workflow
- **[UI Design](doc/UI_DESIGN.md)**: Frontend architecture and design system
- **[Technical Specifications](doc/TECHNICAL.md)**: Technical requirements and patterns
- **[Project Status](doc/PROJECT_STATUS.md)**: Current implementation status
- **[Google OAuth with Swagger](doc/GOOGLE_OAUTH_SWAGGER_SETUP.md)**: Google OAuth integration with Swagger UI

## 🧩 Modules

### ✅ Users Module (Production Ready)

**Features:**
- Google OAuth authentication
- User profile management
- Body metrics tracking with BMI calculation
- Admin role assignment
- JWT token generation

**API Endpoints:**
- `POST /api/v1/auth/google` - Google OAuth login
- `GET /api/v1/users/profile` - Get user profile
- `PUT /api/v1/users/profile` - Update user profile
- `POST /api/v1/users/metrics` - Add body metrics
- `GET /api/v1/users/metrics` - Get user metrics

### ✅ Exercises Module (Production Ready)

**Features:**
- Exercise creation and management
- Muscle group and equipment tracking
- Global and user-specific exercises
- Video URL support
- Search and filtering

**API Endpoints:**
- `GET /api/v1/exercises` - Get all exercises
- `GET /api/v1/exercises/{id}` - Get exercise by ID
- `POST /api/v1/exercises` - Create exercise
- `PUT /api/v1/exercises/{id}` - Update exercise
- `DELETE /api/v1/exercises/{id}` - Delete exercise

### 🔄 Planned Modules

- **Routines Module**: Workout routine creation and management
- **WorkoutLogs Module**: Workout tracking and analytics

## 🧪 Testing

### Run All Tests
```bash
dotnet test
```

### Run Specific Module Tests
```bash
dotnet test dotFitness.Modules.Users.Tests
dotnet test dotFitness.Modules.Exercises.Tests
```

### API Testing
Use the provided HTTP test files in the `tests/api/` directory:

```bash
# Test authentication
dotnet run --project dotFitness.Api &
curl -X POST "https://localhost:5001/api/v1/auth/google" \
  -H "Content-Type: application/json" \
  -d '{"idToken": "your-google-id-token"}'
```

## 📊 Monitoring & Observability

### Health Checks
```bash
# Overall health
curl http://localhost:5000/health

# Module-specific health
curl http://localhost:5000/health/modules
```

### Metrics
```bash
# Performance summary
curl http://localhost:5000/api/v1/metrics/summary

# Module metrics
curl http://localhost:5000/api/v1/metrics/modules
```

### Logging
The application uses structured logging with Serilog. Logs include:
- Module registration events
- Performance metrics
- Error tracking
- User actions

## 🔧 Development

### Adding a New Module

1. **Add module name to registry**
   ```csharp
   // dotFitness.Api/Infrastructure/ModuleRegistry.cs
   public static readonly string[] ModuleNames = 
   {
       "Users",
       "Exercises", 
       "YourNewModule"  // Add here
   };
   ```

2. **Create module structure**
   ```bash
   dotnet new classlib -n dotFitness.Modules.YourNewModule.Domain
   dotnet new classlib -n dotFitness.Modules.YourNewModule.Application
   dotnet new classlib -n dotFitness.Modules.YourNewModule.Infrastructure
   dotnet new xunit -n dotFitness.Modules.YourNewModule.Tests
   ```

3. **Follow the [module creation guide](doc/ADD_NEW_MODULE.md)** for detailed implementation steps.

### Code Quality

- **Clean Architecture**: Maintain proper layer boundaries
- **Result Pattern**: Use explicit error handling
- **Static Mappers**: Prefer compile-time mapping
- **Comprehensive Testing**: Aim for 80%+ code coverage
- **Documentation**: Keep documentation up to date

## 🚀 Deployment

### Docker Deployment
```bash
# Build the application
docker build -t dotfitness .

# Run with MongoDB
docker-compose up -d
```

### Production Configuration
```bash
# Set environment variables
export ASPNETCORE_ENVIRONMENT=Production
export ConnectionStrings__MongoDB="your-production-mongodb-connection"

# Run the application
dotnet run --project dotFitness.Api
```

## 🤝 Contributing

1. **Fork the repository**
2. **Create a feature branch**
   ```bash
   git checkout -b feature/your-feature-name
   ```
3. **Follow the development guidelines**
4. **Write tests for new functionality**
5. **Submit a pull request**

### Development Guidelines

- Follow Clean Architecture principles
- Use the Result pattern for error handling
- Write comprehensive tests
- Update documentation
- Follow the existing code style

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- [Modular Monolith with DDD](https://github.com/MaiQD/modular-monolith-with-ddd) - Architecture inspiration
- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html) - Architectural principles
- [Domain-Driven Design](https://martinfowler.com/bliki/DomainDrivenDesign.html) - Design methodology

## 📞 Support

- **Documentation**: Check the [docs](doc/) directory
- **Issues**: Create an issue on GitHub
- **Discussions**: Use GitHub Discussions for questions

---

**Built with ❤️ using .NET 8 and Clean Architecture** 