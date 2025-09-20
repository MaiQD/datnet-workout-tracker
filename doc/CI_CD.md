# CI/CD Documentation

This document describes the Continuous Integration and Continuous Deployment setup for the dotFitness Workout Tracker project.

## Overview

The project uses GitHub Actions for CI/CD with comprehensive testing, coverage reporting, and security scanning. The pipeline is designed to support the modular monolith architecture with clean separation of concerns.

## Workflows

### 1. PR Build (`pr-build.yml`)
**Trigger**: Pull requests to `main` or `develop` branches
**Purpose**: Validates code changes before merging

**Features**:
- Builds the entire solution
- Runs unit tests (excluding integration/database tests)
- Runs integration tests (including database tests)
- Generates test coverage reports
- Performs security vulnerability scans
- Uploads test results and coverage reports as artifacts

**Test Categories**:
- **Unit Tests**: `Category!=Integration&Category!=Database`
- **Integration Tests**: `Category=Integration|Category=Database`

### 2. Main Build (`main-build.yml`)
**Trigger**: Pushes to `main` branch
**Purpose**: Full validation and deployment preparation

**Features**:
- Same as PR build but runs all tests together
- Generates comprehensive coverage reports
- Uploads artifacts for deployment

### 3. Test Only (`test-only.yml`)
**Trigger**: Manual workflow dispatch
**Purpose**: Run specific test categories for development/debugging

**Options**:
- **Test Category**: `all`, `unit`, `integration`, `database`
- **Configuration**: `Debug`, `Release`

## Test Infrastructure

### Services
- **MongoDB 7.0**: For integration tests
- **PostgreSQL 15**: For database integration tests

### Test Frameworks
- **xUnit**: Primary testing framework
- **FluentAssertions**: Readable test assertions
- **Moq**: Mocking framework
- **Testcontainers**: Containerized database testing

### Coverage
- **Coverlet**: Code coverage collection
- **ReportGenerator**: Coverage report generation
- **Codecov**: Coverage reporting integration

## Test Categories

Tests are categorized using the `[Category]` attribute:

```csharp
[Fact]
[Category("Unit")]
public void Should_Create_Valid_Entity() { }

[Fact]
[Category("Integration")]
public void Should_Handle_Repository_Operations() { }

[Fact]
[Category("Database")]
public void Should_Persist_Data_To_Database() { }
```

## Running Tests Locally

### All Tests
```bash
cd src/dotFitness.WorkoutTracker
dotnet test --configuration Release
```

### Unit Tests Only
```bash
dotnet test --configuration Release --filter "Category!=Integration&Category!=Database"
```

### Integration Tests Only
```bash
dotnet test --configuration Release --filter "Category=Integration|Category=Database"
```

### With Coverage
```bash
dotnet test --configuration Release --collect:"XPlat Code Coverage" --settings coverlet.runsettings
```

### Using Helper Scripts

#### Bash (Linux/macOS)
```bash
# Run all tests
./scripts/run-tests.sh

# Run unit tests only
./scripts/run-tests.sh unit

# Run with Debug configuration
./scripts/run-tests.sh all Debug
```

#### PowerShell (Windows)
```powershell
# Run all tests
.\scripts\run-tests.ps1

# Run unit tests only
.\scripts\run-tests.ps1 unit

# Run with Debug configuration
.\scripts\run-tests.ps1 all Debug
```

## Artifacts

Each workflow generates:
- **Test Results**: TRX files with detailed test execution results
- **Coverage Report**: HTML and Cobertura format coverage reports
- **Build Logs**: Detailed build and test execution logs

## Configuration

### Environment Variables
- `DOTNET_VERSION`: .NET SDK version (9.0.305)
- `DOTNET_SKIP_FIRST_TIME_EXPERIENCE`: Skip first-time setup
- `DOTNET_CLI_TELEMETRY_OPTOUT`: Disable telemetry
- `DOTNET_NOLOGO`: Suppress .NET logo

### Caching
- **NuGet Packages**: Cached based on project file hashes
- **Build Outputs**: Cached for faster subsequent builds

## Integration with Project Architecture

### Modular Monolith Support
The CI/CD pipeline is designed to work with the modular monolith architecture:

- **Module Isolation**: Each module's tests run independently
- **Clean Architecture**: Respects domain/application/infrastructure boundaries
- **Dependency Management**: Uses centralized package management via `Directory.Packages.props`

### Test Categories by Module
- **Users Module**: Comprehensive unit and integration tests
- **Exercises Module**: Unit and integration tests
- **SharedKernel**: Cross-cutting concerns and utilities

### Database Testing Strategy
- **Unit Tests**: Use in-memory databases (Entity Framework Core)
- **Integration Tests**: Use Testcontainers for MongoDB and PostgreSQL
- **Schema Isolation**: Module-specific database fixtures

## Security

### Vulnerability Scanning
- Security scans check for vulnerable NuGet packages
- All dependencies are scanned for known vulnerabilities
- Failed security scans will fail the entire build

### Best Practices
- No secrets stored in code
- Use configuration for sensitive values
- Regular dependency updates

## Troubleshooting

### Common Issues

1. **Test Failures**: Check the test results artifact for detailed failure information
2. **Coverage Issues**: Ensure tests are properly categorized and coverage settings are correct
3. **Service Health**: MongoDB and PostgreSQL services are health-checked before tests run

### Debugging

1. Use the "Test Only" workflow for specific test categories
2. Check the "Actions" tab in GitHub for detailed logs
3. Download artifacts for local analysis

### Performance Issues

1. **Slow Builds**: Check cache hit rates in workflow logs
2. **Test Timeouts**: Ensure database services are healthy
3. **Memory Issues**: Consider running tests in smaller batches

## Future Enhancements

### Planned Features
- **Performance Testing**: Add performance benchmarks
- **Load Testing**: API load testing with realistic data
- **Deployment Automation**: Automated deployment to staging/production
- **Monitoring Integration**: Application performance monitoring

### Scalability Considerations
- **Parallel Test Execution**: Run tests in parallel for faster feedback
- **Matrix Builds**: Test against multiple .NET versions
- **Cross-Platform Testing**: Test on Windows, Linux, and macOS

## Related Documentation

- [Architecture](ARCHITECTURE.md): Overall system architecture
- [Technical](TECHNICAL.md): Technical implementation details
- [Project Status](PROJECT_STATUS.md): Current project status and testing coverage
- [Add New Module](ADD_NEW_MODULE.md): Guidelines for adding new modules with tests
