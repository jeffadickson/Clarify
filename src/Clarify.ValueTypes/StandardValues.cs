using NodaMoney;
using UnitsNet;

namespace Clarify.ValueTypes;

/// <summary>
/// Database-facing adapters for standard Clarify money and engineering types.
/// Store amount + ISO currency separately. Store measurements in canonical SI
/// units, never in a display-dependent unit such as inches or pounds.
/// </summary>
public static class StandardValues
{
    public static Money ReadMoney(decimal amount, string currencyCode) =>
        new(amount, currencyCode);

    public static (decimal Amount, string CurrencyCode) WriteMoney(Money value) =>
        (value.Amount, value.Currency.ToString());

    public static Length ReadLength(double meters) => Length.FromMeters(meters);
    public static double WriteLength(Length length) => length.Meters;

    public static Mass ReadMass(double kilograms) => Mass.FromKilograms(kilograms);
    public static double WriteMass(Mass mass) => mass.Kilograms;

    public static Pressure ReadPressure(double pascals) => Pressure.FromPascals(pascals);
    public static double WritePressure(Pressure pressure) => pressure.Pascals;

    public static Temperature ReadTemperature(double kelvins) => Temperature.FromKelvins(kelvins);
    public static double WriteTemperature(Temperature temperature) => temperature.Kelvins;
}
