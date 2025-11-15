namespace dotFitness.Modules.Users.Application.Settings;

/// <summary>
/// Administrator configuration settings for user role assignment
/// </summary>
public class AdminSettings
{
    public static string AdminSettingsSection => "Users:AdminSettings"; 
    /// <summary>
    /// List of email addresses that should automatically receive Admin role
    /// </summary>
    public List<string> AdminEmails { get; set; } = [];
}
