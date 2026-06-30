#!/usr/bin/env bash
# Builds the solution and runs all tests with code coverage.
set -euo pipefail
echo "Restoring..."
dotnet restore
echo "Building (Release)..."
dotnet build --configuration Release --no-restore
echo "Running tests with coverage..."
dotnet test --configuration Release --no-build --collect:"XPlat Code Coverage"
echo "Done."
