using MediatR;
using dotFitness.Modules.Users.Application.DTOs;
using System.ComponentModel.DataAnnotations;
using dotFitness.Common.Results;

namespace dotFitness.Modules.Users.Application.Commands;

public record AddUserMetricCommand : IRequest<Result<UserMetricDto>>
{
    public AddUserMetricCommand()
    {
    }

    public AddUserMetricCommand(Guid UserId, DateTime Date, double? Weight, double? Height, string? Notes)
    {
        this.UserId = UserId;
        this.Date = Date;
        this.Weight = Weight;
        this.Height = Height;
        this.Notes = Notes;
    }

    [Required] public Guid UserId { get; set; }

    [Required] public DateTime Date { get; init; }

    [Range(0, 1000, ErrorMessage = "Weight must be between 0 and 1000")]
    public double? Weight { get; init; }

    [Range(0, 300, ErrorMessage = "Height must be between 0 and 300")]
    public double? Height { get; init; }

    [StringLength(500)] public string? Notes { get; init; }
}