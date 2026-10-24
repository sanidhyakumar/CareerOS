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
│                           │   /app/data/careeros.db  │  │
│                           └──────────────────────────┘  │
└─────────────────────────────────────────────────────────┘
```

## Clean Architecture Layers

```
CareerOS.Domain          → Entities, Interfaces, Enums (no deps)
    ↑
CareerOS.Application     → Use Cases, MediatR, DTOs, Validators
    ↑
CareerOS.Infrastructure  → EF Core, SQLite, Repositories
    ↑
CareerOS.API             → Controllers, Middleware, DI, Swagger
```

## Features (Phase 0)

- ✅ **Job Tracker** — Track every job application with status pipeline
- ✅ **Contact Manager** — Manage your professional network
- ✅ **Resume Vault** — Store and version your resumes
- ✅ **Dashboard** — Real-time stats on your job search
- ✅ **Local-First** — SQLite, all data stays on your machine
- ✅ **Swagger UI** — Explore the API at `/swagger`

## Getting Started

### Option 1: Docker (Recommended)

**Prerequisites:** Docker Desktop

```bash
# Clone and launch
git clone <repo-url>
cd CareerOS

# Create data directory
mkdir data

# Launch everything
docker compose up --build

# Open CareerOS
open http://localhost:3000

# Explore the API
open http://localhost:5000/swagger
```

### Option 2: Local Development

**Prerequisites:** .NET 9 SDK, Node.js 22, pnpm

```bash
# 1. Backend
cd src/CareerOS.API
dotnet run

# 2. Frontend (new terminal)
cd frontend/careeros-ui
pnpm install
pnpm dev

# Open
open http://localhost:5173
```

## Tech Stack

| Layer | Technology |
|-------|-----------|
| Frontend | React 18 + TypeScript + Material UI v6 + Vite |
| Backend | ASP.NET Core 9 + MediatR + FluentValidation |
| Database | SQLite via Entity Framework Core 9 |
| Logging | Serilog |
| Mapping | AutoMapper |
| API Docs | Swagger / OpenAPI |
| Container | Docker + Docker Compose |
| CI/CD | GitHub Actions |

## Project Structure

```
CareerOS/
├── src/
│   ├── CareerOS.Domain/          # Entities, Value Objects, Interfaces
│   ├── CareerOS.Application/     # CQRS Handlers, DTOs, Validators
│   ├── CareerOS.Infrastructure/  # EF Core, SQLite, Repositories
│   └── CareerOS.API/             # Controllers, Middleware, Startup
├── frontend/
│   └── careeros-ui/              # React + TypeScript + MUI
├── docker/                       # Dockerfiles + nginx config
├── .github/workflows/            # CI/CD
├── data/                         # SQLite database (local)
└── docker-compose.yml
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
