using System.ComponentModel.DataAnnotations;

namespace dotFitness.Modules.Users.Application.DTOs;

public class LoginResponseDto
{
    [Required]
    public Guid UserId { get; set; }
    
    [Required]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    public string DisplayName { get; set; } = string.Empty;
    
    [Required]
    public string AccessToken { get; set; } = string.Empty;
    
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
    
    public int ExpiresIn { get; set; } = 3600; // 1 hour in seconds
    
    public List<string> Roles { get; set; } = [];
    
    public string? ProfilePicture { get; set; }
    
    public string? Gender { get; set; }
    
    public DateTime? DateOfBirth { get; set; }
    
    public string UnitPreference { get; set; } = "Metric";
    
    public bool IsOnboarded { get; set; }
}
