using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using MediatR;
using FluentAssertions;
using dotFitness.Api.Controllers;
using dotFitness.Modules.Users.Application.Commands;
using dotFitness.Modules.Users.Application.DTOs;
using dotFitness.Modules.Users.Application.Queries;
using dotFitness.SharedKernel.Results;
using System.Security.Claims;

namespace dotFitness.Api.Tests.Controllers;

public class UsersControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<ILogger<UsersController>> _loggerMock;
    private readonly UsersController _controller;

    public UsersControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _loggerMock = new Mock<ILogger<UsersController>>();
        _controller = new UsersController(_mediatorMock.Object, _loggerMock.Object);
        
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
    public async Task GetProfile_Should_Return_Ok_When_Profile_Found()
    {
        // Arrange
        var userDto = new UserDto
        {
            Id = 1,
            Email = "test@example.com",
            DisplayName = "Test User"
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetUserProfileQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(userDto));

        // Act
        var result = await _controller.GetProfile();

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(userDto);
    }

    [Fact]
    public async Task GetProfile_Should_Return_NotFound_When_Profile_Not_Found()
    {
        // Arrange
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetUserProfileQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<UserDto>("User not found"));

        // Act
        var result = await _controller.GetProfile();

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task UpdateProfile_Should_Return_Ok_When_Update_Successful()
    {
        // Arrange
        var request = new UpdateUserProfileRequest
        {
            DisplayName = "Updated Name"
        };
        
        var userDto = new UserDto
        {
            Id = 1,
            Email = "test@example.com",
            DisplayName = "Updated Name"
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<UpdateUserProfileCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(userDto));

        // Act
        var result = await _controller.UpdateProfile(request);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(userDto);
    }

    [Fact]
    public async Task GetMetrics_Should_Return_Ok_When_Metrics_Found()
    {
        // Arrange
        var fixedDate = new DateTime(2024, 1, 15, 10, 30, 0, DateTimeKind.Utc);
        var metrics = new List<UserMetricDto>
        {
            new UserMetricDto
            {
                Id = 1,
                UserId = 1,
                Date = fixedDate,
                Weight = 70.5,
                Height = 175.0,
                Bmi = 23.0,
                BmiCategory = "Normal"
            }
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetUserMetricsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(metrics.AsEnumerable()));

        // Act
        var result = await _controller.GetMetrics();

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(metrics);
    }
}
