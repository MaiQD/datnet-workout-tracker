using FluentAssertions;
using FluentValidation.TestHelper;
using dotFitness.Modules.Users.Application.Commands;
using dotFitness.Modules.Users.Application.Validators;
using dotFitness.Modules.Users.Domain.Entities;

namespace dotFitness.Modules.Users.Tests.Application.Validators;

public class LoginWithGoogleCommandValidatorTests
{
    private readonly LoginWithGoogleCommandValidator _validator;

    public LoginWithGoogleCommandValidatorTests()
    {
        _validator = new LoginWithGoogleCommandValidator();
    }

    [Fact]
    public void Should_Pass_Validation_For_Valid_Command()
    {
        // Arrange
        var command = new LoginWithGoogleCommand("valid_google_token_123");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Should_Fail_Validation_For_Missing_Google_Token(string? invalidToken)
    {
        // Arrange
        var command = new LoginWithGoogleCommand(invalidToken!);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.GoogleToken)
            .WithErrorMessage("Google token ID is required");
    }

    [Fact]
    public void Should_Have_Validation_Error_For_Missing_Required_Field()
    {
        // Arrange
        var command = new LoginWithGoogleCommand("");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.GoogleToken);
        result.Errors.Should().Contain(e => e.ErrorMessage == "Google token ID is required");
    }
}
