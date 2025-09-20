using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using dotFitness.Modules.Exercises.Application.Queries;
using dotFitness.Modules.Exercises.Domain.Entities;
using dotFitness.Modules.Exercises.Domain.Repositories;
using dotFitness.Modules.Exercises.Infrastructure.Handlers;

namespace dotFitness.Modules.Exercises.Tests.Infrastructure.Handlers;

public class GetAllMuscleGroupsQueryHandlerTests
{
    private readonly Mock<IMuscleGroupRepository> _repo = new();
    private readonly Mock<ILogger<GetAllMuscleGroupsQueryHandler>> _logger = new();

    [Fact]
    public async Task Should_Return_MuscleGroups_List()
    {
        _repo.Setup(r => r.GetAllForUserAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(dotFitness.SharedKernel.Results.Result.Success<IEnumerable<MuscleGroup>>([
                new MuscleGroup { Id = "mg1", Name = "Chest" }
            ]));

        var handler = new GetAllMuscleGroupsQueryHandler(_repo.Object, _logger.Object);
        var result = await handler.Handle(new GetAllMuscleGroupsQuery(1), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Should().ContainSingle(m => m.Id == "mg1");
    }

    [Fact]
    public async Task Should_Propagate_Failure()
    {
        _repo.Setup(r => r.GetAllForUserAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(dotFitness.SharedKernel.Results.Result.Failure<IEnumerable<MuscleGroup>>("err"));

        var handler = new GetAllMuscleGroupsQueryHandler(_repo.Object, _logger.Object);
        var result = await handler.Handle(new GetAllMuscleGroupsQuery(1), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("err");
    }
}
