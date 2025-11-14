using Microsoft.AspNetCore.Authorization;

namespace dotFitness.Api.Infrastructure.Authorization;

public class PTClientAccessRequirement : IAuthorizationRequirement
{
    public Guid TargetUserId { get; set; }
}
