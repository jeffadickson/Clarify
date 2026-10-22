# AGENTS.md — Clarify engineering instructions

These instructions apply repository-wide to AI coding agents and contributors. Read `docs/DEVELOPMENT-CONTINUATION.md`, `docs/ROADMAP.md`, `docs/REQUIREMENTS.md`, `docs/DESIGN.md`, and the relevant open GitHub issues before modifying code. Where documents conflict, verify the executable implementation and tests; record discrepancies and update stale guidance rather than inventing an authoritative answer.

## Product contract
Clarify is an independent, Clarion-inspired RAD platform: semantic dictionary → ClarifyDSL ⇄ visual Studio → generated readable .NET applications → EF Core persistence → browse/form/master-detail → reports → safe regeneration. Loom is separate and optional. The priority is a verified generated application, not feature-count expansion. Elsa 3 integration is optional and gated; do not create another workflow engine.

## Languages and implementation
- Prefer **C# and .NET 10 LTS** for production engines, CLI utilities, test infrastructure, data access, validators, generators where migration is practical, and long-lived tooling. Existing dependency-free ES modules power the browser Studio and prototype generators; do not rewrite working pieces speculatively. Python is acceptable for one-off analysis and test orchestration, not the default core runtime.
- Use modern, idiomatic C# with nullable reference types, typed APIs, async/await and cancellation for I/O, deterministic data structures/output, dependency injection and explicit lifetime/ownership. Use bounded parallelism/multithreading where it produces measurable benefit; never add races to database transactions, file generation, caches or logging.
- Generated C# must be readable, extensively commented where non-obvious, XML-documented for public/protected APIs and free from hidden runtime dependencies. Avoid bare catch blocks, swallowed failures, unsafe dynamic execution and ad-hoc reflection when typed contracts suffice. Keep templates and generator output deterministic.
- Preserve handwritten code. Generated artifacts live in explicitly generated-owned files/directories; don't overwrite custom logic. Version schemas, generator manifests and provenance. Treat stable identity/rename handling as a prerequisite for safe regeneration.
- Use approved libraries rather than rolling own equivalents. Prefer EF Core for persistence, SQLite for first acceptance path; use NodaMoney and UnitsNet where semantic types are supported; Elsa 3 for optional workflows, OpenTelemetry for observability, and Parlot only as a candidate parser pending proven grammar and round-trip parity. Do not treat an experimental parser as the default.

## Quality and verification
- Write tests **with** implementation: xUnit for C#; Node built-in test runner for existing ESM; Playwright for browser interactions; NSubstitute for isolated boundaries; Testcontainers for real provider tests. Add negative cases, boundary values, race/concurrency tests, deterministic golden fixtures, and C# source-preserving regeneration tests.
- For schema/migrations, verify SQLite data preservation, FK and unique constraints, transaction behavior, backups/recovery and refusal of unsupported/destructive changes. Never silently auto-apply migrations to users' databases.
- Run the repo's `bash scripts/verify.sh` or PowerShell equivalent, relevant targeted tests, web/desktop builds, generated-output diff checks, and CI. Record commands, environment, outcomes, SHA, and exact remaining failures. A committed test is NOT a passed test; don't claim green without execution evidence.
- Keep GitHub Actions checks truthful and required-gate focused. Verify actual push/PR workflow results, not merely a PR-filtered run lookup. No bypass of failing gates just to obtain green.
- Document public C# APIs with `///` XML comments (summary, parameters, returns, exceptions where informative); produce DocFX HTML API docs. Maintain project design/requirements/README and useful runnable examples. Use static analyzers/formatters and enable stricter diagnostics incrementally with baseline evidence. Maintain security/threat-model and dependency audit guidance.
- Secure by default: authentication/authorization at every server boundary, project/entity/row scopes, validated input and output, antiforgery/CSRF protections when relevant, least-privilege secrets, safe cancellation and timeouts. Localhost samples must never be represented as production-hardened.

## Dependency/tool policy
- Respect `global.json` (.NET 10 SDK), explicit NuGet package pins and `NuGet.Config`. Establish committed `packages.lock.json` with actual restore before switching CI to `--locked-mode`; don't claim lockfile completeness beforehand.
- Node.js 22+ for the current pure ESM toolchain. Avoid adding a bundler or npm dependency unless justified by feature/maintenance/measurement and lockfile review. Keep SDK/MAUI/Razor versions mutually compatible. Pin and audit GitHub Actions.
- Standard repository tools: Git/GitHub PRs and issues, dotnet SDK/CLI and `dotnet ef`, Node, xUnit, Playwright, DocFX, OpenTelemetry, EF Core SQLite, and optional Docker/Testcontainers for integration tests. Record exact versions in evidence.
- Prefer small, reviewable commits on branches and PRs. Do not push or merge unless instructed. If asked for N local iterations followed by one push, keep each iteration local and push only once; do not substitute remotely committed iterations. Never invent test results, commit SHAs, review-board feedback or iteration counts.

## Immediate priorities
1. Repair actual `main` CI errors and source/golden/Studio copies from authoritative failing logs. The previous PR #45 merge included unverified Elsa 3.8 API calls, generator parity and regex tests; re-evaluate against present CI before claiming resolution.
2. Make the semantic dictionary → generated ASP.NET Core/Blazor/EF Core/SQLite Equipment Checkout (then Order Entry) application work without handwritten CRUD as its primary implementation. Retain end-to-end tests.
3. Enforce unique active-loan concurrency, authoritative validation, query paging/lookups, identity/permissions, and safe migration/restore drills.
4. Implement round-trip designer/source parity and regenerations that provably preserve user-authored C#.
5. Publish DocFX, add cross-platform clean-clone test evidence, then pursue reports and packaged delivery.

See `docs/DEVELOPMENT-CONTINUATION.md` for a copy/paste Codex task prompt and milestone-specific acceptance criteria.
