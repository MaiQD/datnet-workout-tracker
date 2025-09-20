# CI/CD Workflows

This directory contains GitHub Actions workflows for the dotFitness Workout Tracker project.

## Quick Reference

### Workflows
- **`pr-build.yml`**: Runs on pull requests to `main`/`develop` branches
- **`main-build.yml`**: Runs on pushes to `main` branch  
- **`test-only.yml`**: Manual trigger for specific test categories

### Test Categories
- **Unit**: `Category!=Integration&Category!=Database`
- **Integration**: `Category=Integration|Category=Database`
- **Database**: `Category=Database`

### Local Testing
```bash
# Using helper scripts
./scripts/run-tests.sh [unit|integration|database|all] [Debug|Release]

# Direct dotnet commands
cd src/dotFitness.WorkoutTracker
dotnet test --configuration Release --filter "Category=Unit"
```

## Documentation

📖 **Full CI/CD Documentation**: See [`doc/CI_CD.md`](../../doc/CI_CD.md) for comprehensive details including:
- Complete workflow descriptions
- Test infrastructure setup
- Configuration options
- Troubleshooting guide
- Integration with project architecture
