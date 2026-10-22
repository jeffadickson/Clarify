# Clarify — development continuation / Codex startup prompt

**Updated:** 2026-10-21  
**Repository:** https://github.com/jeffadickson/Clarify  
**Starting point:** inspect current `main` and recent merge PR #45; do not assume its prior CI failures were fixed.  
**Instructions:** read root `AGENTS.md` first. This file is a *task prompt and execution plan*, not a claim of implemented functionality.

## Copy/paste startup prompt

> You are the lead engineer continuing Clarify, an open-source, Clarion-inspired rapid application development environment. Start in the **existing** jeffadickson/Clarify repository, not a new project. Inspect Git status, head commit, README, AGENTS.md, docs/ROADMAP.md, docs/REQUIREMENTS.md, docs/DESIGN.md, docs/PROJECT-STATUS-2026-10-15.md, relevant open issues, and latest actual GitHub Actions runs. Do not assume code compiles or that closed/merged means verified. Create a focused plan and work in a feature branch, preserving `main`.
>
> Goal: turn the working pieces into a verified end-to-end **dictionary → generator → compiled runnable .NET 10/Blazor/EF Core/SQLite CRUD application** with repeatable tests and safe regeneration. The Equipment Checkout sample is the first acceptance fixture; Order Entry (Customer, Product, Order, OrderLine with master-detail and invoice) is the next. Preserve existing Studio, 40-template gallery and ClarifyDSL authored content. Keep Loom optional and separate. Do not expand Elsa features until the RAD acceptance path works; fix Elsa 3.8 compile/API compatibility only against actual installed package metadata, source or reproducible tests.
>
> **First checkpoint: return CI to green.** Re-run/capture Node, xUnit, generated parity, SQLite smoke, browser/Playwright, MAUI where runners exist, and DocFX. Investigate known historical defects, not as assumptions: a malformed regex in `tests/ef-generation.test.mjs`; divergence between `designer/ef-models.mjs` and `src/Clarify.Demo.Web/wwwroot/studio/ef-models.mjs`; Elsa 3.8 `IWorkflowDispatcher`/resume API overloads; generated Razor `@media` escapes; EF Core generated file drift. Fix root causes plus regression tests; do not weaken tests merely to get green. Give every check an observed run URL, SHA and result.
>
> **Second checkpoint: genuine generated app.** Make schema and template metadata generate data models, DbContext, constraints, server validation, async paginated browse, create/edit/delete UI, searchable foreign-key lookups and master-detail behavior. Avoid a demo primarily composed of handwritten Program.cs CRUD. Ensure equipment serials and active-loan uniqueness are enforced in SQLite under concurrent requests, not only a check-then-insert. Database conflicts should yield typed HTTP 409 responses and meaningful UI messages.
>
> **Third checkpoint: robust source ownership and migrations.** Generated files are replaceable and deterministically reproducible, with a manifest/hash and version provenance. A fresh regenerate with unchanged dictionary should yield a clean diff. A changed dictionary should preserve handwritten C#, comments and designer layouts. Stable field/entity IDs and rename handling must avoid destructive drop/recreate mistakes. Start with conservative SQLite migration plans, explicit approval, backups, drift detection, transactionality and restore drills; reject unsupported transformations rather than silently applying them.
>
> **Fourth checkpoint: harden and document.** Enforce server-side authorization and project/entity/row scopes, validation, protected write endpoints and safe defaults. Add end-to-end browser tests, test real async cancellation and concurrency, cross-platform clean-clone build evidence, OpenTelemetry spans without leaking PII, XML comments on public APIs, DocFX HTML, static analysis and dependency/security auditing. Produce an actual walkthrough of the generated application plus screenshots/artifacts only when tests and tools verify them.
>
> Follow C#-first engineering standards in AGENTS.md: expressive code, thorough nontrivial comments, XML API docs, xUnit/NSubstitute/Testcontainers, Node tests for existing ESM, Playwright, EF Core, modern .NET 10 and async/cancellation. Use bounded multithreading only where safe and advantageous, instrument performance claims, and avoid speculative infrastructure/framework rewrites. Commit small coherent changes; honor whatever push policy the user states for this session. Record verification commands/results and open issues. Do not report hypothetical reviewers or test passes as real.
>
> Acceptance to call the RAD vertical slice complete: clean clone builds, all required CI checks pass, deterministic generated artifacts, generated Equipment Checkout runs from its dictionary/template with real browser CRUD/returns, double checkout is rejected under concurrent load, authentic server validation is tested, migrations preserve existing data and restore succeeds, designer/source edits round-trip without loss, documentation builds and points to working examples. If any gate remains open, report it explicitly and stop short of calling the project beta-ready.

## Technology and dependency baseline
| Category | Baseline / decision | Gate |
|---|---|---|
| Language | C# / .NET 10 LTS first for runtime and tools; existing browser Studio ES modules retained | Confirm toolchain under `global.json` |
| UI | Blazor Web / Razor components; MAUI Blazor Hybrid secondary | Web acceptance first; desktop build evidence |
| Persistence | EF Core + SQLite first; PostgreSQL/SQL Server later | Tested FK, unique indexes, migrations and provider semantics |
| Money/units | NodaMoney, UnitsNet (existing project choices) | Typed mappings, rounding and provider behavior |
| Workflow | Elsa 3.x optional, existing 3.8 baseline | Verify exact package API; no competing workflow runtime |
| Parser | Studio JS parser canonical pending Parlot grammar and round-trip parity | Native parser promotion only after exhaustive parity |
| Telemetry | OpenTelemetry | Traces, metrics, privacy and performance validation |
| C# tests | xUnit, NSubstitute, Testcontainers | Unit, integration, concurrency and negatives |
| Browser/JS tests | Node.js 22+, built-in node:test, Playwright | Golden generation parity and actual Chromium UI behavior |
| Docs | Triple-slash XML docs + DocFX HTML, Markdown design and ADRs | Build verified; no undocumented new public APIs |
| CI/security | GitHub Actions, analyzers, formatting, dependency audit, SBOM later | Check run evidence and reproducible release |

## Priority execution order
1. **P0 CI stabilization:** fetch failing jobs, correct exact issues, add regression tests and demonstrate passing runs.
2. **P0 generation parity:** one authoritative generator, synchronized Studio assets and golden fixtures; provenance and clean-diff tests.
3. **P0 Equipment Checkout:** complete generator-driven CRUD, semantic field metadata, concurrency-safe serial/loan constraints, SQLite smoke and Playwright tests.
4. **P0 safe regeneration:** user-code preservation and reviewed migrations/restore, with fixture histories.
5. **P0 server security:** validator generation, least-privilege authorization, CSRF/origin strategy and audit.
6. **P1 Order Entry:** nested order lines, lookups, transactions, typed money and printable invoice.
7. **P1 release quality:** DocFX/CS1591, packaging, benchmark traces, cross-platform matrices; expand Elsa only after RAD acceptance.

## Evidence and reporting
For each milestone log: branch/base SHA, issue and requirement IDs, changed sources and generated files, dependency/version changes, test commands (including OS), passing/failing counts, actual CI run URLs, residual security/performance limitations, and next task. A successful push is not a successful build. Distinguish sample-only functionality from production readiness. Never silently alter scope or skip acceptance gates.

## Quick setup and verification
```sh
git clone https://github.com/jeffadickson/Clarify.git
cd Clarify
git status --short
dotnet --info
node --version
bash scripts/verify.sh
```
On Windows, use `./scripts/verify.ps1`. Read `global.json`, `NuGet.Config`, project files and `.config/dotnet-tools.json` to identify actual pinned versions before changing them. Run targeted tests on each change; run full verification at checkpoints. If .NET, Playwright or Docker is unavailable, report that as missing evidence—not a pass.
