using Riok.Mapperly.Abstractions;
using dotFitness.Modules.Users.Domain.Entities;
using dotFitness.Modules.Users.Application.DTOs;

namespace dotFitness.Modules.Users.Application.Mappers;

[Mapper]
public partial class UserMetricMapper
{
    [MapperIgnoreTarget(nameof(UserMetricDto.BmiCategory))]
    public partial UserMetricDto ToDto(UserMetric userMetric);
    
    // Custom mapping to include calculated fields
    private static string? MapBmiCategory(UserMetric userMetric) => userMetric.GetBmiCategory();
}
