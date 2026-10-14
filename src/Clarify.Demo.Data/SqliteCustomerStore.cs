using Clarify.Forms.Shared;
using Clarify.Observability;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
namespace Clarify.Demo.Data;
public sealed class CustomerRow {
 public long Id {get;set;}
 public string Name {get;set;}="";
 public string? Email {get;set;}
 public string? City {get;set;}
}
public sealed class CustomerDb(DbContextOptions<CustomerDb> options): DbContext(options){
 public DbSet<CustomerRow> Customers=>Set<CustomerRow>();
 protected override void OnModelCreating(ModelBuilder b) {
  b.Entity<CustomerRow>().HasKey(x=>x.Id);
  b.Entity<CustomerRow>().Property(x=>x.Name).HasMaxLength(160).IsRequired();
  b.Entity<CustomerRow>().Property(x=>x.Email).HasMaxLength(254);
  b.Entity<CustomerRow>().Property(x=>x.City).HasMaxLength(120);
 }
}
public sealed class SqliteCustomerStore(IDbContextFactory<CustomerDb> factory): ICustomerStore {
 public async Task<IReadOnlyList<CustomerModel>> ListAsync(CancellationToken ct=default) {
  await using var db=await factory.CreateDbContextAsync(ct);
  return await db.Customers.AsNoTracking().OrderBy(x=>x.Name).Select(x=>new CustomerModel{Id=x.Id,Name=x.Name,Email=x.Email,City=x.City}).ToListAsync(ct);
 }
 public async Task<CustomerModel?> GetAsync(long id,CancellationToken ct=default){
  await using var db=await factory.CreateDbContextAsync(ct);
  return await db.Customers.AsNoTracking().Where(x=>x.Id==id).Select(x=>new CustomerModel{Id=x.Id,Name=x.Name,Email=x.Email,City=x.City}).SingleOrDefaultAsync(ct);
 }
 public async Task<long> SaveAsync(CustomerModel model,CancellationToken ct=default) {
  using var activity=ClarifyTelemetry.StartOperation("clarify.customer.save","Customer");
  var started=Stopwatch.GetTimestamp();
  try {
  var validation=new System.Collections.Generic.List<System.ComponentModel.DataAnnotations.ValidationResult>();
  if(!System.ComponentModel.DataAnnotations.Validator.TryValidateObject(model,new System.ComponentModel.DataAnnotations.ValidationContext(model),validation,true))throw new ArgumentException(string.Join("; ",validation.Select(x=>x.ErrorMessage)));
  await using var db=await factory.CreateDbContextAsync(ct);
  CustomerRow row;
  if(model.Id==0){row=new CustomerRow();db.Customers.Add(row);}
  else{row=await db.Customers.FindAsync(new object[]{model.Id},ct) ?? throw new KeyNotFoundException("Customer not found");}
  row.Name=model.Name;row.Email=model.Email;row.City=model.City;
  await db.SaveChangesAsync(ct);
  ClarifyTelemetry.Saves.Add(1,new KeyValuePair<string,object?>("entity","Customer"));
  return row.Id;
  } catch(Exception ex) {
   ClarifyTelemetry.SaveFailures.Add(1,new KeyValuePair<string,object?>("entity","Customer"));
   activity?.SetStatus(ActivityStatusCode.Error,ex.GetType().Name);
   throw;
  } finally {
   ClarifyTelemetry.SaveDuration.Record(Stopwatch.GetElapsedTime(started).TotalMilliseconds,new KeyValuePair<string,object?>("entity","Customer"));
  }
 }
}
