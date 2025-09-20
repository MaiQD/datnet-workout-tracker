using FluentAssertions;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Moq;
using dotFitness.Modules.Exercises.Domain.Entities;
using dotFitness.Modules.Exercises.Infrastructure.Repositories;
using dotFitness.SharedKernel.Tests.MongoDB;

namespace dotFitness.Modules.Exercises.Tests.Infrastructure.Repositories;

[Collection("MongoDB")]
public class EquipmentRepositoryTests(MongoDbFixture fixture) : IAsyncLifetime
{
    private IMongoDatabase _database = null!;
    private EquipmentRepository _repository = null!;
    private Mock<ILogger<EquipmentRepository>> _loggerMock = null!;

    public async Task InitializeAsync()
    {
        _database = fixture.CreateFreshDatabase();
        _database.GetCollection<Equipment>("equipment");
        _loggerMock = new Mock<ILogger<EquipmentRepository>>();
        _repository = new EquipmentRepository(_database, _loggerMock.Object);
        await Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        await fixture.CleanupDatabaseAsync();
    }

    [Fact]
    public async Task Should_Create_And_Get_By_Id()
    {
        var eq = new Equipment { Name = "Dumbbell", UserId = 1 };
        var created = await _repository.CreateAsync(eq);
        var fetched = await _repository.GetByIdAsync(created.Value!.Id);
        fetched.Value!.Name.Should().Be("Dumbbell");
    }

    [Fact]
    public async Task Should_Get_All_For_User_Including_Global()
    {
        await _repository.CreateAsync(new Equipment { Name = "Global", IsGlobal = true });
        await _repository.CreateAsync(new Equipment { Name = "My Eq", UserId = 1 });
        await _repository.CreateAsync(new Equipment { Name = "Other", UserId = 2 });

        var all = await _repository.GetAllForUserAsync(1);

        all.Value!.Select(x => x.Name).Should().Contain(["Global","My Eq"]);
        all.Value!.Select(x => x.Name).Should().NotContain("Other");
    }

    [Fact]
    public async Task Should_Update_And_Delete()
    {
        var eq = new Equipment { Name = "Dumbbell", UserId = 1 };
        var created = await _repository.CreateAsync(eq);

        var toUpdate = created.Value!;
        toUpdate.Description = "Hex";
        var updated = await _repository.UpdateAsync(toUpdate);
        updated.IsSuccess.Should().BeTrue();

        var deleted = await _repository.DeleteAsync(toUpdate.Id);
        deleted.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Should_Check_Name_Exists_And_Get_By_Name()
    {
        await _repository.CreateAsync(new Equipment { Name = "Dumbbell", UserId = 1 });
        var exists = await _repository.NameExistsAsync("Dumbbell");
        exists.Value.Should().BeTrue();

        var byNameGlobal = await _repository.GetByNameAsync("Dumbbell");
        byNameGlobal.Value.Should().BeNull();

        var byNameUser = await _repository.GetByNameAsync("Dumbbell", userId: 1);
        byNameUser.Value.Should().NotBeNull();
    }
}
