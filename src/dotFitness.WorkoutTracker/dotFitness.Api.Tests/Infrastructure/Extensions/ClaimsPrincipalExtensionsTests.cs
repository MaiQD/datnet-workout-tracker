using FluentAssertions;
using dotFitness.Api.Infrastructure.Extensions;
using System.Security.Claims;

namespace dotFitness.Api.Tests.Infrastructure.Extensions;

public class ClaimsPrincipalExtensionsTests
{
    [Fact]
    public void GetUserId_Should_Return_UserId_When_Valid_Claim_Exists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        }, "test"));

        // Act
        var result = claimsPrincipal.GetUserId();

        // Assert
        result.Should().Be(userId);
    }

    [Fact]
    public void GetUserId_Should_Return_Null_When_No_NameIdentifier_Claim()
    {
        // Arrange
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Email, "test@example.com")
        }, "test"));

        // Act
        var result = claimsPrincipal.GetUserId();

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void GetUserId_Should_Return_Null_When_Invalid_Guid_Format()
    {
        // Arrange
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "invalid-guid")
        }, "test"));

        // Act
        var result = claimsPrincipal.GetUserId();

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void GetUserId_Should_Return_Null_When_Empty_Claim_Value()
    {
        // Arrange
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "")
        }, "test"));

        // Act
        var result = claimsPrincipal.GetUserId();

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void GetRequiredUserId_Should_Return_UserId_When_Valid_Claim_Exists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        }, "test"));

        // Act
        var result = claimsPrincipal.GetRequiredUserId();

        // Assert
        result.Should().Be(userId);
    }

    [Fact]
    public void GetRequiredUserId_Should_Throw_Exception_When_No_NameIdentifier_Claim()
    {
        // Arrange
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Email, "test@example.com")
        }, "test"));

        // Act & Assert
        var exception = Assert.Throws<UnauthorizedAccessException>(() => claimsPrincipal.GetRequiredUserId());
        exception.Message.Should().Be("User ID not found in token or invalid format");
    }

    [Fact]
    public void GetRequiredUserId_Should_Throw_Exception_When_Invalid_Guid_Format()
    {
        // Arrange
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "invalid-guid")
        }, "test"));

        // Act & Assert
        var exception = Assert.Throws<UnauthorizedAccessException>(() => claimsPrincipal.GetRequiredUserId());
        exception.Message.Should().Be("User ID not found in token or invalid format");
    }

    [Fact]
    public void GetRequiredUserId_Should_Throw_Exception_When_Empty_Claim_Value()
    {
        // Arrange
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "")
        }, "test"));

        // Act & Assert
        var exception = Assert.Throws<UnauthorizedAccessException>(() => claimsPrincipal.GetRequiredUserId());
        exception.Message.Should().Be("User ID not found in token or invalid format");
    }
}
