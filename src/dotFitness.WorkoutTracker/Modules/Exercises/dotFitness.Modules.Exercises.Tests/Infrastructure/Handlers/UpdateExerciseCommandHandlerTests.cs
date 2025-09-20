using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using dotFitness.Modules.Exercises.Application.Commands;
using dotFitness.Modules.Exercises.Domain.Entities;
using dotFitness.Modules.Exercises.Domain.Repositories;
using dotFitness.Modules.Exercises.Infrastructure.Handlers;

namespace dotFitness.Modules.Exercises.Tests.Infrastructure.Handlers;

public class UpdateExerciseCommandHandlerTests
{
    private readonly Mock<IExerciseRepository> _repo = new();
    private readonly Mock<ILogger<UpdateExerciseCommandHandler>> _logger = new();

    [Fact]
    [Trait("Category", "Unit")]
    public async Task Should_Update_When_User_Owns_Exercise()
    {
        var existing = new Exercise { Id = "ex1", UserId = 1, Name = "Old" };
        _repo.Setup(r => r.GetByIdAsync("ex1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(dotFitness.SharedKernel.Results.Result.Success<Exercise?>(existing));
        _repo.Setup(r => r.UpdateAsync(existing, It.IsAny<CancellationToken>()))
            .ReturnsAsync(dotFitness.SharedKernel.Results.Result.Success(existing));

        var cmd = new UpdateExerciseCommand(
            ExerciseId: "ex1",
            UserId: 1,
            Name: "New",
            Description: null,
            MuscleGroups: [],
            Equipment: [],
            Instructions: [],
            Difficulty: ExerciseDifficulty.Beginner,
            VideoUrl: null,
            ImageUrl: null,
            Tags: []
        );

        var handler = new UpdateExerciseCommandHandler(_repo.Object, _logger.Object);
        var result = await handler.Handle(cmd, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Name.Should().Be("New");
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task Should_Return_Failure_When_Not_Found()
    {
        _repo.Setup(r => r.GetByIdAsync("ex1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(dotFitness.SharedKernel.Results.Result.Success<Exercise?>(null));

        var cmd = new UpdateExerciseCommand(
            ExerciseId: "ex1",
            UserId: 1,
            Name: "New",
            Description: null,
            MuscleGroups: [],
            Equipment: [],
            Instructions: [],
            Difficulty: ExerciseDifficulty.Beginner,
            VideoUrl: null,
            ImageUrl: null,
            Tags: []
        );

        var handler = new UpdateExerciseCommandHandler(_repo.Object, _logger.Object);
        var result = await handler.Handle(cmd, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Exercise not found");
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task Should_Return_Failure_When_User_Not_Owner()
    {
        var existing = new Exercise { Id = "ex1", UserId = -1, Name = "Old" };
        _repo.Setup(r => r.GetByIdAsync("ex1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(dotFitness.SharedKernel.Results.Result.Success<Exercise?>(existing));

        var cmd = new UpdateExerciseCommand(
            ExerciseId: "ex1",
            UserId: 1,
            Name: "New",
            Description: null,
            MuscleGroups: [],
            Equipment: [],
            Instructions: [],
            Difficulty: ExerciseDifficulty.Beginner,
            VideoUrl: null,
            ImageUrl: null,
            Tags: []
        );

        var handler = new UpdateExerciseCommandHandler(_repo.Object, _logger.Object);
        var result = await handler.Handle(cmd, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("You don't have permission to update this exercise");
    }
}
