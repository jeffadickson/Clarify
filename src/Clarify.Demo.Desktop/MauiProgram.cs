using Clarify.Demo.Data;
using Clarify.Forms.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
namespace Clarify.Demo.Desktop;
public static class MauiProgram {
 public static MauiApp CreateMauiApp(){
  var builder=MauiApp.CreateBuilder();
  builder.UseMauiApp<App>();
  builder.Services.AddMauiBlazorWebView();
  var file=Path.Combine(FileSystem.AppDataDirectory,"clarify-customers.sqlite");
  builder.Services.AddDbContextFactory<CustomerDb>(o=>o.UseSqlite($"Data Source={file}"));
  builder.Services.AddScoped<ICustomerStore,SqliteCustomerStore>();
  var app=builder.Build();
  using(var scope=app.Services.CreateScope()){
   var factory=scope.ServiceProvider.GetRequiredService<IDbContextFactory<CustomerDb>>();
   using var db=factory.CreateDbContext();
   db.Database.EnsureCreated(); // demo-only initialization
  }
  return app;
 }
}