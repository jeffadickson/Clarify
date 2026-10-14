using System.ComponentModel.DataAnnotations;
namespace Clarify.Forms.Shared;

// Explicit order-entry contracts: amounts are integer cents, not floating-point.
// Submitted line prices are captured from the product catalog by the server.
public sealed class ProductModel {
 public long Id {get;set;}
 public string Name {get;set;}="";
 public long PriceCents {get;set;}
}
public sealed class OrderLineModel {
 public long ProductId {get;set;}
 public string ProductName {get;set;}="";
 [Range(1,100000)] public int Quantity {get;set;}=1;
 public long UnitPriceCents {get;set;}
 public long TotalCents=>checked(UnitPriceCents*Quantity);
}
public sealed class OrderModel {
 public long Id {get;set;}
 [Range(1,long.MaxValue)] public long CustomerId {get;set;}
 public DateOnly Date {get;set;}=DateOnly.FromDateTime(DateTime.Today);
 public List<OrderLineModel> Lines {get;set;}=[];
 public long TotalCents=>checked(Lines.Sum(x=>x.TotalCents));
}
public interface IOrderStore {
 Task<IReadOnlyList<ProductModel>> ProductsAsync(CancellationToken ct=default);
 Task<IReadOnlyList<OrderModel>> OrdersAsync(CancellationToken ct=default);
 Task<OrderModel?> GetAsync(long id,CancellationToken ct=default);
 Task<long> SaveAsync(OrderModel order,CancellationToken ct=default);
}
