using System.ComponentModel.DataAnnotations.Schema;
using Clarify.ValueTypes;
using Microsoft.EntityFrameworkCore;
using NodaMoney;
using UnitsNet;

namespace Clarify.Demo.Data;

/// <summary>
/// Example of database-compatible generated storage for strongly typed values.
/// Explicit scalar columns allow SQL indexing/filtering, unlike opaque JSON.
/// </summary>
public sealed class TypedCatalogItem
{
    public long Id { get; set; }
    public string Name { get; set; } = "";

    // Amount and currency are both persisted: never infer USD from locale.
    public decimal PriceAmount { get; set; }
    public string PriceCurrencyCode { get; set; } = "USD";

    // Canonical SI representation: the user may enter inches or millimeters.
    public double LengthMeters { get; set; }

    [NotMapped]
    public Money Price
    {
        get => StandardValues.ReadMoney(PriceAmount, PriceCurrencyCode);
        set
        {
            (PriceAmount, PriceCurrencyCode) = StandardValues.WriteMoney(value);
        }
    }

    [NotMapped]
    public Length Length
    {
        get => StandardValues.ReadLength(LengthMeters);
        set => LengthMeters = StandardValues.WriteLength(value);
    }
}

/// <summary>
/// Separate demonstration schema. Do not mutate the existing sales database
/// with EnsureCreated; production uses explicit reviewed EF Core migrations.
/// </summary>
public sealed class TypedValuesDb(DbContextOptions<TypedValuesDb> options) : DbContext(options)
{
    public DbSet<TypedCatalogItem> Items => Set<TypedCatalogItem>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        var entity = builder.Entity<TypedCatalogItem>();
        entity.HasKey(x => x.Id);
        entity.Property(x => x.Name).IsRequired().HasMaxLength(160);
        entity.Property(x => x.PriceCurrencyCode).IsRequired().HasMaxLength(3);
        entity.Property(x => x.PriceAmount).HasPrecision(19, 4);
        entity.HasIndex(x => new { x.PriceCurrencyCode, x.PriceAmount });
        entity.HasIndex(x => x.LengthMeters);
        entity.Ignore(x => x.Price);
        entity.Ignore(x => x.Length);
    }
}
