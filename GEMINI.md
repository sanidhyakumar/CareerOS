# CareerOS — Project Instructions & Constraints

## ⚠️ Testing Policy (Strict Rule)
- **NO AUTOMATED TESTS**: Automated tests (unit tests, integration tests, test projects, test suites, `tests/` directory) are **STRICTLY NOT REQUIRED** for this project.
- **Do NOT** generate, scaffold, suggest, or add test projects or test files (`*.Tests.csproj`, `*Tests.cs`, etc.) unless explicitly commanded by the user.
- **Verification Strategy**: Verify features and changes strictly via:
  1. Static compilation checks: `dotnet build CareerOS.sln --no-restore`
  2. Frontend build verification: `npm run build --prefix frontend/careeros-ui`
  3. Direct API/manual execution if needed.

