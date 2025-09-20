#!/bin/bash

# Script to add xUnit Trait attributes to test methods
# Usage: ./scripts/add-xunit-traits.sh
# 
# This script is idempotent - it will skip files that already have the traits
# and only add traits to test methods that don't already have them.

set -e

echo "🏷️ Adding xUnit traits to test methods..."

# Function to check if trait already exists in a file
trait_exists() {
    local file="$1"
    local trait_name="$2"
    local trait_value="$3"
    
    # Check if the trait already exists in the file
    grep -q "\[Trait(\"$trait_name\", \"$trait_value\")\]" "$file" 2>/dev/null
}

# Function to add Trait attribute to a file (only to methods that don't have it)
add_traits_to_file() {
    local file="$1"
    local trait_name="$2"
    local trait_value="$3"
    
    echo "Processing: $file with trait: $trait_name=$trait_value"
    
    # Count how many test methods need traits
    local total_tests=$(grep -c "\[Fact\]\|\[Theory\]" "$file" 2>/dev/null || echo "0")
    local tests_with_traits=$(grep -c "\[Trait(\"$trait_name\", \"$trait_value\")\]" "$file" 2>/dev/null || echo "0")
    
    if [ "$total_tests" -eq 0 ]; then
        echo "  ℹ️  No test methods found, skipping..."
        return 0
    fi
    
    if [ "$tests_with_traits" -eq "$total_tests" ]; then
        echo "  ✅ All $total_tests test methods already have trait $trait_name=$trait_value, skipping..."
        return 0
    fi
    
    echo "  📝 Adding traits to $((total_tests - tests_with_traits)) out of $total_tests test methods..."
    
    # Add Trait attribute after [Fact] or [Theory] but only if not already present
    awk '
    /\[Fact\]|\[Theory\]/ {
        print $0
        # Check if the next line already has a Trait attribute
        getline next_line
        if (next_line ~ /\[Trait\("'$trait_name'", "'$trait_value'"\)\]/) {
            print next_line
        } else {
            print "    [Trait(\"'$trait_name'\", \"'$trait_value'\")]"
            print next_line
        }
        next
    }
    { print }
    ' "$file" > "$file.tmp" && mv "$file.tmp" "$file"
}

# Domain tests (Unit category)
echo "📝 Adding Unit trait to Domain tests..."
for file in $(find /Users/datmai/Code/datnet-workout-tracker/src -name "*Tests.cs" -path "*/Domain/*"); do
    add_traits_to_file "$file" "Category" "Unit"
done

# Application tests (Unit category)
echo "📝 Adding Unit trait to Application tests..."
for file in $(find /Users/datmai/Code/datnet-workout-tracker/src -name "*Tests.cs" -path "*/Application/*"); do
    add_traits_to_file "$file" "Category" "Unit"
done

# Infrastructure unit tests (Unit category) - handlers without Testcontainers
echo "📝 Adding Unit trait to Infrastructure unit tests..."
for file in $(find /Users/datmai/Code/datnet-workout-tracker/src -name "*Tests.cs" -path "*/Infrastructure/Handlers/*" -not -path "*/Integrations/*"); do
    add_traits_to_file "$file" "Category" "Unit"
done

# Integration tests (Integration category) - tests using Testcontainers
echo "📝 Adding Integration trait to Integration tests..."
for file in $(find /Users/datmai/Code/datnet-workout-tracker/src -name "*Tests.cs" -path "*/Integrations/*"); do
    add_traits_to_file "$file" "Category" "Integration"
done

# Database tests (Database category) - tests using PostgreSQL
echo "📝 Adding Database trait to Database tests..."
for file in $(find /Users/datmai/Code/datnet-workout-tracker/src -name "*Tests.cs" -path "*/PostgreSQL/*"); do
    add_traits_to_file "$file" "Category" "Database"
done

echo "✅ Added xUnit traits successfully!"
