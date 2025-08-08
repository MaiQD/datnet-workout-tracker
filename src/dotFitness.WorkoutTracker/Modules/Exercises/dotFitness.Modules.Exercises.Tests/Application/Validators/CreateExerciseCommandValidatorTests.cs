using FluentAssertions;
using FluentValidation.TestHelper;
using dotFitness.Modules.Exercises.Application.Commands;
using dotFitness.Modules.Exercises.Application.Validators;
using dotFitness.Modules.Exercises.Domain.Entities;

namespace dotFitness.Modules.Exercises.Tests.Application.Validators;

public class CreateExerciseCommandValidatorTests
{
    private readonly CreateExerciseCommandValidator _validator = new();

    [Fact]
    public void Should_Pass_Validation_For_Valid_Command()
    {
        var command = new CreateExerciseCommand(
            UserId: "user1",
            Name: "Push Up",
            Description: "desc",
            MuscleGroups: new List<string>{"Chest"},
            Equipment: new List<string>{"Bodyweight"},
            Instructions: new List<string>{"Do it"},
            Difficulty: ExerciseDifficulty.Beginner,
            VideoUrl: "https://example.com/v",
            ImageUrl: "https://example.com/i",
            Tags: new List<string>{"home"}
        );

        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Fail_When_UserId_Missing()
    {
        var command = new CreateExerciseCommand(
            UserId: "",
            Name: "Push Up",
            Description: null,
            MuscleGroups: new List<string>{"Chest"},
            Equipment: new List<string>{"Bodyweight"},
            Instructions: new List<string>{"Do it"},
            Difficulty: ExerciseDifficulty.Beginner,
            VideoUrl: null,
            ImageUrl: null,
            Tags: new List<string>()
        );

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.UserId)
            .WithErrorMessage("User ID is required");
    }

    [Fact]
    public void Should_Fail_When_Name_Empty()
    {
        var command = new CreateExerciseCommand(
            UserId: "user1",
            Name: "",
            Description: null,
            MuscleGroups: new List<string>{"Chest"},
            Equipment: new List<string>{"Bodyweight"},
            Instructions: new List<string>{"Do it"},
            Difficulty: ExerciseDifficulty.Beginner,
            VideoUrl: null,
            ImageUrl: null,
            Tags: new List<string>()
        );

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Should_Fail_When_MuscleGroups_Empty()
    {
        var command = new CreateExerciseCommand(
            UserId: "user1",
            Name: "Push Up",
            Description: null,
            MuscleGroups: new List<string>(),
            Equipment: new List<string>{"Bodyweight"},
            Instructions: new List<string>{"Do it"},
            Difficulty: ExerciseDifficulty.Beginner,
            VideoUrl: null,
            ImageUrl: null,
            Tags: new List<string>()
        );

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.MuscleGroups)
            .WithErrorMessage("At least one muscle group is required");
    }

    [Fact]
    public void Should_Fail_When_Instruction_Contains_Empty()
    {
        var command = new CreateExerciseCommand(
            UserId: "user1",
            Name: "Push Up",
            Description: null,
            MuscleGroups: new List<string>{"Chest"},
            Equipment: new List<string>{"Bodyweight"},
            Instructions: new List<string>{""},
            Difficulty: ExerciseDifficulty.Beginner,
            VideoUrl: null,
            ImageUrl: null,
            Tags: new List<string>()
        );

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Instructions)
            .WithErrorMessage("Instructions cannot be empty");
    }

    [Fact]
    public void Should_Fail_When_VideoUrl_Invalid()
    {
        var command = new CreateExerciseCommand(
            UserId: "user1",
            Name: "Push Up",
            Description: null,
            MuscleGroups: new List<string>{"Chest"},
            Equipment: new List<string>{"Bodyweight"},
            Instructions: new List<string>{"Do it"},
            Difficulty: ExerciseDifficulty.Beginner,
            VideoUrl: "not-a-url",
            ImageUrl: null,
            Tags: new List<string>()
        );

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.VideoUrl)
            .WithErrorMessage("Video URL must be a valid URL");
    }
}
