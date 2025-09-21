using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using MediatR;
using FluentAssertions;
using dotFitness.Api.Controllers;
using dotFitness.Modules.Users.Application.Commands;
using dotFitness.Modules.Users.Application.DTOs;
using dotFitness.SharedKernel.Results;

namespace dotFitness.Api.Tests.Controllers;

public class AuthControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<ILogger<AuthController>> _loggerMock;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _loggerMock = new Mock<ILogger<AuthController>>();
        _controller = new AuthController(_mediatorMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task LoginWithGoogle_Should_Return_Ok_When_Login_Successful()
    {
        // Arrange
        var request = new LoginWithGoogleRequest { GoogleToken = "test-token" };
        var loginResponse = new LoginResponseDto
        {
            Token = "jwt-token",
            UserId = 1,
            Email = "test@example.com",
            DisplayName = "Test User",
            ExpiresAt = DateTime.UtcNow.AddHours(1)
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<LoginWithGoogleCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(loginResponse));

        // Act
        var result = await _controller.LoginWithGoogle(request);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(loginResponse);
    }

    [Fact]
    public async Task LoginWithGoogle_Should_Return_BadRequest_When_Login_Fails()
    {
        // Arrange
        var request = new LoginWithGoogleRequest { GoogleToken = "invalid-token" };
        
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<LoginWithGoogleCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<LoginResponseDto>("Invalid token"));

        // Act
        var result = await _controller.LoginWithGoogle(request);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }
}
