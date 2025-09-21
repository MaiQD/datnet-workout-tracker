using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using MediatR;
using FluentAssertions;
using dotFitness.Api.Controllers;
using dotFitness.Modules.Exercises.Application.Commands;
using dotFitness.Modules.Exercises.Application.DTOs;
using dotFitness.Modules.Exercises.Application.Queries;
using dotFitness.Modules.Exercises.Domain.Entities;
using dotFitness.SharedKernel.Results;
using System.Security.Claims;

namespace dotFitness.Api.Tests.Controllers;

public class ExercisesControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<ILogger<ExercisesController>> _loggerMock;
    private readonly ExercisesController _controller;

    public ExercisesControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _loggerMock = new Mock<ILogger<ExercisesController>>();
        _controller = new ExercisesController(_mediatorMock.Object, _loggerMock.Object);
        
        // Setup user claims
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "1")
        };
        var identity = new ClaimsIdentity(claims, "test");
        var principal = new ClaimsPrincipal(identity);
        
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext
            {
                User = principal
            }
        };
    }

    [Fact]
    public async Task GetAllExercises_Should_Return_Ok_When_Exercises_Found()
    {
        // Arrange
        var exercises = new List<ExerciseDto>
        {
            new ExerciseDto("1", "Push Up", "Basic push up exercise", 
                new List<string> { "Chest" }, new List<string> { "Bodyweight" }, 
                new List<string> { "Step 1", "Step 2" }, ExerciseDifficulty.Beginner, 
                null, null, true, null, new List<string> { "strength" }, DateTime.UtcNow, DateTime.UtcNow)
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetAllExercisesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(exercises.AsEnumerable()));

        // Act
        var result = await _controller.GetAllExercises();

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(exercises);
    }

    [Fact]
    public async Task GetExerciseById_Should_Return_Ok_When_Exercise_Found()
    {
        // Arrange
        var exercise = new ExerciseDto("1", "Push Up", "Basic push up exercise", 
            new List<string> { "Chest" }, new List<string> { "Bodyweight" }, 
            new List<string> { "Step 1", "Step 2" }, ExerciseDifficulty.Beginner, 
            null, null, true, null, new List<string> { "strength" }, DateTime.UtcNow, DateTime.UtcNow);

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetExerciseByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(exercise));

        // Act
        var result = await _controller.GetExerciseById("1");

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(exercise);
    }

    [Fact]
    public async Task GetExerciseById_Should_Return_NotFound_When_Exercise_Not_Found()
    {
        // Arrange
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetExerciseByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<ExerciseDto?>(null));

        // Act
        var result = await _controller.GetExerciseById("999");

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task CreateExercise_Should_Return_Created_When_Exercise_Created_Successfully()
    {
        // Arrange
        var request = new CreateExerciseRequest(
            "Push Up",
            "Basic push up exercise",
            new List<string> { "Chest" },
            new List<string> { "Bodyweight" },
            new List<string> { "Step 1", "Step 2" },
            ExerciseDifficulty.Beginner,
            null,
            null,
            new List<string> { "strength" }
        );

        var createdExercise = new ExerciseDto("1", "Push Up", "Basic push up exercise", 
            new List<string> { "Chest" }, new List<string> { "Bodyweight" }, 
            new List<string> { "Step 1", "Step 2" }, ExerciseDifficulty.Beginner, 
            null, null, false, 1, new List<string> { "strength" }, DateTime.UtcNow, DateTime.UtcNow);

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<CreateExerciseCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(createdExercise));

        // Act
        var result = await _controller.CreateExercise(request);

        // Assert
        result.Should().BeOfType<CreatedAtActionResult>();
        var createdResult = result as CreatedAtActionResult;
        createdResult!.Value.Should().BeEquivalentTo(createdExercise);
    }

    [Fact]
    public async Task GetAllMuscleGroups_Should_Return_Ok_When_MuscleGroups_Found()
    {
        // Arrange
        var muscleGroups = new List<MuscleGroupDto>
        {
            new MuscleGroupDto("1", "Chest", "Upper body", true, null, DateTime.UtcNow, DateTime.UtcNow)
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetAllMuscleGroupsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(muscleGroups.AsEnumerable()));

        // Act
        var result = await _controller.GetAllMuscleGroups();

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(muscleGroups);
    }

    [Fact]
    public async Task GetAllEquipment_Should_Return_Ok_When_Equipment_Found()
    {
        // Arrange
        var equipment = new List<EquipmentDto>
        {
            new EquipmentDto("1", "Dumbbells", "Adjustable dumbbells", "Weight training", true, "user1", DateTime.UtcNow, DateTime.UtcNow)
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetAllEquipmentQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(equipment.AsEnumerable()));

        // Act
        var result = await _controller.GetAllEquipment();

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(equipment);
    }
}
