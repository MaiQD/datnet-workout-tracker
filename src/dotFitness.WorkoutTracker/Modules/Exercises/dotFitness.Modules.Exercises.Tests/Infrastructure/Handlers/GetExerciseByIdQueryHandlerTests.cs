using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using dotFitness.Modules.Exercises.Application.Queries;
using dotFitness.Modules.Exercises.Domain.Entities;
using dotFitness.Modules.Exercises.Domain.Repositories;
using dotFitness.Modules.Exercises.Infrastructure.Handlers;

namespace dotFitness.Modules.Exercises.Tests.Infrastructure.Handlers;

public class GetExerciseByIdQueryHandlerTests
{
    private readonly Mock<IExerciseRepository> _repo = new();
    private readonly Mock<ILogger<GetExerciseByIdQueryHandler>> _logger = new();

    [Fact]
    public async Task Should_Return_Dto_When_User_Owns_Or_Global()
    {
        var exercise = new Exercise { Id = "ex1", UserId = "user1", IsGlobal = false, Name = "Push Up" };
        _repo.Setup(r => r.GetByIdAsync("ex1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(dotFitness.SharedKernel.Results.Result.Success<Exercise?>(exercise));

        var handler = new GetExerciseByIdQueryHandler(_repo.Object, _logger.Object);
        var result = await handler.Handle(new GetExerciseByIdQuery("ex1", "user1"), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Name.Should().Be("Push Up");
    }

    [Fact]
    public async Task Should_Return_Null_When_User_Not_Owner_And_Not_Global()
    {
        var exercise = new Exercise { Id = "ex1", UserId = "other", IsGlobal = false, Name = "Push Up" };
        _repo.Setup(r => r.GetByIdAsync("ex1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(dotFitness.SharedKernel.Results.Result.Success<Exercise?>(exercise));

        var handler = new GetExerciseByIdQueryHandler(_repo.Object, _logger.Object);
        var result = await handler.Handle(new GetExerciseByIdQuery("ex1", "user1"), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeNull();
    }

    [Fact]
    public async Task Should_Return_Null_When_Not_Found()
    {
        _repo.Setup(r => r.GetByIdAsync("ex1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(dotFitness.SharedKernel.Results.Result.Success<Exercise?>(null));

        var handler = new GetExerciseByIdQueryHandler(_repo.Object, _logger.Object);
        var result = await handler.Handle(new GetExerciseByIdQuery("ex1", "user1"), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeNull();
    }
}
