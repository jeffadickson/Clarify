# Clarify: Custom C# and embedded workflows — design v0.1
Status: architecture accepted for exploration; runtime not production-verified.

## Product contract
ClarifyDSL forms and visual designer remain declarative, generated C# stays user-readable and standalone, and developer-owned C# is never overwritten by regeneration. Clarify itself remains separate from Loom. A generated application can host durable workflows using a local database without depending on Clarify Studio, Loom, or an external orchestration server.

## C# developer experience
- Project tree separates `Generated/` (managed by compiler) from `Custom/` (user-owned), with README, templates, editor templates and `dotnet` solution.
- Commands, validators, calculated-value providers, custom lookup data sources, workflow steps, authorization policies, and services have stable typed interfaces.
- C# source is indexed by .NET language services; Visual Studio / VS Code debugger attaches to the generated .NET application (do not implement our own debugger).
- Errors in .clarify source resolve to dictionary symbols, custom C# command registrations and source locations.
- Generated DI registration must be deterministic and duplicate/ambiguous handler registrations must fail validation.
- Preview/build buttons cannot execute untrusted source without explicit local consent; web-hosted design services require sandbox/isolation.
- Provide Add C# command/validator/step actions to scaffold a class with xUnit test and a stable registration.
- Build manifests identify owned files and record hashes; regeneration is staged and can be rolled back.
- C# is the extension language, not an escape hatch executed inside an untrusted DSL interpreter.

## Workflow functional model
Workflow definitions may be declared in ClarifyDSL or ordinary C#, but both compile to a version-pinned, typed intermediate definition. Example:

```clarify
workflow OrderApproval for Order {
  version 1
  start when Order.Submitted
  step ValidateOrder
  wait Approval from Manager
  step ReserveInventory
  step PublishOrder
  on error { retry 3 notify Administrator }
}
```

The sample is **proposed syntax**, not yet parsed by the current ClarifyDSL compiler.

## Runtime provider decision
- Initial adapter evaluation: **Workflow Core** as embedded engine (persistence + event waits + retries). Check supported .NET versions, maintainership and activity persistence semantics before locking it down.
- Alternative: **Elsa 3** adapter for more sophisticated visual orchestrations, activity catalogs and bookmark semantics.
- Clarify controls workflow definitions, activity registration, app-specific authorization, schema data types, audit trail and version policies; workflow engine implementation is a replaceable runtime adapter.
- Do not create an independent competing orchestration engine as the default.

## Durable execution semantics
1. Publish immutable workflow definition vN. New instances bind to vN; existing instances keep vN unless a reviewed migration is explicitly run.
2. Persist instance ID, correlation key, definition/version, state, current wait bookmark or activity, retry schedule, step history, idempotency keys, and actor approval records.
3. Transitions and local business state changes should commit atomically within one DB transaction when sharing a database; when not possible, use an outbox, idempotent consumer and reconciliation.
4. Activity handlers **may run more than once** after crashes. Treat external email, payment, hardware, publication or network I/O as at-least-once; enforce idempotency and compensation.
5. Wait for externally correlated events and human approvals without occupying a thread while paused.
6. Use persisted timers and retry schedules; restore after restart; prevent two workers from claiming the same runnable activity using optimistic concurrency/leases.
7. Persistent storage provider: SQLite for local/single-process applications, PostgreSQL or SQL Server for multi-node operations. In-memory engine provider only for tests.
8. Offline mode: local application may continue transitions; queue remote notifications in outbox and synchronize idempotently later. Separate operational completion from central acknowledgment.
9. Operator view: current state, timeline, pending approvals, exceptions, retry, resume and cancel with role-based permissions, signed actor trace and audit.
10. Tracing: OpenTelemetry spans and metrics at workflow start, step, wait, restart and failure; no sensitive payloads in logs.

## Acceptance test reference
Order created -> SubmitOrder command -> ValidateOrder C# activity -> wait for manager approval -> restart app -> approval event correlated to instance -> ReserveInventory -> publish outbox -> final Completed. Exercise crash after external execution but before acknowledgment and verify idempotency. Confirm same UI/behavior in Blazor Web and MAUI Hybrid.

## Cross-project reuse with Loom
Publish an independent `ClarifyDSL.Core` model and UI renderer. Loom may map its DocTypes to Clarify form descriptions and may choose to host the workflow adapter, but Clarify workflow engine is optional and cannot replace Loom's authorization or transactional rules. No automatic Loom dependency or repository edits.

## Initial technology review
- Workflow Core: https://github.com/danielgerlag/workflow-core
- Elsa 3: https://docs.elsaworkflows.io/
- Blazor C# code behind: https://learn.microsoft.com/en-us/dotnet/architecture/blazor-for-web-forms-developers/components
