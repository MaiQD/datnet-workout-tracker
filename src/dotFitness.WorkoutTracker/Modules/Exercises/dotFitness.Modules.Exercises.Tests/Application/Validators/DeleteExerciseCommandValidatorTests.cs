using FluentValidation.TestHelper;
using dotFitness.Modules.Exercises.Application.Commands;
using dotFitness.Modules.Exercises.Application.Validators;

namespace dotFitness.Modules.Exercises.Tests.Application.Validators;

public class DeleteExerciseCommandValidatorTests
{
    private readonly DeleteExerciseCommandValidator _validator = new();

    [Fact]
    public void Should_Pass_For_Valid_Command()
    {
        var command = new DeleteExerciseCommand(ExerciseId: "ex1", UserId: "user1");
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Fail_When_ExerciseId_Missing()
    {
        var command = new DeleteExerciseCommand(ExerciseId: "", UserId: "user1");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.ExerciseId)
            .WithErrorMessage("Exercise ID is required");
    }

    [Fact]
    public void Should_Fail_When_UserId_Missing()
    {
        var command = new DeleteExerciseCommand(ExerciseId: "ex1", UserId: "");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.UserId)
            .WithErrorMessage("User ID is required");
    }
}
