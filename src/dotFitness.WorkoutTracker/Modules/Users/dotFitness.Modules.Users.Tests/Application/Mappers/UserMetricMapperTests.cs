using FluentAssertions;
using dotFitness.Modules.Users.Application.DTOs;
using dotFitness.Modules.Users.Application.Mappers;
using dotFitness.Modules.Users.Domain.Entities;

namespace dotFitness.Modules.Users.Tests.Application.Mappers;

public class UserMetricMapperTests
{
    private readonly UserMetricMapper _mapper;

    public UserMetricMapperTests()
    {
        _mapper = new UserMetricMapper();
    }

    [Fact]
    public void Should_Map_Entity_To_Dto_Correctly()
    {
        // Arrange
        var userMetric = new UserMetric
        {
            Id = "metric123",
            UserId = "user123",
            Date = new DateTime(2024, 1, 1),
            Weight = 70.5,
            Height = 175.0,
            Bmi = 23.02,
            Notes = "Morning measurement",
            CreatedAt = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc),
            UpdatedAt = new DateTime(2024, 1, 1, 10, 5, 0, DateTimeKind.Utc)
        };

        // Act
        var dto = _mapper.ToDto(userMetric);

        // Assert
        dto.Should().NotBeNull();
        dto.Id.Should().Be("metric123");
        dto.UserId.Should().Be("user123");
        dto.Date.Should().Be(new DateTime(2024, 1, 1));
        dto.Weight.Should().Be(70.5);
        dto.Height.Should().Be(175.0);
        dto.Bmi.Should().Be(23.02);
        dto.BmiCategory.Should().Be("Normal weight"); // Calculated from BMI value
        dto.Notes.Should().Be("Morning measurement");
        dto.CreatedAt.Should().Be(new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc));
        dto.UpdatedAt.Should().Be(new DateTime(2024, 1, 1, 10, 5, 0, DateTimeKind.Utc));
    }

    [Fact]
    public void Should_Handle_Null_Optional_Values_In_Mapping()
    {
        // Arrange
        var userMetric = new UserMetric
        {
            Id = "metric123",
            UserId = "user123",
            Date = new DateTime(2024, 1, 1),
            Weight = null, // Null optional field
            Height = null, // Null optional field
            Bmi = null, // Null BMI
            Notes = null, // Null notes
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Act
        var dto = _mapper.ToDto(userMetric);

        // Assert
        dto.Should().NotBeNull();
        dto.Id.Should().Be("metric123");
        dto.UserId.Should().Be("user123");
        dto.Date.Should().Be(new DateTime(2024, 1, 1));
        dto.Weight.Should().BeNull();
        dto.Height.Should().BeNull();
        dto.Bmi.Should().BeNull();
        dto.BmiCategory.Should().Be("Unknown"); // Should be "Unknown" when BMI is null
        dto.Notes.Should().BeNull();
    }

    [Fact]
    public void Should_Map_Weight_Only_Measurement()
    {
        // Arrange
        var userMetric = new UserMetric
        {
            Id = "metric123",
            UserId = "user123",
            Date = new DateTime(2024, 1, 1),
            Weight = 70.0,
            Height = null,
            Bmi = null, // No BMI without height
            Notes = "Weight only",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Act
        var dto = _mapper.ToDto(userMetric);

        // Assert
        dto.Should().NotBeNull();
        dto.Weight.Should().Be(70.0);
        dto.Height.Should().BeNull();
        dto.Bmi.Should().BeNull();
        dto.BmiCategory.Should().Be("Unknown");
        dto.Notes.Should().Be("Weight only");
    }

    [Fact]
    public void Should_Map_Height_Only_Measurement()
    {
        // Arrange
        var userMetric = new UserMetric
        {
            Id = "metric123",
            UserId = "user123",
            Date = new DateTime(2024, 1, 1),
            Weight = null,
            Height = 175.0,
            Bmi = null, // No BMI without weight
            Notes = "Height only",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Act
        var dto = _mapper.ToDto(userMetric);

        // Assert
        dto.Should().NotBeNull();
        dto.Weight.Should().BeNull();
        dto.Height.Should().Be(175.0);
        dto.Bmi.Should().BeNull();
        dto.BmiCategory.Should().Be("Unknown");
        dto.Notes.Should().Be("Height only");
    }

    [Theory]
    [InlineData(17.0, "Underweight")]
    [InlineData(22.0, "Normal weight")]
    [InlineData(27.0, "Overweight")]
    [InlineData(32.0, "Obese")]
    public void Should_Map_BMI_Categories_Correctly(double bmi, string expectedCategory)
    {
        // Arrange
        var userMetric = new UserMetric
        {
            Id = "metric123",
            UserId = "user123",
            Date = new DateTime(2024, 1, 1),
            Weight = 70.0,
            Height = 175.0,
            Bmi = bmi,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Act
        var dto = _mapper.ToDto(userMetric);

        // Assert
        dto.Should().NotBeNull();
        dto.Bmi.Should().Be(bmi);
        dto.BmiCategory.Should().Be(expectedCategory);
    }

    [Fact]
    public void Should_Preserve_DateTime_Precision()
    {
        // Arrange
        var preciseDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var preciseCreatedAt = new DateTime(2024, 1, 1, 10, 30, 45, 123, DateTimeKind.Utc);
        var preciseUpdatedAt = new DateTime(2024, 1, 1, 11, 45, 30, 456, DateTimeKind.Utc);
        
        var userMetric = new UserMetric
        {
            Id = "metric123",
            UserId = "user123",
            Date = preciseDate,
            Weight = 70.0,
            Height = 175.0,
            CreatedAt = preciseCreatedAt,
            UpdatedAt = preciseUpdatedAt
        };

        // Act
        var dto = _mapper.ToDto(userMetric);

        // Assert
        dto.Should().NotBeNull();
        dto.Date.Should().Be(preciseDate);
        dto.CreatedAt.Should().Be(preciseCreatedAt);
        dto.UpdatedAt.Should().Be(preciseUpdatedAt);
    }

    [Fact]
    public void Should_Map_Complete_Measurement_With_All_Fields()
    {
        // Arrange
        var userMetric = new UserMetric
        {
            Id = "metric123",
            UserId = "user123",
            Date = new DateTime(2024, 1, 15),
            Weight = 72.5,
            Height = 180.0,
            Bmi = 22.34,
            Notes = "Complete health check measurement with all metrics recorded",
            CreatedAt = new DateTime(2024, 1, 15, 9, 0, 0, DateTimeKind.Utc),
            UpdatedAt = new DateTime(2024, 1, 15, 9, 30, 0, DateTimeKind.Utc)
        };

        // Act
        var dto = _mapper.ToDto(userMetric);

        // Assert
        dto.Should().NotBeNull();
        dto.Id.Should().Be("metric123");
        dto.UserId.Should().Be("user123");
        dto.Date.Should().Be(new DateTime(2024, 1, 15));
        dto.Weight.Should().Be(72.5);
        dto.Height.Should().Be(180.0);
        dto.Bmi.Should().Be(22.34);
        dto.BmiCategory.Should().Be("Normal weight");
        dto.Notes.Should().Be("Complete health check measurement with all metrics recorded");
        dto.CreatedAt.Should().Be(new DateTime(2024, 1, 15, 9, 0, 0, DateTimeKind.Utc));
        dto.UpdatedAt.Should().Be(new DateTime(2024, 1, 15, 9, 30, 0, DateTimeKind.Utc));
    }

    [Fact]
    public void Should_Handle_Edge_Case_BMI_Values()
    {
        // Test boundary BMI values
        var testCases = new[]
        {
            (bmi: 18.4, expected: "Underweight"),
            (bmi: 18.5, expected: "Normal weight"),
            (bmi: 24.9, expected: "Normal weight"),
            (bmi: 25.0, expected: "Overweight"),
            (bmi: 29.9, expected: "Overweight"),
            (bmi: 30.0, expected: "Obese")
        };

        foreach (var (bmi, expected) in testCases)
        {
            // Arrange
            var userMetric = new UserMetric
            {
                Id = "metric123",
                UserId = "user123",
                Date = new DateTime(2024, 1, 1),
                Bmi = bmi,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Act
            var dto = _mapper.ToDto(userMetric);

            // Assert
            dto.BmiCategory.Should().Be(expected, $"BMI {bmi} should map to category '{expected}'");
        }
    }

    [Fact]
    public void Should_Map_Empty_Notes_String()
    {
        // Arrange
        var userMetric = new UserMetric
        {
            Id = "metric123",
            UserId = "user123",
            Date = new DateTime(2024, 1, 1),
            Weight = 70.0,
            Notes = "", // Empty string
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Act
        var dto = _mapper.ToDto(userMetric);

        // Assert
        dto.Should().NotBeNull();
        dto.Notes.Should().Be("");
    }

    [Fact]
    public void Should_Map_Large_Weight_And_Height_Values()
    {
        // Arrange
        var userMetric = new UserMetric
        {
            Id = "metric123",
            UserId = "user123",
            Date = new DateTime(2024, 1, 1),
            Weight = 999.99, // Large weight value
            Height = 250.0, // Large height value
            Bmi = 159.99, // Calculated BMI
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Act
        var dto = _mapper.ToDto(userMetric);

        // Assert
        dto.Should().NotBeNull();
        dto.Weight.Should().Be(999.99);
        dto.Height.Should().Be(250.0);
        dto.Bmi.Should().Be(159.99);
        dto.BmiCategory.Should().Be("Obese"); // Should still categorize correctly
    }
}
