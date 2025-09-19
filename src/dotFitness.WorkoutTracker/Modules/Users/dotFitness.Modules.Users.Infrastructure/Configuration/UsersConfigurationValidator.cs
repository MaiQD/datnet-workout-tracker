using dotFitness.SharedKernel.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace dotFitness.Modules.Users.Infrastructure.Configuration;

/// <summary>
/// Validates Users module specific configuration
/// </summary>
public class UsersConfigurationValidator : IModuleConfigurationValidator
{
    /// <summary>
    /// The name of the module this validator handles
    /// </summary>
    public string ModuleName => "Users";

    /// <summary>
    /// Validates Users module configuration
    /// </summary>
    /// <param name="moduleSection">The module configuration section</param>
    /// <param name="logger">Logger for validation results</param>
    /// <returns>Module validation result</returns>
    public ModuleValidationResult ValidateConfiguration(IConfigurationSection moduleSection, ILogger logger)
    {
        var result = new ModuleValidationResult { ModuleName = "Users" };

        logger.LogDebug("Validating Users module configuration");

        // Validate JWT settings
        var jwtSection = moduleSection.GetSection("JwtSettings");
        if (jwtSection.Exists())
        {
            var requiredJwtSettings = new[] { "SecretKey", "Issuer", "Audience" };
            foreach (var setting in requiredJwtSettings)
            {
                if (string.IsNullOrEmpty(jwtSection[setting]))
                {
                    result.AddError($"JWT setting '{setting}' is missing or empty");
                }
            }

            // Validate JWT secret key strength
            var secretKey = jwtSection["SecretKey"];
            if (!string.IsNullOrEmpty(secretKey) && secretKey.Length < 32)
            {
                result.AddWarning("JWT SecretKey should be at least 32 characters for security");
            }

            // Validate expiration
            var expirationStr = jwtSection["ExpirationInHours"];
            if (!string.IsNullOrEmpty(expirationStr) && int.TryParse(expirationStr, out var expiration))
            {
                if (expiration <= 0 || expiration > 168) // 1 week max
                {
                    result.AddError("JWT ExpirationInHours must be between 1 and 168 (1 week)");
                }
            }
        }
        else
        {
            result.AddError("JwtSettings section is required for Users module");
        }

        // Validate Admin settings
        var adminSection = moduleSection.GetSection("AdminSettings");
        if (adminSection.Exists())
        {
            var adminEmails = adminSection.GetSection("AdminEmails").Get<string[]>();
            if (adminEmails == null || adminEmails.Length == 0)
            {
                result.AddWarning("No admin emails configured");
            }
            else
            {
                foreach (var email in adminEmails)
                {
                    if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
                    {
                        result.AddError($"Invalid admin email format: {email}");
                    }
                }
            }
        }
        else
        {
            result.AddWarning("AdminSettings not found for Users module (optional)");
        }

        // Validate Google OAuth settings (optional)
        var googleSection = moduleSection.GetSection("GoogleOAuth");
        if (googleSection.Exists())
        {
            var clientId = googleSection["ClientId"];
            var clientSecret = googleSection["ClientSecret"];

            if (string.IsNullOrEmpty(clientId))
            {
                result.AddError("Google OAuth ClientId is required when GoogleOAuth section is present");
            }
            if (string.IsNullOrEmpty(clientSecret))
            {
                result.AddError("Google OAuth ClientSecret is required when GoogleOAuth section is present");
            }
        }

        // Validate database connections
        ValidateDatabaseConnections(result, logger);

        result.IsValid = !result.Errors.Any();
        
        if (result.IsValid)
        {
            logger.LogDebug("Users module configuration validation passed");
        }
        else
        {
            logger.LogWarning("Users module configuration validation failed with {ErrorCount} errors", result.Errors.Count);
        }

        return result;
    }

    /// <summary>
    /// Validates database connection configurations for Users module
    /// </summary>
    private static void ValidateDatabaseConnections(ModuleValidationResult result, ILogger logger)
    {
        // Note: In a real implementation, we would check if connection strings are accessible
        // For now, we just log that this validation should be done
        logger.LogDebug("Database connection validation for Users module - checking PostgreSQL and MongoDB availability");
        
        // This could be extended to actually test connections, but that might be
        // expensive to do at startup validation time
        result.AddWarning("Database connection validation not implemented (connections tested at runtime via health checks)");
    }
}
