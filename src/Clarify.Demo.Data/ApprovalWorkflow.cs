using Microsoft.EntityFrameworkCore;
using Clarify.Extensibility;

namespace Clarify.Demo.Data;

/// <summary>A database-persisted approval workflow with a deliberately small, explicit state model.
/// This is the first embedded workflow implementation, not a full Workflow Core adapter.</summary>
public sealed class ApprovalInstance {
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string DefinitionId { get; set; } = "OrderApproval";
    public int Version { get; set; } = 1;
    public long OrderId { get; set; }
    public string Status { get; set; } = "WaitingApproval";
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedUtc { get; set; } = DateTime.UtcNow;
    public string CorrelationKey { get; set; } = "";
}
public sealed class ApprovalEvent {
    public long Id { get; set; }
    public string InstanceId { get; set; } = "";
    public string IdempotencyKey { get; set; } = "";
    public string Actor { get; set; } = "";
    public string Action { get; set; } = "";
    public DateTime RecordedUtc { get; set; } = DateTime.UtcNow;
}
public sealed class ApprovalDb(DbContextOptions<ApprovalDb> options) : DbContext(options) {
    public DbSet<ApprovalInstance> Instances => Set<ApprovalInstance>();
    public DbSet<ApprovalEvent> Events => Set<ApprovalEvent>();
    protected override void OnModelCreating(ModelBuilder b) {
        b.Entity<ApprovalInstance>().HasKey(x => x.Id);
        b.Entity<ApprovalInstance>().HasIndex(x => x.CorrelationKey).IsUnique();
        b.Entity<ApprovalEvent>().HasKey(x => x.Id);
        b.Entity<ApprovalEvent>().HasIndex(x => x.IdempotencyKey).IsUnique();
        b.Entity<ApprovalEvent>().HasIndex(x => x.InstanceId);
    }
}
public sealed class ApprovalWorkflowService(IDbContextFactory<ApprovalDb> factory) {
    public async Task<WorkflowSnapshot> SubmitAsync(long orderId, string correlationKey, CancellationToken ct = default) {
        if (orderId < 1 || string.IsNullOrWhiteSpace(correlationKey)) throw new ArgumentException("Valid order ID and correlation key required");
        await using var db = await factory.CreateDbContextAsync(ct);
        var prior = await db.Instances.SingleOrDefaultAsync(x => x.CorrelationKey == correlationKey, ct);
        if (prior != null) {
            if (prior.OrderId != orderId) throw new InvalidOperationException("Correlation key reused for another order");
            return Snapshot(prior);
        }
        var instance = new ApprovalInstance { OrderId = orderId, CorrelationKey = correlationKey };
        db.Instances.Add(instance);
        await db.SaveChangesAsync(ct);
        return Snapshot(instance);
    }
    public async Task<WorkflowSnapshot?> FindAsync(string id, CancellationToken ct = default) {
        await using var db=await factory.CreateDbContextAsync(ct);
        var value=await db.Instances.AsNoTracking().SingleOrDefaultAsync(x=>x.Id==id,ct);
        return value is null ? null : Snapshot(value);
    }
    public async Task<IReadOnlyList<WorkflowSnapshot>> ListAsync(CancellationToken ct = default) {
        await using var db=await factory.CreateDbContextAsync(ct);
        return (await db.Instances.AsNoTracking().OrderByDescending(x=>x.CreatedUtc).ToListAsync(ct)).Select(Snapshot).ToList();
    }
    public async Task<WorkflowSnapshot> DecideAsync(string id, bool approved, string actor, string idempotencyKey, CancellationToken ct = default) {
        if (string.IsNullOrWhiteSpace(actor) || string.IsNullOrWhiteSpace(idempotencyKey)) throw new ArgumentException("Actor and idempotency key required");
        await using var db = await factory.CreateDbContextAsync(ct);
        await using var tx = await db.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable,ct);
        var instance=await db.Instances.SingleOrDefaultAsync(x=>x.Id==id,ct) ?? throw new KeyNotFoundException("Workflow not found");
        var used=await db.Events.AsNoTracking().SingleOrDefaultAsync(x=>x.IdempotencyKey==idempotencyKey,ct);
        if (used != null) {
            if (used.InstanceId!=id || used.Action!=(approved?"Approve":"Reject")) throw new InvalidOperationException("Idempotency key conflict");
            return Snapshot(instance);
        }
        if (instance.Status!="WaitingApproval") throw new InvalidOperationException("Workflow is not awaiting approval");
        instance.Status=approved?"Approved":"Rejected";
        instance.UpdatedUtc=DateTime.UtcNow;
        db.Events.Add(new ApprovalEvent{InstanceId=id,Actor=actor,Action=approved?"Approve":"Reject",IdempotencyKey=idempotencyKey});
        await db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);
        return Snapshot(instance);
    }
    private static WorkflowSnapshot Snapshot(ApprovalInstance x) =>
      new(x.Id,x.DefinitionId,x.Version,x.Status,x.Status=="WaitingApproval"?"ManagerApproval":null);
}
