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
    
    public List<string> Roles { get; set; } = new();
    
    public DateTime ExpiresAt { get; set; }
}
