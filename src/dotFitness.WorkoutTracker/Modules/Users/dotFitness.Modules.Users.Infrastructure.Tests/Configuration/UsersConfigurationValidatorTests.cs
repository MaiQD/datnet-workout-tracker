using dotFitness.Modules.Users.Infrastructure.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace dotFitness.Modules.Users.Infrastructure.Tests.Configuration;

public class UsersConfigurationValidatorTests
{
    [Fact]
    public void Validate_Should_Return_Success_When_Configuration_Is_Valid()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:PostgreSQL"] = "Host=localhost;Database=test;Username=test;Password=test",
                ["AdminSettings:AdminEmails:0"] = "admin@dotfitness.com"
            })
            .Build();

        var loggerMock = new Mock<ILogger<UsersConfigurationValidator>>();
        var validator = new UsersConfigurationValidator( loggerMock.Object);

        // Act
        var result = validator.ValidateConfiguration(Mock.Of<IConfigurationSection>(),configuration);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Error.Should().BeNullOrEmpty();
    }

    [Fact]
    public void Validate_Should_Return_Failure_When_PostgreSQL_Connection_String_Is_Missing()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["AdminSettings:AdminEmails:0"] = "admin@dotfitness.com"
            })
            .Build();

        var loggerMock = new Mock<ILogger<UsersConfigurationValidator>>();
        var validator = new UsersConfigurationValidator(loggerMock.Object);

        // Act
        var result = validator.Validate(configuration);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("PostgreSQL connection string is missing");
    }

    [Fact]
    public void Validate_Should_Return_Failure_When_Admin_Emails_Are_Missing()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:PostgreSQL"] = "Host=localhost;Database=test;Username=test;Password=test"
            })
            .Build();

        var loggerMock = new Mock<ILogger<UsersConfigurationValidator>>();
        var validator = new UsersConfigurationValidator(loggerMock.Object);

        // Act
        var result = validator.Validate(configuration);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Admin emails are missing");
    }

    [Fact]
    public void Validate_Should_Return_Failure_When_Admin_Emails_Are_Empty()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:PostgreSQL"] = "Host=localhost;Database=test;Username=test;Password=test",
                ["AdminSettings:AdminEmails"] = ""
            })
            .Build();

        var loggerMock = new Mock<ILogger<UsersConfigurationValidator>>();
        var validator = new UsersConfigurationValidator(loggerMock.Object);

        // Act
        var result = validator.Validate(configuration);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Admin emails are missing");
    }

    [Fact]
    public void Validate_Should_Return_Success_When_Using_Aspire_Connection_String()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:dotFitnessDb-pg"] = "Host=localhost;Database=test;Username=test;Password=test",
                ["AdminSettings:AdminEmails:0"] = "admin@dotfitness.com"
            })
            .Build();

        var loggerMock = new Mock<ILogger<UsersConfigurationValidator>>();
        var validator = new UsersConfigurationValidator(loggerMock.Object);

        // Act
        var result = validator.Validate(configuration);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Error.Should().BeNullOrEmpty();
    }

    [Fact]
    public void Validate_Should_Return_Success_When_Admin_Emails_Have_Multiple_Values()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:PostgreSQL"] = "Host=localhost;Database=test;Username=test;Password=test",
                ["AdminSettings:AdminEmails:0"] = "admin1@dotfitness.com",
                ["AdminSettings:AdminEmails:1"] = "admin2@dotfitness.com"
            })
            .Build();

        var loggerMock = new Mock<ILogger<UsersConfigurationValidator>>();
        var validator = new UsersConfigurationValidator(loggerMock.Object);

        // Act
        var result = validator.Validate(configuration);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Error.Should().BeNullOrEmpty();
    }
}
