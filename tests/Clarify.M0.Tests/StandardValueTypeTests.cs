using Clarify.Demo.Data;
using Microsoft.EntityFrameworkCore;
using NodaMoney;
using UnitsNet;
using Xunit;

namespace Clarify.M0.Tests;

/// <summary>
/// A real SQLite round-trip verifies both the strong C# types and the physical
/// columns. This is not an EF InMemory test: actual SQL storage is exercised.
/// </summary>
public sealed class StandardValueTypeTests
{
    [Fact]
    public async Task MoneyAndLengthRoundTripThroughSqlite()
    {
        var path = Path.Combine(Path.GetTempPath(),
            "clarify-values-" + Guid.NewGuid().ToString("N") + ".db");
        try
        {
            var options = new DbContextOptionsBuilder<TypedValuesDb>()
                .UseSqlite("Data Source=" + path).Options;
            await using (var db = new TypedValuesDb(options))
            {
                await db.Database.EnsureCreatedAsync();
                db.Items.Add(new TypedCatalogItem
                {
                    Name = "Precision rod",
                    Price = new Money(19.95m, "EUR"),
                    Length = Length.FromInches(12)
                });
                await db.SaveChangesAsync();
            }

            await using (var db = new TypedValuesDb(options))
            {
                var item = await db.Items.AsNoTracking().SingleAsync();
                Assert.Equal(19.95m, item.Price.Amount);
                Assert.Equal("EUR", item.PriceCurrencyCode);
                Assert.InRange(item.Length.Meters, 0.3047999, 0.3048001);
                Assert.Equal(19.95m, item.PriceAmount);

                // Query on persisted scalar columns, not an unmapped .NET property.
                var eurCount = await db.Items.CountAsync(x => x.PriceCurrencyCode == "EUR");
                Assert.Equal(1, eurCount);
            }
        }
        finally
        {
            if (File.Exists(path)) File.Delete(path);
        }
    }

    [Fact]
    public void DifferentCurrenciesAreNotInterchangeable()
    {
        var usd = new Money(10m, "USD");
        var eur = new Money(10m, "EUR");
        Assert.NotEqual(usd, eur);
    }
}
