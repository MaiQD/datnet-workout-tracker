using FluentAssertions;
using dotFitness.Modules.Exercises.Domain.Entities;

namespace dotFitness.Modules.Exercises.Tests.Domain.Entities;

public class EquipmentTests
{
    [Fact]
    public void Should_Create_Valid_Equipment_With_Required_Properties()
    {
        var equipment = new Equipment
        {
            Name = "Dumbbell",
            Description = "Free weight",
            Category = "Weights"
        };

        equipment.Id.Should().NotBeNullOrEmpty();
        equipment.Name.Should().Be("Dumbbell");
        equipment.Description.Should().Be("Free weight");
        equipment.Category.Should().Be("Weights");
        equipment.IsGlobal.Should().BeFalse();
        equipment.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        equipment.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Should_Update_Equipment_And_Refresh_UpdatedAt()
    {
        var equipment = new Equipment { Name = "Barbell" };
        var originalUpdatedAt = equipment.UpdatedAt;
        Thread.Sleep(10);

        equipment.UpdateEquipment(description: "Olympic", category: "Weights");

        equipment.Description.Should().Be("Olympic");
        equipment.Category.Should().Be("Weights");
        equipment.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }
}
