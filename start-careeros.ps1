<#
.SYNOPSIS
    Starts CareerOS (API + UI) locally without requiring Docker.
#>

Write-Host "====================================================" -ForegroundColor Cyan
Write-Host "         🚀 Starting CareerOS (Local-First)         " -ForegroundColor Cyan
Write-Host "====================================================" -ForegroundColor Cyan

# Ensure SQLite data folder exists
if (-not (Test-Path "data")) {
    New-Item -ItemType Directory -Path "data" | Out-Null
}

# 1. Start Backend API in a separate process window
Write-Host "`n[1/2] Launching Backend API (.NET 9 + SQLite on port 5000)..." -ForegroundColor Yellow
$apiProcess = Start-Process pwsh -ArgumentList "-NoExit", "-Command", "dotnet run --project src\CareerOS.API\CareerOS.API.csproj --urls http://localhost:5000" -PassThru

# 2. Start Frontend UI
Write-Host "[2/2] Launching Frontend UI (React + MUI on port 3000)..." -ForegroundColor Yellow
Write-Host "`nReady! URLs:" -ForegroundColor Green
Write-Host "  👉 CareerOS Web App : http://localhost:3000" -ForegroundColor Cyan
Write-Host "  👉 Swagger API Docs : http://localhost:5000/swagger" -ForegroundColor Cyan
Write-Host "  👉 Health Check     : http://localhost:5000/api/health" -ForegroundColor Cyan
Write-Host "`nPress Ctrl+C in this terminal to stop the UI." -ForegroundColor Gray

Set-Location "frontend\careeros-ui"
npm run dev

