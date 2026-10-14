# Clarify Core Identity and Application Permissions

## Architectural rule
Every generated Clarify application owns a permission namespace. ASP.NET Core Identity provides authentication (or optionally an external OIDC provider); Clarify's **application memberships, roles and permission grants** decide authorization. A platform Administrator is not automatically permitted to edit every application's business records.

## Data model
- `ClarifyUser`: stable host identity (ASP.NET Core Identity).
- `ClarifyApplication`: stable ID for each generated application.
- `ClarifyApplicationMembership`: user + application, including disabled state.
- `ClarifyApplicationRole`: named role scoped to one application.
- `ClarifyMembershipRole`: one user can have multiple roles within one application.
- `ClarifyRolePermission`: grants an action code within a given application's role.

For example:

| Application | Role | Grants |
|---|---|---|
| `sales` | `SalesClerk` | `orders.read`, `orders.create` |
| `sales` | `Approver` | `orders.read`, `orders.approve` |
| `inventory` | `Viewer` | `inventory.read` |
| `inventory` | `Administrator` | `inventory.read`, `inventory.edit` |

A user can be a sales Approver and inventory Viewer. Global host Identity roles do not imply app grants.

## Authorization path
A generated action must call `IAuthorizationService.AuthorizeAsync(user,new ApplicationPermissionResource(appId,permission),new ApplicationPermissionRequirement())` before reading or changing application data. Reject unauthorized calls at the server, even when buttons are hidden by the UI.

Row/record-level conditions are additional predicates. Approvals must verify actor identity, application permission, tenant boundary, task assignment, and no-self-approval rules. The generic `RecordOwnerId` parameter currently allows an owner-only condition when provided; it is not a substitute for a full row policy engine.

## Separation from Elsa
Elsa owns workflow execution and human task state. Clarify/host identity maps stable user IDs and groups to Elsa task candidates/assignees. An Elsa approval endpoint must enforce `orders.approve` for the *specific application and order* before signaling completion.

## Defaults
- Empty role set = deny.
- Missing/disabled application membership = deny.
- No matching grant = deny.
- Cross-application grants never transfer.
- Permission changes require audit records and administrator authorization.
- Tenant isolation, immutable audit logs, user management and external OIDC are next milestones.
- A default administrator must be created via explicit, secure bootstrap action; never ship seeded production credentials.

## Existing code
`ApplicationPermissions.cs`, `ClarifyIdentity.cs` and `Program.cs` contain the first EF Core/Identity model and registration. These are scaffolding, **not a verified production authorization feature**. The anonymous demonstration customer/order pages need application-level authorization before deployment. Migration scripts, role management UI, claim/release and audit logging are still outstanding.
