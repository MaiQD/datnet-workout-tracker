using dotFitness.Common.Authorization;
using FluentAssertions;

namespace dotFitness.Common.Tests.Authorization;

public class AuthorizationConstantsTests
{
    [Fact]
    public void Policies_Should_Have_Correct_Values()
    {
        // Assert
        AuthorizationPolicies.AdminOnly.Should().Be("AdminOnly");
        AuthorizationPolicies.PtOnly.Should().Be("PTOnly");
        AuthorizationPolicies.UserOnly.Should().Be("UserOnly");
        AuthorizationPolicies.SelfOrAdmin.Should().Be("SelfOrAdmin");
        AuthorizationPolicies.ResourceOwner.Should().Be("ResourceOwner");
        AuthorizationPolicies.PtAssignedOrAdmin.Should().Be("PTAssignedOrAdmin");
        AuthorizationPolicies.OwnerOrPtOrAdmin.Should().Be("OwnerOrPTOrAdmin");
    }

    [Fact]
    public void Roles_Should_Have_Correct_Values()
    {
        // Assert
        Roles.Admin.Should().Be("Admin");
        Roles.Pt.Should().Be("PT");
        Roles.User.Should().Be("User");
    }

    [Fact]
    public void Policies_Should_Not_Be_Empty()
    {
        // Assert
        AuthorizationPolicies.AdminOnly.Should().NotBeNullOrEmpty();
        AuthorizationPolicies.PtOnly.Should().NotBeNullOrEmpty();
        AuthorizationPolicies.UserOnly.Should().NotBeNullOrEmpty();
        AuthorizationPolicies.SelfOrAdmin.Should().NotBeNullOrEmpty();
        AuthorizationPolicies.ResourceOwner.Should().NotBeNullOrEmpty();
        AuthorizationPolicies.PtAssignedOrAdmin.Should().NotBeNullOrEmpty();
        AuthorizationPolicies.OwnerOrPtOrAdmin.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void Roles_Should_Not_Be_Empty()
    {
        // Assert
        Roles.Admin.Should().NotBeNullOrEmpty();
        Roles.Pt.Should().NotBeNullOrEmpty();
        Roles.User.Should().NotBeNullOrEmpty();
    }
}
