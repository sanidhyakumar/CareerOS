# CareerOS — Product Requirements Document (PRD)

> **Document Version**: 1.0.0 — Production Release  
> **Status**: Approved & Baselined  
> **Classification**: 100% Local-First System Specification  
> **System Name**: CareerOS (Career Management Operating System)

---

## 1. Executive Summary & Vision

**CareerOS** is a sovereign, 100% local-first, offline-first career management operating system designed for professionals, senior engineers, architects, leaders, and multi-career households. 

Unlike conventional SaaS job tracking tools and cloud spreadsheets that monetize user data, leak confidential compensation numbers, and impose subscription locks, CareerOS executes **entirely on localhost (`127.0.0.1`)**. The user owns their entire dataset contained within a single SQLite database (`data/careeros.db`) and a local file storage repository (`data/storage/`).

---

## 2. Core Architectural & Product Principles

Every functional requirement and implementation detail in CareerOS must strictly satisfy these non-negotiable principles:

| Principle | Product & Architectural Mandate |
|---|---|
| **100% Local-First** | All backend APIs, relational tables, FTS5 indexes, and documents operate locally. Zero network calls or external backend dependencies. |
| **Offline-First** | Complete functionality operates with or without network connectivity. |
| **Zero Cloud / Zero SaaS** | Zero telemetry, zero external database instances, zero phone-home tracking. |
| **User Data Sovereignty** | The user owns all data. The local SQLite database and vault folders are completely portable and Git-versionable. |
| **Multi-Profile Root Boundary** | `CareerProfile` is the supreme aggregate root for all domain records (Applications, Resumes, Documents, Contacts, Interviews, Sponsorships, Knowledge Base, Credentials). |
| **Strictly No Soft Delete** | Soft deletion is prohibited. No `IsDeleted`, `DeletedAt`, or `DeletedBy` columns. When records are deleted, they are permanently removed via SQL `DELETE`. |
| **User-Confirmed Permanent Deletion** | Any deletion of jobs, documents, applications, contacts, or credentials requires explicit confirmation from the user in the UI. Single-click accidental deletion is prevented. |
| **Zero Automatic Data Deletion** | The system never automatically deletes, prunes, or expires user data via background timers or TTLs. Reminders and expiry alerts are non-destructive notices. |
| **Additive Schema Evolutions** | Database upgrades must preserve 100% of existing user data through backward-compatible migrations (`CREATE TABLE IF NOT EXISTS`, `ALTER TABLE ... ADD COLUMN ...`). |
| **Immutability via Versioning & Snapshots** | Historical integrity is preserved through explicit document versioning (`Document.Version`, `ChildVersions`) and cryptographic application snapshots (`ApplicationSnapshot`), NOT soft-delete shadow rows. |
| **Profile-Aware Subsystems** | Search, analytics, document storage, and APIs must support scoped execution per `CareerProfile` as well as cross-workspace aggregation. |

---

## 3. User Personas & Workspaces

1. **Staff & Principal Engineers / Technology Architects**: Requiring multi-archetype resume tailoring (e.g., Staff Systems vs. AI/ML Lead), deep STAR story preparation, compensation tracking, and private portal credential vaults.
2. **Dual-Career Households & Families**: Requiring distinct, segregated profiles (Self, Spouse, Children) within a unified family dashboard without data cross-contamination.
3. **International Candidates**: Requiring proactive visa tracking, work authorization compliance, dual-intent tracking, and evidence document binders for global sponsorship programs.
4. **Career Consultants & Mentors**: Managing multiple client job searches with isolated document trees and distinct pipeline metrics.

---

## 4. Functional Specifications Across All 20 Roadmap Phases

### Phase 0 — Foundation & Clean Architecture
- **FR-0.1**: Four-tier Clean Architecture solution (.NET 9 Domain, Application, Infrastructure, API).
- **FR-0.2**: Single-command launcher (`start-careeros.ps1`, `start-careeros.bat`) and Docker orchestration (`docker compose up --build`).
- **FR-0.3**: Local SQLite persistence with EF Core 9 and WAL mode enabled.
- **FR-0.4**: Swagger UI interactive OpenAPI documentation at `/swagger`.

### Phase 1 — Core Storage Engine
- **FR-1.1**: Local filesystem storage engine (`LocalFileStorageService`) with subfolder segregation.
- **FR-1.2**: Metadata engine calculating cryptographic SHA-256 hashes, byte sizes, and MIME types.
- **FR-1.3**: Document versioning preserving version chains (`Version`, `IsLatestVersion`, `RootDocumentId`).
- **FR-1.4**: User-confirmed permanent file deletion with optional physical file purging.

### Phase 2 — Personal Dossier & Compliance
- **FR-2.1**: Personal profile records: bio, headline, multi-email, and multi-phone directories.
- **FR-2.2**: Citizenship registry with passport metadata and primary nationality flag.
- **FR-2.3**: Work authorizations tracking expiration dates and sponsorship needs.
- **FR-2.4**: Address management with relocation preference indicators.

### Phase 3 — Cryptographic Document Vault
- **FR-3.1**: Categorization: Identity, Academics, Certifications, Employment, Immigration, and General.
- **FR-3.2**: Expiry alerts (Expired, Expiring Soon within 90 days) without automated data modification.
- **FR-3.3**: Usage tracking audit trail linking documents to applications, visas, and companies.

### Phase 4 — Resume Center & Strategy Engine
- **FR-4.1**: Career archetype personas with target roles, industries, and skill sets.
- **FR-4.2**: Tailored resume versions with ATS keyword matching and changelogs.
- **FR-4.3**: Reusable cover letters and audience-specific portfolio variations.
- **FR-4.4**: Submission logging tracking company, position, and submission channels.
- **FR-4.5**: Cryptographic freeze of resume content, SHA-256 hash, and cover letter markdown upon submission.

### Phase 5 — Company CRM & Sponsorship Intelligence
- **FR-5.1**: Comprehensive company database: industry, size, stage, websites, tech blogs.
- **FR-5.2**: Corporate sponsorship policies (H-1B, O-1, TN, E-3, Green Card) and relocation packages.
- **FR-5.3**: Personal ratings (1-5 stars), pros/cons, and private evaluation notes.

### Phase 6 — Job Position Management
- **FR-6.1**: Multi-position tracking linked to target companies.
- **FR-6.2**: Compensation bounds (`MinSalary`, `MaxSalary`, `Currency`), equity, and bonuses.
- **FR-6.3**: Seniority levels (Junior to Principal/Director) and workplace models (Remote, Hybrid, Onsite).

### Phase 7 — Application Tracking Pipeline
- **FR-7.1**: Job application lifecycle tracking across 8 pipeline stages.
- **FR-7.2**: Immutable application snapshots capturing exact submission evidence.
- **FR-7.3**: Chronological audit timeline logging status changes, interview rounds, and notes.

### Phase 8 — Contact Management & CRM Network
- **FR-8.1**: Professional network CRM categorizing recruiters, hiring managers, and referrals.
- **FR-8.2**: Interaction history logging dates, communication channels, summaries, and follow-up deadlines.

### Phase 9 — Interview Center & Debriefs
- **FR-9.1**: Multi-round interview tracking (Screen, Technical, System Design, Behavioral, Final).
- **FR-9.2**: Video meeting links, panel interviewer associations, and self-ratings.
- **FR-9.3**: Structured debrief journals capturing questions asked and technical feedback.

### Phase 10 — Knowledge Base & STAR Story Wiki
- **FR-10.1**: Offline personal wiki with Markdown support for STAR behavioral stories.
- **FR-10.2**: Bi-directional backlinks parsing Obsidian-style `[[article-slug]]` links.
- **FR-10.3**: Categorization by System Design, Technical Concepts, Behavioral, and Company Research.

### Phase 11 — Sponsorship Center & Evidence Binders
- **FR-11.1**: Dedicated visa program tracking (US H-1B/O-1/EB-2, Canada LMIA/Express Entry, EU Blue Card, Australia TSS 482).
- **FR-11.2**: Filing deadlines, fiscal cycles, dual-intent tracking, and PR pathways.
- **FR-11.3**: Document evidence binders associating vault credentials with visa filings.

### Phase 12 — Visual Kanban Pipeline
- **FR-12.1**: 6-stage drag-and-drop board: `Interested` ➔ `Saved` ➔ `Applied` ➔ `Interviewing` ➔ `Offer` ➔ `Rejected`.
- **FR-12.2**: Real-time lifecycle sync between card movement and application status.
- **FR-12.3**: Quick filters for priority tiers, companies, and roles.

### Phase 13 — Follow-Up Engine & Proactive Reminders
- **FR-13.1**: Automated detection of stale applications (no update in 7 or 14 days).
- **FR-13.2**: Expiry alerts for visas, passports, and certifications (30, 60, 90 days out).
- **FR-13.3**: Non-destructive snooze (1, 3, 7, 14 days) and dismiss actions.

### Phase 14 — Global Search (SQLite FTS5 + BM25)
- **FR-14.1**: Universal full-text search across all 7 catalog entity types simultaneously.
- **FR-14.2**: Porter stemming tokenization and BM25 relevance ranking.
- **FR-14.3**: Spotlight modal (`Ctrl+K`) with debounced querying and keyboard navigation.
- **FR-14.4**: Profile-aware search filtering: scope by specific `CareerProfile` or search across all family profiles.

### Phase 15 — Career Intelligence & Analytics
- **FR-15.1**: Stage-by-stage application conversion funnel analytics.
- **FR-15.2**: Response velocity metrics and interview pass rates by round type.
- **FR-15.3**: Profile-scoped analytics: filter by specific `CareerProfile` or aggregate across the entire workspace.

### Phase 16 — Security Vault & Authenticated Encryption
- **FR-16.1**: Military-grade AES-256 GCM encryption for portal credentials and secret notes.
- **FR-16.2**: PBKDF2-HMAC-SHA256 key derivation with 100,000 iterations and cryptographically random salt.
- **FR-16.3**: Zeroization: keys securely erased from memory upon vault locking via `CryptographicOperations.ZeroMemory`.

### Phase 17 — Import, Export & Data Portability
- **FR-17.1**: All-in-one ZIP archive containing relational JSON state, tabular CSVs, Markdown wiki files, and SHA-256 manifest.
- **FR-17.2**: Full backup JSON export and transactional restore covering `CareerProfiles`, preferences, applications, and all entities.
- **FR-17.3**: Clean CSV exports formatted for external spreadsheet analysis.

### Phase 18 — Multi-Person Workspace Foundation
- **FR-18.1**: `CareerProfile` as the root aggregate boundary for all persistent records.
- **FR-18.2**: 7 pre-seeded default profiles (Sanidhya Kumar, Komal Kumari, Pranay Singh, Nayansh Singh, Friend, Client, Other).
- **FR-18.3**: Segregated filesystem paths under `data/storage/profiles/{slug}/`.
- **FR-18.4**: Top-bar `ProfileSwitcher` with 1-click active profile context switching.
- **FR-18.5**: Automatic backfill assigning all legacy unassigned records to the primary workspace profile.

### Phase 19 — Production Hardening & Scale Benchmarks
- **FR-19.1**: SQLite WAL mode, memory-mapped I/O (256 MB), and 64 MB page cache.
- **FR-19.2**: Composite database indexes for high-throughput queries.
- **FR-19.3**: Scale certified for > 10,000 applications, > 50,000 documents, and > 100,000 FTS5 search records with P95 query latency < 3.5ms.

### Phase 20 — Release Complete & Documentation
- **FR-20.1**: Offline User Manual ([USER_MANUAL.md](file:///c:/Projects/CareerOS/docs/USER_MANUAL.md)).
- **FR-20.2**: Deep Architectural Blueprint ([ARCHITECTURE.md](file:///c:/Projects/CareerOS/docs/ARCHITECTURE.md)).
- **FR-20.3**: Database Schema & Governance Specification ([DATABASE_SCHEMA.md](file:///c:/Projects/CareerOS/docs/DATABASE_SCHEMA.md)).
- **FR-20.4**: Production Release & QA Checklist ([RELEASE_CHECKLIST.md](file:///c:/Projects/CareerOS/docs/RELEASE_CHECKLIST.md)).

---

## 5. Non-Functional Requirements (NFRs)

| Category | Requirement | Target Metric |
|---|---|---|
| **Performance** | API Read Latency | P95 < 5ms for SQLite reads; P95 < 2ms for indexed queries |
| **Search** | FTS5 Query Throughput | P95 < 3.5ms across 100,000 records with Porter stemming |
| **Availability** | Local High-Availability | 100% autonomous; zero dependency on cloud or internet uptime |
| **Security** | Data at Rest Encryption | AES-256 GCM (96-bit nonce, 128-bit tag); master key never persisted |
| **Integrity** | Binary Document Integrity | SHA-256 cryptographic checksum verified on every upload |
| **Safety** | Deletion Safety | 100% of user deletions require explicit confirmation; zero soft-delete shadow clutter |
| **Portability** | Backup & Migration | 100% data preservation during upgrades via additive migrations |
| **Memory** | API Footprint | Sub-80 MB working set in normal operating state |

---

## 6. Verification & Quality Acceptance Criteria

1. **Zero Automated Tests Rule**: In strict compliance with project policies, no unit or integration test projects exist or are scaffolded.
2. **Static Compilation**: `dotnet build CareerOS.sln --no-restore -m:1 /nodeReuse:false` passes with 0 Warnings and 0 Errors.
3. **Frontend Production Build**: `npm run build --prefix frontend/careeros-ui` completes with 0 TypeScript or bundling errors.
4. **Single-Command Launch**: Native scripts (`start-careeros.ps1`, `start-careeros.bat`) and Docker orchestration (`docker compose up`) launch the system into a healthy operating state.

---

*CareerOS — 100% Local-First. Sovereign, Offline, and Permanent.*

