using dotFitness.Common.Results;
using FluentAssertions;
using Moq;
using dotFitness.Modules.Exercises.Application.Queries;
using dotFitness.Modules.Exercises.Domain.Entities;
using dotFitness.Modules.Exercises.Domain.Repositories;

namespace dotFitness.Modules.Exercises.Tests.Infrastructure.Handlers;

public class GetSmartExerciseSuggestionsQueryHandlerTests
{
    private readonly Mock<IExerciseRepository> _repo = new();
    private readonly Mock<IUserPreferencesProjectionRepository> _prefsRepo = new();

    [Fact]
    [Trait("Category", "Unit")]
    public async Task Should_Boost_Score_With_User_Preferences()
    {
        // Arrange repository returning two exercises
        var ex1 = new Exercise { Id = "ex1", Name = "Dumbbell Curl", MuscleGroups = ["Biceps"], Equipment = ["Dumbbells"] };
        var ex2 = new Exercise { Id = "ex2", Name = "Bench Press", MuscleGroups = ["Chest"], Equipment = ["Barbell"] };
        _repo.Setup(r => r.GetAllForUserAsync(Guid.NewGuid(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<IEnumerable<Exercise>>([ex1, ex2]));

        // Arrange preferences
        _prefsRepo.Setup(r => r.GetByUserIdAsync(Guid.NewGuid(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<UserPreferencesProjection?>(new UserPreferencesProjection
            {
                UserId = Guid.NewGuid(),
                FocusMuscleGroupIds = ["Biceps"],
                AvailableEquipmentIds = ["Dumbbells"]
            }));

        var handler = new GetSmartExerciseSuggestionsQueryHandler(_repo.Object, _prefsRepo.Object);

        // Act
        var result = await handler.Handle(new GetSmartExerciseSuggestionsQuery(Guid.NewGuid(), Limit: 2), CancellationToken.None);

        // Assert: ex1 should rank before ex2 due to preference boosts
        result.IsSuccess.Should().BeTrue();
        var list = result.Value!.ToList();
        list[0].Id.Should().Be("ex1");
        list[1].Id.Should().Be("ex2");
    }

    // No extra helpers needed with repository abstraction
}


