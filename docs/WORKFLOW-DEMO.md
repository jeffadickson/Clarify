# Clarify embedded workflow M0 — executable persistence slice

## Implemented
The web host exposes a **development-loopback-only** workflow proof of concept:
- `POST /api/workflows/orders/{orderId}/submit` with JSON `{"correlationKey":"order-42"}`
- `GET /api/workflows` list instances
- `GET /api/workflows/{id}` inspect waiting/completed state
- `POST /api/workflows/{id}/decision` with JSON `{"approved":true,"actor":"manager","idempotencyKey":"approve-42"}`

The `ApprovalWorkflowService` is backed by EF Core SQLite and logs idempotent approval/rejection events. Start returns `WaitingApproval`; decision returns `Approved` or `Rejected`. State persists across app restarts in `clarify-workflows.sqlite`, independent of the customer demo SQLite file. A file-backed xUnit acceptance test creates a fresh service after submit and asserts recovery and idempotency.

### Try locally
```bash
dotnet run --project src/Clarify.Demo.Web --urls http://127.0.0.1:5000
curl -X POST http://127.0.0.1:5000/api/workflows/orders/42/submit -H "Content-Type: application/json" -d '{"correlationKey":"order-42"}'
# Copy the returned instanceId; restart the server
curl http://127.0.0.1:5000/api/workflows/<instanceId>
curl -X POST http://127.0.0.1:5000/api/workflows/<instanceId>/decision -H "Content-Type: application/json" -d '{"approved":true,"actor":"manager","idempotencyKey":"approve-42"}'
```

## Critical restrictions
- This is a **bounded persisted approval state machine**, not yet full Workflow Core integration.
- The demo endpoints have **no authentication** and are blocked unless the host is in Development and the request originates from a loopback address. They must not be enabled in production.
- Actor is demo-supplied, not identity-authenticated; real approvals require policy checks and audited user principals.
- No timeouts, distributed lease workers, callbacks to inventory, activity retries, durable outbox, or automatic workflow definition compilation yet.
- `EnsureCreated` is still used for M0; EF migrations must replace it for versioned schema evolution.
- In multi-process concurrent submission or approval races, unique constraints may reject one request; production needs conflict translation and coordinated retries.

## Acceptance and verification
`dotnet test tests/Clarify.M0.Tests/Clarify.M0.Tests.csproj` includes a restart-and-approval test. GitHub Actions will attempt to build, test and publish. Current authoring environment lacks dotnet/Docker; no passing .NET build or runtime smoke check is claimed.
