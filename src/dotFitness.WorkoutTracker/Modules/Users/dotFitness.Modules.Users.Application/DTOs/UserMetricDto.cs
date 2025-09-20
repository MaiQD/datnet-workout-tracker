using System.ComponentModel.DataAnnotations;

namespace dotFitness.Modules.Users.Application.DTOs;

public class UserMetricDto
{
    [Required]
    public int Id { get; set; }
    
    [Required]
    public int UserId { get; set; }
    
    [Required]
    public DateTime Date { get; set; }
    
    [Range(0, 1000)]
    public double? Weight { get; set; }
    
    [Range(0, 300)]
    public double? Height { get; set; }
    
    public double? Bmi { get; set; }
    
    public string? BmiCategory { get; set; }
    
    [StringLength(500)]
    public string? Notes { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime UpdatedAt { get; set; }
}
