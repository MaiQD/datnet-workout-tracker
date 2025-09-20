using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using dotFitness.Modules.Exercises.Application.Queries;
using dotFitness.Modules.Exercises.Domain.Entities;
using dotFitness.Modules.Exercises.Domain.Repositories;
using dotFitness.Modules.Exercises.Infrastructure.Handlers;

namespace dotFitness.Modules.Exercises.Tests.Infrastructure.Handlers;

public class GetAllEquipmentQueryHandlerTests
{
    private readonly Mock<IEquipmentRepository> _repo = new();
    private readonly Mock<ILogger<GetAllEquipmentQueryHandler>> _logger = new();

    [Fact]
    public async Task Should_Return_Equipment_List()
    {
        _repo.Setup(r => r.GetAllForUserAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(dotFitness.SharedKernel.Results.Result.Success<IEnumerable<Equipment>>([
                new Equipment { Id = "eq1", Name = "Dumbbell" }
            ]));

        var handler = new GetAllEquipmentQueryHandler(_repo.Object, _logger.Object);
        var result = await handler.Handle(new GetAllEquipmentQuery(1), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Should().ContainSingle(e => e.Id == "eq1");
    }

    [Fact]
    public async Task Should_Propagate_Failure()
    {
        _repo.Setup(r => r.GetAllForUserAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(dotFitness.SharedKernel.Results.Result.Failure<IEnumerable<Equipment>>("err"));

        var handler = new GetAllEquipmentQueryHandler(_repo.Object, _logger.Object);
        var result = await handler.Handle(new GetAllEquipmentQuery(1), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("err");
    }
}
