using System.Net;
using System.Text.Json;
using dotFitness.Modules.Users.Application.Services;
using dotFitness.Modules.Users.Infrastructure.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace dotFitness.Modules.Users.Infrastructure.Tests.Services;

public class GoogleAuthServiceTests
{
    private readonly Mock<ILogger<GoogleAuthService>> _loggerMock;
    private readonly Mock<HttpClient> _httpClientMock;
    private readonly GoogleAuthService _service;

    public GoogleAuthServiceTests()
    {
        _loggerMock = new Mock<ILogger<GoogleAuthService>>();
        _httpClientMock = new Mock<HttpClient>();
        _service = new GoogleAuthService(_loggerMock.Object, _httpClientMock.Object);
    }

    [Fact]
    public async Task GetUserInfoAsync_Should_Return_User_Info_When_Valid_Token()
    {
        // Arrange
        var token = "valid_token";
        var expectedUserInfo = new GoogleUserInfo(
            "google123",
            "test@example.com",
            "Test User",
            "https://example.com/profile.jpg"
        );

        var responseContent = JsonSerializer.Serialize(new
        {
            id = "google123",
            email = "test@example.com",
            name = "Test User",
            picture = "https://example.com/profile.jpg"
        });

        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseContent)
        };

        _httpClientMock
            .Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(httpResponse);

        // Act
        var result = await _service.GetUserInfoAsync(token);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be("google123");
        result.Email.Should().Be("test@example.com");
        result.Name.Should().Be("Test User");
        result.ProfilePicture.Should().Be("https://example.com/profile.jpg");
    }

    [Fact]
    public async Task GetUserInfoAsync_Should_Return_Null_When_Invalid_Token()
    {
        // Arrange
        var token = "invalid_token";
        var httpResponse = new HttpResponseMessage(HttpStatusCode.Unauthorized);

        _httpClientMock
            .Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(httpResponse);

        // Act
        var result = await _service.GetUserInfoAsync(token);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetUserInfoAsync_Should_Return_Null_When_Http_Error()
    {
        // Arrange
        var token = "valid_token";
        var httpResponse = new HttpResponseMessage(HttpStatusCode.InternalServerError);

        _httpClientMock
            .Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(httpResponse);

        // Act
        var result = await _service.GetUserInfoAsync(token);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetUserInfoAsync_Should_Return_Null_When_Invalid_Json()
    {
        // Arrange
        var token = "valid_token";
        var responseContent = "invalid json";

        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseContent)
        };

        _httpClientMock
            .Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(httpResponse);

        // Act
        var result = await _service.GetUserInfoAsync(token);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetUserInfoAsync_Should_Return_Null_When_Exception_Thrown()
    {
        // Arrange
        var token = "valid_token";

        _httpClientMock
            .Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new HttpRequestException("Network error"));

        // Act
        var result = await _service.GetUserInfoAsync(token);

        // Assert
        result.Should().BeNull();
    }
}
