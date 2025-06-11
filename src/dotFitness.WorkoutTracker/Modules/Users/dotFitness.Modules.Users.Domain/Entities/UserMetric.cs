using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using dotFitness.SharedKernel.Interfaces;
using UnitsNet;
using UnitsNet.Units;

namespace dotFitness.Modules.Users.Domain.Entities;

public class UserMetric : IEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    [BsonElement("userId")]
    [BsonRequired]
    public string UserId { get; set; } = string.Empty;

    [BsonElement("date")]
    [BsonRequired]
    public DateTime Date { get; set; } = DateTime.UtcNow.Date; // Store as date only

    [BsonElement("weight")]
    public double? Weight { get; set; } // in kg or lbs depending on user preference

    [BsonElement("height")]
    public double? Height { get; set; } // in cm or inches depending on user preference

    [BsonElement("bmi")]
    public double? Bmi { get; set; } // Calculated BMI

    [BsonElement("notes")]
    public string? Notes { get; set; }

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public void CalculateBmi(UnitPreference unitPreference)
    {
        if (!Weight.HasValue || !Height.HasValue || Weight <= 0 || Height <= 0)
        {
            Bmi = null;
            return;
        }

        Mass weight;
        Length height;

        // Create UnitsNet objects based on user preference
        if (unitPreference == UnitPreference.Imperial)
        {
            weight = Mass.FromPounds(Weight.Value);
            height = Length.FromInches(Height.Value);
        }
        else
        {
            weight = Mass.FromKilograms(Weight.Value);
            height = Length.FromCentimeters(Height.Value);
        }

        // Convert to metric for BMI calculation (kg/m²)
        var weightInKg = weight.Kilograms;
        var heightInMeters = height.Meters;

        Bmi = Math.Round(weightInKg / (heightInMeters * heightInMeters), 2);
        UpdatedAt = DateTime.UtcNow;
    }

    public string GetBmiCategory()
    {
        if (!Bmi.HasValue) return "Unknown";

        return Bmi.Value switch
        {
            < 18.5 => "Underweight",
            >= 18.5 and < 25 => "Normal weight",
            >= 25 and < 30 => "Overweight",
            >= 30 => "Obese",
            _ => "Unknown"
        };
    }

    public void UpdateMetrics(double? weight = null, double? height = null, string? notes = null)
    {
        var updated = false;
    
        if (weight is > 0)
        {
            Weight = weight;
            updated = true;
        }
    
        if (height is > 0)
        {
            Height = height;
            updated = true;
        }
    
        if (notes != null)
        {
            Notes = notes;
            updated = true;
        }
    
        if (updated)
            UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Gets the weight in the specified unit preference
    /// </summary>
    public double? GetWeightInUnit(UnitPreference unitPreference)
    {
        if (!Weight.HasValue) return null;

        // Assume stored weight is in the same unit as specified
        return Weight.Value;
    }

    /// <summary>
    /// Gets the height in the specified unit preference
    /// </summary>
    public double? GetHeightInUnit(UnitPreference unitPreference)
    {
        if (!Height.HasValue) return null;

        // Assume stored height is in the same unit as specified
        return Height.Value;
    }

    /// <summary>
    /// Converts weight from one unit to another
    /// </summary>
    public static double ConvertWeight(double weight, UnitPreference from, UnitPreference to)
    {
        if (from == to) return weight;

        return from == UnitPreference.Imperial 
            ? Mass.FromPounds(weight).Kilograms
            : Mass.FromKilograms(weight).Pounds;
    }

    /// <summary>
    /// Converts height from one unit to another
    /// </summary>
    public static double ConvertHeight(double height, UnitPreference from, UnitPreference to)
    {
        if (from == to) return height;

        return from == UnitPreference.Imperial 
            ? Length.FromInches(height).Centimeters
            : Length.FromCentimeters(height).Inches;
    }
}
