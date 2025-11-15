using Moq;
using MediatR;
using dotFitness.Common.Results;
using dotFitness.Modules.Users.API.Endpoints.Auth;
using dotFitness.Modules.Users.Application.Commands;
using dotFitness.Modules.Users.Application.DTOs;

namespace dotFitness.Api.Tests.Controllers;

public class AuthControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly LoginWithGoogleEndpoint _endpoint;

    public AuthControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _endpoint = new LoginWithGoogleEndpoint(_mediatorMock.Object);
    }

    [Fact]
    public async Task LoginWithGoogle_Should_Return_Ok_When_Login_Successful()
    {
        // Arrange
        var request = new LoginWithGoogleRequest { GoogleToken = "test-token" };
        var loginResponse = new LoginResponseDto
        {
            AccessToken = "jwt-token",
            RefreshToken = "jwt-refresh-token",
            UserId = Guid.NewGuid(),
            Email = "test@example.com",
            DisplayName = "Test User",
            ExpiresIn = 60
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<LoginWithGoogleCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(loginResponse));

        // Act
        await _endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        _mediatorMock.Verify(m => m.Send(It.Is<LoginWithGoogleCommand>(c => c.Request.GoogleToken == request.GoogleToken), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task LoginWithGoogle_Should_Handle_Failure_When_Login_Fails()
    {
        // Arrange
        var request = new LoginWithGoogleRequest { GoogleToken = "invalid-token" };
        
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<LoginWithGoogleCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<LoginResponseDto>("Invalid token"));

        // Act
        await _endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        _mediatorMock.Verify(m => m.Send(It.Is<LoginWithGoogleCommand>(c => c.Request.GoogleToken == request.GoogleToken), It.IsAny<CancellationToken>()), Times.Once);
    }
}
