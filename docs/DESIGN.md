# Clarify detailed design — v0.2
Status: proposed technical design; implementation not yet started.

## 1. Product boundaries
Clarify is a separate RAD product: model/dictionary, generator, Studio, and optional adapters. Loom remains an independently governed Frappe-inspired project. No Clarify core package imports Loom. Optional `Clarify.Targets.Loom` may consume a **versioned published** Loom API with contract tests. First supported target is independently deployable ASP.NET Core with EF Core. Generated apps have no design-time dependency on Clarify Studio.

## 2. Logical components
- `Clarify.Model`: immutable semantic AST, stable IDs, versioned model schema, canonical serialization; entity types, keys, relationships, validation, domain types, reporting and UI metadata.
- `Clarify.Validation`: graph integrity, calculated-field dependency analysis, relation consistency, migration risk, target capability analysis.
- `Clarify.Compiler`: parse > normalize > validate > resolve extensions > intermediate application representation (IAR) > emit generators > reproducibility manifest.
- `Clarify.Targets.AspNetCore`: emit C# services, API controllers/minimal endpoints, authorization, DI, EF Core mappings and migrations.
- `Clarify.Targets.Web`: emits responsive web form, browse and lookup components; initial renderer choice is a recorded architecture decision.
- `Clarify.Studio`: local-first browser IDE hosted on localhost by default, project/dictionary explorer, WYSIWYG forms, report designer, preview and errors.
- `Clarify.Reporting`: report AST and separate HTML/print-PDF renderers, group breaks, totals, expressions and authorization.
- `Clarify.Extensions`: schema-bound manifests, typed design/build hooks and runtime composition, ordered dependency graph, signed/pinned packages, separate-process hosting for untrusted code.
- `Clarify.Cli`: init/import/validate/generate/diff/build/test/migrate-plan commands, all usable without the IDE.
- `Clarify.Targets.Loom` (future independent integration): optional adapter with no reverse dependency.

## 3. Schema contract
Each entity and field carries a stable UUID separate from its display name; renames are detectable. Canonical JSON is normative and YAML a friendly serialization. Domain/column pools define inherited types and metadata with explicit overrides. Model validation rejects computed-expression cycles, inconsistent keys, unsupported provider features, unsafe transformations and ambiguous relationship behavior.

## 4. Generated application isolation
Generation writes only to `Generated/**` and a generated manifest. Custom code lives in `App/**` and implements typed hooks, DI decorators, partial classes or interfaces. Rebuilding never overwrites user files. Stable extension points are versioned, and compatibility breaks produce diagnostics. Output should be understandable and debuggable, not a proprietary bytecode.

## 5. Transactions and correctness
For order entry: server-side validation, price snapshots, decimal currency with explicit rounding, optimistic version fields, database transaction over order and detail records, idempotency token for retryable saves, and foreign-key constraints. App logs trace IDs, not secrets. No UI-only integrity enforcement.

## 6. Persistence and Loom
EF Core is the initial persistence adapter and uses provider-specific capabilities through explicit profiles: SQLite first; PostgreSQL and SQL Server verified through containers. Do not leak DbContext types into semantic dictionary APIs. Loom integration maps semantic entities into Loom-published models/services through a narrow generated adapter. If an advanced feature lacks Loom support, generation must produce a diagnostic, not silently substitute behavior.

## 7. Testing strategy (mandatory)
- **xUnit**: all deterministic unit, compiler, schema and application tests.
- **NSubstitute**: mocks for generator IO boundaries, project manifests, time, handlers and extension sequencing; do *not* mock the database to claim relational correctness.
- **Testcontainers for .NET**: PostgreSQL and SQL Server (and other real service dependencies as needed) for integration/transaction/migration tests.
- SQLite in-memory with a kept-open connection for fast relational tests; NSubstitute for contract tests.
- Golden snapshot tests verify deterministic generated source; compile and boot generated app in CI.
- Playwright tests verify actual browser-generated browses and forms.
- Test IDs map back to requirement IDs and appear in a generated traceability report. Environment-dependent tests identify Docker prerequisites.
- CI matrix: PR fast lane (model/generator/SQLite), container integration lane, browser lane, nightly cross-provider parity/performance.

## 8. Extensions
Extension manifests declare compatible generator versions, hooks, order, dependencies, target capabilities, config schema and requested permissions. A deterministic topological sort precedes generation; cycles and conflicting writers fail fast. Design-time transformations operate on immutable semantic models and return patches. Runtime components use DI, middleware and ordered domain command pipelines, not source string injection. Third-party .NET code is not presumed sandboxed in-process.

## 9. Reports
Report model contains parameters, dataset queries, hierarchical bands/groups, expressions, aggregates, break conditions, locale-aware formatting, page semantics and output. HTML preview and paginated PDF are separate targets with parity tests for data, but not presumed pixel-identical. Invoice reference report is first acceptance fixture.

## 10. Build plan
M0 — documentation, contracts and CI. M1 — dictionary + validation + SQLite schema import. M2 — deterministic generator + compilable standalone customer CRUD. M3 — Studio form/browse designer. M4 — master-detail order entry. M5 — report designer and invoice. M6 — extensions, multi-provider tests, optional Loom adapter. Each milestone requires traced passing tests; do not equate written requirements with completed work.

## 11. Architecture decisions outstanding
ADR-002 frontend renderer; ADR-003 JSON vs YAML authoring truth; ADR-004 plugin trust/hosting; ADR-005 schema migration ownership; ADR-006 API generator style; ADR-007 Visual Designer event model.


## 12. Five-pass RAD/database review (2026-10-14)
See [expert review](REVIEW-5-PASSES.md) and requirements REV1..REV5. The following supersede any broad assumption that provider independence is free:
- Record a capability profile for each database; disallow unsupported schema/query semantics unless explicit reviewed degradation.
- Use separate provider-specific migration outputs. Require dry-run/backfill and production DBA approval for destructive transformations; forbid automatic production migration by default.
- SQLite's limitations on decimal ordering/comparison, sequences, and schema DDL must be tested. Never use floating point as the silent money storage fallback.
- For RAD completeness, treat menu/navigation generation, wizard speed, stateful browse, fast keyboard entry, report grouping, permissions, audit and bulk editing as first-class design surfaces.
- Visual edits are user-owned declarative patches with stable IDs; generator writes are staged and atomic, with conflict previews and rollback.
- Generated applications are portable and runnable without Loom or Studio; optional Loom adapter is isolated behind a capability contract.
- Beta criteria require demonstrably successful nondeveloper workflows, strict provider parity checks, fault injection, traceability reports, security/accessibility tests, and generated output code review.
- Distinguish **requirements tracker** (stable IDs, status, evidence) from **STRIDE security model**. A machine-readable tracker and CI checker are now required; Markdown alone is insufficient.
