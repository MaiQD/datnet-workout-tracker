using System.Text.Json;

namespace dotFitness.SharedKernel.Configuration;

/// <summary>
/// Result of module configuration validation
/// </summary>
public class ModuleConfigurationValidationResult
{
    public ModuleConfigurationValidationResult()
    {
        ModuleValidations = new Dictionary<string, ModuleValidationResult>();
        GlobalErrors = [];
        GlobalWarnings = [];
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
        Errors = [];
        Warnings = [];
    }

    public string ModuleName { get; set; } = string.Empty;
    public List<string> Errors { get; set; }
    public List<string> Warnings { get; set; }
    public bool IsValid { get; set; }

    public void AddError(string error) => Errors.Add(error);
    public void AddWarning(string warning) => Warnings.Add(warning);
}
