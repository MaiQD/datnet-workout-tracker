using dotFitness.Common.Configuration;
using dotFitness.Modules.Users.Application.Services;
using dotFitness.Modules.Users.Domain.Entities;
using dotFitness.Modules.Users.Infrastructure.Configuration;
using dotFitness.Modules.Users.Infrastructure.Data;
using dotFitness.Modules.Users.Infrastructure.Settings;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

namespace dotFitness.Modules.Users.Infrastructure.Tests.Configuration;

public class UsersModuleInstallerTests
{
    [Fact]
    public void InstallServices_Should_Register_Required_Services()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:PostgreSQL"] = "Host=localhost;Database=test;Username=test;Password=test",
                ["AdminSettings:AdminEmails:0"] = "admin@dotfitness.com"
            })
            .Build();

        var installer = new UsersModuleInstaller();

        // Act
        installer.InstallServices(services, configuration);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        
        // Verify that key services are registered
        var userManager = serviceProvider.GetService<UserManager<ApplicationUser>>();
        var signInManager = serviceProvider.GetService<SignInManager<ApplicationUser>>();
        var googleAuthService = serviceProvider.GetService<IGoogleAuthService>();
        var userService = serviceProvider.GetService<IUserService>();
        var context = serviceProvider.GetService<UsersDbContext>();
        
        userManager.Should().NotBeNull();
        signInManager.Should().NotBeNull();
        googleAuthService.Should().NotBeNull();
        userService.Should().NotBeNull();
        context.Should().NotBeNull();
    }

    [Fact]
    public void InstallServices_Should_Configure_Admin_Settings()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["AdminSettings:AdminEmails:0"] = "admin@dotfitness.com",
                ["AdminSettings:AdminEmails:1"] = "admin2@dotfitness.com"
            })
            .Build();

        var installer = new UsersModuleInstaller();

        // Act
        installer.InstallServices(services, configuration);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var adminSettings = serviceProvider.GetRequiredService<IOptions<AdminSettings>>();
        
        adminSettings.Value.AdminEmails.Should().Contain("admin@dotfitness.com");
        adminSettings.Value.AdminEmails.Should().Contain("admin2@dotfitness.com");
    }

    [Fact]
    public void InstallServices_Should_Configure_Identity_Options()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:PostgreSQL"] = "Host=localhost;Database=test;Username=test;Password=test"
            })
            .Build();

        var installer = new UsersModuleInstaller();

        // Act
        installer.InstallServices(services, configuration);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var identityOptions = serviceProvider.GetRequiredService<IOptions<IdentityOptions>>();
        
        identityOptions.Value.SignIn.RequireConfirmedAccount.Should().BeFalse();
        identityOptions.Value.User.RequireUniqueEmail.Should().BeTrue();
        identityOptions.Value.Password.RequireDigit.Should().BeFalse();
        identityOptions.Value.Password.RequiredLength.Should().Be(6);
        identityOptions.Value.Password.RequireNonAlphanumeric.Should().BeFalse();
        identityOptions.Value.Password.RequireUppercase.Should().BeFalse();
    }

    [Fact]
    public void InstallServices_Should_Configure_Authorization_Policies()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:PostgreSQL"] = "Host=localhost;Database=test;Username=test;Password=test"
            })
            .Build();

        var installer = new UsersModuleInstaller();

        // Act
        installer.InstallServices(services, configuration);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var authorizationService = serviceProvider.GetRequiredService<IAuthorizationService>();
        
        authorizationService.Should().NotBeNull();
    }

    [Fact]
    public void InstallServices_Should_Register_Health_Checks()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:PostgreSQL"] = "Host=localhost;Database=test;Username=test;Password=test"
            })
            .Build();

        var installer = new UsersModuleInstaller();

        // Act
        installer.InstallServices(services, configuration);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var healthCheckService = serviceProvider.GetRequiredService<Microsoft.Extensions.Diagnostics.HealthChecks.IHealthCheck>();
        
        healthCheckService.Should().NotBeNull();
    }

    [Fact]
    public void InstallServices_Should_Register_Configuration_Validator()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:PostgreSQL"] = "Host=localhost;Database=test;Username=test;Password=test"
            })
            .Build();

        var installer = new UsersModuleInstaller();

        // Act
        installer.InstallServices(services, configuration);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var configurationValidator = serviceProvider.GetService<IModuleConfigurationValidator>();
        
        configurationValidator.Should().NotBeNull();
    }

    [Fact]
    public void InstallServices_Should_Register_Database_Migration_Service()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:PostgreSQL"] = "Host=localhost;Database=test;Username=test;Password=test"
            })
            .Build();

        var installer = new UsersModuleInstaller();

        // Act
        installer.InstallServices(services, configuration);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var migrationService = serviceProvider.GetService<Microsoft.Extensions.Hosting.IHostedService>();
        
        migrationService.Should().NotBeNull();
    }
}
