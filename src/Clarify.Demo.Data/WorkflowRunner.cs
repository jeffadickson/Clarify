using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace Clarify.Demo.Data;

/// <summary>Published, immutable workflow definition compiled from ClarifyDSL.</summary>
public sealed record WorkflowOperation(string Kind, string Name, string? Role = null);
public sealed record WorkflowPlan(string Name, string Entity, int Version, string Trigger, WorkflowOperation[] Operations, int MaxRetries = 3) {
    public void Validate() {
        if (string.IsNullOrWhiteSpace(Name) || Version < 1 || Operations.Length == 0 || MaxRetries is < 0 or > 100)
            throw new ArgumentException("Invalid workflow plan.");
        foreach (var operation in Operations)
            if (operation.Kind is not ("step" or "wait") || string.IsNullOrWhiteSpace(operation.Name))
                throw new ArgumentException("Unknown workflow operation.");
    }
}
public sealed class WorkflowRun {
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string Definition { get; set; } = "";
    public int Version { get; set; }
    public string PlanJson { get; set; } = "";
    public string CorrelationKey { get; set; } = "";
    public int NextIndex { get; set; }
    public string Status { get; set; } = "Ready";
    public string? WaitingFor { get; set; }
    public string? Error { get; set; }
    public int Attempts { get; set; }
    public DateTime UpdatedUtc { get; set; } = DateTime.UtcNow;
}
public sealed class WorkflowReceipt {
    public long Id { get; set; }
    public string RunId { get; set; } = "";
    public string Token { get; set; } = "";
    public string Operation { get; set; } = "";
}
public sealed class WorkflowRuntimeDb(DbContextOptions<WorkflowRuntimeDb> options) : DbContext(options) {
    public DbSet<WorkflowRun> Runs => Set<WorkflowRun>();
    public DbSet<WorkflowReceipt> Receipts => Set<WorkflowReceipt>();
    protected override void OnModelCreating(ModelBuilder b) {
        b.Entity<WorkflowRun>().HasKey(x => x.Id);
        b.Entity<WorkflowRun>().HasIndex(x => x.CorrelationKey).IsUnique();
        b.Entity<WorkflowReceipt>().HasKey(x => x.Id);
        b.Entity<WorkflowReceipt>().HasIndex(x => x.Token).IsUnique();
    }
}
public interface IWorkflowActivity {
    string Name { get; }
    /// <summary>May run more than once after a crash; use the stable token to deduplicate effects.</summary>
    Task ExecuteAsync(string runId, string token, CancellationToken cancellationToken);
}
public sealed class WorkflowRunner(
    IDbContextFactory<WorkflowRuntimeDb> factory,
    IEnumerable<IWorkflowActivity> activities) {
    private readonly IReadOnlyDictionary<string,IWorkflowActivity> handlers =
        activities.ToDictionary(x => x.Name, StringComparer.Ordinal);

    public async Task<WorkflowRun> StartAsync(WorkflowPlan plan, string correlationKey, CancellationToken ct = default) {
        plan.Validate();
        if (string.IsNullOrWhiteSpace(correlationKey)) throw new ArgumentException("Missing correlation key");
        await using var db = await factory.CreateDbContextAsync(ct);
        var existing = await db.Runs.AsNoTracking().SingleOrDefaultAsync(x => x.CorrelationKey == correlationKey, ct);
        if (existing != null) {
            if (existing.Definition != plan.Name || existing.Version != plan.Version)
                throw new InvalidOperationException("Correlation key conflicts with a different definition");
            return existing;
        }
        var run = new WorkflowRun {
            Definition=plan.Name,Version=plan.Version,
            PlanJson=JsonSerializer.Serialize(plan),CorrelationKey=correlationKey
        };
        db.Runs.Add(run);
        await db.SaveChangesAsync(ct);
        return run;
    }
    public async Task<WorkflowRun?> GetAsync(string id, CancellationToken ct=default) {
        await using var db = await factory.CreateDbContextAsync(ct);
        return await db.Runs.AsNoTracking().SingleOrDefaultAsync(x=>x.Id==id,ct);
    }
    /// <summary>Advance a run until it reaches a durable wait, error, or completion.</summary>
    public async Task<WorkflowRun> AdvanceAsync(string id, CancellationToken ct=default) {
        await using var db=await factory.CreateDbContextAsync(ct);
        var run=await db.Runs.SingleAsync(x=>x.Id==id,ct);
        var plan=JsonSerializer.Deserialize<WorkflowPlan>(run.PlanJson) ?? throw new InvalidOperationException("Invalid persisted workflow definition");
        plan.Validate();
        while (run.NextIndex < plan.Operations.Length && run.Status is "Ready" or "Failed") {
            var op=plan.Operations[run.NextIndex];
            if (op.Kind=="wait") {
                run.Status="Waiting";run.WaitingFor=op.Name;run.UpdatedUtc=DateTime.UtcNow;
                await db.SaveChangesAsync(ct);break;
            }
            if (!handlers.TryGetValue(op.Name,out var handler))
                throw new InvalidOperationException("No registered workflow activity: "+op.Name);
            var token=run.Id+":"+run.NextIndex;
            try {
                // Replaying an activity is safe only if the handler independently deduplicates using token.
                await handler.ExecuteAsync(run.Id,token,ct);
                run.NextIndex++;run.Attempts=0;run.Error=null;run.Status="Ready";
                run.UpdatedUtc=DateTime.UtcNow;
                await db.SaveChangesAsync(ct);
            } catch(Exception ex) when (ex is not OperationCanceledException) {
                run.Attempts++;
                run.Error=ex.Message;run.Status="Failed";run.UpdatedUtc=DateTime.UtcNow;
                await db.SaveChangesAsync(ct);break;
            }
        }
        if (run.NextIndex==plan.Operations.Length && run.Status=="Ready") {
            run.Status="Completed";run.WaitingFor=null;run.UpdatedUtc=DateTime.UtcNow;
            await db.SaveChangesAsync(ct);
        }
        return run;
    }
    public async Task<WorkflowRun> SignalAsync(string id,string eventName,string token,CancellationToken ct=default) {
        if (string.IsNullOrWhiteSpace(token)) throw new ArgumentException("Idempotency token is required");
        await using var db=await factory.CreateDbContextAsync(ct);
        await using var transaction=await db.Database.BeginTransactionAsync(ct);
        var run=await db.Runs.SingleAsync(x=>x.Id==id,ct);
        var receipt=await db.Receipts.AsNoTracking().SingleOrDefaultAsync(x=>x.Token==token,ct);
        if(receipt!=null) {
            if(receipt.RunId!=id || receipt.Operation!=eventName)throw new InvalidOperationException("Conflicting signal token");
            return run;
        }
        if(run.Status!="Waiting" || run.WaitingFor!=eventName)
            throw new InvalidOperationException("Workflow is not waiting for "+eventName);
        run.NextIndex++;run.Status="Ready";run.WaitingFor=null;run.UpdatedUtc=DateTime.UtcNow;
        db.Receipts.Add(new WorkflowReceipt{RunId=id,Token=token,Operation=eventName});
        await db.SaveChangesAsync(ct);await transaction.CommitAsync(ct);
        return run;
    }
}
