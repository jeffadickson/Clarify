# Clarify Project Status — 2026-10-15

**Assessment:** integrated RAD framework **development prototype**, not beta or release candidate. Source commits cover many subsystems, but clean-clone .NET, browser, and security acceptance are not yet demonstrated by a green end-to-end CI run.

## Capability matrix

| Subsystem | What exists in source | Outstanding release gates |
|---|---|---|
| Dictionary and visual schema editing | Entity/field/relationship editor, locally saved drafts and JSON export | Authoritative project saves, stable IDs, migrations across providers |
| ER diagram (issue #39) | Native/external edges, layered layout, zoom/pan, saved positions, keyboard movement, SVG export; Playwright CI tests added | Observed browser pass; large-schema routing/UX |
| ClarifyDSL (issue #38) | Source-aware browser parser and parallel native Parlot parser, C# AST/diagnostics, 40-template parity fixtures | Observed .NET parity pass; full grammar; no default-parser replacement |
| Templates (issue #32) | 40 versioned manifests, type/control metadata, parameter binding, Studio preview/edit | Real generated browse, master-detail, nested CRUD, upgrades preserving author edits |
| Data/EF Core (issue #37) | Generated entities, contexts, indexes, FK constraints; SQLite relational smoke fixture | Verified runtime build; provider parity and deep migrations |
| Migration safety | Conservative nullable-column SQLite plan and approval/backup/transactional CLI | Verified database restore drill; comprehensive EF migration planning |
| Forms and validation (issue #36) | Initial form controls and DataAnnotations, metadata validation | Generated authoritative server validation, searchable lookup, typed unit editors |
| Money and units (issue #35) | NodaMoney/UnitsNet standard library, semantic metadata, sample SQLite mapping | Generalized generated mappings and form editors, SQL/provider tests |
| Identity (issue #25) | Identity/OIDC host configuration, scoped app roles/permissions model | Enforce every generated operation; secure account admin/onboarding; migrations and audit |
| Observability (issue #31) | OpenTelemetry library and generated bootstrap; sample CRUD spans | Generator across all targets, privacy/export validation |
| Async-first (issue #33) | Cancellable/paged customer browse and generator contracts | Actual generated repositories for all entities, concurrency benchmarks |
| Elsa workflow integration | Workflow DSL/bootstrap and sample execution | Production persistence/user tasks/security |
| C# authoring & documentation (#41) | First-class C# extension architecture, XML docs enabled, DocFX, generator comment standards | Finish handwritten and generated API coverage, verify output and CS1591 clean |
| Packaging/release | .NET CLI and CI scripts, web and desktop foundations | Clean-clone build and verified Windows/macOS packaging |

## What changed this increment
- ER layout/keyboard/zoom/SVG browser acceptance tests are configured for Chromium.
- Native parser received bounds diagnostics and remains **parallel**, not a replacement for Studio.
- Studio authoring and API documentation are tracked as separate issues (#40 and #41).
- Every .NET project now emits XML documentation; DocFX and a public API audit have been configured.
- EF Core entity and async browse generators emit docs; generated examples are refreshed.
- Full public C# documentation remains an active backlog, not a completed claim.

## Release milestones in order

1. **Green clean-clone CI**: .NET 10, Node tests, native parser parity, SQLite smoke, Playwright and DocFX.
2. **Integrated visual dictionary → generated running app** with keys, relations, browses, forms and master-detail.
3. **Robust regeneration**: versioned schema migrations, stable identifiers, user-owned C# and form edits preserved.
4. **Enforced authorization and validation** on every generated server data boundary.
5. **Deployment-ready runtime**: structured API docs, reports, workflows, cross-platform packaging, security/scale tests.

## Evidence policy

'Committed' means source is present in GitHub; it does **not** mean green build or working production deployment. GitHub combined statuses and PR-filtered runs for the last inspected commit were empty. CI availability and results need independent verification before marking acceptance gates complete. No percentage-complete estimate is meaningful until the integrated end-to-end pipeline is passing.

## Final documentation and browser-testing increment
- DocFX 2.81.0 is pinned in `.config/dotnet-tools.json`, alongside existing `dotnet-ef`.
- `Directory.Build.props` turns on XML documentation output for all .NET projects.
- Core EF Core, async repository, Blazor Customer component and Elsa workflow generators were updated to emit `///` documentation; checked-in generated files were synchronized.
- `tools/audit-csharp-docs.mjs` creates an inventory of missing public comments, with a generated-source gate.
- Shared Blazor CI now installs Chromium Playwright and invokes the ER tests, then runs DocFX and uploads the generated docs/coverage report when successful.
- `docs/CSHARP-DOCUMENTATION.md` formalizes the code-writing and code-generation documentation standard; issue #41 tracks whole-codebase completion.

**Still not closed:** there is no observed passing full .NET/Playwright/DocFX pipeline, and the handwritten API XML documentation backlog has not been measured or eliminated by a successful repository-wide audit. Issues #38, #39, #40 and #41 remain open on these explicit verification/quality gates.
