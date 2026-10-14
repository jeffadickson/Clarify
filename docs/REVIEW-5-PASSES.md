# Clarify — five-pass RAD and database expert review
Date: 2026-10-14
Status: design critique completed; implementation not claimed.

## Pass 1 — End-user RAD workflow
**Challenge:** Can a new user create a useful application without code, then improve it without losing work?
**Findings:** navigation/menu generation, fast lookup, bulk data entry, saved filters, UX undo and wizard flow were missing. Dictionary alone is not the application; the user journey must be a testable product feature.
**Decision:** Add workflow-driven IDE requirements REV1 and a benchmark from empty project to working multi-entity app.
**Residual risk:** Need real usability research, not assumption-driven speed claims.

## Pass 2 — Database design correctness
**Challenge:** Will a generated application survive 10 years of schema evolution and multiple SQL providers?
**Findings:** Need index/query planning, collation, unique/null semantics, composite keys, conflict detection, migration review, SQL escape hatches, backup/restore and drift handling. Provider neutrality must not hide provider-specific behavior.
**Decision:** Add REV2, provider capability matrix and explicit DBA gate for destructive migrations.
**Special issue:** SQLite EF Core lacks native schemas/sequences/database-generated concurrency tokens; migrations may use rebuilds; decimal ordering and comparisons may be client evaluated. Provider migration sets should be separate. Review precision/scale and never silently use float for money.

## Pass 3 — Business application completeness
**Challenge:** Could a team replace its forms, reports, spreadsheets and business rules with Clarify?
**Findings:** Approval processes, report pagination, aggregates, asynchronous exports, audit history, granular permissions, rule engine and high-volume browsing require dedicated designs.
**Decision:** Add REV3 and define reports and permissions as foundational subsystems, not addons.
**Residual risk:** Workflow engine should be pluggable; avoid reinventing Loom.

## Pass 4 — Maintenance and third-party extensions
**Challenge:** Does regeneration preserve all developer work, and can packages evolve safely?
**Findings:** Need owned-file boundaries, visual-layout patches, staged atomic output, versioned intermediate representation, extension ordering, dependency/version negotiation, backward compatibility, debug symbols and a strict Loom adapter boundary.
**Decision:** Add REV4 and default to no required Loom dependency; offer compatibility-proven Loom target later.
**Residual risk:** Out-of-process plugin hosting remains a design study.

## Pass 5 — Adversarial acceptance and governance
**Challenge:** Can we empirically demonstrate Clarify's productivity and correctness?
**Findings:** Requirement IDs without automated traceability are insufficient. Need real user task studies, performance benchmarks, generated-app portability, resilient corruption recovery, CI matrix, and negative/fault testing.
**Decision:** Add REV5 and explicit beta exit gates.
**Residual risk:** P0 effort is still ambitious; use vertical slices rather than declaring all P0 as day-one requirements.

## Cross-pass priorities
1. First vertical slice: empty project > define/import Customer/Product/Order/OrderLine > generate > edit/browse > save correctly > produce invoice > inspect generated C#.
2. Capture stopwatch and user success metrics on the workflow, compare a hand-written C# + EF Core baseline and modern Clarion's demonstrated tasks, controlling for developer skill.
3. Build provider parity suite, including key/constraint, collation, concurrency, decimal, transaction, migration and report-query cases.
4. Independent generated output is a non-negotiable constraint; Loom is an optional target that reuses mature services but is not a prerequisite.
5. Adopt typed, parseable requirement records and validate requirement-to-test-to-commit evidence in CI. A Markdown checklist alone is not a fully operating tracker.

## References
- EF Core SQLite limitations: https://learn.microsoft.com/en-us/ef/core/providers/sqlite/limitations
- EF Core applying migrations: https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/applying
- EF Core multiple providers: https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/providers
- EF Core concurrency: https://learn.microsoft.com/en-us/ef/core/saving/concurrency
