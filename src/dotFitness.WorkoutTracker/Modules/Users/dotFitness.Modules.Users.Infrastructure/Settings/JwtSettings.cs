namespace dotFitness.Modules.Users.Infrastructure.Settings;

/// <summary>
/// JWT configuration settings for token generation and validation
/// </summary>
public class JwtSettings
{
    /// <summary>
    /// The secret key used for signing JWT tokens
    /// </summary>
    public string SecretKey { get; set; } = string.Empty;
    
    /// <summary>
    /// The issuer of the JWT token
    /// </summary>
    public string Issuer { get; set; } = string.Empty;
    
    /// <summary>
    /// The intended audience of the JWT token
    /// </summary>
    public string Audience { get; set; } = string.Empty;
    
    /// <summary>
    /// Token expiration time in hours (default: 24 hours)
    /// </summary>
    public int ExpirationHours { get; set; } = 24;
}
