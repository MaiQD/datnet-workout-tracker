using System.ComponentModel.DataAnnotations;

namespace dotFitness.Modules.Users.Application.DTOs;

/// <summary>
/// Request DTO for Google OAuth login
/// </summary>
public class LoginWithGoogleRequest
{
    /// <summary>
    /// The Google OAuth access token
    /// </summary>
    [Required]
    public string GoogleToken { get; set; } = string.Empty;
}
