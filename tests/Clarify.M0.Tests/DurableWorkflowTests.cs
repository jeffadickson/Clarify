using Clarify.Demo.Data;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Clarify.M0.Tests;

public sealed class DurableWorkflowTests {
 [Fact]
 public async Task ApprovalSurvivesFreshDatabaseContextsAndIsIdempotent() {
  var file=Path.Combine(Path.GetTempPath(),"clarify-workflow-"+Guid.NewGuid().ToString("N")+".sqlite");
  try {
   var options=new DbContextOptionsBuilder<ApprovalDb>().UseSqlite($"Data Source={file}").Options;
   await using(var db=new ApprovalDb(options)) await db.Database.EnsureCreatedAsync();
   var first=new ApprovalWorkflowService(new StaticApprovalFactory(options));
   var started=await first.SubmitAsync(42,"order-42");
   Assert.Equal("WaitingApproval",started.Status);
   // Construct a NEW service / context factory, as after an application restart.
   var afterRestart=new ApprovalWorkflowService(new StaticApprovalFactory(options));
   Assert.Equal("WaitingApproval",(await afterRestart.FindAsync(started.InstanceId))!.Status);
   var approved=await afterRestart.DecideAsync(started.InstanceId,true,"manager","approve-42");
   Assert.Equal("Approved",approved.Status);
   var replay=await afterRestart.DecideAsync(started.InstanceId,true,"manager","approve-42");
   Assert.Equal("Approved",replay.Status);
   Assert.Equal(started.InstanceId,(await first.SubmitAsync(42,"order-42")).InstanceId);
   await using var db2=new ApprovalDb(options);
   Assert.Single(await db2.Events.ToListAsync());
  } finally { if(File.Exists(file))File.Delete(file); }
 }
 private sealed class StaticApprovalFactory(DbContextOptions<ApprovalDb> options):IDbContextFactory<ApprovalDb> {
  public ApprovalDb CreateDbContext()=>new(options);
 }
}
