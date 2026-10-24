# CareerOS — Comprehensive User Manual

Welcome to **CareerOS**, your **100% Local-First Career Management Operating System**. CareerOS gives you sovereign, private control over your career data, applications, interviews, documents, and credentials without relying on third-party cloud services or SaaS platforms.

---

## 📑 Table of Contents
1. [Quick Start & Launch Options](#1-quick-start--launch-options)
2. [Multi-Person Workspace & Profile Switcher](#2-multi-person-workspace--profile-switcher)
3. [Personal Profile Management](#3-personal-profile-management)
4. [Document Vault & Compliance Tracker](#4-document-vault--compliance-tracker)
5. [Resume Center & Archetype Personas](#5-resume-center--archetype-personas)
6. [Company CRM & Job Position Tracking](#6-company-crm--job-position-tracking)
7. [Visual Kanban Pipeline](#7-visual-kanban-pipeline)
8. [Application Tracking & Immutable Snapshots](#8-application-tracking--immutable-snapshots)
9. [Interview Center & Debriefs](#9-interview-center--debriefs)
10. [Knowledge Base & STAR Story Wiki](#10-knowledge-base--star-story-wiki)
11. [Visa Sponsorship Intelligence](#11-visa-sponsorship-intelligence)
12. [Security Vault & Credential Locker](#12-security-vault--credential-locker)
13. [Follow-Up Engine & Proactive Reminders](#13-follow-up-engine--proactive-reminders)
14. [Global Search Spotlight (Ctrl+K)](#14-global-search-spotlight-ctrlk)
15. [Career Intelligence & Analytics](#15-career-intelligence--analytics)
16. [Import, Export & Data Portability](#16-import-export--data-portability)

---

## 1. Quick Start & Launch Options

### ⚡ One-Click Local Launcher (Recommended)
CareerOS runs natively on Windows with no container overhead needed.

- **PowerShell**:
  ```powershell
  .\start-careeros.ps1
  ```
- **Windows Command Prompt / Double-Click**:
  ```cmd
  start-careeros.bat
  ```

### 🐳 Docker & Docker Compose
To run in isolated containers:
```powershell
docker compose up --build
```
Stop containers:
```powershell
docker compose down
```

### 🌐 Accessing the Application
- **Frontend Web UI**: [http://localhost:3000](http://localhost:3000)
- **Interactive Swagger API**: [http://localhost:5000/swagger](http://localhost:5000/swagger)
- **Health Check**: [http://localhost:5000/health](http://localhost:5000/health)

---

## 2. Multi-Person Workspace & Profile Switcher

CareerOS natively supports multiple career profiles within a single household, consulting client roster, or personal workspace.

### Profile Context Header
In the top right corner of the navigation bar, the active profile is always displayed with a colored avatar badge and relationship tag (e.g., `Self`, `Spouse`, `Child`, `Client`).

### Switching Profiles
1. Click the profile badge in the top navigation bar.
2. Select any registered profile:
   - **Sanidhya Kumar** (Primary / Self)
   - **Komal Kumari** (Spouse)
   - **Pranay Singh** (Child)
   - **Nayansh Singh** (Child)
   - **Friend / Client / Other**
3. The application instantly updates its active context without reloading.

### Family Workspace Hub (`/profiles`)
Navigate to **Family Workspace** in the left drawer to view:
- **Family Dashboard**: Aggregate counts of all family applications, interviews, vault documents, and offers.
- **Profile Summaries**: Side-by-side cards comparing active pipelines and target roles.
- **Cross-Family Activity Stream**: Real-time chronological audit feed showing applications submitted and interviews scheduled across all profiles.
- **Add / Edit Profile**: Create custom profiles with tailored color codes, relationship classifications, and target compensation brackets.

---

## 3. Personal Profile Management

Navigate to **Profile** (`/profile`) to manage your personal dossier:
- **Core Identity**: First name, last name, preferred name, email aliases, phone numbers, and bio.
- **Citizenship Registry**: Multi-citizenship tracking, primary nationality status, and passport linkage.
- **Work Authorizations & Visas**: Track work rights, validity windows, and renewal deadlines.
- **Addresses & Mobility**: Permanent residences, mailing addresses, and relocation willingness flags.
- **Social Links**: Centralized directory for LinkedIn, GitHub, tech blogs, and personal portfolios.

---

## 4. Document Vault & Compliance Tracker

The **Document Vault** (`/documents`) provides cryptographic document management:
- **Categories**: Identity, Academics, Certifications, Employment, Immigration, and General.
- **Cryptographic Integrity**: Every uploaded document automatically computes and validates a SHA-256 checksum.
- **Version History**: Upload revisions without losing historical links (`v1`, `v2`, etc.).
- **Expiry Alerts**: Real-time badges flag documents that are **Expired** or **Expiring Soon** (within 90 days).
- **Usage Tracking**: See exactly which job applications, visa filings, or companies have referenced each document.

---

## 5. Resume Center & Archetype Personas

Navigate to **Resumes** (`/resumes`) to manage diverse career strategies:
- **Career Archetypes**: Create personas tailored for specific tracks (e.g., *Staff Systems Architect* vs. *VP Engineering*).
- **Tailored Versions**: Clone base resumes to create company-specific versions with custom ATS keywords.
- **Cover Letters**: Reusable, markdown-formatted pitch letters linked directly to resume versions.
- **Portfolio Variations**: Curate specific case study URLs for technical or leadership audiences.
- **Submission Tracking**: Historical record of every company, position, and submission channel for each resume variation.

---

## 6. Company CRM & Job Position Tracking

### Company Management (`/companies`)
- Track target employers with industry classification, company size, stage, and tech blog links.
- Monitor corporate sponsorship policies (Supports H1B, Transfers only, No sponsorship) and relocation packages.
- Rate employers (1-5 stars) and maintain private notes.

### Job Position Management (`/jobs`)
- Support multiple open positions per target company.
- Record compensation ranges, seniority levels (Staff, Principal, Lead), and employment models (Remote, Hybrid, Onsite).
- Direct linking between positions and subsequent job applications.

---

## 7. Visual Kanban Pipeline

The **Board** (`/board`) provides drag-and-drop workflow visualization:
- **6 Pipeline Stages**: `Interested` ➔ `Saved` ➔ `Applied` ➔ `Interviewing` ➔ `Offer` ➔ `Rejected`.
- **Drag-and-Drop Interaction**: Move cards smoothly powered by `dnd-kit`.
- **Real-Time Lifecycle Sync**: Dragging a card automatically updates underlying application statuses and logs timeline events.
- **Quick Filters**: Search by keyword, company, or priority tier (Dream, High, Medium, Low).

---

## 8. Application Tracking & Immutable Snapshots

Navigate to **Applications** (`/applications`) to review comprehensive submission histories:
- **Submission Snapshot System**: Every submitted application cryptographically freezes the exact resume version, SHA-256 hash, and cover letter text used at that moment in time.
- **Audit Timeline**: View timestamps for submission, status transitions, interview rounds, and notes.
- **Referral Association**: Link applications to contacts who provided internal referrals.

---

## 9. Interview Center & Debriefs

The **Interview Center** (`/interviews`) keeps your interview loops organized:
- **Round Types**: Recruiter Screen, Technical Coding, System Design, Behavioral, Hiring Manager, and Final Round.
- **Formats**: Video Call (Zoom/Meet/Teams), Phone, Onsite, or Take-home assessment.
- **Debrief Journal**: Record questions asked, technical challenges, and feedback notes immediately after each interview.
- **Self-Rating**: Score your performance (1-5 stars) to identify areas for preparation.

---

## 10. Knowledge Base & STAR Story Wiki

The **Knowledge Base** (`/wiki`) provides an offline personal wiki:
- **STAR Stories**: Structure behavioral interview examples with Situation, Task, Action, and Result formatting.
- **Bi-directional Backlinks**: Connect articles seamlessly using Obsidian-style `[[article-slug]]` wiki syntax.
- **Categorization**: Filter by General, System Design, Technical Concepts, Behavioral, STAR Stories, and Company Research.
- **Markdown Editor**: Full markdown rendering with code blocks, tables, and task lists.

---

## 11. Visa Sponsorship Intelligence

The **Sponsorship Center** (`/sponsorship`) tracks immigration opportunities:
- **Global Visa Programs**: Dedicated schemas for US H-1B / O-1 / EB-2, Canada LMIA / Express Entry, EU Blue Card, and Australia TSS 482.
- **Employer Eligibility & Policy**: Track dual intent support, minimum salary thresholds, and path to permanent residency.
- **Document Evidence Binder**: Link passport copies, degree certificates, and employment verification letters to specific sponsorship filings.

---

## 12. Security Vault & Credential Locker

The **Security Vault** (`/security`) provides military-grade credential storage:
- **Authenticated AES-256 GCM**: All stored passwords and secret application notes are encrypted with 256-bit keys and authenticated tags.
- **PBKDF2 Key Derivation**: Your master passphrase is never stored on disk. It is stretched using 100,000 iterations of HMAC-SHA256.
- **Memory Zeroization**: Decrypted keys are erased from memory upon locking.
- **Strong Password Generator**: Generate cryptographically secure random 16-character passwords for new job application portals.

---

## 13. Follow-Up Engine & Proactive Reminders

The **Follow-Up Console** (`/followup`) prevents opportunities from slipping through the cracks:
- **Stale Applications**: Flags applications with no update after 7 or 14 days.
- **Expiring Documents**: Alerts you 30, 60, and 90 days before passport or visa expiration.
- **Upcoming Interviews**: Proactive reminders 24-48 hours before scheduled rounds.
- **Snooze & Dismiss**: Snooze notifications for 1, 3, 7, or 14 days, or mark them resolved.

---

## 14. Global Search Spotlight (Ctrl+K)

Press `Ctrl+K` (or `Cmd+K` on macOS) anywhere within CareerOS:
- **Instant Universal Search**: Searches across Companies, Jobs, Applications, Contacts, Documents, Wiki Articles, and Interviews simultaneously.
- **SQLite FTS5 & BM25**: Uses native full-text search with Porter stemming and BM25 relevance ranking.
- **Highlighted Snippets**: Matches are illuminated in `<mark>` tags.
- **Profile Ownership Display**: Every search result indicates which family profile owns the record.

---

## 15. Career Intelligence & Analytics

The **Analytics Dashboard** (`/dashboard`) surfaces actionable metrics:
- **Conversion Funnel**: Stage-by-stage drop-off analytics from initial submission to final accepted offer.
- **Response Velocity**: Response rate percentage and average days to first recruiter callback.
- **Interview Pass Rates**: Pass rate percentage broken down by technical, system design, and behavioral rounds.
- **Profile Scope Filter**: Toggle between **Cross-Profile / All Profiles** and individual family member analytics.

---

## 16. Import, Export & Data Portability

Navigate to **Backup & Export** (`/portability`) to safeguard your data:
- **All-in-One ZIP Archive**: Complete backup including relational JSON database state, tabular CSV files, Markdown files, and SHA-256 manifest.
- **Tabular CSV Spreadsheets**: Export Jobs, Applications, Companies, and Contacts for analysis in Excel or Google Sheets.
- **Markdown Wiki Export**: Export your knowledge base and STAR stories into standalone markdown files ready for Obsidian or Notion.
- **JSON Restore Engine**: Restore complete system snapshots with transaction safety and conflict detection.

---

*CareerOS — 100% Local-First. Your Data, Your Career, Your Control.*
