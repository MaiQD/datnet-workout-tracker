using Microsoft.AspNetCore.Authorization;

namespace dotFitness.Api.Infrastructure.Authorization;

public class ResourceOwnerRequirement : IAuthorizationRequirement
{
    public Guid? OwnerUserId { get; set; }
}
