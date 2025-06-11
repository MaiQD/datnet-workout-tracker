using FluentAssertions;
using dotFitness.Modules.Users.Domain.Entities;

namespace dotFitness.Modules.Users.Tests.Domain.Entities;

public class UserMetricTests
{
    [Fact]
    public void Should_Create_Valid_UserMetric_With_Required_Properties()
    {
        // Arrange & Act
        var userMetric = new UserMetric
        {
            UserId = "user123",
            Date = new DateTime(2024, 1, 1),
            Weight = 70.5,
            Height = 175.0
        };

        // Assert
        userMetric.Id.Should().NotBeNullOrEmpty();
        userMetric.UserId.Should().Be("user123");
        userMetric.Date.Should().Be(new DateTime(2024, 1, 1));
        userMetric.Weight.Should().Be(70.5);
        userMetric.Height.Should().Be(175.0);
        userMetric.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        userMetric.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Theory]
    [InlineData(70.0, 175.0, UnitPreference.Metric, 22.86)] // kg, cm
    [InlineData(154.0, 69.0, UnitPreference.Imperial, 22.73)] // lbs, inches
    public void Should_Calculate_BMI_Correctly(double weight, double height, UnitPreference unitPreference, double expectedBmi)
    {
        // Arrange
        var userMetric = new UserMetric
        {
            UserId = "user123",
            Weight = weight,
            Height = height
        };

        // Act
        userMetric.CalculateBmi(unitPreference);

        // Assert
        userMetric.Bmi.Should().BeApproximately(expectedBmi, 0.01);
    }

    [Fact]
    public void Should_Set_BMI_To_Null_When_Weight_Is_Missing()
    {
        // Arrange
        var userMetric = new UserMetric
        {
            UserId = "user123",
            Height = 175.0
        };

        // Act
        userMetric.CalculateBmi(UnitPreference.Metric);

        // Assert
        userMetric.Bmi.Should().BeNull();
    }

    [Fact]
    public void Should_Set_BMI_To_Null_When_Height_Is_Missing()
    {
        // Arrange
        var userMetric = new UserMetric
        {
            UserId = "user123",
            Weight = 70.0
        };

        // Act
        userMetric.CalculateBmi(UnitPreference.Metric);

        // Assert
        userMetric.Bmi.Should().BeNull();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void Should_Set_BMI_To_Null_When_Weight_Is_Invalid(double invalidWeight)
    {
        // Arrange
        var userMetric = new UserMetric
        {
            UserId = "user123",
            Weight = invalidWeight,
            Height = 175.0
        };

        // Act
        userMetric.CalculateBmi(UnitPreference.Metric);

        // Assert
        userMetric.Bmi.Should().BeNull();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void Should_Set_BMI_To_Null_When_Height_Is_Invalid(double invalidHeight)
    {
        // Arrange
        var userMetric = new UserMetric
        {
            UserId = "user123",
            Weight = 70.0,
            Height = invalidHeight
        };

        // Act
        userMetric.CalculateBmi(UnitPreference.Metric);

        // Assert
        userMetric.Bmi.Should().BeNull();
    }

    [Theory]
    [InlineData(17.0, "Underweight")]
    [InlineData(22.0, "Normal weight")]
    [InlineData(27.0, "Overweight")]
    [InlineData(32.0, "Obese")]
    [InlineData(null, "Unknown")]
    public void Should_Return_Correct_BMI_Category(double? bmi, string expectedCategory)
    {
        // Arrange
        var userMetric = new UserMetric
        {
            UserId = "user123",
            Bmi = bmi
        };

        // Act
        var category = userMetric.GetBmiCategory();

        // Assert
        category.Should().Be(expectedCategory);
    }

    [Fact]
    public void Should_Update_Weight_Successfully()
    {
        // Arrange
        var userMetric = new UserMetric
        {
            UserId = "user123",
            Weight = 70.0
        };
        var originalUpdatedAt = userMetric.UpdatedAt;

        Thread.Sleep(10); // Ensure time difference

        // Act
        userMetric.UpdateMetrics(weight: 75.0);

        // Assert
        userMetric.Weight.Should().Be(75.0);
        userMetric.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    [Fact]
    public void Should_Update_Height_Successfully()
    {
        // Arrange
        var userMetric = new UserMetric
        {
            UserId = "user123",
            Height = 175.0
        };
        var originalUpdatedAt = userMetric.UpdatedAt;

        Thread.Sleep(10); // Ensure time difference

        // Act
        userMetric.UpdateMetrics(height: 180.0);

        // Assert
        userMetric.Height.Should().Be(180.0);
        userMetric.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    [Fact]
    public void Should_Update_Notes_Successfully()
    {
        // Arrange
        var userMetric = new UserMetric
        {
            UserId = "user123",
            Notes = "Old notes"
        };
        var originalUpdatedAt = userMetric.UpdatedAt;

        Thread.Sleep(10); // Ensure time difference

        // Act
        userMetric.UpdateMetrics(notes: "New notes");

        // Assert
        userMetric.Notes.Should().Be("New notes");
        userMetric.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Should_Not_Update_Invalid_Weight(double invalidWeight)
    {
        // Arrange
        var userMetric = new UserMetric
        {
            UserId = "user123",
            Weight = 70.0
        };
        var originalWeight = userMetric.Weight;
        var originalUpdatedAt = userMetric.UpdatedAt;

        Thread.Sleep(10); // Ensure time difference

        // Act
        userMetric.UpdateMetrics(weight: invalidWeight);

        // Assert
        userMetric.Weight.Should().Be(originalWeight);
        userMetric.UpdatedAt.Should().Be(originalUpdatedAt);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Should_Not_Update_Invalid_Height(double invalidHeight)
    {
        // Arrange
        var userMetric = new UserMetric
        {
            UserId = "user123",
            Height = 175.0
        };
        var originalHeight = userMetric.Height;
        var originalUpdatedAt = userMetric.UpdatedAt;

        Thread.Sleep(10); // Ensure time difference

        // Act
        userMetric.UpdateMetrics(height: invalidHeight);

        // Assert
        userMetric.Height.Should().Be(originalHeight);
        userMetric.UpdatedAt.Should().Be(originalUpdatedAt);
    }

    [Fact]
    public void Should_Return_Weight_In_Unit()
    {
        // Arrange
        var userMetric = new UserMetric
        {
            UserId = "user123",
            Weight = 70.0
        };

        // Act
        var weight = userMetric.GetWeightInUnit(UnitPreference.Metric);

        // Assert
        weight.Should().Be(70.0);
    }

    [Fact]
    public void Should_Return_Null_Weight_When_Not_Set()
    {
        // Arrange
        var userMetric = new UserMetric
        {
            UserId = "user123"
        };

        // Act
        var weight = userMetric.GetWeightInUnit(UnitPreference.Metric);

        // Assert
        weight.Should().BeNull();
    }

    [Fact]
    public void Should_Return_Height_In_Unit()
    {
        // Arrange
        var userMetric = new UserMetric
        {
            UserId = "user123",
            Height = 175.0
        };

        // Act
        var height = userMetric.GetHeightInUnit(UnitPreference.Metric);

        // Assert
        height.Should().Be(175.0);
    }

    [Fact]
    public void Should_Return_Null_Height_When_Not_Set()
    {
        // Arrange
        var userMetric = new UserMetric
        {
            UserId = "user123"
        };

        // Act
        var height = userMetric.GetHeightInUnit(UnitPreference.Metric);

        // Assert
        height.Should().BeNull();
    }

    [Theory]
    [InlineData(70.0, UnitPreference.Metric, UnitPreference.Imperial, 154.32)] // kg to lbs
    [InlineData(154.32, UnitPreference.Imperial, UnitPreference.Metric, 70.0)] // lbs to kg
    [InlineData(70.0, UnitPreference.Metric, UnitPreference.Metric, 70.0)] // same unit
    public void Should_Convert_Weight_Between_Units(double weight, UnitPreference from, UnitPreference to, double expected)
    {
        // Act
        var converted = UserMetric.ConvertWeight(weight, from, to);

        // Assert
        converted.Should().BeApproximately(expected, 0.01);
    }

    [Theory]
    [InlineData(175.0, UnitPreference.Metric, UnitPreference.Imperial, 68.9)] // cm to inches
    [InlineData(68.9, UnitPreference.Imperial, UnitPreference.Metric, 175.0)] // inches to cm
    [InlineData(175.0, UnitPreference.Metric, UnitPreference.Metric, 175.0)] // same unit
    public void Should_Convert_Height_Between_Units(double height, UnitPreference from, UnitPreference to, double expected)
    {
        // Act
        var converted = UserMetric.ConvertHeight(height, from, to);

        // Assert
        converted.Should().BeApproximately(expected, 0.1);
    }
}
