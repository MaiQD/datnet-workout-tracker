# DatNet Workout Tracker

A cross-platform fitness tracking application built with .NET API and Blazor Web App for web

## Features

- Track your workouts with sets, reps, and weights
- Create and manage workout routines
- View workout history and progress
- Track your workouts with detailed exercise information
- Store your exercise library with YouTube tutorials and equipment requirements
- Calendar view to visualize your workout history
- Responsive design for web and mobile
- Syncfusion Blazor UI components for rich user experience

## Technology Stack

- **.NET 8** - Latest .NET platform
- **Blazor Web App** - For web interface
- **MongoDB Atlas** - Cloud database for data storage
- **Syncfusion Blazor UI** - Component library (Community License)

## Project Structure

Following Modular Monolith architecture, the solution is divided into several projects:


## Getting Started

### Prerequisites

- .NET 8 SDK
- Visual Studio 2022, Rider or Visual Studio Code

### Setup

1. Clone the repository
2. Run docker-compose to start MongoDB and other services
   ```bash
   docker-compose up -d
   ```
3. Configure MongoDB connection string 
4. Register for a free Syncfusion Community license and add it to appsettings.json

### Running the Web App

```
dotnet run
```

## License

This project is licensed under the MIT License - see the LICENSE file for details.
