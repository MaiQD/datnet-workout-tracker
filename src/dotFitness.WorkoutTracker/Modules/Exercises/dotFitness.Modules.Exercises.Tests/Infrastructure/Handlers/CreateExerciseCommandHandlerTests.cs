using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using dotFitness.Modules.Exercises.Application.Commands;
using dotFitness.Modules.Exercises.Domain.Entities;
using dotFitness.Modules.Exercises.Domain.Repositories;
using dotFitness.Modules.Exercises.Infrastructure.Handlers;

namespace dotFitness.Modules.Exercises.Tests.Infrastructure.Handlers;

public class CreateExerciseCommandHandlerTests
{
    private readonly Mock<IExerciseRepository> _repo = new();
    private readonly Mock<ILogger<CreateExerciseCommandHandler>> _logger = new();

    [Fact]
    [Trait("Category", "Unit")]
    public async Task Should_Create_Exercise_And_Return_Dto()
    {
        var cmd = new CreateExerciseCommand(
            UserId: 1,
            Name: "Push Up",
            Description: "desc",
            MuscleGroups: ["Chest"],
            Equipment: ["Bodyweight"],
            Instructions: ["Do it"],
            Difficulty: ExerciseDifficulty.Beginner,
            VideoUrl: null,
            ImageUrl: null,
            Tags: []
        );

        _repo.Setup(r => r.CreateAsync(It.IsAny<Exercise>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Exercise e, CancellationToken _) => dotFitness.SharedKernel.Results.Result.Success(e));

        var handler = new CreateExerciseCommandHandler(_repo.Object, _logger.Object);

        var result = await handler.Handle(cmd, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Name.Should().Be("Push Up");
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task Should_Propagate_Failure_From_Repository()
    {
        var cmd = new CreateExerciseCommand(
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

        _repo.Setup(r => r.CreateAsync(It.IsAny<Exercise>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(dotFitness.SharedKernel.Results.Result.Failure<Exercise>("err"));

        var handler = new CreateExerciseCommandHandler(_repo.Object, _logger.Object);
        var result = await handler.Handle(cmd, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("err");
    }
}
