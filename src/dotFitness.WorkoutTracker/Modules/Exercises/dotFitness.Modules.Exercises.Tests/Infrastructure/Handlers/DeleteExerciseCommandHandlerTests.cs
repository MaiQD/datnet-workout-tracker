using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using dotFitness.Modules.Exercises.Application.Commands;
using dotFitness.Modules.Exercises.Domain.Entities;
using dotFitness.Modules.Exercises.Domain.Repositories;
using dotFitness.Modules.Exercises.Infrastructure.Handlers;

namespace dotFitness.Modules.Exercises.Tests.Infrastructure.Handlers;

public class DeleteExerciseCommandHandlerTests
{
    private readonly Mock<IExerciseRepository> _repo = new();
    private readonly Mock<ILogger<DeleteExerciseCommandHandler>> _logger = new();

    [Fact]
    public async Task Should_Delete_When_User_Owns_Exercise()
    {
        var existing = new Exercise { Id = "ex1", UserId = 1 };
        _repo.Setup(r => r.GetByIdAsync("ex1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(dotFitness.SharedKernel.Results.Result.Success<Exercise?>(existing));
        _repo.Setup(r => r.DeleteAsync("ex1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(dotFitness.SharedKernel.Results.Result.Success());

        var handler = new DeleteExerciseCommandHandler(_repo.Object, _logger.Object);
        var result = await handler.Handle(new DeleteExerciseCommand("ex1", 1), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Should_Return_Failure_When_Not_Found()
    {
        _repo.Setup(r => r.GetByIdAsync("ex1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(dotFitness.SharedKernel.Results.Result.Success<Exercise?>(null));

        var handler = new DeleteExerciseCommandHandler(_repo.Object, _logger.Object);
        var result = await handler.Handle(new DeleteExerciseCommand("ex1", 1), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Exercise not found");
    }

    [Fact]
    public async Task Should_Return_Failure_When_User_Not_Owner()
    {
        var existing = new Exercise { Id = "ex1", UserId = -1, IsGlobal = false };
        _repo.Setup(r => r.GetByIdAsync("ex1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(dotFitness.SharedKernel.Results.Result.Success<Exercise?>(existing));

        var handler = new DeleteExerciseCommandHandler(_repo.Object, _logger.Object);
        var result = await handler.Handle(new DeleteExerciseCommand("ex1", 1), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("You don't have permission to delete this exercise");
    }
}
