using System.ComponentModel.DataAnnotations;

namespace dotFitness.Modules.Users.Application.DTOs;

public class LoginResponseDto
{
    [Required]
    public string Token { get; set; } = string.Empty;
    
    [Required]
    public string UserId { get; set; } = string.Empty;
    
    [Required]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    public string DisplayName { get; set; } = string.Empty;
    
    public string? ProfilePicture { get; set; }
    
    public List<string> Roles { get; set; } = new();
    
    public DateTime ExpiresAt { get; set; }
    
    public string? Gender { get; set; }
    
    public DateTime? DateOfBirth { get; set; }
    
    public string UnitPreference { get; set; } = "Metric";
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime UpdatedAt { get; set; }
}
