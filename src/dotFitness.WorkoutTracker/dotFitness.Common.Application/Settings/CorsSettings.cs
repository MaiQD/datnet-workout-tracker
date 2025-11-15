namespace dotFitness.Common.Application.Settings;

public class CorsSettings
{
    public static string CorsSettingsSection => nameof(CorsSettings);
    public string[] AllowedOrigins { get; set; } = [];
}
