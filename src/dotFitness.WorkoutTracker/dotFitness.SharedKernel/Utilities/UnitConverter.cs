using UnitsNet;
using UnitsNet.Units;

namespace dotFitness.SharedKernel.Utilities;

/// <summary>
/// Utility class for unit conversions using UnitsNet
/// </summary>
public static class UnitConverter
{
    /// <summary>
    /// Converts weight between different units
    /// </summary>
    /// <param name="value">The weight value to convert</param>
    /// <param name="fromUnit">Source unit (kg, lbs, g, etc.)</param>
    /// <param name="toUnit">Target unit (kg, lbs, g, etc.)</param>
    /// <returns>Converted weight value</returns>
    public static double ConvertWeight(double value, MassUnit fromUnit, MassUnit toUnit)
    {
        var mass = UnitsNet.Mass.From(value, fromUnit);
        return mass.As(toUnit);
    }

    /// <summary>
    /// Converts length/height between different units
    /// </summary>
    /// <param name="value">The length value to convert</param>
    /// <param name="fromUnit">Source unit (cm, m, inches, feet, etc.)</param>
    /// <param name="toUnit">Target unit (cm, m, inches, feet, etc.)</param>
    /// <returns>Converted length value</returns>
    public static double ConvertLength(double value, LengthUnit fromUnit, LengthUnit toUnit)
    {
        var length = UnitsNet.Length.From(value, fromUnit);
        return length.As(toUnit);
    }

    /// <summary>
    /// Converts temperature between different units
    /// </summary>
    /// <param name="value">The temperature value to convert</param>
    /// <param name="fromUnit">Source unit (Celsius, Fahrenheit, Kelvin)</param>
    /// <param name="toUnit">Target unit (Celsius, Fahrenheit, Kelvin)</param>
    /// <returns>Converted temperature value</returns>
    public static double ConvertTemperature(double value, TemperatureUnit fromUnit, TemperatureUnit toUnit)
    {
        var temperature = UnitsNet.Temperature.From(value, fromUnit);
        return temperature.As(toUnit);
    }

    /// <summary>
    /// Converts duration/time between different units
    /// </summary>
    /// <param name="value">The duration value to convert</param>
    /// <param name="fromUnit">Source unit (seconds, minutes, hours, etc.)</param>
    /// <param name="toUnit">Target unit (seconds, minutes, hours, etc.)</param>
    /// <returns>Converted duration value</returns>
    public static double ConvertDuration(double value, DurationUnit fromUnit, DurationUnit toUnit)
    {
        var duration = UnitsNet.Duration.From(value, fromUnit);
        return duration.As(toUnit);
    }

    /// <summary>
    /// Common weight conversion helpers
    /// </summary>
    public static class Weight
    {
        public static double KilogramsToPounds(double kg) => ConvertWeight(kg, MassUnit.Kilogram, MassUnit.Pound);
        public static double PoundsToKilograms(double lbs) => ConvertWeight(lbs, MassUnit.Pound, MassUnit.Kilogram);
        public static double KilogramsToGrams(double kg) => ConvertWeight(kg, MassUnit.Kilogram, MassUnit.Gram);
        public static double GramsToKilograms(double g) => ConvertWeight(g, MassUnit.Gram, MassUnit.Kilogram);
    }

    /// <summary>
    /// Common length conversion helpers
    /// </summary>
    public static class Length
    {
        public static double CentimetersToInches(double cm) => ConvertLength(cm, LengthUnit.Centimeter, LengthUnit.Inch);
        public static double InchesToCentimeters(double inches) => ConvertLength(inches, LengthUnit.Inch, LengthUnit.Centimeter);
        public static double MetersToFeet(double m) => ConvertLength(m, LengthUnit.Meter, LengthUnit.Foot);
        public static double FeetToMeters(double ft) => ConvertLength(ft, LengthUnit.Foot, LengthUnit.Meter);
        public static double CentimetersToMeters(double cm) => ConvertLength(cm, LengthUnit.Centimeter, LengthUnit.Meter);
        public static double MetersToCentimeters(double m) => ConvertLength(m, LengthUnit.Meter, LengthUnit.Centimeter);
    }

    /// <summary>
    /// Common time conversion helpers
    /// </summary>
    public static class Time
    {
        public static double SecondsToMinutes(double seconds) => ConvertDuration(seconds, DurationUnit.Second, DurationUnit.Minute);
        public static double MinutesToSeconds(double minutes) => ConvertDuration(minutes, DurationUnit.Minute, DurationUnit.Second);
        public static double MinutesToHours(double minutes) => ConvertDuration(minutes, DurationUnit.Minute, DurationUnit.Hour);
        public static double HoursToMinutes(double hours) => ConvertDuration(hours, DurationUnit.Hour, DurationUnit.Minute);
    }
}
