using Clarify.Demo.Data;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Clarify.M0.Tests;

public sealed class WorkflowRunnerTests {
    [Fact]
    public async Task CompiledPlan_ResumesAfterRestartWithoutRepeatingCompletedActivity() {
        var file=Path.Combine(Path.GetTempPath(),"clarify-run-"+Guid.NewGuid().ToString("N")+".sqlite");
        try {
            var options=new DbContextOptionsBuilder<WorkflowRuntimeDb>().UseSqlite($"Data Source={file}").Options;
            await using(var db=new WorkflowRuntimeDb(options)) await db.Database.EnsureCreatedAsync();
            var done=new List<string>();
            var activities=new IWorkflowActivity[] {
                new RecordedActivity("ValidateOrder",done),
                new RecordedActivity("ReserveInventory",done),
                new RecordedActivity("PublishOrder",done)
            };
            var plan=new WorkflowPlan("OrderApproval","Order",1,"Order.Submitted",[
                new("step","ValidateOrder"),new("wait","Approval","Manager"),
                new("step","ReserveInventory"),new("step","PublishOrder")]);
            var runner=new WorkflowRunner(new TestFactory(options),activities);
            var run=await runner.StartAsync(plan,"order:42:submit");
            var waiting=await runner.AdvanceAsync(run.Id);
            Assert.Equal("Waiting",waiting.Status);
            Assert.Equal(new[]{"ValidateOrder"},done);
            // Fresh runner/factory emulates application restart.
            var restarted=new WorkflowRunner(new TestFactory(options),activities);
            var prior=await restarted.GetAsync(run.Id);
            Assert.Equal("Waiting",prior!.Status);
            await restarted.SignalAsync(run.Id,"Approval","approved-42");
            await restarted.SignalAsync(run.Id,"Approval","approved-42"); // idempotent replay
            var completed=await restarted.AdvanceAsync(run.Id);
            Assert.Equal("Completed",completed.Status);
            Assert.Equal(new[]{"ValidateOrder","ReserveInventory","PublishOrder"},done);
            Assert.Equal("Completed",(await restarted.AdvanceAsync(run.Id)).Status);
            Assert.Equal(3,done.Count);
        } finally { if(File.Exists(file))File.Delete(file); }
    }
    private sealed class TestFactory(DbContextOptions<WorkflowRuntimeDb> options):IDbContextFactory<WorkflowRuntimeDb> {
        public WorkflowRuntimeDb CreateDbContext()=>new(options);
    }
    private sealed class RecordedActivity(string name,List<string> events):IWorkflowActivity {
        public string Name=>name;
        public Task ExecuteAsync(string runId,string token,CancellationToken ct) {
            events.Add(name);
            return Task.CompletedTask;
        }
    }
}
