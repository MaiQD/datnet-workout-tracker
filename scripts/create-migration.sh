#!/bin/bash

# dotFitness - Simple Migration Creation Script
# Usage: ./create-migration.sh <module> <migration-name>

set -e

# Colors
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m'

# Get script directory and project root
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"

# Module paths - using functions for better compatibility
get_module_path() {
    case "$1" in
        "users") echo "$PROJECT_ROOT/src/dotFitness.WorkoutTracker/Modules/Users/dotFitness.Modules.Users.Infrastructure" ;;
        "exercises") echo "$PROJECT_ROOT/src/dotFitness.WorkoutTracker/Modules/Exercises/dotFitness.Modules.Exercises.Infrastructure" ;;
        "routines") echo "$PROJECT_ROOT/src/dotFitness.WorkoutTracker/Modules/Routines/dotFitness.Modules.Routines.Infrastructure" ;;
        "workoutlogs") echo "$PROJECT_ROOT/src/dotFitness.WorkoutTracker/Modules/WorkoutLogs/dotFitness.Modules.WorkoutLogs.Infrastructure" ;;
        *) echo "" ;;
    esac
}

# Available modules
AVAILABLE_MODULES="users exercises routines workoutlogs"

# Show usage
show_usage() {
    echo -e "${BLUE}Usage:${NC} $0 <module> <migration-name>"
    echo ""
    echo -e "${BLUE}Available modules:${NC}"
    for module in $AVAILABLE_MODULES; do
        echo "  - $module"
    done
    echo ""
    echo -e "${BLUE}Examples:${NC}"
    echo "  $0 users AddUserPreferences"
    echo "  $0 exercises UpdateExerciseSchema"
    echo "  $0 routines CreateRoutineTemplate"
}

# Check for help options
if [ "$1" = "-h" ] || [ "$1" = "--help" ] || [ "$1" = "-?" ]; then
    show_usage
    exit 0
fi

# Check arguments
if [ $# -ne 2 ]; then
    echo -e "${RED}Error: Missing required arguments${NC}"
    show_usage
    exit 1
fi

MODULE=$1
MIGRATION_NAME=$2

# Check if module exists
MODULE_PATH=$(get_module_path "$MODULE")
if [ -z "$MODULE_PATH" ]; then
    echo -e "${RED}Error: Unknown module '$MODULE'${NC}"
    show_usage
    exit 1
fi

# Check if module path exists
if [ ! -d "$MODULE_PATH" ]; then
    echo -e "${RED}Error: Module path does not exist: $MODULE_PATH${NC}"
    echo -e "${YELLOW}Note: This module might not be implemented yet.${NC}"
    exit 1
fi

# Find project file
PROJECT_FILE=$(find "$MODULE_PATH" -name "*.csproj" | head -1)
if [ -z "$PROJECT_FILE" ]; then
    echo -e "${RED}Error: No .csproj file found in module path: $MODULE_PATH${NC}"
    exit 1
fi

echo -e "${BLUE}Creating migration for $MODULE module...${NC}"
echo -e "${BLUE}Migration: $MIGRATION_NAME${NC}"
echo -e "${BLUE}Project: $PROJECT_FILE${NC}"
echo ""

# Build project first
echo -e "${YELLOW}Building project...${NC}"
if ! dotnet build "$PROJECT_FILE" --verbosity quiet; then
    echo -e "${RED}Error: Build failed. Please fix build errors before creating migration.${NC}"
    exit 1
fi
echo -e "${GREEN}✓ Project built successfully${NC}"

# Create migration
echo -e "${YELLOW}Creating migration '$MIGRATION_NAME'...${NC}"
echo -e "${YELLOW}Note: Database connection errors are normal when creating migrations.${NC}"
if dotnet ef migrations add "$MIGRATION_NAME" --project "$PROJECT_FILE" --no-build; then
    echo -e "${GREEN}✓ Migration '$MIGRATION_NAME' created successfully!${NC}"
    echo ""
    echo -e "${BLUE}Next steps:${NC}"
    echo "  1. Review migration files in: $MODULE_PATH/Migrations/"
    echo "  2. Apply migration: dotnet ef database update --project $PROJECT_FILE"
    echo "  3. Test your changes"
    echo ""
    echo -e "${BLUE}To undo this migration:${NC}"
    echo "  dotnet ef migrations remove --project $PROJECT_FILE"
    echo ""
    echo -e "${YELLOW}Note: If you see EF Core tools version warnings, update with:${NC}"
    echo "  dotnet tool update --global dotnet-ef"
else
    echo -e "${RED}Error: Failed to create migration '$MIGRATION_NAME'${NC}"
    exit 1
fi
