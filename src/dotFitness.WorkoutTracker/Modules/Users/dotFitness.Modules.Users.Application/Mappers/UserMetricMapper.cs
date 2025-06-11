using Riok.Mapperly.Abstractions;
using dotFitness.Modules.Users.Domain.Entities;
using dotFitness.Modules.Users.Application.DTOs;

namespace dotFitness.Modules.Users.Application.Mappers;

[Mapper]
public partial class UserMetricMapper
{
    [MapProperty(nameof(@MapBmiCategory), nameof(UserMetricDto.BmiCategory))]
    public partial UserMetricDto ToDto(UserMetric userMetric);
    
    private string MapBmiCategory(UserMetric userMetric) => userMetric.GetBmiCategory();
}
