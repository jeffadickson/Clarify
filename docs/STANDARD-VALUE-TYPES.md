# Standard Clarify Money and Engineering Quantity Types

**Decision:** NodaMoney and UnitsNet are standard dependencies in Clarify-generated .NET applications, not optional plugins. Authors can still use ordinary `decimal`, `double` or custom C# types where appropriate.

## Included
- `src/Clarify.ValueTypes`: pinned NodaMoney 2.8.0 and UnitsNet 5.75.1, plus typed canonical database adapters.
- `src/Clarify.Demo.Data/TypedValuesDb.cs`: commented EF Core example storing `Money` as amount + currency code and `Length` as meters. The domain properties are strongly typed; underlying scalar columns remain available to SQL.
- `tests/Clarify.M0.Tests/StandardValueTypeTests.cs`: SQLite save, reopen and query tests.
- The visual dictionary recognizes `money`, `length`, `mass`, `pressure`, and `temperature`.

## Relational storage design
| Dictionary type | C# | Persisted columns | Caveat |
|---|---|---|---|
| money | NodaMoney.Money | decimal amount + ISO currency code | Currency-specific minor units and rounding policy; do not mix currencies in sums |
| length | UnitsNet.Length | double meters | Floating point precision; configurable tolerance |
| mass | UnitsNet.Mass | double kilograms | Display unit may differ from storage unit |
| pressure | UnitsNet.Pressure | double pascals | Distinguish gauge/absolute pressure at domain level |
| temperature | UnitsNet.Temperature | double kelvins | Distinguish absolute temperature and temperature difference |

The EF Core sample uses **two explicit money columns** rather than an opaque string converter, because the amount and currency should be independently queryable/indexable. A typed `Money` property is mapped via the scalar columns and intentionally marked `[NotMapped]`. This is a proven EF Core-compatible design pattern, but the committed SQLite tests have not yet been observed passing.

For production finance, SQL Server/PostgreSQL should use precise `decimal/numeric` columns and explicit precision. SQLite does not natively implement a true fixed-point decimal type; comparisons/sorting on decimal provider values require provider-specific validation. For USD-only existing demo records, integer cents remain supported and must be migrated deliberately, never silently reinterpreted. For multi-currency data, sum/group by currency or convert using an explicitly recorded exchange rate.

For scientific data, double-precision canonical SI storage is convenient, but if exact decimal precision is essential, a domain-specific decimal representation is preferable. Temperature difference is not interchangeable with absolute temperature.

## User experience
In the dictionary designer select a semantic type; generated forms should use currency-aware or unit-aware editors. Display units and cultures are presentation preferences, **not storage schema changes**. A 12-inch length persists as 0.3048 meters, and the user may display it as millimeters or inches.

## Remaining generation work
The typed values library, dictionary choices and EF Core example are committed. Automatic generation of the EF Core mappings and Blazor editors from arbitrary dictionary fields, migrations, PostgreSQL/SQL Server provider tests, per-field currency/unit policies, serialization and arithmetic validation are still required. Track them separately; do not claim full type generation from the example alone.

## References
- https://www.nuget.org/packages/NodaMoney/
- https://www.nuget.org/packages/UnitsNet/
- https://learn.microsoft.com/en-us/ef/core/modeling/value-conversions
