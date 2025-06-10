using Riok.Mapperly.Abstractions;
using dotFitness.Modules.Users.Domain.Entities;
using dotFitness.Modules.Users.Application.DTOs;

namespace dotFitness.Modules.Users.Application.Mappers;

[Mapper]
public partial class UserMetricMapper
{
    public partial UserMetricDto ToDto(UserMetric userMetric);
    
    // Custom mapping to include calculated fields
    private string? MapBmiCategory(UserMetric userMetric) => userMetric.GetBmiCategory();
}
