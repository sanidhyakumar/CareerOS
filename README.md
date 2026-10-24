# 🚀 CareerOS

> **100% Local-First Career Management Operating System**

CareerOS is a self-hosted, privacy-first career management platform that runs entirely on your machine. No cloud, no subscriptions, no data leaks — just you and your career data.

---

## Architecture

```
┌─────────────────────────────────────────────────────────┐
│                    CareerOS System                       │
│                                                         │
│  ┌──────────────┐         ┌──────────────────────────┐  │
│  │  React UI    │ ──────► │   ASP.NET Core 9 API     │  │
│  │  Port :3000  │  HTTP   │   Port :5000             │  │
│  │  MUI + Vite  │         │   MediatR + CQRS         │  │
│  └──────────────┘         └────────────┬─────────────┘  │
│                                        │                 │
│                           ┌────────────▼─────────────┐  │
│                           │   SQLite Database        │  │
│                           │   data/careeros.db       │  │
│                           └──────────────────────────┘  │
└─────────────────────────────────────────────────────────┘
```

## Clean Architecture Layers

```
CareerOS.Domain          → Entities, Interfaces, Enums (no external deps)
    ↑
CareerOS.Application     → CQRS Handlers, MediatR, FluentValidation, AutoMapper
    ↑
CareerOS.Infrastructure  → EF Core 9, SQLite, Repositories, Unit of Work
    ↑
CareerOS.API             → Controllers, Middleware, Serilog, Swagger
```

## Features (Phase 0)

- ✅ **Job Tracker** — Track every job application across 8 pipeline stages
- ✅ **Contact Manager** — Maintain your professional network & CRM
- ✅ **Resume Vault** — Store, tag, and manage resume versions with default selection
- ✅ **Dashboard** — Real-time metrics on applications, interviews, and offers
- ✅ **Local-First & Offline** — SQLite persistence, all data stays on your machine
- ✅ **Swagger UI** — Interactive OpenAPI documentation at `/swagger`

---

## 🏃 How to Run CareerOS

### Method 1: Instant Quick Start (No Docker Required)

Run CareerOS locally on your machine with a single command:

#### PowerShell:
```powershell
.\start-careeros.ps1
```

#### Windows Command Prompt / Double-Click:
```cmd
start-careeros.bat
```

This automatically starts both the backend API (port 5000) and the React frontend (port 3000).

---

### Method 2: Manual Local Development

**Prerequisites:**
- [.NET SDK](https://dotnet.microsoft.com/download) (.NET 9 or .NET 10 preview)
- [Node.js](https://nodejs.org/) (v20+ or v22+) & npm

```powershell
# 1. Start the Backend API (Terminal 1)
dotnet run --project src\CareerOS.API\CareerOS.API.csproj --urls http://localhost:5000

# 2. Start the Frontend UI (Terminal 2)
cd frontend\careeros-ui
npm install      # only needed first time
npm run dev
```

---

### Method 3: Docker & Docker Compose

**Prerequisites:**
- [Docker Desktop for Windows](https://www.docker.com/products/docker-desktop/) installed and running.

```powershell
# Launch all containers
docker compose up --build
```

To stop containers:
```powershell
docker compose down
```

---

## 🌐 Application Endpoints

Once launched, access CareerOS via your browser:

| Service | URL | Description |
|---|---|---|
| **Web Application** | [http://localhost:3000](http://localhost:3000) | React + Material UI Web UI |
| **Swagger Explorer** | [http://localhost:5000/swagger](http://localhost:5000/swagger) | Interactive API documentation |
| **Health Check** | [http://localhost:5000/api/health](http://localhost:5000/api/health) | API health & version endpoint |
| **Dashboard Stats** | [http://localhost:5000/api/dashboard/stats](http://localhost:5000/api/dashboard/stats) | Live aggregated metrics |

---

## 🔧 Troubleshooting

### `docker: The term 'docker' is not recognized...`
This means Docker Desktop is not installed or not added to your system `PATH`.
- **Solution 1 (Instant):** Use **Method 1** (`.\start-careeros.ps1`), which runs CareerOS directly without Docker.
- **Solution 2:** Install Docker Desktop via PowerShell (as Administrator):
  ```powershell
  winget install Docker.DockerDesktop
  ```
  Then restart your terminal and launch Docker Desktop.

---

## Tech Stack

| Layer | Technology |
|---|---|
| Frontend | React 18, TypeScript, Material UI v6, Vite |
| Backend | ASP.NET Core 9, MediatR 12, FluentValidation, AutoMapper |
| Database | SQLite via Entity Framework Core 9 |
| Logging | Serilog (Console + Rolling File Sink) |
| API Docs | Swagger / OpenAPI (Swashbuckle) |
| Containerization | Docker (Multi-stage build) + Docker Compose |
| CI/CD | GitHub Actions (`.github/workflows/ci.yml`) |

## Project Structure

```
CareerOS/
├── src/
│   ├── CareerOS.Domain/          # Core entities, Enums, Interfaces, Domain Exceptions
│   ├── CareerOS.Application/     # CQRS Handlers, DTOs, Validators, AutoMapper Profiles
│   ├── CareerOS.Infrastructure/  # EF Core SQLite context, Configurations, Repositories
│   └── CareerOS.API/             # Controllers, RFC-7807 Error Middleware, Serilog
├── frontend/
│   └── careeros-ui/              # React 18 + TS + Material UI UI
├── docker/
│   ├── Dockerfile.api            # Multi-stage .NET 9 API image with curl healthcheck
│   ├── Dockerfile.ui             # Multi-stage Node build & nginx runtime
│   └── nginx.conf                # SPA router fallback & security headers
├── .github/workflows/            # CI/CD pipeline definition
├── data/                         # SQLite database storage (careeros.db)
├── start-careeros.ps1            # One-click PowerShell launcher
├── start-careeros.bat            # One-click Windows CMD launcher
├── docker-compose.yml            # Multi-container orchestration
└── CareerOS.sln                  # .NET Solution file
```

## Roadmap

- **Phase 0** ✅ Foundation & Architecture
- **Phase 1** 🔜 Job Pipeline Kanban Board
- **Phase 2** 🔜 AI-Powered Resume Tailoring
- **Phase 3** 🔜 Interview Prep & Notes
- **Phase 4** 🔜 Networking CRM
- **Phase 5** 🔜 Analytics & Insights

---

*Built with ❤️ — Local-First, Privacy-First, Career-First.*
