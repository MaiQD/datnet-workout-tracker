using FluentAssertions;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Moq;
using dotFitness.Modules.Exercises.Domain.Entities;
using dotFitness.Modules.Exercises.Infrastructure.Repositories;
using dotFitness.SharedKernel.Tests.MongoDB;

namespace dotFitness.Modules.Exercises.Tests.Infrastructure.Repositories;

[Collection("MongoDB")]
public class ExerciseRepositoryTests(MongoDbFixture fixture) : IAsyncLifetime
{
    private IMongoDatabase _database = null!;
    private ExerciseRepository _repository = null!;
    private Mock<ILogger<ExerciseRepository>> _loggerMock = null!;

    public async Task InitializeAsync()
    {
        _database = fixture.CreateFreshDatabase();
        _database.GetCollection<Exercise>("exercises");
        _loggerMock = new Mock<ILogger<ExerciseRepository>>();
        _repository = new ExerciseRepository(_database, _loggerMock.Object);
        await Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        await fixture.CleanupDatabaseAsync();
    }

    [Fact]
    public async Task Should_Create_Exercise_Successfully()
    {
        var exercise = new Exercise { Name = "Push Up", UserId = "user1" };

        var result = await _repository.CreateAsync(exercise);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Id.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Should_Retrieve_Exercise_By_Id()
    {
        var exercise = new Exercise { Name = "Push Up", UserId = "user1" };
        var created = await _repository.CreateAsync(exercise);

        var fetched = await _repository.GetByIdAsync(created.Value!.Id);

        fetched.IsSuccess.Should().BeTrue();
        fetched.Value!.Id.Should().Be(created.Value!.Id);
    }

    [Fact]
    public async Task Should_Update_Exercise()
    {
        var exercise = new Exercise { Name = "Push Up", UserId = "user1" };
        var created = await _repository.CreateAsync(exercise);

        var toUpdate = created.Value!;
        toUpdate.Name = "Wide Push Up";
        var updated = await _repository.UpdateAsync(toUpdate);

        updated.IsSuccess.Should().BeTrue();
        updated.Value!.Name.Should().Be("Wide Push Up");
    }

    [Fact]
    public async Task Should_Delete_Exercise()
    {
        var exercise = new Exercise { Name = "Push Up", UserId = "user1" };
        var created = await _repository.CreateAsync(exercise);

        var deleted = await _repository.DeleteAsync(created.Value!.Id);
        deleted.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Should_Get_All_For_User_Including_Global()
    {
        await _repository.CreateAsync(new Exercise { Name = "Global", IsGlobal = true });
        await _repository.CreateAsync(new Exercise { Name = "Mine", UserId = "user1", IsGlobal = false });
        await _repository.CreateAsync(new Exercise { Name = "Other", UserId = "user2", IsGlobal = false });

        var all = await _repository.GetAllForUserAsync("user1");

        all.IsSuccess.Should().BeTrue();
        all.Value!.Select(x => x.Name).Should().Contain(new[]{"Global","Mine"});
        all.Value!.Select(x => x.Name).Should().NotContain("Other");
    }

    [Fact]
    public async Task Should_Search_With_Filters()
    {
        await _repository.CreateAsync(new Exercise { Name = "Chest Press", UserId = "user1", MuscleGroups = ["Chest"], Equipment = ["Dumbbell"], Difficulty = ExerciseDifficulty.Beginner });
        await _repository.CreateAsync(new Exercise { Name = "Squat", UserId = "user1", MuscleGroups = ["Legs"], Equipment = ["Barbell"], Difficulty = ExerciseDifficulty.Advanced });

        var result = await _repository.SearchAsync("user1", searchTerm: "chest", muscleGroups: ["Chest"], equipment: ["Dumbbell"], difficulty: ExerciseDifficulty.Beginner);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Should().ContainSingle(x => x.Name == "Chest Press");
    }

    [Fact]
    public async Task Should_Get_By_User_Id_Only_User_Specific()
    {
        await _repository.CreateAsync(new Exercise { Name = "Global", IsGlobal = true });
        await _repository.CreateAsync(new Exercise { Name = "Mine", UserId = "user1", IsGlobal = false });
        await _repository.CreateAsync(new Exercise { Name = "Other", UserId = "user2", IsGlobal = false });

        var mineOnly = await _repository.GetByUserIdAsync("user1");

        mineOnly.IsSuccess.Should().BeTrue();
        var names = mineOnly.Value!.Select(x => x.Name).ToList();
        names.Should().Contain("Mine");
        names.Should().NotContain(new[] { "Global", "Other" });
    }

    [Fact]
    public async Task Should_Get_Global_Exercises_Only()
    {
        await _repository.CreateAsync(new Exercise { Name = "Global A", IsGlobal = true });
        await _repository.CreateAsync(new Exercise { Name = "Global B", IsGlobal = true });
        await _repository.CreateAsync(new Exercise { Name = "Mine", UserId = "user1", IsGlobal = false });

        var globals = await _repository.GetGlobalExercisesAsync();

        globals.IsSuccess.Should().BeTrue();
        var names = globals.Value!.Select(x => x.Name).ToList();
        names.Should().Contain(new[] { "Global A", "Global B" });
        names.Should().NotContain("Mine");
    }

    [Fact]
    public async Task Should_ExistsAsync_Work_Correctly()
    {
        var created = await _repository.CreateAsync(new Exercise { Name = "Push Up", UserId = "user1" });

        var exists = await _repository.ExistsAsync(created.Value!.Id);
        var notExists = await _repository.ExistsAsync("605c72f5f2b3c23f04d5d9aa");

        exists.Value.Should().BeTrue();
        notExists.Value.Should().BeFalse();
    }

    [Fact]
    public async Task Should_UserOwnsExerciseAsync_Respect_Global_And_Ownership()
    {
        var global = await _repository.CreateAsync(new Exercise { Name = "Global", IsGlobal = true });
        var mine = await _repository.CreateAsync(new Exercise { Name = "Mine", UserId = "user1" });
        var other = await _repository.CreateAsync(new Exercise { Name = "Other", UserId = "user2" });

        var ownsGlobal = await _repository.UserOwnsExerciseAsync(global.Value!.Id, "user1");
        var ownsMine = await _repository.UserOwnsExerciseAsync(mine.Value!.Id, "user1");
        var ownsOther = await _repository.UserOwnsExerciseAsync(other.Value!.Id, "user1");

        ownsGlobal.Value.Should().BeTrue(); // global visible to all
        ownsMine.Value.Should().BeTrue();
        ownsOther.Value.Should().BeFalse();
    }

    [Fact]
    public async Task Should_Update_Return_Failure_When_Not_Found()
    {
        var result = await _repository.UpdateAsync(new Exercise { Id = "605c72f5f2b3c23f04d5d9aa", Name = "X" });
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task Should_Delete_Return_Failure_When_Not_Found()
    {
        var result = await _repository.DeleteAsync("605c72f5f2b3c23f04d5d9aa");
        result.IsFailure.Should().BeTrue();
    }
}
