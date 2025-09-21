using Riok.Mapperly.Abstractions;
using dotFitness.Modules.Users.Domain.Entities;
using dotFitness.Modules.Users.Application.DTOs;

namespace dotFitness.Modules.Users.Application.Mappers;

[Mapper]
public static partial class UserMetricMapper
{
    [MapProperty(nameof(@MapBmiCategory), nameof(UserMetricDto.BmiCategory))]
    public static partial UserMetricDto ToDto(UserMetric userMetric);
    public static partial IEnumerable<UserMetricDto> ToDto(IEnumerable<UserMetric> userMetrics);
    
    private static string MapBmiCategory(UserMetric userMetric) => userMetric.GetBmiCategory();
}
