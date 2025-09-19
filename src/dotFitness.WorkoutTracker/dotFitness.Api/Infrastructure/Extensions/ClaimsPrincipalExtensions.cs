using System.Security.Claims;

namespace dotFitness.Api.Infrastructure.Extensions;

public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Extracts the user ID from claims and converts it to an integer.
    /// </summary>
    /// <param name="principal">The claims principal</param>
    /// <returns>The user ID as an integer, or null if not found or invalid</returns>
    public static int? GetUserId(this ClaimsPrincipal principal)
    {
        var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(userIdClaim))
            return null;
            
        if (int.TryParse(userIdClaim, out var userId))
            return userId;
            
        return null;
    }
    
    /// <summary>
    /// Extracts the user ID from claims and converts it to an integer.
    /// Throws an exception if not found or invalid.
    /// </summary>
    /// <param name="principal">The claims principal</param>
    /// <returns>The user ID as an integer</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown when user ID is not found or invalid</exception>
    public static int GetRequiredUserId(this ClaimsPrincipal principal)
    {
        var userId = principal.GetUserId();
        
        if (!userId.HasValue)
            throw new UnauthorizedAccessException("User ID not found in token or invalid format");
            
        return userId.Value;
    }
}
