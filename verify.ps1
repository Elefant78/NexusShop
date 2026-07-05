# Builds the solution and runs all tests with code coverage.
# Runs from the script's own folder, so you can start it from anywhere.
# Usage:  .\verify.ps1   (in PowerShell)
$ErrorActionPreference = "Stop"
Set-Location -Path $PSScriptRoot

function Invoke-Step($label, [scriptblock]$cmd) {
    Write-Host $label -ForegroundColor Cyan
    & $cmd
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Step failed (exit code $LASTEXITCODE)." -ForegroundColor Red
        exit $LASTEXITCODE
    }
}

Invoke-Step "Restoring..."              { dotnet restore }
Invoke-Step "Building (Release)..."     { dotnet build --configuration Release --no-restore }
Invoke-Step "Running tests with coverage..." { dotnet test --configuration Release --no-build --collect:"XPlat Code Coverage" }

Write-Host "Done." -ForegroundColor Green
