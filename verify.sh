#!/usr/bin/env bash
# Builds the solution and runs all tests with code coverage.
# Runs from the script's own folder, so you can start it from anywhere.
set -euo pipefail
cd "$(dirname "$0")"
echo "Restoring..."
dotnet restore
echo "Building (Release)..."
dotnet build --configuration Release --no-restore
echo "Running tests with coverage..."
dotnet test --configuration Release --no-build --collect:"XPlat Code Coverage"
echo "Done."
