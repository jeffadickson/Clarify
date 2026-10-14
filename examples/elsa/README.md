# Elsa-backed Clarify Order Approval — commented reference

These source files demonstrate Elsa 3.8's **actual** bookmark APIs and workflow resumer, rather than the earlier custom SQLite approval state machine.

## Included
- `WaitForManagerApproval.cs`: a real blocking Elsa Activity using `CreateBookmarkArgs`, stable business correlation, a resume callback and one-time consumption.
- `ManagerApprovalController.cs`: a real Elsa `IWorkflowResumer` integration protected by an ASP.NET Core authorization policy, with identity obtained from `HttpContext.User`.

## Host integration

Install/reference matching Elsa 3.8 packages and include these files in the **Elsa host project**, then register the activity:

```csharp
// Inside builder.Services.AddElsa(elsa => { ... }):
elsa.AddActivity<WaitForManagerApproval>();

// In the ASP.NET Core host (not anonymous endpoints):
builder.Services.AddControllers();
builder.Services.AddAuthorization(options =>
    options.AddPolicy("ClarifyManager", policy =>
        policy.RequireAuthenticatedUser().RequireRole("Manager")));

// After app.UseAuthentication(); app.UseAuthorization():
app.MapControllers();
```

**These are integration examples, not an activated Clarify runtime.** Import a published workflow containing `WaitForManagerApproval` into the generated Elsa combined host. Use the Elsa Studio diagram or `IWorkflowDefinitionImporter` as documented upstream. A compiled ClarifyDSL-to-Elsa definition adapter has not yet been completed.

## Restart acceptance procedure

1. With Elsa EF Core SQLite management *and runtime* stores enabled, publish the approval workflow version 1.
2. Start an instance with `OrderCorrelation=order:42` and confirm it creates a `Clarify.ManagerApproval` bookmark.
3. Stop the entire Elsa host process, restart it with the same SQLite database and signing configuration.
4. Authenticate a principal in the Manager role; submit a decision through the protected controller.
5. Verify Elsa resumed the **same instance**, recorded the actor, and followed the Approved/Rejected outcome.
6. Attempt duplicate resume and confirm there is no second business effect.

## Production rules

Bookmark auto-burn is **not** an exactly-once external effect guarantee. ReserveInventory and PublishOrder must use a durable outbox or idempotency key. The decision endpoint also needs resource-level authorization: a user with Manager role may not necessarily approve every order. Add explicit order-level permission checks and audit persistence before deployment.

The upstream Elsa 3.8 implementation guide is:
https://github.com/elsa-workflows/elsa-gitbook/blob/main/activities/blocking-and-triggers/README.md
