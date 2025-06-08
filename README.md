# DatNet Workout Tracker

A comprehensive fitness tracking application built with .NET 8 and Blazor Server, following a **Modular Monolith** architecture pattern. Track your workouts, manage exercise routines, monitor progress through analytics, and maintain your fitness journey data with a modern, responsive interface.

![.NET 8](https://img.shields.io/badge/.NET-8.0-blue)
![Blazor](https://img.shields.io/badge/Blazor-Server-purple)
![MongoDB](https://img.shields.io/badge/MongoDB-Atlas-green)
![License](https://img.shields.io/badge/License-MIT-yellow)

## ✨ Features

### Core Functionality
- **🏋️ Workout Management** - Create, track, and manage detailed workout sessions
- **📚 Exercise Library** - Comprehensive database with categories, instructions, and muscle groups
- **📋 Routine Builder** - Design and save custom workout routines with scheduling
- **📊 Progress Analytics** - Visual charts and statistics for fitness progress tracking
- **📅 Calendar Integration** - Schedule and view workout sessions on an interactive calendar
- **🔐 Secure Authentication** - Google OAuth 2.0 integration for user management
- **📱 Responsive UI** - Modern, mobile-friendly interface using Syncfusion Blazor components

### Advanced Features
- Set tracking with reps, weight, and rest times
- Exercise categorization by muscle groups and difficulty levels
- Workout history and progress visualization
- Routine scheduling for recurring workouts
- Personal fitness analytics and insights

## 🛠️ Technology Stack

### Core Technologies
| Component | Technology | Version | Purpose |
|-----------|------------|---------|---------|
| **Framework** | .NET | 8.0 | Main application framework |
| **UI Framework** | Blazor Server | 8.0 | Server-side web UI |
| **Database** | MongoDB Atlas | Latest | Document database |
| **Authentication** | Google OAuth 2.0 | - | User authentication |
| **UI Components** | Syncfusion Blazor | 27.1.48 | Professional UI components |
| **Containerization** | Docker | - | Development environment |

### Key Dependencies
- `Microsoft.AspNetCore.Authentication.Google` - OAuth integration
- `MongoDB.Driver` - Database connectivity
- `Syncfusion.Blazor.*` - UI component suite

## 🏗️ Architecture

### Modular Monolith Pattern

```
┌─────────────────────────────────────────────────────────┐
│                    WebApp (Host)                        │
│  ┌─────────────────────────────────────────────────────┐│
│  │              Blazor Server UI                       ││
│  └─────────────────────────────────────────────────────┘│
├─────────────────────────────────────────────────────────┤
│                    Module Layer                         │
│  ┌─────────┬─────────┬─────────┬─────────┬───────────┐  │
│  │  Users  │Exercises│Workouts │Routines │Analytics  │  │
│  │ Module  │ Module  │ Module  │ Module  │  Module   │  │
│  └─────────┴─────────┴─────────┴─────────┴───────────┘  │
├─────────────────────────────────────────────────────────┤
│                   Shared Layer                          │
│  └─────── Infrastructure & Common Services ─────────┘  │
├─────────────────────────────────────────────────────────┤
│                 Data Layer                              │
│  └──────────────── MongoDB Atlas ──────────────────┘  │
└─────────────────────────────────────────────────────────┘
```

### Project Structure

```
src/
├── DatNetWorkoutTracker.WebApp/          # Main Blazor Server application
├── DatNetWorkoutTracker.Shared/          # Common infrastructure and utilities
└── Modules/                              # Business domain modules
    ├── DatNetWorkoutTracker.Users/       # User management
    ├── DatNetWorkoutTracker.Exercises/   # Exercise library
    ├── DatNetWorkoutTracker.Workouts/    # Workout sessions
    ├── DatNetWorkoutTracker.Routines/    # Workout routines
    └── DatNetWorkoutTracker.Analytics/   # Progress analytics
```

## 🚀 Getting Started

### Prerequisites

- **.NET 8 SDK** or later
- **Docker & Docker Compose** (for local MongoDB)
- **IDE**: Visual Studio 2022, VS Code, or JetBrains Rider
- **Google Cloud Account** (for OAuth setup)

### Quick Start

1. **Clone the Repository**
   ```bash
   git clone <repository-url>
   cd datnet-workout-tracker
   ```

2. **Start MongoDB**
   ```bash
   docker-compose up -d
   ```

3. **Configure Google OAuth**
   
   Create `src/DatNetWorkoutTracker.WebApp/appsettings.Development.json`:
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
   - Navigate to `https://localhost:5001`

### Google OAuth Setup

1. **Create Google Cloud Project**
   - Go to [Google Cloud Console](https://console.cloud.google.com/)
   - Create a new project

2. **Enable APIs & Create Credentials**
   - Enable Google+ API
   - Create OAuth 2.0 Client ID
   - Add redirect URI: `https://localhost:5001/signin-google`

3. **Update Configuration**
   - Add ClientId and ClientSecret to `appsettings.Development.json`

## 📖 Documentation

For comprehensive technical documentation, see [TECHNICAL_DOCUMENTATION.md](./TECHNICAL_DOCUMENTATION.md)

### Key Topics Covered
- **Architecture Details** - Modular monolith design patterns
- **Database Design** - MongoDB collections and relationships
- **API Specifications** - Service interfaces and endpoints
- **Deployment Guide** - Docker and production deployment
- **Performance Optimization** - Caching and scaling strategies

## 🧪 Development

### Running Tests
```bash
dotnet test
```

### Database Management
```bash
# Start MongoDB
docker-compose up -d

# Stop MongoDB
docker-compose down

# View logs
docker-compose logs mongo
```

### Building for Production
```bash
dotnet publish src/DatNetWorkoutTracker.WebApp -c Release -o ./publish
```

## 🚢 Deployment

### Docker Deployment
The application includes full Docker support for containerized deployment.

### Supported Platforms
- **Azure App Service** (recommended)
- **AWS Elastic Beanstalk**
- **Google Cloud Run**
- **Self-hosted with Docker**

## 🤝 Contributing

We welcome contributions! Please see our [Contributing Guidelines](./CONTRIBUTING.md) for details.

### Development Guidelines
- Follow SOLID principles
- Maintain modular architecture
- Include comprehensive tests
- Update documentation

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- **Syncfusion** for the excellent Blazor UI components
- **MongoDB** for the flexible document database
- **Google** for OAuth authentication services

---

**Built with ❤️ using .NET 8 and Blazor Server**

This project is licensed under the MIT License - see the LICENSE file for details.
