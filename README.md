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

## Features

### Phase 0 — Foundation & Architecture
- ✅ **Job Tracker** — Track every job application across 8 pipeline stages
- ✅ **Contact Manager** — Maintain your professional network & CRM
- ✅ **Resume Vault** — Store, tag, and manage resume versions with default selection
- ✅ **Dashboard** — Real-time metrics on applications, interviews, and offers
- ✅ **Local-First & Offline** — SQLite persistence, all data stays on your machine
- ✅ **Swagger UI** — Interactive OpenAPI documentation at `/swagger`

### Phase 1 — Core Storage Engine
- ✅ **SQLite Schema & EF Core** — Relational storage with `Countries`, `Regions`, `Documents`, `Tags`, and `DocumentTags`
- ✅ **File Storage Service** — Local filesystem storage engine with folder segregation, stream reading, and file lifecycle
- ✅ **Metadata Engine** — Cryptographic SHA-256 hash calculation, MIME type detection, byte sizing, and version tracking
- ✅ **Document Versioning** — Immutable version chain (`version`, `isLatestVersion`, `rootDocumentId`) with parent-child linkage
- ✅ **File Operations** — Upload, new version upload, move across subfolders, download, and cascade delete
- ✅ **Document Management UI** — React + TypeScript + Material UI interface for browsing, filtering, uploading, and version viewing

### Phase 2 — Personal Profile
- ✅ **Personal Information** — Core identity, professional headline, multiple emails, phones, bio, and avatar
- ✅ **Citizenship Registry** — Multi-citizenship support, primary nationality flag, passport records, and country linkages
- ✅ **Work Authorization & Visas** — Track visas, expiration dates, country rights, and future sponsorship requirements
- ✅ **Addresses & Relocation** — Current, mailing, and target locations with remote work & relocation preference flags
- ✅ **Social & Portfolio Links** — LinkedIn, GitHub, personal site, and tech platform footprint with custom ordering
- ✅ **Unified Profile UI** — Multi-tabbed responsive React interface with live validation, dialogs, and country dropdowns

### Phase 3 — Document Vault
- ✅ **Document Vault Categories** — Segregated categorization across Identity, Academics, Certifications, Employment, Immigration, and General
- ✅ **Expiry Tracking & Proactive Alerts** — Document number, issuing authority, issue date, expiration date, days-until-expiry calculations, and visual alert indicators (Expired, Expiring Soon, Active)
- ✅ **Document Usage Tracking** — Entity-linkage audit trail (Applications, Companies, Visas, Work Authorizations) tracking where and why each document was used or submitted
- ✅ **Advanced Search & Category Filtering** — Quick tabbed filtering by category, text search across descriptions and document numbers, and expiry status filters
- ✅ **Document Version Control** — Version chains preserving previous revisions while maintaining document identity and linked usages
- ✅ **Rich Vault UI** — Modern tabbed Material UI vault interface with category badges, expiration status alerts, usage viewer dialog, and version management

### Phase 4 — Resume Center
- ✅ **Resume Profile Strategies** — Multiple career archetype personas (e.g., Staff Systems Lead, AI/ML Architect, Engineering Manager) with target roles, target industries, and skills taxonomy
- ✅ **Tailored Resume Versions & Cloning** — Company-specific resume variations, base templates, ATS keyword tailoring, change logs, and direct integration with Document Vault
- ✅ **Cover Letter Management** — Reusable cover letter templates and customized application pitches with recipient targeting and markdown support
- ✅ **Portfolio Variations** — Audience-specific portfolio links, targeted case studies, and featured project showcases
- ✅ **Submission Tracking** — End-to-end logging of where, when, and through what channel (LinkedIn, Referral, Direct) each resume version was submitted
- ✅ **Immutable Snapshot System** — Automatic cryptographic freeze of exact resume content/SHA-256, cover letter text, and portfolio URL at the moment of submission
- ✅ **Resume Strategy Analytics** — Real-time analytics measuring response rates, interview conversion, offer generation, and strategy effectiveness across channels
- ✅ **Unified Resume Center UI** — Multi-tab React + Material UI console with profile management, version clone tools, snapshot inspection modals, and performance charts

### Phase 5 — Company CRM
- ✅ **Company CRM & Database** — Store comprehensive employer records: Name, Description, Headquarters, Size, Stage, Founded Year, Websites, Tech Blog, LinkedIn, Glassdoor rating
- ✅ **Visa Sponsorship Tracking** — Track company-specific visa sponsorship policies (Yes, Case-by-Case, No), supported visa categories (H-1B, O-1, TN, E-3, Green Card), and filing notes
- ✅ **Relocation Policies** — Log relocation support tiers (Full Package, Domestic Only, International) and assistance details
- ✅ **Evaluation & Ratings** — 1-to-5 star personal ratings, engineering culture score, work-life balance rating, pros/cons, and personal assessment notes
- ✅ **Dream Targets** — Star and filter high-priority target employers
- ✅ **Search & Filtering Engine** — Filter by keyword, industry, visa sponsorship policy, minimum personal rating, and target status
- ✅ **Reusable Entity Architecture** — Foreign key integration with Jobs (`Job.CompanyId`) and Contacts (`Contact.CompanyId`)
- ✅ **Company CRM UI** — Multi-tab React + Material UI directory with grid cards and table view toggles, company stats banner, quick web/careers links, and dialog forms

### Phase 6 — Job Position Management
- ✅ **Multi-Position Tracking** — Link multiple distinct job opportunities and requisitions to a single employer/company entity
- ✅ **Compensation Architecture** — Structured base salary bounds (`MinSalary`, `MaxSalary`, `Currency`), free-form text compensation, equity packages, and bonus structures
- ✅ **Seniority & Role Categorization** — Multi-level seniority tracking (Intern, Junior, Mid, Senior, Lead, Staff, Principal, Director, VP)
- ✅ **Workplace & Employment Policies** — Remote, Hybrid, and On-Site workplace models, coupled with Full-Time, Contract, Part-Time, and Internship employment types
- ✅ **Priority Tiering** — Starred Dream Priority, High, Medium, and Low classifications to optimize job search focus
- ✅ **Tech Stack & Requirement Tagging** — Free-text and comma-delimited tech stack inventory with real-time substring search
- ✅ **Pipeline Analytics & KPI Dashboard** — Total positions, active pipeline count, dream targets, remote positions count, and average salary midpoint calculator
- ✅ **Interactive Positions Console** — Grid and table view toggles, multi-faceted filtering toolbar, comprehensive position creation and edit modal with full company dropdowns

### Phase 7 — Application Tracking
- ✅ **End-to-End Application Engine** — Track every job submission across 11 lifecycle stages (Draft, Applied, Under Review, Phone Screen, Technical Interview, Behavioral Interview, Final Round, Offer Received, Accepted, Rejected, Withdrawn)
- ✅ **Resume & Cover Letter Linkages** — Directly bind each application to specific tailored resume versions, cover letters, and portfolio editions
- ✅ **Interactive Timeline Engine** — Automatic and manual chronological milestone logs tracking date, stage evolution, feedback, and recruiter notes
- ✅ **Cryptographic Submission Snapshots** — Permanently freezes exact resume content, SHA-256 fingerprint, cover letter text, portfolio URL, and job requisition specs at the precise moment of application
- ✅ **Pipeline & Conversion Analytics** — Real-time metrics for total applications, active pipeline, live interviews, offers received, and application-to-offer conversion rate
- ✅ **Unified Applications UI Console** — Responsive Material UI interface with KPI counters, stage filter chips, cards & table view toggles, interactive timeline modal, snapshot inspector dialog, and fast stage advance shortcuts

### Phase 8 — Contact Management
- ✅ **CRM Contact Classification** — Comprehensive taxonomy supporting Recruiters, Hiring Managers, HR People Partners, Employee Referrals, Immigration Legal Counsel, Peer Colleagues, and Executives
- ✅ **Relationship & Follow-Up Tracking** — Starred favorite contacts, last contact date tracking, and automated upcoming follow-up deadline alerts
- ✅ **Interaction History & Touchpoint Log** — Full communication logs across Email, LinkedIn, Phone, Coffee Chat, Video Call, and Interviews with follow-up requirement flags
- ✅ **Employer & Job Association** — Link contacts directly to Companies and specific Job requisitions with company auto-resolution
- ✅ **CRM Analytics & Pipeline Dashboard** — Real-time KPI counters tracking total contacts, recruiters, hiring managers, employee referrals, immigration contacts, and follow-ups due
- ✅ **Modern CRM Console UI** — Multi-tab category filtering, keyword search, interactive communication history modal, fast star toggling, and direct email/phone/LinkedIn links

### Phase 9 — Interview Center
- ✅ **Comprehensive Interview Stage Coverage** — Explicit round management for Recruiter Screens, Technical Coding, System Design, Behavioral & Leadership, Engineering Manager, and Final Round Onsites
- ✅ **Timeline & Calendar Views** — Dual visual perspective with chronological stage-by-stage vertical timeline and schedule agenda cards with meeting links
- ✅ **CRM Interviewer Binding** — Link interviewers directly to CRM contact profiles with contact information and roles
- ✅ **Preparation & Questions Archive** — Dedicated storage for talking points, STAR stories to highlight, technical problem prompts, and whiteboard questions asked
- ✅ **Debrief & Self-Assessment Engine** — Instant outcome recording (Pending, Passed, Strong Hire, Hire, Neutral, Did Not Advance), debrief notes, and 1-to-5 star self-rating
- ✅ **Interview Conversion Analytics** — Real-time tracking of total rounds, upcoming pipeline, completed stages, technical rounds, system design, and overall pass rate percentage
- ✅ **Interactive Interview Center UI** — Multi-view switcher (Timeline, Calendar Schedule, Data Table), stage filter tabs, quick outcome completion modal, and direct video call launch shortcuts

### Phase 10 — Knowledge Base
- ✅ **Personal Career Wiki** — Full Markdown support, article summaries, tag indexing, slug URLs, and view tracking
- ✅ **Bi-Directional Backlink Engine** — Automatic wiki-link extraction `[[slug]]` and `[[slug|Link Title]]` generating inbound and outbound reference graphs
- ✅ **STAR Story Vault** — Dedicated behavioral interview preparation framework (Situation, Task, Action, Result) with quick-insert templates
- ✅ **System Design & Architecture Notes** — Templates for functional requirements, capacity estimation, component diagrams, and bottlenecks
- ✅ **Company & Technical Research** — Reusable company culture notes, engineering values, and reverse-interview questions
- ✅ **Tag Cloud & Category Filters** — Interactive multi-category classification (STAR Stories, System Design, Company Research, Interview Notes, Coding Patterns, Career Strategy)
- ✅ **Knowledge Base UI** — Split-screen reading/previewing pane, quick-navigation backlink network, starred/pinned favorites, and template insertion helpers

### Phase 11 — Sponsorship Center
- ✅ **Global Visa Program Registry** — Comprehensive coverage of US H-1B, L-1, O-1, TN, E-3, Canada LMIA/GTS, EU Blue Card, Australia TSS 482, and UK Skilled Worker
- ✅ **Eligibility Assessment Engine** — Multi-status classification (Eligible, Conditionally Eligible, Pending Review, Ineligible, Sponsored)
- ✅ **Availability & Quota Tracking** — Statutory availability models (Open Year-Round, Cap-Subject Annual Lottery, Quota Waitlist, Expedited Processing, Paused)
- ✅ **Salary Thresholds & Fiscal Windows** — Minimum wage bounds, currency selection, lottery registration dates, and application filing deadlines
- ✅ **Credential Evidence Checklist** — Verification tracking for degree diplomas, transcripts, credential evaluations, employment verification letters, and direct integration with Document Vault
- ✅ **PR & Dual Intent Pathways** — Direct tagging and filtering for permanent residency transitions and dual-intent immigrant visas
- ✅ **Unified Sponsorship Center UI** — Dual-perspective view switcher (Interactive Cards with progress meters and Data Table), preset template launchers, and evidence checklist manager dialog

### Phase 12 — Kanban Board
- ✅ **Interactive Drag-and-Drop Workflow** — Full `@dnd-kit` implementation (`@dnd-kit/core`, `@dnd-kit/sortable`, `@dnd-kit/utilities`) with smooth pointer/touch interactions, drop previews, and animations
- ✅ **Standard 6-Column Pipeline** — Explicit workflow columns: `Interested` ➔ `Saved` ➔ `Applied` ➔ `Interviewing` ➔ `Offer` ➔ `Rejected`
- ✅ **Cross-Entity Real-Time Synchronization** — Dragging cards between columns automatically synchronizes underlying `Job.Status` and `JobApplication.Status` lifecycles
- ✅ **Card Opportunity Details** — Priority badges (Dream, High, Medium, Low), company, compensation, location, tech tags, and notes
- ✅ **Optimistic State & Instant Persistence** — Zero-flicker optimistic drag updates coupled with backend PATCH persistence
- ✅ **Dedicated Kanban Board UI** — Column headers with color coding, card counters, quick card creator, search and priority filter toolbar

### Phase 13 — Follow-Up Engine
- ✅ **Automated Proactive Detection** — Background engine scanning for stale applications (> 7/14 days), expiring documents (< 30 days or expired), upcoming interviews (< 48 hours), and overdue contact interactions
- ✅ **Intelligent Deduplication & Severity Scoring** — Deduplication key tracking preventing duplicate alerts with multi-tier severity grading (Critical, High, Medium, Low)
- ✅ **Snooze & Dismiss Lifecycle** — Flexible snooze windows (1 day, 3 days, 1 week, 2 weeks) and resolution dismissal
- ✅ **Custom Reminders** — Ability to author custom action items and interview follow-up directives with direct navigation links
- ✅ **Follow-Up Reminder Dashboard** — Dedicated console with category filter tabs, high/critical urgency alerts, on-demand proactive scanner trigger, and instant action launchers

### Phase 14 — Global Search
- ✅ **SQLite FTS5 Full-Text Search Engine** — Virtual table `GlobalSearchEntries` utilizing SQLite's built-in FTS5 module with Porter stemming and Unicode61 tokenization
- ✅ **Unified Cross-Entity Indexing** — Comprehensive catalog indexing across 7 core entities: Companies, Jobs, Applications, Contacts, Documents, Knowledge Base (Wiki), and Interviews
- ✅ **BM25 Relevance Scoring & Snippet Generation** — Native SQLite FTS5 `bm25()` ranking function combined with `snippet()` extraction highlighting search terms in HTML `<mark>` tags
- ✅ **Resilient Fallback Mechanism** — Automatic token sanitization, prefix matching (`"term"*`), and zero-downtime LIKE query fallback for special characters or partial keywords
- ✅ **Global Quick Search Spotlight (Ctrl+K)** — Instant modal command palette accessible anywhere across CareerOS with live debounced search, keyboard navigation (↑/↓/Enter), and direct entity deep-linking
- ✅ **Dedicated Search Center UI** — Full-page search console at `/search` featuring real-time index metrics, category facet filtering, BM25 score chips, and on-demand index rebuild trigger

### Phase 15 — Analytics Dashboard
- ✅ **Executive Career Intelligence Console** — Full-spectrum KPI ribbon tracking total applications, response rate %, avg response time (days), interview pass rate %, total offers, and average compensation
- ✅ **Application Conversion Funnel** — 6-step conversion tracking (`Applied` ➔ `Phone Screen` ➔ `Technical Round` ➔ `Final Round` ➔ `Offer Received` ➔ `Offer Accepted`) with step-by-step conversion % and drop-off analysis
- ✅ **Application Velocity & Monthly Trajectory** — Dynamic `Recharts` area chart plotting monthly velocity across applications sent, interviews conducted, and offers extended over time
- ✅ **Interview Performance & Round Telemetry** — Round distribution and pass rates across Recruiter Screen, Technical Coding, System Design, Behavioral, and Final rounds with 5-star candidate ratings
- ✅ **Rejection & Drop-Off Analytics** — Stage-by-stage exit tracking (Resume Screen, Phone Screen, Technical, Final, Ghosted/Stale) with donor charts and feedback notes summaries
- ✅ **Tailored Resume Strategy Effectiveness** — Performance matrix measuring submission counts, callback rates, and offer conversion across tailored resume versions and career archetype profiles
- ✅ **Visa Sponsorship Intelligence** — Domestic vs visa-requiring pipeline distribution, employer sponsorship policy breakdown, and sponsorship conversion yields

### Phase 16 — Security Layer
- ✅ **Authenticated AES-256 GCM Cryptography** — Industry-standard AES-256 Galois/Counter Mode encryption with 96-bit random nonces and 128-bit authentication tags protecting portal passwords and secret notes
- ✅ **PBKDF2 Key Derivation with SHA-256** — 100,000 iterations of PBKDF2 stretching user master passphrases into 256-bit encryption keys with cryptographically random 32-byte salts
- ✅ **Zero-Knowledge Local Key Lifecycle** — In-memory active key management with instant zeroization (`ZeroMemory`) on lock, ensuring decrypted keys never touch disk
- ✅ **Portal Password Vault** — Structured credential storage with service presets (Workday, Greenhouse, Lever, Ashby, Taleo, SmartRecruiters, LinkedIn), portal links, usernames, and secret application notes
- ✅ **Built-in Strong Password Generator** — Cryptographically secure (`crypto.getRandomValues`) random 16-character password generator with symbols and numbers
- ✅ **Interactive Security Console UI** — Master passphrase setup and unlock modals, password reveal toggles, quick copy-to-clipboard, and password age review indicators

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
│   ├── CareerOS.Domain/          # Core entities (Documents, Tags, Countries, Regions), Enums, Interfaces
│   ├── CareerOS.Application/     # CQRS Handlers, DTOs, Validators, AutoMapper Profiles, FileStorage Abstractions
│   ├── CareerOS.Infrastructure/  # EF Core SQLite context, Configurations, Storage & Metadata Engines, Repositories
│   └── CareerOS.API/             # Controllers (Documents, Tags, Countries, Regions), Middleware, Serilog
├── frontend/
│   └── careeros-ui/              # React 18 + TS + Material UI UI (Documents Vault, Jobs, Contacts)
├── docker/
│   ├── Dockerfile.api            # Multi-stage .NET 9 API image with curl healthcheck
│   ├── Dockerfile.ui             # Multi-stage Node build & nginx runtime
│   └── nginx.conf                # SPA router fallback & security headers
├── .github/workflows/            # CI/CD pipeline definition
├── data/                         # SQLite database storage (careeros.db) & local files (data/storage/)
├── start-careeros.ps1            # One-click PowerShell launcher
├── start-careeros.bat            # One-click Windows CMD launcher
├── docker-compose.yml            # Multi-container orchestration
└── CareerOS.sln                  # .NET Solution file
```

## Roadmap

- **Phase 0** ✅ Foundation & Architecture
- **Phase 1** ✅ Core Storage Engine (SQLite, Documents, Versions, SHA-256, Tags, Countries, Regions)
- **Phase 2** ✅ Personal Profile (Info, Citizenships, Visas, Authorizations, Addresses, Social Links)
- **Phase 3** 🔜 Job Pipeline Kanban Board
- **Phase 4** 🔜 AI-Powered Resume Tailoring
- **Phase 5** 🔜 Interview Prep & Notes
- **Phase 6** 🔜 Networking CRM
- **Phase 7** 🔜 Analytics & Insights

---

*Built with ❤️ — Local-First, Privacy-First, Career-First.*
