using Microsoft.AspNetCore.Authorization;

namespace dotFitness.Api.Infrastructure.Authorization;

public class SelfOrAdminRequirement : IAuthorizationRequirement
{
    public Guid? TargetUserId { get; set; }
}
