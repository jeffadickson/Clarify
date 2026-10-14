using Clarify.Forms.Shared;
using Clarify.Observability;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace Clarify.Demo.Data;
// Separate demo database ensures existing CustomerDb.EnsureCreated() databases
// are not silently altered. Migration consolidation remains a release gate.
public sealed class CatalogProduct {public long Id{get;set;} public string Name{get;set;}=""; public long PriceCents{get;set;}}
public sealed class SalesOrder {public long Id{get;set;} public long CustomerId{get;set;} public DateOnly Date{get;set;} public List<SalesLine> Lines{get;set;}=[];}
public sealed class SalesLine {public long Id{get;set;} public long OrderId{get;set;} public SalesOrder Order{get;set;}=null!;public long ProductId{get;set;} public int Quantity{get;set;} public long UnitPriceCents{get;set;}}
public sealed class SalesDb(DbContextOptions<SalesDb> options):DbContext(options) {
 public DbSet<CatalogProduct> Products=>Set<CatalogProduct>();
 public DbSet<SalesOrder> Orders=>Set<SalesOrder>();
 protected override void OnModelCreating(ModelBuilder b) {
  b.Entity<CatalogProduct>().HasKey(x=>x.Id);b.Entity<CatalogProduct>().Property(x=>x.Name).IsRequired().HasMaxLength(160);
  b.Entity<SalesOrder>().HasKey(x=>x.Id);
  b.Entity<SalesOrder>().HasMany(x=>x.Lines).WithOne(x=>x.Order).HasForeignKey(x=>x.OrderId).OnDelete(DeleteBehavior.Cascade);
  b.Entity<SalesLine>().HasKey(x=>x.Id);
  b.Entity<SalesLine>().HasIndex(x=>x.ProductId);
  b.Entity<SalesLine>().HasOne<CatalogProduct>().WithMany().HasForeignKey(x=>x.ProductId).OnDelete(DeleteBehavior.Restrict);
 }
}
public sealed class SqliteOrderStore(IDbContextFactory<SalesDb> sales,IDbContextFactory<CustomerDb> customers):IOrderStore {
 public async Task<IReadOnlyList<ProductModel>> ProductsAsync(CancellationToken ct=default) {
  await using var db=await sales.CreateDbContextAsync(ct);
  return await db.Products.AsNoTracking().OrderBy(x=>x.Name).Select(x=>new ProductModel{Id=x.Id,Name=x.Name,PriceCents=x.PriceCents}).ToListAsync(ct);
 }
 public async Task<IReadOnlyList<OrderModel>> OrdersAsync(CancellationToken ct=default){
  await using var db=await sales.CreateDbContextAsync(ct);
  var all=await db.Orders.AsNoTracking().Include(x=>x.Lines).OrderByDescending(x=>x.Id).ToListAsync(ct);
  var names=await db.Products.AsNoTracking().ToDictionaryAsync(x=>x.Id,x=>x.Name,ct);
  return all.Select(x=>Map(x,names)).ToList();
 }
 public async Task<OrderModel?> GetAsync(long id,CancellationToken ct=default){
  await using var db=await sales.CreateDbContextAsync(ct);
  var order=await db.Orders.AsNoTracking().Include(x=>x.Lines).SingleOrDefaultAsync(x=>x.Id==id,ct);
  if(order is null)return null;
  var names=await db.Products.AsNoTracking().ToDictionaryAsync(x=>x.Id,x=>x.Name,ct);
  return Map(order,names);
 }
 private static OrderModel Map(SalesOrder x,Dictionary<long,string> names)=>new() {
  Id=x.Id,CustomerId=x.CustomerId,Date=x.Date,
  Lines=x.Lines.Select(l=>new OrderLineModel{ProductId=l.ProductId,ProductName=names.GetValueOrDefault(l.ProductId,"Unknown"),Quantity=l.Quantity,UnitPriceCents=l.UnitPriceCents}).ToList()
 };
 public async Task<long> SaveAsync(OrderModel request,CancellationToken ct=default) {
  using var activity=ClarifyTelemetry.StartOperation("clarify.order.save","Order");
  var started=Stopwatch.GetTimestamp();
  try {
  if(request.CustomerId<1||request.Lines.Count==0||request.Lines.Any(x=>x.Quantity<1||x.Quantity>100000))
    throw new ArgumentException("Select a customer and at least one valid order line.");
  await using(var db=await customers.CreateDbContextAsync(ct))
   if(!await db.Customers.AnyAsync(x=>x.Id==request.CustomerId,ct))throw new ArgumentException("Unknown customer");
  await using var salesDb=await sales.CreateDbContextAsync(ct);
  await using var tx=await salesDb.Database.BeginTransactionAsync(ct);
  var ids=request.Lines.Select(x=>x.ProductId).Distinct().ToArray();
  var catalog=await salesDb.Products.Where(x=>ids.Contains(x.Id)).ToDictionaryAsync(x=>x.Id,ct);
  if(catalog.Count!=ids.Length)throw new ArgumentException("Unknown product");
  SalesOrder order;
  if(request.Id==0){order=new SalesOrder();salesDb.Orders.Add(order);}
  else {order=await salesDb.Orders.Include(x=>x.Lines).SingleOrDefaultAsync(x=>x.Id==request.Id,ct)
      ??throw new KeyNotFoundException("Order not found");salesDb.RemoveRange(order.Lines);}
  order.CustomerId=request.CustomerId;order.Date=request.Date;
  // Preserve the original unit price for an existing order line;
  // new lines snapshot the catalog price at order time.
  var prior=order.Lines.ToLookup(x=>x.ProductId);
  order.Lines=request.Lines.Select(x=>new SalesLine{
    ProductId=x.ProductId,Quantity=x.Quantity,
    UnitPriceCents=request.Id>0 && prior[x.ProductId].Any()
      ?prior[x.ProductId].First().UnitPriceCents:catalog[x.ProductId].PriceCents
  }).ToList();
  foreach(var line in order.Lines) _=checked(line.UnitPriceCents*line.Quantity);
  await salesDb.SaveChangesAsync(ct);await tx.CommitAsync(ct);
  ClarifyTelemetry.Saves.Add(1,new KeyValuePair<string,object?>("entity","Order"));
  return order.Id;
  } catch(Exception ex) {
   ClarifyTelemetry.SaveFailures.Add(1,new KeyValuePair<string,object?>("entity","Order"));
   activity?.SetStatus(ActivityStatusCode.Error,ex.GetType().Name);
   throw;
  } finally {
   ClarifyTelemetry.SaveDuration.Record(Stopwatch.GetElapsedTime(started).TotalMilliseconds,new KeyValuePair<string,object?>("entity","Order"));
  }
 }
}
