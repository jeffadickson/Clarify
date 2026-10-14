using Clarify.Examples.Elsa;
using Elsa.Extensions;

// Integration compile fixture. This process intentionally listens only on
// loopback and does not start a public approval server.
var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://127.0.0.1:0");
builder.Services.AddElsa(elsa => elsa.AddActivity<WaitForManagerApproval>());
builder.Services.AddControllers();
builder.Services.AddAuthorization(options =>
    options.AddPolicy("ClarifyManager", policy =>
        policy.RequireAuthenticatedUser().RequireRole("Manager")));

// NOTE: A real host also configures Elsa identity, runtime EF Core persistence,
// authentication middleware and order-level authorization. Those are separate
// integration gates. This project compiles the Elsa activity/controller APIs.
var app = builder.Build();
app.MapGet("/health", () => Results.Ok("Clarify Elsa approval API compiled"));
app.MapControllers();
app.Run();
