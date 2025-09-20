#!/bin/bash

# dotFitness Test Runner Script
# Usage: ./scripts/run-tests.sh [unit|integration|database|all] [Debug|Release]

set -e

# Default values
TEST_CATEGORY=${1:-all}
CONFIGURATION=${2:-Release}
PROJECT_DIR="src/dotFitness.WorkoutTracker"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

echo -e "${BLUE}🏃‍♂️ dotFitness Test Runner${NC}"
echo -e "${BLUE}========================${NC}"
echo -e "Test Category: ${YELLOW}$TEST_CATEGORY${NC}"
echo -e "Configuration: ${YELLOW}$CONFIGURATION${NC}"
echo ""

# Change to project directory
cd "$PROJECT_DIR"

# Build the solution
echo -e "${BLUE}🔨 Building solution...${NC}"
dotnet build --configuration $CONFIGURATION --verbosity normal

if [ $? -ne 0 ]; then
    echo -e "${RED}❌ Build failed!${NC}"
    exit 1
fi

echo -e "${GREEN}✅ Build successful!${NC}"
echo ""

# Run tests based on category
case $TEST_CATEGORY in
    "unit")
        echo -e "${BLUE}🧪 Running unit tests...${NC}"
        dotnet test --no-build --configuration $CONFIGURATION --verbosity normal \
            --collect:"XPlat Code Coverage" \
            --results-directory ./TestResults \
            --logger trx \
            --settings coverlet.runsettings \
            --filter "Category!=Integration&Category!=Database" \
            -- \
            DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=cobertura
        ;;
    "integration")
        echo -e "${BLUE}🔗 Running integration tests...${NC}"
        dotnet test --no-build --configuration $CONFIGURATION --verbosity normal \
            --collect:"XPlat Code Coverage" \
            --results-directory ./TestResults \
            --logger trx \
            --settings coverlet.runsettings \
            --filter "Category=Integration" \
            -- \
            DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=cobertura
        ;;
    "database")
        echo -e "${BLUE}🗄️ Running database tests...${NC}"
        dotnet test --no-build --configuration $CONFIGURATION --verbosity normal \
            --collect:"XPlat Code Coverage" \
            --results-directory ./TestResults \
            --logger trx \
            --settings coverlet.runsettings \
            --filter "Category=Database" \
            -- \
            DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=cobertura
        ;;
    "all")
        echo -e "${BLUE}🧪 Running all tests...${NC}"
        dotnet test --no-build --configuration $CONFIGURATION --verbosity normal \
            --collect:"XPlat Code Coverage" \
            --results-directory ./TestResults \
            --logger trx \
            --settings coverlet.runsettings \
            -- \
            DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=cobertura
        ;;
    *)
        echo -e "${RED}❌ Invalid test category: $TEST_CATEGORY${NC}"
        echo -e "Valid options: ${YELLOW}unit${NC}, ${YELLOW}integration${NC}, ${YELLOW}database${NC}, ${YELLOW}all${NC}"
        exit 1
        ;;
esac

if [ $? -eq 0 ]; then
    echo -e "${GREEN}✅ Tests completed successfully!${NC}"
    
    # Generate coverage report if coverage files exist
    if [ -d "TestResults" ] && [ "$(ls -A TestResults/*/coverage.cobertura.xml 2>/dev/null)" ]; then
        echo -e "${BLUE}📊 Generating coverage report...${NC}"
        
        # Install report generator if not already installed
        dotnet tool install --global dotnet-reportgenerator-globaltool --version 5.2.0 --verbosity quiet 2>/dev/null || true
        
        # Generate report
        reportgenerator \
            -reports:"TestResults/**/coverage.cobertura.xml" \
            -targetdir:"TestResults/CoverageReport" \
            -reporttypes:"Html;Cobertura" \
            -assemblyfilters:"-*.Tests*" \
            -verbosity:Warning
        
        if [ $? -eq 0 ]; then
            echo -e "${GREEN}✅ Coverage report generated: TestResults/CoverageReport/index.html${NC}"
        else
            echo -e "${YELLOW}⚠️ Coverage report generation failed${NC}"
        fi
    fi
    
    echo ""
    echo -e "${GREEN}🎉 All done!${NC}"
else
    echo -e "${RED}❌ Tests failed!${NC}"
    exit 1
fi
