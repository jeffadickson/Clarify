using Clarify.Generated;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

// Executable acceptance smoke for actual generated EF Core relationships.
// Runs entirely in an isolated in-memory SQLite database.
await using var connection = new SqliteConnection("Data Source=:memory:;Foreign Keys=True");
await connection.OpenAsync();
var options = new DbContextOptionsBuilder<SalesDbContext>()
    .UseSqlite(connection).Options;
await using var db = new SalesDbContext(options);
await db.Database.EnsureCreatedAsync();

var customer = new Customer { Name = "Example Customer" };
var product = new Product { Name = "Test Product", PriceCents = 1499 };
db.CustomerRows.Add(customer);
db.ProductRows.Add(product);
await db.SaveChangesAsync();

var order = new Order { CustomerId = customer.Id, Date = new DateOnly(2026, 10, 15) };
db.OrderRows.Add(order);
await db.SaveChangesAsync();

var line = new OrderLine { OrderId = order.Id, ProductId = product.Id, Quantity = 2, UnitPriceCents = 1499 };
db.OrderLineRows.Add(line);
await db.SaveChangesAsync();

if (await db.OrderLineRows.CountAsync() != 1)
    throw new Exception("Generated child relationship did not persist.");
// Restrict prevents deleting a referenced Customer.
db.CustomerRows.Remove(customer);
try {
    await db.SaveChangesAsync();
    throw new Exception("Restrict relationship unexpectedly allowed parent deletion.");
}
catch (DbUpdateException) {
    db.Entry(customer).State = EntityState.Unchanged;
}
// Deleting the order cascades to its line, but not to the product.
db.OrderRows.Remove(order);
await db.SaveChangesAsync();
if (await db.OrderLineRows.AnyAsync() || !await db.ProductRows.AnyAsync())
    throw new Exception("Cascade/delete semantics do not match the dictionary.");
Console.WriteLine("Clarify generated EF Core relationship smoke passed.");
