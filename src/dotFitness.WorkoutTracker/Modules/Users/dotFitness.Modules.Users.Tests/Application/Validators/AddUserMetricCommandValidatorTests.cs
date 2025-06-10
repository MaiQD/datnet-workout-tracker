using FluentAssertions;
using FluentValidation.TestHelper;
using dotFitness.Modules.Users.Application.Commands;
using dotFitness.Modules.Users.Application.Validators;

namespace dotFitness.Modules.Users.Tests.Application.Validators;

public class AddUserMetricCommandValidatorTests
{
    private readonly AddUserMetricCommandValidator _validator;

    public AddUserMetricCommandValidatorTests()
    {
        _validator = new AddUserMetricCommandValidator();
    }

    [Fact]
    public void Should_Pass_Validation_For_Valid_Command_With_Weight_And_Height()
    {
        // Arrange
        var command = new AddUserMetricCommand(
            UserId: "user123",
            Date: new DateTime(2024, 1, 1),
            Weight: 70.5,
            Height: 175.0,
            Notes: "Morning measurement"
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Pass_Validation_For_Valid_Command_With_Weight_Only()
    {
        // Arrange
        var command = new AddUserMetricCommand(
            UserId: "user123",
            Date: new DateTime(2024, 1, 1),
            Weight: 70.5,
            Height: null,
            Notes: null
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Pass_Validation_For_Valid_Command_With_Height_Only()
    {
        // Arrange
        var command = new AddUserMetricCommand(
            UserId: "user123",
            Date: new DateTime(2024, 1, 1),
            Weight: null,
            Height: 175.0,
            Notes: null
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Should_Fail_Validation_For_Missing_User_Id(string? invalidUserId)
    {
        // Arrange
        var command = new AddUserMetricCommand(
            UserId: invalidUserId!,
            Date: new DateTime(2024, 1, 1),
            Weight: 70.0,
            Height: null,
            Notes: null
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UserId)
            .WithErrorMessage("User ID is required");
    }

    [Fact]
    public void Should_Fail_Validation_For_Missing_Date()
    {
        // Arrange
        var command = new AddUserMetricCommand(
            UserId: "user123",
            Date: default(DateTime),
            Weight: 70.0,
            Height: null,
            Notes: null
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Date)
            .WithErrorMessage("Date is required");
    }

    [Fact]
    public void Should_Fail_Validation_For_Future_Date()
    {
        // Arrange
        var futureDate = DateTime.UtcNow.AddDays(2);
        var command = new AddUserMetricCommand(
            UserId: "user123",
            Date: futureDate,
            Weight: 70.0,
            Height: null,
            Notes: null
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Date)
            .WithErrorMessage("Date cannot be in the future");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Should_Fail_Validation_For_Invalid_Weight(double invalidWeight)
    {
        // Arrange
        var command = new AddUserMetricCommand(
            UserId: "user123",
            Date: new DateTime(2024, 1, 1),
            Weight: invalidWeight,
            Height: null,
            Notes: null
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Weight)
            .WithErrorMessage("Weight must be between 0 and 1000");
    }

    [Fact]
    public void Should_Fail_Validation_For_Weight_Too_High()
    {
        // Arrange
        var command = new AddUserMetricCommand(
            UserId: "user123",
            Date: new DateTime(2024, 1, 1),
            Weight: 1001, // Over 1000
            Height: null,
            Notes: null
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Weight)
            .WithErrorMessage("Weight must be between 0 and 1000");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Should_Fail_Validation_For_Invalid_Height(double invalidHeight)
    {
        // Arrange
        var command = new AddUserMetricCommand(
            UserId: "user123",
            Date: new DateTime(2024, 1, 1),
            Weight: null,
            Height: invalidHeight,
            Notes: null
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Height)
            .WithErrorMessage("Height must be between 0 and 300");
    }

    [Fact]
    public void Should_Fail_Validation_For_Height_Too_High()
    {
        // Arrange
        var command = new AddUserMetricCommand(
            UserId: "user123",
            Date: new DateTime(2024, 1, 1),
            Weight: null,
            Height: 301, // Over 300
            Notes: null
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Height)
            .WithErrorMessage("Height must be between 0 and 300");
    }

    [Fact]
    public void Should_Fail_Validation_For_Notes_Too_Long()
    {
        // Arrange
        var longNotes = new string('A', 501); // 501 characters
        var command = new AddUserMetricCommand(
            UserId: "user123",
            Date: new DateTime(2024, 1, 1),
            Weight: 70.0,
            Height: null,
            Notes: longNotes
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Notes)
            .WithErrorMessage("Notes cannot exceed 500 characters");
    }

    [Fact]
    public void Should_Fail_Validation_When_No_Metrics_Provided()
    {
        // Arrange
        var command = new AddUserMetricCommand(
            UserId: "user123",
            Date: new DateTime(2024, 1, 1),
            Weight: null,
            Height: null,
            Notes: "Just notes, no metrics"
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("At least one metric (weight or height) must be provided");
    }

    [Fact]
    public void Should_Pass_Validation_For_Valid_Weight_Range()
    {
        // Arrange
        var command = new AddUserMetricCommand(
            UserId: "user123",
            Date: new DateTime(2024, 1, 1),
            Weight: 65.5, // Valid weight
            Height: null,
            Notes: null
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Weight);
    }

    [Fact]
    public void Should_Pass_Validation_For_Valid_Height_Range()
    {
        // Arrange
        var command = new AddUserMetricCommand(
            UserId: "user123",
            Date: new DateTime(2024, 1, 1),
            Weight: null,
            Height: 175.5, // Valid height
            Notes: null
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Height);
    }

    [Fact]
    public void Should_Pass_Validation_For_Valid_Notes_Length()
    {
        // Arrange
        var validNotes = new string('A', 500); // Exactly 500 characters
        var command = new AddUserMetricCommand(
            UserId: "user123",
            Date: new DateTime(2024, 1, 1),
            Weight: 70.0,
            Height: null,
            Notes: validNotes
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Notes);
    }

    [Fact]
    public void Should_Pass_Validation_For_Today_Date()
    {
        // Arrange
        var command = new AddUserMetricCommand(
            UserId: "user123",
            Date: DateTime.UtcNow.Date, // Today
            Weight: 70.0,
            Height: null,
            Notes: null
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Date);
    }
}
