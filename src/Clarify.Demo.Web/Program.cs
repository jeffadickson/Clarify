using Clarify.Forms.Shared;
using Clarify.Demo.Data;
using Microsoft.EntityFrameworkCore;
using Clarify.Demo.Web.Components;
var builder=WebApplication.CreateBuilder(args);
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
var file=Path.Combine(builder.Environment.ContentRootPath,"clarify-customers.sqlite");
builder.Services.AddDbContextFactory<CustomerDb>(o=>o.UseSqlite($"Data Source={file}"));
builder.Services.AddScoped<ICustomerStore,SqliteCustomerStore>();
builder.Services.AddDbContextFactory<ApprovalDb>(o=>o.UseSqlite($"Data Source={Path.Combine(builder.Environment.ContentRootPath,"clarify-workflows.sqlite")}"));
builder.Services.AddScoped<ApprovalWorkflowService>();
var app=builder.Build();
using(var scope=app.Services.CreateScope()){
 var factory=scope.ServiceProvider.GetRequiredService<IDbContextFactory<CustomerDb>>();
 await using var db=await factory.CreateDbContextAsync();
 await db.Database.EnsureCreatedAsync(); // demo only; replace with migration plan
 // A separate context maps the workflow tables; EnsureCreated does NOT add tables to an existing DB.
 // For the M0 demo use a separate workflow SQLite database until EF migrations are consolidated.
}
using(var scope=app.Services.CreateScope()){
 var factory=scope.ServiceProvider.GetRequiredService<IDbContextFactory<ApprovalDb>>();
 await using var db=await factory.CreateDbContextAsync();
 await db.Database.EnsureCreatedAsync(); // temporary demo initialization
}
// Demonstration-only API: unauthenticated workflow operations must never be exposed remotely.
app.Use(async (ctx,next)=>{
 if(ctx.Request.Path.StartsWithSegments("/api/workflows") &&
    (!app.Environment.IsDevelopment() ||
     !System.Net.IPAddress.IsLoopback(ctx.Connection.RemoteIpAddress ?? System.Net.IPAddress.None))) {
  ctx.Response.StatusCode=403;
  await ctx.Response.WriteAsync("Demo workflow endpoints restricted to development loopback.");
  return;
 }
 await next();
});
app.MapGet("/api/workflows", async (ApprovalWorkflowService service,CancellationToken ct)=> await service.ListAsync(ct));
app.MapGet("/api/workflows/{id}", async (string id,ApprovalWorkflowService service,CancellationToken ct)=> (await service.FindAsync(id,ct)) is { } snapshot ? Results.Ok(snapshot) : Results.NotFound());
app.MapPost("/api/workflows/orders/{orderId:long}/submit", async (long orderId, SubmitRequest request,ApprovalWorkflowService service,CancellationToken ct)=> Results.Ok(await service.SubmitAsync(orderId,request.CorrelationKey,ct)));
app.MapPost("/api/workflows/{id}/decision", async (string id, DecisionRequest request,ApprovalWorkflowService service,CancellationToken ct)=> Results.Ok(await service.DecideAsync(id,request.Approved,request.Actor,request.IdempotencyKey,ct)));
app.UseStaticFiles();
app.UseAntiforgery();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode().AddAdditionalAssemblies(typeof(CustomerPage).Assembly);
app.Run();

public sealed record SubmitRequest(string CorrelationKey);
public sealed record DecisionRequest(bool Approved,string Actor,string IdempotencyKey);

