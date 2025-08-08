using FluentAssertions;
using dotFitness.Modules.Exercises.Application.Mappers;
using dotFitness.Modules.Exercises.Domain.Entities;

namespace dotFitness.Modules.Exercises.Tests.Application.Mappers;

public class ExerciseMapperTests
{
    [Fact]
    public void Should_Map_Exercise_Entity_To_Dto_Correctly()
    {
        var e = new Exercise
        {
            Id = "ex1",
            Name = "Push Up",
            Description = "desc",
            MuscleGroups = new List<string>{"Chest","Triceps"},
            Equipment = new List<string>{"Bodyweight"},
            Instructions = new List<string>{"Do it"},
            Difficulty = ExerciseDifficulty.Intermediate,
            VideoUrl = "http://video",
            ImageUrl = "http://image",
            IsGlobal = false,
            UserId = "user1",
            Tags = new List<string>{"home"},
            CreatedAt = new DateTime(2024,1,1,0,0,0,DateTimeKind.Utc),
            UpdatedAt = new DateTime(2024,1,2,0,0,0,DateTimeKind.Utc)
        };

        var dto = ExerciseMapper.ToDto(e);

        dto.Should().NotBeNull();
        dto.Id.Should().Be("ex1");
        dto.Name.Should().Be("Push Up");
        dto.Description.Should().Be("desc");
        dto.MuscleGroups.Should().BeEquivalentTo(new[]{"Chest","Triceps"});
        dto.Equipment.Should().BeEquivalentTo(new[]{"Bodyweight"});
        dto.Instructions.Should().BeEquivalentTo(new[]{"Do it"});
        dto.Difficulty.Should().Be(ExerciseDifficulty.Intermediate);
        dto.VideoUrl.Should().Be("http://video");
        dto.ImageUrl.Should().Be("http://image");
        dto.IsGlobal.Should().BeFalse();
        dto.UserId.Should().Be("user1");
        dto.Tags.Should().BeEquivalentTo(new[]{"home"});
        dto.CreatedAt.Should().Be(new DateTime(2024,1,1,0,0,0,DateTimeKind.Utc));
        dto.UpdatedAt.Should().Be(new DateTime(2024,1,2,0,0,0,DateTimeKind.Utc));
    }
}

public class MuscleGroupMapperTests
{
    [Fact]
    public void Should_Map_MuscleGroup_Entity_To_Dto_Correctly()
    {
        var mg = new MuscleGroup
        {
            Id = "mg1",
            Name = "Chest",
            Description = "desc",
            BodyRegion = BodyRegion.Upper,
            Aliases = new List<string>{"Pecs"},
            IsGlobal = true,
            UserId = null,
            CreatedAt = new DateTime(2024,1,1,0,0,0,DateTimeKind.Utc),
            UpdatedAt = new DateTime(2024,1,2,0,0,0,DateTimeKind.Utc)
        };

        var dto = MuscleGroupMapper.ToDto(mg);

        dto.Should().NotBeNull();
        dto.Id.Should().Be("mg1");
        dto.Name.Should().Be("Chest");
        dto.Description.Should().Be("desc");
    }
}

public class EquipmentMapperTests
{
    [Fact]
    public void Should_Map_Equipment_Entity_To_Dto_Correctly()
    {
        var eq = new Equipment
        {
            Id = "eq1",
            Name = "Dumbbell",
            Description = "desc",
            Category = "Weights",
            IsGlobal = true,
            UserId = null,
            CreatedAt = new DateTime(2024,1,1,0,0,0,DateTimeKind.Utc),
            UpdatedAt = new DateTime(2024,1,2,0,0,0,DateTimeKind.Utc)
        };

        var dto = EquipmentMapper.ToDto(eq);

        dto.Should().NotBeNull();
        dto.Id.Should().Be("eq1");
        dto.Name.Should().Be("Dumbbell");
        dto.Description.Should().Be("desc");
        dto.Category.Should().Be("Weights");
    }
}
