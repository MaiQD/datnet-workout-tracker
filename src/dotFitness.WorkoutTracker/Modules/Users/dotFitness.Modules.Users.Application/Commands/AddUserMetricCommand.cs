using MediatR;
using dotFitness.Modules.Users.Application.DTOs;
using dotFitness.SharedKernel.Results;
using System.ComponentModel.DataAnnotations;

namespace dotFitness.Modules.Users.Application.Commands;

public class AddUserMetricCommand : IRequest<Result<UserMetricDto>>
{
    [Required]
    public string UserId { get; set; } = string.Empty;
    
    [Required]
    public DateTime Date { get; set; }
    
    [Range(0, 1000, ErrorMessage = "Weight must be between 0 and 1000")]
    public double? Weight { get; set; }
    
    [Range(0, 300, ErrorMessage = "Height must be between 0 and 300")]
    public double? Height { get; set; }
    
    [StringLength(500)]
    public string? Notes { get; set; }
}
