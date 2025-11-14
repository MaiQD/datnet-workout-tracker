using System.Security.Claims;

namespace dotFitness.Common.API.Extensions;

/// <summary>
/// Extension methods for ClaimsPrincipal
/// </summary>
public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Extracts the user ID from claims and converts it to a Guid.
    /// </summary>
    /// <param name="principal">The claims principal</param>
    /// <returns>The user ID as a Guid, or null if not found or invalid</returns>
    public static Guid? GetUserId(this ClaimsPrincipal principal)
    {
        var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(userIdClaim))
            return null;
            
        if (Guid.TryParse(userIdClaim, out var userId))
            return userId;
            
        return null;
    }
    
    /// <summary>
    /// Extracts the user ID from claims and converts it to a Guid.
    /// Throws an exception if not found or invalid.
    /// </summary>
    /// <param name="principal">The claims principal</param>
    /// <returns>The user ID as a Guid</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown when user ID is not found or invalid</exception>
    public static Guid GetRequiredUserId(this ClaimsPrincipal principal)
    {
        var userId = principal.GetUserId();
        
        if (!userId.HasValue)
            throw new UnauthorizedAccessException("User ID not found in token or invalid format");
            
        return userId.Value;
    }
}

