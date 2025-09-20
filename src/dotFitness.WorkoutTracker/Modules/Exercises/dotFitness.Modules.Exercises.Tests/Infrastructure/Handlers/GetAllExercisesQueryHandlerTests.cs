using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using dotFitness.Modules.Exercises.Application.Queries;
using dotFitness.Modules.Exercises.Domain.Entities;
using dotFitness.Modules.Exercises.Domain.Repositories;
using dotFitness.Modules.Exercises.Infrastructure.Handlers;

namespace dotFitness.Modules.Exercises.Tests.Infrastructure.Handlers;

public class GetAllExercisesQueryHandlerTests
{
    private readonly Mock<IExerciseRepository> _repo = new();
    private readonly Mock<ILogger<GetAllExercisesQueryHandler>> _logger = new();

    [Fact]
    [Trait("Category", "Unit")]
    public async Task Should_Return_All_For_User_When_No_Criteria()
    {
        _repo.Setup(r => r.GetAllForUserAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(dotFitness.SharedKernel.Results.Result.Success<IEnumerable<Exercise>>([
                new Exercise { Id = "ex1", Name = "Push Up" }
            ]));

        var handler = new GetAllExercisesQueryHandler(_repo.Object, _logger.Object);
        var result = await handler.Handle(new GetAllExercisesQuery(1), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Should().ContainSingle(e => e.Id == "ex1");
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task Should_Use_Search_When_Criteria_Provided()
    {
        _repo.Setup(r => r.SearchAsync(1, "push", It.IsAny<List<string>?>(), It.IsAny<List<string>?>(), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(dotFitness.SharedKernel.Results.Result.Success<IEnumerable<Exercise>>([
                new Exercise { Id = "ex2", Name = "Push Up" }
            ]));

        var handler = new GetAllExercisesQueryHandler(_repo.Object, _logger.Object);
        var result = await handler.Handle(new GetAllExercisesQuery(1, SearchTerm: "push"), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Should().ContainSingle(e => e.Id == "ex2");
    }
}
