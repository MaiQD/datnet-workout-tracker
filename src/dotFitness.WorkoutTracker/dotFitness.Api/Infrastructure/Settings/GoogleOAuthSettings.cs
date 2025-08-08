namespace dotFitness.Api.Infrastructure.Settings;

/// <summary>
/// Google OAuth configuration settings for Swagger integration
/// </summary>
public class GoogleOAuthSettings
{
    /// <summary>
    /// Google OAuth Client ID
    /// </summary>
    public string ClientId { get; set; } = string.Empty;
    
    /// <summary>
    /// Google OAuth Client Secret
    /// </summary>
    public string ClientSecret { get; set; } = string.Empty;
    
    /// <summary>
    /// OAuth redirect URI for Swagger
    /// </summary>
    public string RedirectUri { get; set; } = string.Empty;
} 