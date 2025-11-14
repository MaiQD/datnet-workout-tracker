using FluentAssertions;
using dotFitness.Api.Infrastructure.Authorization;

namespace dotFitness.Api.Tests.Infrastructure.Authorization;

public class AuthorizationRequirementTests
{
    [Fact]
    public void SelfOrAdminRequirement_Should_Be_Created()
    {
        // Act
        var requirement = new SelfOrAdminRequirement();

        // Assert
        requirement.Should().NotBeNull();
    }

    [Fact]
    public void ResourceOwnerRequirement_Should_Be_Created()
    {
        // Act
        var requirement = new ResourceOwnerRequirement();

        // Assert
        requirement.Should().NotBeNull();
    }

    [Fact]
    public void PTClientAccessRequirement_Should_Be_Created()
    {
        // Act
        var requirement = new PTClientAccessRequirement();

        // Assert
        requirement.Should().NotBeNull();
    }

    [Fact]
    public void OwnerOrPTOrAdminRequirement_Should_Be_Created()
    {
        // Act
        var requirement = new OwnerOrPTOrAdminRequirement();

        // Assert
        requirement.Should().NotBeNull();
    }
}
