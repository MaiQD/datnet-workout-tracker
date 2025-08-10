using System.Text.Json;

namespace dotFitness.Api.Infrastructure;

/// <summary>
/// Validates module configuration at startup to ensure all required settings are present
/// </summary>
public static class ModuleConfigurationValidator
{
    public static readonly string[] ModuleNames = { "Users", "Exercises", "Routines", "WorkoutLogs" };

    /// <summary>
    /// Validates configuration for all modules
    /// </summary>
    /// <param name="configuration">The configuration to validate</param>
    /// <param name="logger">Logger for validation results</param>
    /// <returns>Validation result with details</returns>
    public static ModuleConfigurationValidationResult ValidateModuleConfiguration(IConfiguration configuration, ILogger logger)
    {
        var result = new ModuleConfigurationValidationResult();
        var moduleNames = ModuleNames;

        logger.LogInformation("Starting module configuration validation for {ModuleCount} modules", moduleNames.Length);

        foreach (var moduleName in moduleNames)
        {
            var moduleValidation = ValidateModuleConfiguration(configuration, moduleName, logger);
            result.ModuleValidations[moduleName] = moduleValidation;
        }

        // Validate global module settings
        ValidateGlobalModuleSettings(configuration, result, logger);

        // Log validation summary
        var validModules = result.ModuleValidations.Values.Count(v => v.IsValid);
        var invalidModules = result.ModuleValidations.Values.Count(v => !v.IsValid);

        logger.LogInformation("Module configuration validation completed: {ValidModules} valid, {InvalidModules} invalid", 
            validModules, invalidModules);

        if (invalidModules > 0)
        {
            logger.LogWarning("Module configuration validation found issues. Check logs for details.");
        }

        return result;
    }

    /// <summary>
    /// Validates configuration for a specific module
    /// </summary>
    /// <param name="configuration">The configuration to validate</param>
    /// <param name="moduleName">Name of the module to validate</param>
    /// <param name="logger">Logger for validation results</param>
    /// <returns>Module-specific validation result</returns>
    private static ModuleValidationResult ValidateModuleConfiguration(IConfiguration configuration, string moduleName, ILogger logger)
    {
        var result = new ModuleValidationResult { ModuleName = moduleName };
        var moduleSection = configuration.GetSection($"Modules:{moduleName}");

        logger.LogDebug("Validating configuration for module: {ModuleName}", moduleName);

        // Check if module section exists
        if (!moduleSection.Exists())
        {
            result.AddWarning($"Module configuration section 'Modules:{moduleName}' not found (module may be optional)");
        }
        else
        {
            // Validate module-specific settings based on module name
            switch (moduleName.ToLowerInvariant())
            {
                case "users":
                    ValidateUsersModuleConfiguration(moduleSection, result, logger);
                    break;
                case "exercises":
                    ValidateExercisesModuleConfiguration(moduleSection, result, logger);
                    break;
                default:
                    ValidateGenericModuleConfiguration(moduleSection, result, logger);
                    break;
            }
        }

        result.IsValid = !result.Errors.Any();
        return result;
    }

    /// <summary>
    /// Validates Users module specific configuration
    /// </summary>
    private static void ValidateUsersModuleConfiguration(IConfigurationSection moduleSection, ModuleValidationResult result, ILogger logger)
    {
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
        }

        // Validate Admin settings
        var adminSection = moduleSection.GetSection("AdminSettings");
        if (!adminSection.Exists())
        {
            result.AddWarning("Admin settings not found for Users module (optional)");
            logger.LogDebug("Admin settings not found for Users module (optional)");
        }
    }

    /// <summary>
    /// Validates Exercises module specific configuration
    /// </summary>
    private static void ValidateExercisesModuleConfiguration(IConfigurationSection moduleSection, ModuleValidationResult result, ILogger logger)
    {
        // Check for exercise-specific settings
        var maxExercisesPerUser = moduleSection.GetValue<int?>("MaxExercisesPerUser");
        if (maxExercisesPerUser.HasValue && maxExercisesPerUser.Value <= 0)
        {
            result.AddError("MaxExercisesPerUser must be greater than 0");
        }

        var enableGlobalExercises = moduleSection.GetValue<bool?>("EnableGlobalExercises");
        if (enableGlobalExercises.HasValue)
        {
            logger.LogDebug("Global exercises enabled: {Enabled}", enableGlobalExercises.Value);
        }
    }

    /// <summary>
    /// Validates Routines module specific configuration
    /// </summary>
    private static void ValidateRoutinesModuleConfiguration(IConfigurationSection moduleSection, ModuleValidationResult result, ILogger logger)
    {
        // Routines module is not implemented yet, so this is just a placeholder
        result.AddWarning("Routines module configuration validation not implemented (module not yet available)");
        logger.LogDebug("Routines module configuration validation skipped (module not yet available)");
    }

    /// <summary>
    /// Validates WorkoutLogs module specific configuration
    /// </summary>
    private static void ValidateWorkoutLogsModuleConfiguration(IConfigurationSection moduleSection, ModuleValidationResult result, ILogger logger)
    {
        // WorkoutLogs module is not implemented yet, so this is just a placeholder
        result.AddWarning("WorkoutLogs module configuration validation not implemented (module not yet available)");
        logger.LogDebug("WorkoutLogs module configuration validation skipped (module not yet available)");
    }

    /// <summary>
    /// Validates generic module configuration
    /// </summary>
    private static void ValidateGenericModuleConfiguration(IConfigurationSection moduleSection, ModuleValidationResult result, ILogger logger)
    {
        // Check for common module settings
        var enabled = moduleSection.GetValue<bool?>("Enabled");
        if (enabled.HasValue && !enabled.Value)
        {
            result.AddWarning("Module is disabled in configuration");
        }
    }

    /// <summary>
    /// Validates global module settings
    /// </summary>
    private static void ValidateGlobalModuleSettings(IConfiguration configuration, ModuleConfigurationValidationResult result, ILogger logger)
    {
        var globalModuleSection = configuration.GetSection("Modules:Global");
        
        if (globalModuleSection.Exists())
        {
            var moduleTimeout = globalModuleSection.GetValue<int?>("ModuleLoadTimeout");
            if (moduleTimeout.HasValue && moduleTimeout.Value <= 0)
            {
                result.AddGlobalError("ModuleLoadTimeout must be greater than 0");
            }
        }
    }
}

/// <summary>
/// Result of module configuration validation
/// </summary>
public class ModuleConfigurationValidationResult
{
    public ModuleConfigurationValidationResult()
    {
        ModuleValidations = new Dictionary<string, ModuleValidationResult>();
        GlobalErrors = new List<string>();
        GlobalWarnings = new List<string>();
    }

    public Dictionary<string, ModuleValidationResult> ModuleValidations { get; set; }
    public List<string> GlobalErrors { get; set; }
    public List<string> GlobalWarnings { get; set; }

    public bool IsValid => !GlobalErrors.Any() && ModuleValidations.Values.All(v => v.IsValid);

    public void AddGlobalError(string error) => GlobalErrors.Add(error);
    public void AddGlobalWarning(string warning) => GlobalWarnings.Add(warning);

    public string ToJson() => JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
}

/// <summary>
/// Result of individual module validation
/// </summary>
public class ModuleValidationResult
{
    public ModuleValidationResult()
    {
        Errors = new List<string>();
        Warnings = new List<string>();
    }

    public string ModuleName { get; set; } = string.Empty;
    public List<string> Errors { get; set; }
    public List<string> Warnings { get; set; }
    public bool IsValid { get; set; }

    public void AddError(string error) => Errors.Add(error);
    public void AddWarning(string warning) => Warnings.Add(warning);
} 