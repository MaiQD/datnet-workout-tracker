using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace dotFitness.SharedKernel.Configuration;

/// <summary>
/// Interface for module-specific configuration validators
/// </summary>
public interface IModuleConfigurationValidator
{
    /// <summary>
    /// The name of the module this validator handles
    /// </summary>
    string ModuleName { get; }

    /// <summary>
    /// Validates the module's configuration section
    /// </summary>
    /// <param name="moduleSection">The module configuration section</param>
    /// <param name="logger">Logger for validation results</param>
    /// <returns>Module validation result</returns>
    ModuleValidationResult ValidateConfiguration(IConfigurationSection moduleSection, ILogger logger);
}
