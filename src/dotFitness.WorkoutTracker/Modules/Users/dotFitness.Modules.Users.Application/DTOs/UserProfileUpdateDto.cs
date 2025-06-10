using System.ComponentModel.DataAnnotations;

namespace dotFitness.Modules.Users.Application.DTOs;

public class UserProfileUpdateDto
{
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string DisplayName { get; set; } = string.Empty;
    
    [StringLength(10)]
    public string? Gender { get; set; }
    
    public DateTime? DateOfBirth { get; set; }
    
    [Required]
    [StringLength(20)]
    public string UnitPreference { get; set; } = "Metric";
}
