using FluentValidation.TestHelper;
using dotFitness.Modules.Exercises.Application.Commands;
using dotFitness.Modules.Exercises.Application.Validators;
using dotFitness.Modules.Exercises.Domain.Entities;

namespace dotFitness.Modules.Exercises.Tests.Application.Validators;

public class UpdateExerciseCommandValidatorTests
{
    private readonly UpdateExerciseCommandValidator _validator = new();

    [Fact]
    [Trait("Category", "Unit")]
    public void Should_Pass_Validation_For_Valid_Command()
    {
        var command = new UpdateExerciseCommand(
            ExerciseId: "ex1",
            UserId: 1,
            Name: "Push Up",
            Description: "desc",
            MuscleGroups: ["Chest"],
            Equipment: ["Bodyweight"],
            Instructions: ["Do it"],
            Difficulty: ExerciseDifficulty.Advanced,
            VideoUrl: "https://example.com/v",
            ImageUrl: "https://example.com/i",
            Tags: ["home"]
        );

        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Should_Fail_When_ExerciseId_Missing()
    {
        var command = new UpdateExerciseCommand(
            ExerciseId: "",
            UserId: 1,
            Name: "Push Up",
            Description: null,
            MuscleGroups: ["Chest"],
            Equipment: ["Bodyweight"],
            Instructions: ["Do it"],
            Difficulty: ExerciseDifficulty.Beginner,
            VideoUrl: null,
            ImageUrl: null,
            Tags: []
        );

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.ExerciseId)
            .WithErrorMessage("Exercise ID is required");
    }
}
