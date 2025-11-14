using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using dotFitness.Api.Infrastructure.Configuration;
using dotFitness.Api.Infrastructure.Authorization;
using dotFitness.Common.Services;
using dotFitness.Modules.Users.Infrastructure.Data;

namespace dotFitness.Api.Tests.Infrastructure.Configuration;

public class AuthorizationConfigurationTests
{
    [Fact]
    public void AddApiAuthorization_Should_Register_Authorization_Policies()
    {
        // Arrange
        var services = new ServiceCollection();
        
        // Add required dependencies
        services.AddLogging();
        services.AddDbContext<UsersDbContext>(options => 
            options.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()));

        // Act
        services.AddApiAuthorization();

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var authorizationService = serviceProvider.GetRequiredService<IAuthorizationService>();
        
        // Verify that the service is registered
        authorizationService.Should().NotBeNull();
    }

    [Fact]
    public void AddApiAuthorization_Should_Register_Authorization_Handlers()
    {
        // Arrange
        var services = new ServiceCollection();
        
        // Add required dependencies
        services.AddLogging();
        services.AddDbContext<UsersDbContext>(options => 
            options.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()));

        // Act
        services.AddApiAuthorization();

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        
        // Note: These handlers are registered as scoped services, so we need to create a scope to get them
        using var scope = serviceProvider.CreateScope();
        
        // Get all IAuthorizationHandler services and verify they include our custom handlers
        var authorizationHandlers = scope.ServiceProvider.GetServices<IAuthorizationHandler>().ToList();
        
        // Verify that we have at least 4 handlers (our custom ones plus any default ones)
        authorizationHandlers.Should().HaveCountGreaterOrEqualTo(4);
        
        // Verify that our custom handlers are registered by checking their types
        var handlerTypes = authorizationHandlers.Select(h => h.GetType()).ToList();
        handlerTypes.Should().Contain(typeof(SelfOrAdminHandler));
        handlerTypes.Should().Contain(typeof(ResourceOwnerHandler));
        handlerTypes.Should().Contain(typeof(PTClientAccessHandler));
        handlerTypes.Should().Contain(typeof(OwnerOrPTOrAdminHandler));
    }

    [Fact]
    public void AddApiAuthorization_Should_Register_Security_Audit_Service()
    {
        // Arrange
        var services = new ServiceCollection();
        
        // Add required dependencies
        services.AddLogging();
        services.AddDbContext<UsersDbContext>(options => 
            options.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()));

        // Act
        services.AddApiAuthorization();

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        
        // Verify that the security audit service is registered
        using var scope = serviceProvider.CreateScope();
        var securityAuditService = scope.ServiceProvider.GetService<ISecurityAuditService>();
        
        securityAuditService.Should().NotBeNull();
    }
}
