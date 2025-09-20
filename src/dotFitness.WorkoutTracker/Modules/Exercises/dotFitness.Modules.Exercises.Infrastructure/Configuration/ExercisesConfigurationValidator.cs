using dotFitness.SharedKernel.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace dotFitness.Modules.Exercises.Infrastructure.Configuration;

/// <summary>
/// Validates Exercises module specific configuration
/// </summary>
public class ExercisesConfigurationValidator : IModuleConfigurationValidator
{
    /// <summary>
    /// The name of the module this validator handles
    /// </summary>
    public string ModuleName => "Exercises";

    /// <summary>
    /// Validates Exercises module configuration
    /// </summary>
    /// <param name="moduleSection">The module configuration section</param>
    /// <param name="logger">Logger for validation results</param>
    /// <returns>Module validation result</returns>
    public ModuleValidationResult ValidateConfiguration(IConfigurationSection moduleSection, ILogger logger)
    {
        var result = new ModuleValidationResult { ModuleName = "Exercises" };

        logger.LogDebug("Validating Exercises module configuration");

        // Check for exercise-specific settings
        var maxExercisesPerUserStr = moduleSection["MaxExercisesPerUser"];
        if (!string.IsNullOrEmpty(maxExercisesPerUserStr) && int.TryParse(maxExercisesPerUserStr, out var maxExercisesPerUser))
        {
            if (maxExercisesPerUser <= 0)
            {
                result.AddError("MaxExercisesPerUser must be greater than 0");
            }
            else if (maxExercisesPerUser > 10000)
            {
                result.AddWarning("MaxExercisesPerUser is very high (>10000), consider if this is intentional");
            }
        }

        var enableGlobalExercisesStr = moduleSection["EnableGlobalExercises"];
        if (!string.IsNullOrEmpty(enableGlobalExercisesStr) && bool.TryParse(enableGlobalExercisesStr, out var enableGlobalExercises))
        {
            logger.LogDebug("Global exercises enabled: {Enabled}", enableGlobalExercises);
        }

        // Validate muscle group settings
        var autoGenerateMuscleGroupsStr = moduleSection["AutoGenerateMuscleGroups"];
        if (!string.IsNullOrEmpty(autoGenerateMuscleGroupsStr) && bool.TryParse(autoGenerateMuscleGroupsStr, out var autoGenerateMuscleGroups) && autoGenerateMuscleGroups)
        {
            logger.LogDebug("Auto-generation of muscle groups is enabled");
        }

        // Validate equipment settings
        var defaultEquipmentEnabledStr = moduleSection["DefaultEquipmentEnabled"];
        if (!string.IsNullOrEmpty(defaultEquipmentEnabledStr) && bool.TryParse(defaultEquipmentEnabledStr, out var defaultEquipmentEnabled))
        {
            logger.LogDebug("Default equipment enabled: {Enabled}", defaultEquipmentEnabled);
        }

        // Validate exercise validation rules
        var requireMuscleGroupsStr = moduleSection["RequireMuscleGroups"];
        if (!string.IsNullOrEmpty(requireMuscleGroupsStr) && bool.TryParse(requireMuscleGroupsStr, out var requireMuscleGroups) && requireMuscleGroups)
        {
            logger.LogDebug("Muscle groups are required for exercises");
        }

        var allowDuplicateNamesStr = moduleSection["AllowDuplicateNames"];
        if (!string.IsNullOrEmpty(allowDuplicateNamesStr) && bool.TryParse(allowDuplicateNamesStr, out var allowDuplicateNames) && allowDuplicateNames)
        {
            result.AddWarning("Duplicate exercise names are allowed - this may cause confusion");
        }

        // Validate indexing settings
        var enableFullTextSearchStr = moduleSection["EnableFullTextSearch"];
        if (!string.IsNullOrEmpty(enableFullTextSearchStr) && bool.TryParse(enableFullTextSearchStr, out var enableFullTextSearch) && enableFullTextSearch)
        {
            logger.LogDebug("Full-text search indexing is enabled for exercises");
        }

        result.IsValid = !result.Errors.Any();
        
        if (result.IsValid)
        {
            logger.LogDebug("Exercises module configuration validation passed");
        }
        else
        {
            logger.LogWarning("Exercises module configuration validation failed with {ErrorCount} errors", result.Errors.Count);
        }

        return result;
    }
}
