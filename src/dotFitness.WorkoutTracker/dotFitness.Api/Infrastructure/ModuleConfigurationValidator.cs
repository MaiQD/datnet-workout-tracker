using dotFitness.SharedKernel.Configuration;

namespace dotFitness.Api.Infrastructure;

/// <summary>
/// Validates module configuration at startup to ensure all required settings are present
/// </summary>
public static class ModuleConfigurationValidator
{

    /// <summary>
    /// Validates configuration for all modules using auto-discovered validators
    /// </summary>
    /// <param name="configuration">The configuration to validate</param>
    /// <param name="logger">Logger for validation results</param>
    /// <param name="moduleValidators">Auto-discovered module validators</param>
    /// <returns>Validation result with details</returns>
    public static ModuleConfigurationValidationResult ValidateModuleConfiguration(
        IConfiguration configuration, 
        ILogger logger, 
        IEnumerable<IModuleConfigurationValidator> moduleValidators)
    {
        var result = new ModuleConfigurationValidationResult();
        var discoveredModules = moduleValidators.ToList();

        logger.LogInformation("Starting module configuration validation for {ModuleCount} discovered modules: {Modules}", 
            discoveredModules.Count, string.Join(", ", discoveredModules.Select(v => v.ModuleName)));

        foreach (var validator in discoveredModules)
        {
            var moduleSection = configuration.GetSection($"Modules:{validator.ModuleName}");
            var moduleValidation = ValidateModuleConfiguration(configuration, validator, moduleSection, logger);
            result.ModuleValidations[validator.ModuleName] = moduleValidation;
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
    /// Validates configuration for a specific module using its validator
    /// </summary>
    /// <param name="configuration">The configuration to validate</param>
    /// <param name="validator">The module validator</param>
    /// <param name="moduleSection">The module configuration section</param>
    /// <param name="logger">Logger for validation results</param>
    /// <returns>Module-specific validation result</returns>
    private static ModuleValidationResult ValidateModuleConfiguration(
        IConfiguration configuration, 
        IModuleConfigurationValidator validator, 
        IConfigurationSection moduleSection, 
        ILogger logger)
    {
        logger.LogDebug("Validating configuration for module: {ModuleName}", validator.ModuleName);

        // Check if module section exists
        if (!moduleSection.Exists())
        {
            var result = new ModuleValidationResult { ModuleName = validator.ModuleName };
            result.AddWarning($"Module configuration section 'Modules:{validator.ModuleName}' not found (module may be optional)");
            result.IsValid = !result.Errors.Any();
            return result;
        }

        // Use the module's own validator
        return validator.ValidateConfiguration(moduleSection, logger);
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
