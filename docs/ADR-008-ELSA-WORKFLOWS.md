# ADR-008: Elsa 3 as Clarify's workflow engine

**Date:** 2026-10-14
**Status:** Proposed for implementation; benchmark and runtime verification pending
**Decision:** Adopt Elsa Workflows 3.8.x as the preferred production embedded workflow runtime for Clarify-generated .NET applications. Keep a versioned `IWorkflowGateway` adapter. Retire the custom sequential runner from the production path after parity tests (retain it only as a teaching/compatibility fixture). Do not silently migrate existing instances.

## Reasons
- Elsa includes durable workflow instances, event-correlated bookmarks, definitions/versioning, scheduled activities and an established runtime.
- Elsa Studio adds visual workflow authoring and inspection. 3.8 ships sequence/state-machine designer capabilities and supports .NET 10.
- Can run in-process inside ASP.NET Core generated applications with EF Core-backed persistence, without a separately operated Temporal-style service.
- Clarify can generate C# activity classes and call Elsa from user-defined commands.

## Important boundary: DSL and Studio
ClarifyDSL remains **the human-readable source of truth** for Clarify-managed workflows. Parse DSL into a typed, versioned workflow AST; compile the supported subset to Elsa definitions. Changes in Clarify visual editor must produce minimal DSL patches, and DSL changes re-render the graph. Do not pretend Elsa Studio's native JSON is automatically losslessly equivalent: any conversion back requires stable node identifiers and explicit diagnostics on unsupported Elsa features. An integrated Elsa Studio panel may provide advanced editing, but save/export must preserve or clearly flag unmappable constructs. Prefer initial one-way DSL→Elsa compilation followed by explicit two-way parity tests before enabling general bidirectional editing.

## Runtime interfaces
`Clarify.Workflow.Abstractions`: start, signal, cancel, query, read history, enumerate waits, apply versioned definition, register typed activities.
`Clarify.Workflow.Elsa`: optional package using matching Elsa 3.8.x packages, EF Core-backed definition/instance/bookmark stores. Keep versions consistently pinned.
Generated application chooses `AddClarifyElsaWorkflow(...)`; simple CRUD projects don't pull in the engine.

## Security, correctness and failure semantics
- Manager approvals require an authenticated principal and authorization policy; never accept a caller-supplied actor string as proof of identity.
- Correlation/idempotency for signals; activity external effects require transactional outbox/inbox or independent deduplication. Persisted bookmark alone does not guarantee exactly-once external effects.
- Use versioned definitions, explicit in-flight migration policy and permission-gated repair/retry/cancel.
- Test restart while waiting, crash after outbound effect before checkpoint, concurrent competing workers, timer recovery, duplicate signals, handler failure/retry, and DB migration.
- Do not enable remote Studio or admin APIs by default.
- SQLite for local single-process deployment, PostgreSQL/SQL Server for multiworker deployments after container-based verification.

## Acceptance
1. .NET 10 project restores, builds, and starts using pinned Elsa 3.8 package versions.
2. OrderApproval: validate → manager wait → persisted restart → authenticated approve → inventory reservation → publish.
3. No repeated external effect when activity is retried after injected crash.
4. DSL and visual graph edits round-trip with stable identifiers for supported grammar.
5. xUnit + NSubstitute unit tests, Testcontainers PostgreSQL and SQL Server tests, browser Playwright verification.
6. Windows desktop workflow inspection uses shared UI; engine may be hosted locally or accessed via a backend-neutral contract.

## Alternatives
- Workflow Core: smaller dependency and embedded support, but weaker integrated visual/runtime design offering.
- Stateless: good for record state transitions, not long-running orchestration.
- CoreWF: a migration-oriented .NET workflow option without equivalent contemporary RAD designer.
- Dapr Workflow: durable but requires sidecar.
- Temporal: operationally robust, requires separate service.

## References
https://www.elsaworkflows.io/get-started
https://www.elsaworkflows.io/get-started/elsa-server-and-studio
https://docs.elsaworkflows.io/guides/persistence

**Implementation note:** This ADR does not claim that Elsa is already installed, compiled, or running in Clarify.
