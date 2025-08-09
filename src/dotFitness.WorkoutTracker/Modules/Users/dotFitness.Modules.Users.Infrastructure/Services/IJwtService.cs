using dotFitness.Modules.Users.Domain.Entities;

namespace dotFitness.Modules.Users.Infrastructure.Services;

public interface IJwtService
{
    string GenerateToken(User user);
    DateTime GetExpirationTime();
}
