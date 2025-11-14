using dotFitness.Common.Security;
using FluentAssertions;

namespace dotFitness.Common.Tests.Security;

public class SecurityEventTypesTests
{
    [Fact]
    public void SecurityEventTypes_Should_Have_Correct_Values()
    {
        // Assert
        SecurityEventTypes.LoginSuccess.Should().Be("LoginSuccess");
        SecurityEventTypes.LoginFailure.Should().Be("LoginFailure");
        SecurityEventTypes.Logout.Should().Be("Logout");
        SecurityEventTypes.TokenRefresh.Should().Be("TokenRefresh");
        SecurityEventTypes.UnauthorizedAccess.Should().Be("UnauthorizedAccess");
        SecurityEventTypes.ForbiddenAccess.Should().Be("ForbiddenAccess");
        SecurityEventTypes.ResourceAccessAttempt.Should().Be("ResourceAccessAttempt");
        SecurityEventTypes.ResourceModificationAttempt.Should().Be("ResourceModificationAttempt");
        SecurityEventTypes.UserCreation.Should().Be("UserCreation");
        SecurityEventTypes.UserUpdate.Should().Be("UserUpdate");
        SecurityEventTypes.RoleAssignment.Should().Be("RoleAssignment");
        SecurityEventTypes.PasswordChange.Should().Be("PasswordChange");
        SecurityEventTypes.AccountLockout.Should().Be("AccountLockout");
        SecurityEventTypes.AccountUnlock.Should().Be("AccountUnlock");
        SecurityEventTypes.RateLimitExceeded.Should().Be("RateLimitExceeded");
    }

    [Fact]
    public void SecurityEventTypes_Should_Not_Be_Empty()
    {
        // Assert
        SecurityEventTypes.LoginSuccess.Should().NotBeNullOrEmpty();
        SecurityEventTypes.LoginFailure.Should().NotBeNullOrEmpty();
        SecurityEventTypes.Logout.Should().NotBeNullOrEmpty();
        SecurityEventTypes.TokenRefresh.Should().NotBeNullOrEmpty();
        SecurityEventTypes.UnauthorizedAccess.Should().NotBeNullOrEmpty();
        SecurityEventTypes.ForbiddenAccess.Should().NotBeNullOrEmpty();
        SecurityEventTypes.ResourceAccessAttempt.Should().NotBeNullOrEmpty();
        SecurityEventTypes.ResourceModificationAttempt.Should().NotBeNullOrEmpty();
        SecurityEventTypes.UserCreation.Should().NotBeNullOrEmpty();
        SecurityEventTypes.UserUpdate.Should().NotBeNullOrEmpty();
        SecurityEventTypes.RoleAssignment.Should().NotBeNullOrEmpty();
        SecurityEventTypes.PasswordChange.Should().NotBeNullOrEmpty();
        SecurityEventTypes.AccountLockout.Should().NotBeNullOrEmpty();
        SecurityEventTypes.AccountUnlock.Should().NotBeNullOrEmpty();
        SecurityEventTypes.RateLimitExceeded.Should().NotBeNullOrEmpty();
    }
}
