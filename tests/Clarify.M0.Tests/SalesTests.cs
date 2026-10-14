using Clarify.Demo.Data;
using Clarify.Forms.Shared;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Clarify.M0.Tests;

// Checks a real SQLite transaction and verifies server-side catalog price snapshots.
public sealed class SalesTests {
 [Fact]
 public async Task SavesOrderAndLinesAcrossFreshContexts() {
  var a=Path.Combine(Path.GetTempPath(),Guid.NewGuid()+".sqlite");
  var b=Path.Combine(Path.GetTempPath(),Guid.NewGuid()+".sqlite");
  try {
   var so=new DbContextOptionsBuilder<SalesDb>().UseSqlite("Data Source="+a).Options;
   var co=new DbContextOptionsBuilder<CustomerDb>().UseSqlite("Data Source="+b).Options;
   await using(var db=new SalesDb(so)){
    await db.Database.EnsureCreatedAsync();
    db.Products.Add(new CatalogProduct{Name="Meter",PriceCents=44900});
    await db.SaveChangesAsync();
   }
   await using(var db=new CustomerDb(co)){
    await db.Database.EnsureCreatedAsync();
    db.Customers.Add(new CustomerRow{Name="Customer"});
    await db.SaveChangesAsync();
   }
   var sf=new Factory<SalesDb>(so);
   var cf=new Factory<CustomerDb>(co);
   var store=new SqliteOrderStore(sf,cf);
   var id=await store.SaveAsync(new OrderModel{CustomerId=1,Lines=[new(){ProductId=1,Quantity=2,UnitPriceCents=1}]});
   var again=new SqliteOrderStore(new Factory<SalesDb>(so),new Factory<CustomerDb>(co));
   var saved=await again.GetAsync(id);
   Assert.NotNull(saved);
   Assert.Equal(89800,saved.TotalCents);
   Assert.Equal(44900,Assert.Single(saved.Lines).UnitPriceCents);
  } finally {
   foreach(var path in new[]{a,b})if(File.Exists(path))File.Delete(path);
  }
 }
 private sealed class Factory<T>(DbContextOptions<T> options):IDbContextFactory<T> where T:DbContext {
  public T CreateDbContext()=>(T)Activator.CreateInstance(typeof(T),options)!;
 }
}
