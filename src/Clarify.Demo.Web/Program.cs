using Clarify.Forms.Shared;
using Clarify.Demo.Data;
using Microsoft.EntityFrameworkCore;
using Clarify.Demo.Web.Components;
var builder=WebApplication.CreateBuilder(args);
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
var file=Path.Combine(builder.Environment.ContentRootPath,"clarify-customers.sqlite");
builder.Services.AddDbContextFactory<CustomerDb>(o=>o.UseSqlite($"Data Source={file}"));
builder.Services.AddScoped<ICustomerStore,SqliteCustomerStore>();
var app=builder.Build();
using(var scope=app.Services.CreateScope()){
 var factory=scope.ServiceProvider.GetRequiredService<IDbContextFactory<CustomerDb>>();
 await using var db=await factory.CreateDbContextAsync();
 await db.Database.EnsureCreatedAsync(); // demo only; replace with migration plan
}
app.UseStaticFiles();
app.UseAntiforgery();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode().AddAdditionalAssemblies(typeof(CustomerPage).Assembly);
app.Run();
