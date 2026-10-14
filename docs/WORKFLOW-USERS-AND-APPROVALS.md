# Users, roles and approvals in Clarify workflows

## Recommended model
**Elsa owns workflow execution and suspended task state; Clarify (or the host identity provider) owns identity, tenants, directory, and business authorization.**

### Example
```clarify
workflow OrderApproval for Order {
  version 1
  start when Order.Submitted
  step ValidateOrder
  wait Approval from Manager
  step ReserveInventory
  step PublishOrder
}
```
`Manager` is a *role/candidate group*, not a hard-coded login. An instance for order 42 can have a task with assignee null and candidate group `SalesManagers`. Eligible managers see it in Available; one claims it, moving it to Assigned to me. Other managers cannot complete the claimed task without an authorized reassignment. Admins can supervise only with an explicit privilege. A requester must not approve their own request if separation-of-duties policy forbids it.

## Runtime actor contract
- Authenticated user IDs and group IDs are stable, opaque, tenant-scoped identifiers supplied by the host.
- Task records bind workflow instance, activity/bookmark, tenant, order, candidate groups, assignee, state, revision, due date and permitted outcomes.
- The API verifies identity, tenant, claim, candidate membership, order-level permission, expected revision and idempotency operation ID **before** resuming Elsa.
- Audit the actual authenticated principal, old/new assignment, outcome and timestamps. Do not trust `actor` from the request body.
- Signal/restart may occur after a database commit; transactional outbox/inbox prevents losing a completed task or publishing the same effect twice.

## Elsa version choice
**Elsa 3.9.0 introduced an integrated User Tasks module** with `UserTask` activity, persistent task projections, assignees/candidate users/groups, claim/release/assign/complete, Studio inbox (Assigned to me, Available, History and manager All), revisions, operation IDs and a permission catalog. The existing Clarify scaffold pins **Elsa 3.8.0**, whose task assignment/inbox responsibilities would require custom Clarify code. Recommend a *reviewed upgrade to Elsa 3.9.x* before implementing native task assignment, to avoid reinventing features now available upstream.

For 3.9, wire Clarify's identity/groups to Elsa User Tasks via the provider/participant contracts; embed the task workspace in Clarify Studio or render the authorized Elsa task API. Do not assume an Elsa global admin role implies authorization to approve arbitrary orders.

## Versioned delivery plan
1. Verify Elsa 3.8 generated workflow class compilation and persisted bookmark test.
2. Upgrade Elsa templates/packages together to 3.9.x in an isolated branch with clean CI.
3. Use native User Tasks for manager approval, with host-driven participant lookup and task-level authorization.
4. Test two competing managers, stale claim revision, duplicate completion operation ID, tenant isolation and approval restart.
5. Preserve ClarifyDSL syntax, mapping `wait Approval from Manager` to native UserTask only when the host advertises that capability.

## Current state
The source generator `workflow/generate-elsa.mjs` produces an actual Elsa `WorkflowBase` class referencing our `WaitForManagerApproval` activity, with three no-op WriteLine demonstration steps. Full activity registration, Elsa persistence, 3.9 User Tasks, authentication, and restart verification are **not implemented and must not be claimed as working**.

References:
- https://github.com/elsa-workflows/elsa-gitbook/blob/main/guides/running-workflows/user-tasks.md
- https://github.com/elsa-workflows/elsa-core/blob/main/specs/013-user-tasks/contracts/studio-contract.md
