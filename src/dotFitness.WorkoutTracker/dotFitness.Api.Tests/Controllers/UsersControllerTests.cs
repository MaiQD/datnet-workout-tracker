using Moq;
using MediatR;
using FluentAssertions;
using FastEndpoints;
using dotFitness.Common.Results;
using dotFitness.Modules.Users.API.Endpoints.Users;
using dotFitness.Modules.Users.Application.Commands;
using dotFitness.Modules.Users.Application.DTOs;
using dotFitness.Modules.Users.Application.Queries;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace dotFitness.Api.Tests.Controllers;

public class UsersControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Guid _testUserId;

    public UsersControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _testUserId = Guid.NewGuid();
    }

    private TEndpoint CreateEndpointWithUser<TEndpoint>(Func<IMediator, TEndpoint> factory) where TEndpoint : class
    {
        var endpoint = factory(_mediatorMock.Object);
        
        // Setup HttpContext with user claims
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, _testUserId.ToString()),
            new Claim(ClaimTypes.Role, "User")
        };
        var identity = new ClaimsIdentity(claims, "test");
        var principal = new ClaimsPrincipal(identity);

        var httpContext = new DefaultHttpContext
        {
            User = principal
        };

        // Use reflection to set HttpContext
        var httpContextProperty = endpoint.GetType().BaseType?.GetProperty("HttpContext", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        httpContextProperty?.SetValue(endpoint, httpContext);
        
        return endpoint;
    }

    [Fact]
    public async Task GetProfile_Should_Return_Ok_When_Profile_Found()
    {
        // Arrange
        var userDto = new UserDto
        {
            Id = _testUserId,
            Email = "test@example.com",
            DisplayName = "Test User"
        };

        _mediatorMock
            .Setup(m => m.Send(It.Is<GetUserProfileQuery>(q => q.UserId == _testUserId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(userDto));

        var endpoint = CreateEndpointWithUser(m => new GetUserProfileEndpoint(m));

        // Act
        await endpoint.HandleAsync(new EmptyRequest(), CancellationToken.None);

        // Assert
        _mediatorMock.Verify(m => m.Send(It.Is<GetUserProfileQuery>(q => q.UserId == _testUserId), It.IsAny<CancellationToken>()), Times.Once);
        endpoint.Response.Should().BeEquivalentTo(userDto);
    }

    [Fact]
    public async Task GetProfile_Should_Handle_Failure_When_Profile_Not_Found()
    {
        // Arrange
        _mediatorMock
            .Setup(m => m.Send(It.Is<GetUserProfileQuery>(q => q.UserId == _testUserId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<UserDto>("User not found"));

        var endpoint = CreateEndpointWithUser(m => new GetUserProfileEndpoint(m));

        // Act
        await endpoint.HandleAsync(new EmptyRequest(), CancellationToken.None);

        // Assert
        _mediatorMock.Verify(m => m.Send(It.Is<GetUserProfileQuery>(q => q.UserId == _testUserId), It.IsAny<CancellationToken>()), Times.Once);
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
            Id = _testUserId,
            Email = "test@example.com",
            DisplayName = "Updated Name"
        };

        _mediatorMock
            .Setup(m => m.Send(It.Is<UpdateUserProfileCommand>(c => c.UserId == _testUserId && c.Request.DisplayName == request.DisplayName), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(userDto));

        var endpoint = CreateEndpointWithUser(m => new UpdateUserProfileEndpoint(m));

        // Act
        await endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        _mediatorMock.Verify(m => m.Send(It.Is<UpdateUserProfileCommand>(c => c.UserId == _testUserId), It.IsAny<CancellationToken>()), Times.Once);
        endpoint.Response.Should().BeEquivalentTo(userDto);
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
                UserId = _testUserId,
                Date = fixedDate,
                Weight = 70.5,
                Height = 175.0,
                Bmi = 23.0,
                BmiCategory = "Normal"
            }
        };

        _mediatorMock
            .Setup(m => m.Send(It.Is<GetUserMetricsQuery>(q => q.UserId == _testUserId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(metrics.AsEnumerable()));

        var endpoint = CreateEndpointWithUser(m => new GetUserMetricsEndpoint(m));

        // Act
        var request = new GetUserMetricsRequest();
        await endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        _mediatorMock.Verify(m => m.Send(It.Is<GetUserMetricsQuery>(q => q.UserId == _testUserId), It.IsAny<CancellationToken>()), Times.Once);
        endpoint.Response.Should().BeEquivalentTo(metrics);
    }
}
