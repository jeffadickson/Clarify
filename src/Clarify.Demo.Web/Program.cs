using Clarify.Forms.Shared;
using Clarify.Observability;
using Clarify.Demo.Data;
using Microsoft.EntityFrameworkCore;
using Clarify.Demo.Web.Components;
using Clarify.Demo.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
var builder=WebApplication.CreateBuilder(args);
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddGeneratedObservability(builder.Configuration);
// Identity is a host service; Elsa may refer to the same stable user and role IDs.
builder.Services.AddDbContext<ClarifyIdentityDb>(o =>
    o.UseSqlite($"Data Source={Path.Combine(builder.Environment.ContentRootPath, "clarify-identity.sqlite")}"));
builder.Services.AddClarifyAuthentication(builder.Configuration);
builder.Services.AddClarifyAuthorization();
builder.Services.AddScoped<IAuthorizationHandler, OrderApprovalHandler>();
builder.Services.AddScoped<IAuthorizationHandler, ApplicationPermissionHandler>();
var file=Path.Combine(builder.Environment.ContentRootPath,"clarify-customers.sqlite");
builder.Services.AddDbContextFactory<CustomerDb>(o=>o.UseSqlite($"Data Source={file}"));
builder.Services.AddScoped<ICustomerStore,SqliteCustomerStore>();
builder.Services.AddDbContextFactory<SalesDb>(o=>o.UseSqlite("Data Source=clarify-sales.sqlite"));
builder.Services.AddScoped<IOrderStore,SqliteOrderStore>();
builder.Services.AddDbContextFactory<ApprovalDb>(o=>o.UseSqlite($"Data Source={Path.Combine(builder.Environment.ContentRootPath,"clarify-workflows.sqlite")}"));
builder.Services.AddScoped<ApprovalWorkflowService>();
builder.Services.AddDbContextFactory<WorkflowRuntimeDb>(o=>o.UseSqlite($"Data Source={Path.Combine(builder.Environment.ContentRootPath,"clarify-runtime.sqlite")}"));
builder.Services.AddScoped<WorkflowRunner>();
builder.Services.AddScoped<IWorkflowActivity>(_=>new DemoWorkflowActivity("ValidateOrder"));
builder.Services.AddScoped<IWorkflowActivity>(_=>new DemoWorkflowActivity("ReserveInventory"));
builder.Services.AddScoped<IWorkflowActivity>(_=>new DemoWorkflowActivity("PublishOrder"));
var app=builder.Build();
using(var scope=app.Services.CreateScope()){
    var identityDb=scope.ServiceProvider.GetRequiredService<ClarifyIdentityDb>();
    // Demo initialization only. Production must use reviewed EF Core migrations.
    await identityDb.Database.EnsureCreatedAsync();
}
app.UseAuthentication();
app.UseAuthorization();
app.MapApplicationAccess();
// Preview parser stays read-only and available only for local development.
if (app.Environment.IsDevelopment() &&
    System.Net.IPAddress.IsLoopback(app.Urls.Count > 0
        ? new System.Net.IPEndPoint(System.Net.IPAddress.Loopback, 0).Address
        : System.Net.IPAddress.Loopback))
{
    app.MapNativeDslPreview();
}
// Local-only Identity API for experimenting with users, sign-in and roles.
// Do not expose open registration in production; use an administrator-controlled
// enrollment flow or external OIDC provider.
if (app.Environment.IsDevelopment() &&
    builder.Configuration.GetValue<bool>("Clarify:Authentication:EnableLocalRegistration") &&
    (builder.Configuration["Clarify:Authentication:Provider"] ?? "Identity") == "Identity")
    app.MapGroup("/identity").MapIdentityApi<ClarifyUser>();
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
using(var scope=app.Services.CreateScope()){
 var factory=scope.ServiceProvider.GetRequiredService<IDbContextFactory<WorkflowRuntimeDb>>();
 await using var db=await factory.CreateDbContextAsync();
 await db.Database.EnsureCreatedAsync(); // proof of concept; migrations required
}
using(var scope=app.Services.CreateScope()){
 var dbFactory=scope.ServiceProvider.GetRequiredService<IDbContextFactory<SalesDb>>();
 await using var sales=await dbFactory.CreateDbContextAsync();
 await sales.Database.EnsureCreatedAsync();
 if (!await sales.Products.AnyAsync()) {
  sales.Products.AddRange(new CatalogProduct { Name = "Digital Multimeter", PriceCents = 44900 }, new CatalogProduct { Name = "Signal Generator", PriceCents = 129950 });
  await sales.SaveChangesAsync();
 }
}
// Source mutation endpoints are restricted to Development on loopback.
app.Use(async (ctx,next) => {
 if (ctx.Request.Path.StartsWithSegments("/api/studio")) {
  var remote=ctx.Connection.RemoteIpAddress;
  if (!app.Environment.IsDevelopment() || remote is null ||
      !System.Net.IPAddress.IsLoopback(remote) ||
      (ctx.Request.Method != "GET" &&
       (!ctx.Request.HasJsonContentType() ||
        ctx.Request.Headers.Origin.Count != 1 ||
        !Uri.TryCreate(ctx.Request.Headers.Origin.ToString(),UriKind.Absolute,out var origin) ||
        origin.Host != ctx.Request.Host.Host || origin.Port != ctx.Request.Host.Port))) {
    ctx.Response.StatusCode=403;
    return;
  }
 }
 await next();
});
if (app.Environment.IsDevelopment()) app.MapStudioSource();
// Demonstration-only API: unauthenticated workflow operations must never be exposed remotely.
app.Use(async (ctx,next)=>{
 if(ctx.Request.Path.StartsWithSegments("/api/workflows") &&
    (!app.Environment.IsDevelopment() ||
     !System.Net.IPAddress.IsLoopback(ctx.Connection.RemoteIpAddress ?? System.Net.IPAddress.None))) {
  ctx.Response.StatusCode=403;
  await ctx.Response.WriteAsync("Demo workflow endpoints restricted to development loopback.");
  return;
 }
 // Mutations of the unauthenticated demo API must not accept cross-site requests.
 if (ctx.Request.Path.StartsWithSegments("/api/workflows") &&
     !HttpMethods.IsGet(ctx.Request.Method)) {
   var origin = ctx.Request.Headers.Origin.ToString();
   if (!string.IsNullOrEmpty(origin) &&
       (!Uri.TryCreate(origin, UriKind.Absolute, out var parsedOrigin) ||
        !(parsedOrigin.Host.Equals("localhost", StringComparison.OrdinalIgnoreCase) ||
          (System.Net.IPAddress.TryParse(parsedOrigin.Host, out var parsedIp) &&
           System.Net.IPAddress.IsLoopback(parsedIp))) ||
        parsedOrigin.Port != ctx.Request.Host.Port)) {
     ctx.Response.StatusCode=403;
     await ctx.Response.WriteAsync("Cross-origin workflow operations are disabled.");
     return;
   }
   if (!ctx.Request.HasJsonContentType()) {
     ctx.Response.StatusCode=415;
     return;
   }
 }
 await next();
});
app.MapPost("/api/workflows/runs",async (RunRequest request,WorkflowRunner runner,CancellationToken ct)=>{
 var plan=System.Text.Json.JsonSerializer.Deserialize<WorkflowPlan>(
  await File.ReadAllTextAsync(Path.Combine(app.Environment.ContentRootPath,"OrderApproval.plan.json"),ct));
 if(plan is null) return Results.BadRequest("Missing published plan");
 return Results.Ok(await runner.StartAsync(plan,request.CorrelationKey,ct));
});
app.MapGet("/api/workflows/runs/{id}",async (string id,WorkflowRunner runner,CancellationToken ct)=>
 (await runner.GetAsync(id,ct)) is { } run ? Results.Ok(run) : Results.NotFound());
app.MapPost("/api/workflows/runs/{id}/advance",async (string id,WorkflowRunner runner,CancellationToken ct)=>
 Results.Ok(await runner.AdvanceAsync(id,ct)));
app.MapPost("/api/workflows/runs/{id}/signal",async (string id,RunSignal request,WorkflowRunner runner,CancellationToken ct)=>
 Results.Ok(await runner.SignalAsync(id,request.Event,request.IdempotencyKey,ct)));
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


public sealed record RunRequest(string CorrelationKey);
public sealed record RunSignal(string Event,string IdempotencyKey);
public sealed class DemoWorkflowActivity(string name): IWorkflowActivity {
 public string Name=>name;
 public Task ExecuteAsync(string runId,string token,CancellationToken cancellationToken) {
  // No external effects. Real implementations must persistently deduplicate by token.
  return Task.CompletedTask;
 }
}
