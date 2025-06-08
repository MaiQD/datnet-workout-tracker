# DatNet Workout Tracker

A cross-platform fitness tracking application built with .NET MAUI Blazor and Blazor Web App sharing a common code base.

## Features

- Track your workouts with detailed exercise information
- Store your exercise library with YouTube tutorials and equipment requirements
- Calendar view to visualize your workout history
- Works on web, mobile, and desktop platforms from a single codebase
- Offline data synchronization for mobile devices

## Technology Stack

- **.NET 8** - Latest .NET platform
- **Blazor Web App** - For web interface
- **.NET MAUI Blazor Hybrid** - For mobile and desktop apps
- **MongoDB Atlas** - Cloud database for data storage
- **Syncfusion Blazor UI** - Component library (Community License)
- **Shared Razor Components** - Reusable UI components across web and mobile

## Project Structure

- **FitnessTracker.RazorComponents** - Shared library with models, services, and UI components
- **FitnessTracker.Web** - Blazor Web App for browser access
- **FitnessTracker.MauiClient** - MAUI Blazor app for iOS, Android, macOS and Windows

## Getting Started

### Prerequisites

- .NET 8 SDK
- Visual Studio 2022 or Visual Studio Code
- MAUI workload installed for mobile development

### Setup

1. Clone the repository
2. Configure MongoDB connection string in `FitnessTracker.RazorComponents/appsettings.json` and `FitnessTracker.MauiClient/appsettings.json`
3. Register for a free Syncfusion Community license and add it to appsettings.json

### Running the Web App

```
cd FitnessTracker.Web/FitnessTracker.Web
dotnet run
```

### Running the MAUI App

For Android:
```
cd FitnessTracker.MauiClient
dotnet build -t:Run -f net8.0-android
```

For iOS (requires Mac):
```
cd FitnessTracker.MauiClient
dotnet build -t:Run -f net8.0-ios
```

## License

This project is licensed under the MIT License - see the LICENSE file for details.
