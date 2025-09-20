using System.ComponentModel.DataAnnotations;

namespace dotFitness.Modules.Users.Application.DTOs;

public class UserDto
{
    
    [Required]
    public int Id { get; set; }
    
    [Required]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    public string DisplayName { get; set; } = string.Empty;
    
    public string? ProfilePicture { get; set; }
    
    public string? GoogleId { get; set; }
    
    public string LoginMethod { get; set; } = string.Empty;
    
    public List<string> Roles { get; set; } = [];
    
    public string? Gender { get; set; }
    
    public DateTime? DateOfBirth { get; set; }
    
    public string UnitPreference { get; set; } = "Metric";
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime UpdatedAt { get; set; }
}
