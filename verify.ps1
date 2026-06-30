# Builds the solution and runs all tests with code coverage.
# Usage:  pwsh ./verify.ps1   (or run in Developer PowerShell for VS)
$ErrorActionPreference = "Stop"

Write-Host "Restoring..." -ForegroundColor Cyan
dotnet restore

Write-Host "Building (Release)..." -ForegroundColor Cyan
dotnet build --configuration Release --no-restore

Write-Host "Running tests with coverage..." -ForegroundColor Cyan
dotnet test --configuration Release --no-build --collect:"XPlat Code Coverage"

Write-Host "Done." -ForegroundColor Green
