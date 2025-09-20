# dotFitness Test Runner Script (PowerShell)
# Usage: .\scripts\run-tests.ps1 [unit|integration|database|all] [Debug|Release]

param(
    [Parameter(Position=0)]
    [ValidateSet("unit", "integration", "database", "all")]
    [string]$TestCategory = "all",
    
    [Parameter(Position=1)]
    [ValidateSet("Debug", "Release")]
    [string]$Configuration = "Release"
)

$ErrorActionPreference = "Stop"
$ProjectDir = "src/dotFitness.WorkoutTracker"

# Colors for output
$Red = "`e[31m"
$Green = "`e[32m"
$Yellow = "`e[33m"
$Blue = "`e[34m"
$Reset = "`e[0m"

Write-Host "${Blue}🏃‍♂️ dotFitness Test Runner${Reset}"
Write-Host "${Blue}========================${Reset}"
Write-Host "Test Category: ${Yellow}$TestCategory${Reset}"
Write-Host "Configuration: ${Yellow}$Configuration${Reset}"
Write-Host ""

# Change to project directory
Set-Location $ProjectDir

try {
    # Build the solution
    Write-Host "${Blue}🔨 Building solution...${Reset}"
    dotnet build --configuration $Configuration --verbosity normal
    
    if ($LASTEXITCODE -ne 0) {
        throw "Build failed!"
    }
    
    Write-Host "${Green}✅ Build successful!${Reset}"
    Write-Host ""
    
    # Run tests based on category
    $testCommand = @(
        "test",
        "--no-build",
        "--configuration", $Configuration,
        "--verbosity", "normal",
        "--collect:XPlat Code Coverage",
        "--results-directory", "./TestResults",
        "--logger", "trx",
        "--settings", "coverlet.runsettings",
        "--",
        "DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=cobertura"
    )
    
    switch ($TestCategory) {
        "unit" {
            Write-Host "${Blue}🧪 Running unit tests...${Reset}"
            $testCommand += "--filter", "Category!=Integration&Category!=Database"
        }
        "integration" {
            Write-Host "${Blue}🔗 Running integration tests...${Reset}"
            $testCommand += "--filter", "Category=Integration"
        }
        "database" {
            Write-Host "${Blue}🗄️ Running database tests...${Reset}"
            $testCommand += "--filter", "Category=Database"
        }
        "all" {
            Write-Host "${Blue}🧪 Running all tests...${Reset}"
        }
    }
    
    & dotnet @testCommand
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "${Green}✅ Tests completed successfully!${Reset}"
        
        # Generate coverage report if coverage files exist
        $coverageFiles = Get-ChildItem -Path "TestResults" -Recurse -Filter "coverage.cobertura.xml" -ErrorAction SilentlyContinue
        
        if ($coverageFiles) {
            Write-Host "${Blue}📊 Generating coverage report...${Reset}"
            
            # Install report generator if not already installed
            try {
                dotnet tool install --global dotnet-reportgenerator-globaltool --version 5.2.0 --verbosity quiet 2>$null
            }
            catch {
                # Tool might already be installed, ignore error
            }
            
            # Generate report
            & reportgenerator `
                -reports:"TestResults/**/coverage.cobertura.xml" `
                -targetdir:"TestResults/CoverageReport" `
                -reporttypes:"Html;Cobertura" `
                -assemblyfilters:"-*.Tests*" `
                -verbosity:Warning
            
            if ($LASTEXITCODE -eq 0) {
                Write-Host "${Green}✅ Coverage report generated: TestResults/CoverageReport/index.html${Reset}"
            }
            else {
                Write-Host "${Yellow}⚠️ Coverage report generation failed${Reset}"
            }
        }
        
        Write-Host ""
        Write-Host "${Green}🎉 All done!${Reset}"
    }
    else {
        throw "Tests failed!"
    }
}
catch {
    Write-Host "${Red}❌ Error: $($_.Exception.Message)${Reset}"
    exit 1
}
finally {
    # Return to original directory
    Set-Location -Path "..\.."
}
