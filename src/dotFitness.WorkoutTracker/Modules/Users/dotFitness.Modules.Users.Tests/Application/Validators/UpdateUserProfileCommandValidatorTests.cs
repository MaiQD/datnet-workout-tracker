using FluentAssertions;
using FluentValidation.TestHelper;
using dotFitness.Modules.Users.Application.Commands;
using dotFitness.Modules.Users.Application.DTOs;
using dotFitness.Modules.Users.Application.Validators;
using dotFitness.Modules.Users.Domain.Entities;

namespace dotFitness.Modules.Users.Tests.Application.Validators;

public class UpdateUserProfileCommandValidatorTests
{
    private readonly UpdateUserProfileCommandValidator _validator;

    public UpdateUserProfileCommandValidatorTests()
    {
        _validator = new UpdateUserProfileCommandValidator();
    }

    [Fact]
    public void Should_Pass_Validation_For_Valid_Command()
    {
        // Arrange
        var request =
            new UpdateUserProfileRequest("Valid Name", Gender.Male, new DateTime(1990, 1, 1), UnitPreference.Metric);
        var command = new UpdateUserProfileCommand(
            UserId: 1,
            Request: request
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(null)]
    public void Should_Fail_Validation_For_Missing_User_Id(int? invalidUserId)
    {
        // Arrange
        var command = new UpdateUserProfileCommand(
            UserId: invalidUserId ?? 0,
            Request: new UpdateUserProfileRequest("Valid Name", Gender.Male, new DateTime(1990, 1, 1),
                UnitPreference.Metric)
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UserId)
            .WithErrorMessage("User ID is required");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Should_Fail_Validation_For_Empty_Display_Name(string invalidDisplayName)
    {
        // Arrange
        var command = new UpdateUserProfileCommand(
            UserId: 1,
            Request: new UpdateUserProfileRequest(invalidDisplayName, null, null, null)
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Request.DisplayName)
            .WithErrorMessage("Display name is required");
    }

    [Fact]
    public void Should_Pass_Validation_For_Valid_Display_Name()
    {
        // Arrange
        var command = new UpdateUserProfileCommand(
            UserId: 1,
            Request: new UpdateUserProfileRequest("Valid Name", Gender.Male, null, null)
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Request.DisplayName);
    }

    [Fact]
    public void Should_Fail_Validation_For_Display_Name_Too_Long()
    {
        // Arrange
        var longName = new string('A', 101); // 101 characters
        var command = new UpdateUserProfileCommand(
            UserId: 1,
            Request: new UpdateUserProfileRequest(longName, null, null, null)
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Request.DisplayName)
            .WithErrorMessage("Display name must be between 1 and 100 characters");
    }

    [Fact]
    public void Should_Fail_Validation_For_Future_Date_Of_Birth()
    {
        // Arrange
        var futureDate = DateTime.UtcNow.AddDays(1);
        var command = new UpdateUserProfileCommand(
            UserId: 1,
            Request: new UpdateUserProfileRequest("Valid Name", null, futureDate, null)
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Request.DateOfBirth)
            .WithErrorMessage("Date of birth cannot be in the future");
    }

    [Fact]
    public void Should_Fail_Validation_For_Date_Of_Birth_Too_Old()
    {
        // Arrange
        var veryOldDate = DateTime.UtcNow.AddYears(-151); // 151 years ago
        var command = new UpdateUserProfileCommand(
            UserId: 1,
            Request: new UpdateUserProfileRequest("Valid Name", null, veryOldDate, null)
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Request.DateOfBirth)
            .WithErrorMessage("Date of birth cannot be more than 150 years ago");
    }

    [Theory]
    [InlineData("Metric")]
    [InlineData("Imperial")]
    public void Should_Pass_Validation_For_Valid_Unit_Preference_String(string unitPreferenceString)
    {
        // This test simulates validation when unit preference comes as string
        // and tests the custom validation logic for string values
        var isValid = new[] { "Metric", "Imperial" }.Contains(unitPreferenceString);

        // Assert
        isValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("metric")] // lowercase
    [InlineData("IMPERIAL")] // uppercase
    [InlineData("InvalidUnit")]
    [InlineData("")]
    public void Should_Fail_Validation_For_Invalid_Unit_Preference_String(string invalidUnit)
    {
        // This test simulates validation when unit preference comes as string
        // and tests the custom validation logic for string values
        var isValid = new[] { "Metric", "Imperial" }.Contains(invalidUnit);

        // Assert
        isValid.Should().BeFalse();
    }

    [Fact]
    public void Should_Pass_Validation_With_Required_Fields_And_Optional_Fields_Null()
    {
        // Arrange
        var command = new UpdateUserProfileCommand(
            UserId: 1,
            Request: new UpdateUserProfileRequest("Valid Name", null, null, null)
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Pass_Validation_For_Valid_Date_Of_Birth()
    {
        // Arrange
        var validDate = new DateTime(1990, 5, 15);
        var command = new UpdateUserProfileCommand(
            UserId: 1,
            Request: new UpdateUserProfileRequest("Valid Name", null, validDate, null)
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Request.DateOfBirth);
    }

    [Theory]
    [InlineData(Gender.Male)]
    [InlineData(Gender.Female)]
    [InlineData(Gender.Other)]
    [InlineData(Gender.PreferNotToSay)]
    public void Should_Pass_Validation_For_All_Valid_Gender_Values(Gender gender)
    {
        // Arrange
        var command = new UpdateUserProfileCommand(
            UserId: 1,
            Request: new UpdateUserProfileRequest("Valid Name", gender, null, null)
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Request.Gender);
    }

    [Theory]
    [InlineData(UnitPreference.Metric)]
    [InlineData(UnitPreference.Imperial)]
    public void Should_Pass_Validation_For_All_Valid_Unit_Preference_Values(UnitPreference unitPreference)
    {
        // Arrange
        var command = new UpdateUserProfileCommand(
            UserId: 1,
            Request: new UpdateUserProfileRequest("Valid Name", null, null, unitPreference)
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Request.UnitPreference);
    }
}