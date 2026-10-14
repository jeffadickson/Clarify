# ClarifyDSL → embedded workflow runtime

## What is executable now
The `OrderApproval.clarify` definition is parsed into a versioned JSON plan. The web application loads the published plan and uses a SQLite-backed `WorkflowRunner` to start, advance, wait for an event, resume, and finish the workflow. The runtime resolves activities by their C# `IWorkflowActivity.Name` using dependency injection. The initial demo handlers are **intentionally no-op** because real inventory and publishing services have not been integrated.

```sh
node --test workflow/workflowdsl.test.mjs workflow/compile.test.mjs
node workflow/compile.mjs
dotnet run --project src/Clarify.Demo.Web --urls http://127.0.0.1:5000
```

These endpoints are development-loopback-only. In another terminal:

```sh
curl -X POST http://127.0.0.1:5000/api/workflows/runs \
 -H 'Content-Type: application/json' \
 -d '{"correlationKey":"demo-order-123"}'
# Copy the returned Id, then:
curl -X POST http://127.0.0.1:5000/api/workflows/runs/ID/advance -H 'Content-Type: application/json' -d '{}'
# Status should become Waiting with WaitingFor=Approval.
# Stop and restart the app; the run persists in clarify-runtime.sqlite.
curl http://127.0.0.1:5000/api/workflows/runs/ID
curl -X POST http://127.0.0.1:5000/api/workflows/runs/ID/signal \
 -H 'Content-Type: application/json' \
 -d '{"event":"Approval","idempotencyKey":"approval-123"}'
curl -X POST http://127.0.0.1:5000/api/workflows/runs/ID/advance \
 -H 'Content-Type: application/json' -d '{}'
# Now status should be Completed.
```

## Design and correctness
- A run persists its **published plan snapshot** and version. Subsequent DSL changes do not rewrite in-flight definitions.
- `AdvanceAsync` records its completed cursor; `SignalAsync` commits cursor advance and unique signal token in one database transaction.
- Activity execution is **at least once**: the worker can crash after activity effects but before saving the cursor. User-owned activity handlers MUST deduplicate external effects using their stable token. The demo does not yet provide a transactional outbox.
- The retry budget is bounded; a failed activity stays `Failed` when retries are exhausted. There is no scheduled retry worker yet.
- This is a sequential workflow runner, not full Workflow Core, Elsa, parallel branches, subflows, timeouts, compensation or a visual branching compiler.
- The graphical editor displays a sequence and supports arrow-based step repositioning. Full source-preserving CST and branching designer are still incomplete.
- The HTTP API is **not authenticated**; its development-loopback restrictions make it unsuitable for production manager approvals.

## Automated verification
- `workflow/workflowdsl.test.mjs`: DSL parsing and source-edit validation.
- `workflow/compile.test.mjs`: plan emission and invalid syntax.
- `tests/Clarify.M0.Tests/WorkflowRunnerTests.cs`: file-based SQLite restart/wait/signal/replay/completion acceptance scenario.
- `.github/workflows/shared-blazor.yml`: runs DSL tests, regenerates plan and Razor, checks drift, builds, tests and publishes web artifact.

**Execution evidence status:** Source and tests committed, but .NET runtime tests / GitHub Actions completion have not been confirmed in this environment. This is not a verified release.

## Next steps for general workflows
- Typed roles/principals, authorization and audit trail.
- Durable leases and multiworker concurrency, retries with persisted deadlines, outbox and compensation.
- Branching, parallel joins, timers, subworkflows and version migration tooling.
- C# engine/parser integration (today JS compiler emits JSON plan consumed by C#).
- Independently verified .NET 10 CI and native desktop workflow UI.
