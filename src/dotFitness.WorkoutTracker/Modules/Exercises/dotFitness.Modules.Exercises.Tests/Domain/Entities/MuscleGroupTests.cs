using FluentAssertions;
using dotFitness.Modules.Exercises.Domain.Entities;

namespace dotFitness.Modules.Exercises.Tests.Domain.Entities;

public class MuscleGroupTests
{
    [Fact]
    public void Should_Create_Valid_MuscleGroup_With_Required_Properties()
    {
        var mg = new MuscleGroup
        {
            Name = "Chest",
            Description = "Pectorals",
            BodyRegion = BodyRegion.Upper
        };

        mg.Id.Should().NotBeNullOrEmpty();
        mg.Name.Should().Be("Chest");
        mg.Description.Should().Be("Pectorals");
        mg.BodyRegion.Should().Be(BodyRegion.Upper);
        mg.IsGlobal.Should().BeFalse();
        mg.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        mg.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Should_Update_MuscleGroup_And_Refresh_UpdatedAt()
    {
        var mg = new MuscleGroup { Name = "Chest" };
        var originalUpdatedAt = mg.UpdatedAt;
        Thread.Sleep(10);

        mg.UpdateMuscleGroup(description: "Pecs", bodyRegion: BodyRegion.Upper, aliases: new List<string>{"Pecs"});

        mg.Description.Should().Be("Pecs");
        mg.BodyRegion.Should().Be(BodyRegion.Upper);
        mg.Aliases.Should().Contain("Pecs");
        mg.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }
}
