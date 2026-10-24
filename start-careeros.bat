@echo off
title CareerOS Launcher
echo ====================================================
echo          Launching CareerOS (Local-First)
echo ====================================================

if not exist data mkdir data

echo [1/2] Starting Backend API on http://localhost:5000 ...
start "CareerOS Backend API" dotnet run --project src\CareerOS.API\CareerOS.API.csproj --urls http://localhost:5000

echo [2/2] Starting Frontend UI on http://localhost:3000 ...
cd frontend\careeros-ui
npm run dev

