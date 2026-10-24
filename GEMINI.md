# CareerOS — Project Instructions & Constraints

## ⚠️ Testing Policy (Strict Rule)
- **NO AUTOMATED TESTS**: Automated tests (unit tests, integration tests, test projects, test suites, `tests/` directory) are **STRICTLY NOT REQUIRED** for this project.
- **Do NOT** generate, scaffold, suggest, or add test projects or test files (`*.Tests.csproj`, `*Tests.cs`, etc.) unless explicitly commanded by the user.
- **Verification Strategy**: Verify features and changes strictly via:
  1. Static compilation checks: `dotnet build CareerOS.sln --no-restore -m:1 /nodeReuse:false`
  2. Frontend build verification: `npm run build --prefix frontend/careeros-ui`
  3. Direct API/manual execution if needed.

## 🗑️ Data Deletion, Immutability & Schema Evolution Policy (Strict Rule)
- **No Soft Delete**: Soft deletion patterns (flagging records as deleted while retaining them in the active table) are **STRICTLY PROHIBITED**.
- **No IsDeleted, DeletedAt, DeletedBy Fields**: Domain entities, models, DTOs, and database schemas must **NEVER** contain `IsDeleted`, `DeletedAt`, `DeletedBy`, or equivalent soft-delete columns.
- **User-Confirmed Permanent Deletion Only**: Deletion of any record, entity, or document must be **explicitly initiated and confirmed by the user**. When confirmed, physical database records are deleted permanently.
- **No Automatic Data Deletion**: The system must **NEVER** silently or automatically delete data via background cron jobs, TTL expirations, or silent cleanup tasks.
- **Backward-Compatible Migrations**: All database schema changes and EF Core configurations must be 100% backward-compatible with existing SQLite database files (`careeros.db`).
- **Preserve Existing Data During Upgrades**: Any upgrade, schema migration, or system startup must preserve 100% of existing user data without corruption or loss.
- **Additive Schema Changes**: Always prefer additive changes (`CREATE TABLE IF NOT EXISTS`, `ALTER TABLE ... ADD COLUMN ...`, nullable columns, default values) so that existing databases remain intact.
- **Keep Document Versioning & Immutable Snapshots**: Historical traceability is achieved exclusively through:
  1. **Document Versioning**: Explicit version chains (`Document.Version`, `ChildVersions`, `RootDocumentId`) preserving revisions as active versioned entities.
  2. **Application Snapshots**: Cryptographically frozen, immutable submission snapshots (`ApplicationSnapshot`) capturing exact resume content, SHA-256 hashes, and cover letter text at time of submission.
