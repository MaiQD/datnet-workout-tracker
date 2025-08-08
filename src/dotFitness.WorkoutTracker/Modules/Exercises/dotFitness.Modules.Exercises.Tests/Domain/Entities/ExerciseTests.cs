using FluentAssertions;
using dotFitness.Modules.Exercises.Domain.Entities;

namespace dotFitness.Modules.Exercises.Tests.Domain.Entities;

public class ExerciseTests
{
    [Fact]
    public void Should_Create_Valid_Exercise_With_Required_Properties()
    {
        var exercise = new Exercise
        {
            Name = "Push Up",
            Description = "A bodyweight chest exercise"
        };

        exercise.Id.Should().NotBeNullOrEmpty();
        exercise.Name.Should().Be("Push Up");
        exercise.Description.Should().Be("A bodyweight chest exercise");
        exercise.Difficulty.Should().Be(ExerciseDifficulty.Beginner);
        exercise.IsGlobal.Should().BeFalse();
        exercise.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        exercise.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Should_Update_Exercise_And_Refresh_UpdatedAt()
    {
        var exercise = new Exercise { Name = "Push Up" };
        var originalUpdatedAt = exercise.UpdatedAt;

        Thread.Sleep(10);
        exercise.UpdateExercise(description: "Updated", difficulty: ExerciseDifficulty.Intermediate);

        exercise.Description.Should().Be("Updated");
        exercise.Difficulty.Should().Be(ExerciseDifficulty.Intermediate);
        exercise.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    [Fact]
    public void Should_Add_And_Remove_MuscleGroup()
    {
        var exercise = new Exercise { Name = "Push Up" };
        var updatedAt1 = exercise.UpdatedAt;
        Thread.Sleep(10);

        exercise.AddMuscleGroup("Chest");
        exercise.MuscleGroups.Should().Contain("Chest");
        exercise.UpdatedAt.Should().BeAfter(updatedAt1);

        var updatedAt2 = exercise.UpdatedAt;
        Thread.Sleep(10);
        exercise.RemoveMuscleGroup("Chest");
        exercise.MuscleGroups.Should().NotContain("Chest");
        exercise.UpdatedAt.Should().BeAfter(updatedAt2);
    }

    [Fact]
    public void Should_Add_And_Remove_Equipment()
    {
        var exercise = new Exercise { Name = "Push Up" };
        exercise.AddEquipment("Bodyweight");
        exercise.Equipment.Should().Contain("Bodyweight");
        exercise.RemoveEquipment("Bodyweight");
        exercise.Equipment.Should().NotContain("Bodyweight");
    }

    [Fact]
    public void Should_Add_And_Remove_Tags()
    {
        var exercise = new Exercise { Name = "Push Up" };
        exercise.AddTag("home");
        exercise.Tags.Should().Contain("home");
        exercise.RemoveTag("home");
        exercise.Tags.Should().NotContain("home");
    }
}
