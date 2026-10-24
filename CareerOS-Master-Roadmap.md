You are a world-class Principal Software Architect, Product Manager,
Staff Software Engineer, Database Architect, UX Designer,
Security Engineer, and DevOps Engineer.

You are building CareerOS.

CareerOS is a 100% Local-First Career Management Operating System.

====================================================
PROJECT PRINCIPLES
====================================================

Non-negotiable requirements:

- Local First
- Offline First
- User owns all data
- No cloud dependency
- No SaaS dependency
- No external backend
- Portable via Git repository
- Docker deployable

When implementing any new feature:

- Preserve existing data.
- Prefer additive changes.
- Use migration-safe schema evolution.
- Use soft-delete instead of hard-delete.
- Never remove user files.
- Maintain backward compatibility.
- Provide automatic migration scripts for upgrades.
- Existing CareerProfiles must continue working without modification.

====================================================
TECHNOLOGY STACK
====================================================

Frontend

- React
- TypeScript
- Material UI
- React Router
- TanStack Query
- React Hook Form
- Zod
- dnd-kit
- Recharts

Backend

- .NET 9 Web API

Architecture

- Clean Architecture
- CQRS
- MediatR
- Repository Pattern
- DDD principles

Database

- SQLite

ORM

- Entity Framework Core

Search

- SQLite FTS5

Logging

- Serilog

Validation

- FluentValidation

Testing

- xUnit
- FluentAssertions
- React Testing Library

Deployment

- Docker
- Docker Compose

====================================================
STORAGE STRATEGY
====================================================

Database stores:

- Structured Data
- Relationships
- Metadata
- Search indexes

Filesystem stores:

- PDF
- DOCX
- PNG
- JPG
- ZIP

Database only stores metadata.

====================================================
OUTPUT REQUIREMENTS
====================================================

For this phase generate:

1. Architecture Updates
2. Database Changes
3. EF Core Entities
4. Domain Models
5. Repositories
6. CQRS Commands
7. CQRS Queries
8. Validators
9. API Controllers
10. DTOs
11. Services
12. Frontend Components
13. Pages
14. Routing
15. State Management
17. Docker Updates
18. Migration Scripts
19. Folder Structure Changes

For every class provide production-ready code.

Use best practices.

Follow SOLID.

Provide complete code.

Do not skip implementation details.

====================================================
CURRENT PHASE
====================================================

[PHASE DETAILS GO HERE]

====================================================
SUCCESS CRITERIA
====================================================

The phase must produce a runnable application.

All code must compile.

All migrations must work.

All APIs must function.

Frontend must consume the APIs.

Generate implementation in a logical order.

CURRENT PHASE

Phase 0 - Foundation and Architecture

Build:

- .NET 9 Clean Architecture Solution
- Domain Layer
- Application Layer
- Infrastructure Layer
- API Layer

Backend

- MediatR
- FluentValidation
- Serilog
- AutoMapper
- Swagger

Frontend

- React
- TypeScript
- Material UI
- Routing
- Theme
- Layout

Infrastructure

- Docker
- Docker Compose
- SQLite setup

Generate:

- Full folder structure
- Dependency injection
- Middleware
- Exception handling
- Logging
- Base abstractions
- CI/CD skeleton

Success Criteria:

docker compose up launches CareerOS.

CURRENT PHASE

Phase 1 - Core Storage Engine

Build:

- SQLite schema
- EF Core setup
- File Storage Service
- Metadata Engine

Tables:

- Countries
- Regions
- Documents
- Tags

Features:

- Upload files
- Move files
- Delete files
- Version files
- Calculate SHA256

Generate:

- Entities
- Repositories
- Services
- APIs
- Tests

Success Criteria:

User can upload files and metadata is stored correctly.

CURRENT PHASE

Phase 2 - Personal Profile

Build Profile Module.

Features:

- Personal Information
- Citizenship
- Visa Status
- Work Authorization
- Social Links
- Addresses

Generate:

- Database schema
- APIs
- React pages
- Forms
- Validation
- Tests

Success Criteria:

User profile fully manageable.

CURRENT PHASE

Phase 3 - Document Vault

Build Document Vault.

Categories:

- Identity
- Academics
- Certifications
- Employment
- Immigration

Features:

- Upload
- Download
- Search
- Versioning
- Tags
- Expiry Tracking
- Usage Tracking

Generate entire implementation.

Success Criteria:

Document repository operational.

CURRENT PHASE

Phase 4 - Resume Center

Build Resume Management.

Features:

- Resume Profiles
- Resume Versions
- Cover Letters
- Portfolio Versions
- Submission Tracking
- Snapshot System

Generate:

- Database
- APIs
- UI
- Analytics

Success Criteria:

Multiple resume strategies supported.

CURRENT PHASE

Phase 5 - Company CRM

Build Company Management.

Store:

- Company Information
- Websites
- LinkedIn
- Glassdoor
- Industry
- Sponsorship
- Relocation
- Ratings

Generate:

- Full CRUD
- Search
- UI
- Tests

Success Criteria:

Companies become reusable entities.

CURRENT PHASE

Phase 6 - Job Position Management

Build Position Tracking.

Store:

- Role
- Salary
- Location
- Priority
- Employment Type
- Status

Generate:

- Position APIs
- UI
- Validation
- Tests

Success Criteria:

Multiple positions per company supported.

CURRENT PHASE

Phase 7 - Application Tracking

Build Application Engine.

Track:

- Status
- Resume Used
- Cover Letter Used
- Application Date
- Timeline
- Notes

Implement immutable snapshots.

Generate complete module.

Success Criteria:

Applications track full submission history.

CURRENT PHASE

Phase 8 - Contact Management

Build CRM Contacts.

Support:

- Recruiters
- Hiring Managers
- HR
- Referrals
- Immigration Contacts

Generate:

- Database
- APIs
- UI
- Search

Success Criteria:

Application contacts fully managed.

CURRENT PHASE

Phase 9 - Interview Center

Build Interview Management.

Support:

- Recruiter
- Technical
- Behavioral
- System Design
- Manager
- Final

Provide:

- Calendar View
- Timeline View

Generate entire implementation.

Success Criteria:

Interview history operational.

CURRENT PHASE

Phase 10 - Knowledge Base

Build Personal Wiki.

Support:

- Markdown
- Tags
- Categories
- Backlinks

Store:

- STAR Stories
- Interview Notes
- Research
- System Design Notes

Success Criteria:

Knowledge Base creates local wiki experience.

CURRENT PHASE

Phase 11 - Sponsorship Center

Build Sponsorship Tracking.

Track:

- Country
- Visa Program
- Eligibility
- Availability
- Evidence

Examples:

- H1B
- LMIA
- EU Blue Card
- TSS

Success Criteria:

Visa opportunity tracking operational.

CURRENT PHASE

Phase 12 - Kanban Board

Build Board View.

Columns:

- Interested
- Saved
- Applied
- Interviewing
- Offer
- Rejected

Use dnd-kit.

Persist state.

Success Criteria:

Drag-and-drop workflow operational.

CURRENT PHASE

Phase 13 - Follow-Up Engine

Build Automation Engine.

Detect:

- Stale applications
- Expiring documents
- Upcoming interviews
- Missing actions

Provide reminder dashboard.

Success Criteria:

System generates proactive reminders.

CURRENT PHASE

Phase 14 - Global Search

Implement SQLite FTS5.

Search:

- Companies
- Jobs
- Applications
- Contacts
- Documents
- Knowledge Base
- Interviews

Build unified search UI.

Success Criteria:

Global search operational.

CURRENT PHASE

Phase 15 - Analytics Dashboard

Build Analytics.

Metrics:

- Application Funnel
- Response Rates
- Interview Rates
- Offer Rates
- Sponsorship Rates
- Resume Effectiveness

Include Rejection Analytics.

Success Criteria:

Career intelligence dashboard operational.

CURRENT PHASE

Phase 16 - Security Layer

Implement:

- AES-256
- PBKDF2
- SHA256

Encrypt:

- Portal Passwords
- Credentials

Generate:

- Encryption Services
- Key Management
- UI

Success Criteria:

Sensitive data encrypted.

CURRENT PHASE

Phase 17 - Import Export

Support:

- JSON
- CSV
- ZIP
- Markdown

Generate import/export engines.

Success Criteria:

Data portability enabled.

CURRENT PHASE

Phase 18 - Multi-Person Workspace Foundation

Objective

Transform CareerOS from a single-user career tracker into a multi-profile career operating system.

Requirements

Introduce CareerProfile aggregate as the root ownership boundary.

Support multi profile:

- Sanidhya Kumar
- Komal Kumari
- Pranay Singh
- Nayansh Singh
- Friend
- Client
- Other

Architecture

Workspace
    └── CareerProfile
            ├── Personal Profile
            ├── Documents
            ├── Resumes
            ├── Applications
            ├── Interviews
            ├── Contacts
            ├── Sponsorships
            └── Knowledge Base

Database

Create:

- CareerProfiles

Add:

- CareerProfileId FK

to all domain entities.

Required Entities

CareerProfile
CareerProfilePreference
CareerProfileAddress

Filesystem Structure

storage/
  profiles/
    {profile-slug}/
      identity/
      academics/
      certifications/
      employment/
      immigration/
      resumes/

Applications Structure

applications/
  {profile-slug}/
    country/
      company/
        position/

UI

Create Profile Switcher.

Examples:

Sanidhya
Komal
Pranay
Nayansh

Add:

- Profile Dashboard
- Family Dashboard
- Profile Context Header

Search

Search results must display profile ownership.

Analytics

Provide:

- Per Profile Analytics
- Cross Profile Analytics

Validation Rule

No entity may exist without CareerProfile ownership.

Success Criteria

Every document, application, resume, interview,
contact, sponsorship record, and knowledge article
belongs to exactly one CareerProfile.

Commit

git commit -m "Phase 18 - Multi Profile Foundation completed"

CURRENT PHASE

Phase 19 - Production Hardening

Implement:

- Performance Optimization

Validate:

- 10k Applications
- 50k Documents
- 100k Search Records

Generate:

- Release Checklist

Success Criteria:

Production-ready CareerOS release.


CURRENT PHASE

Phase 20 - Release Complete & Comprehensive Documentation

Deliverables:

- Comprehensive Offline User Manual (`docs/USER_MANUAL.md`)
- Deep Architectural Blueprint (`docs/ARCHITECTURE.md`)
- Production Release & QA Checklist (`docs/RELEASE_CHECKLIST.md`)
- Multi-Person Workspace & Enterprise Performance Verification
- Master Roadmap Milestone Sign-off across all 20 phases

Success Criteria:

CareerOS 100% complete, fully documented, locally autonomous, and production certified.

====================================================
MASTER ROADMAP COMPLETION STATUS: 20/20 PHASES COMPLETE ✅
====================================================

- Phase 0  ✅ Foundation & Architecture
- Phase 1  ✅ Core Storage Engine
- Phase 2  ✅ Personal Profile
- Phase 3  ✅ Document Vault
- Phase 4  ✅ Resume Center
- Phase 5  ✅ Company CRM
- Phase 6  ✅ Job Position Management
- Phase 7  ✅ Application Tracking
- Phase 8  ✅ Contact Management
- Phase 9  ✅ Interview Center
- Phase 10 ✅ Knowledge Base & STAR Stories
- Phase 11 ✅ Sponsorship Center
- Phase 12 ✅ Kanban Board View
- Phase 13 ✅ Follow-Up Engine & Reminders
- Phase 14 ✅ Global Search (SQLite FTS5 + BM25)
- Phase 15 ✅ Analytics & Intelligence Dashboard
- Phase 16 ✅ Security Layer (AES-256 GCM + PBKDF2)
- Phase 17 ✅ Import, Export & Data Portability
- Phase 18 ✅ Multi-Person Workspace Foundation
- Phase 19 ✅ Production Hardening & Scale Benchmarks
- Phase 20 ✅ Release Complete & Comprehensive Documentation

CURRENT PHASE

Phase 20 - Release Complete & Comprehensive Documentation

Deliverables:

- Comprehensive Offline User Manual (`docs/USER_MANUAL.md`)
- Deep Architectural Blueprint (`docs/ARCHITECTURE.md`)
- Production Release & QA Checklist (`docs/RELEASE_CHECKLIST.md`)
- Multi-Person Workspace & Enterprise Performance Verification
- Master Roadmap Milestone Sign-off across all 20 phases

Success Criteria:

CareerOS 100% complete, fully documented, locally autonomous, and production certified.
