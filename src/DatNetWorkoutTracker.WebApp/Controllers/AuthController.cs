using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using DatNetWorkoutTracker.Users.Services;

namespace DatNetWorkoutTracker.WebApp.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IGoogleAuthService _googleAuthService;

    public AuthController(IUserService userService, IGoogleAuthService googleAuthService)
    {
        _userService = userService;
        _googleAuthService = googleAuthService;
    }

    [HttpGet("google")]
    public IActionResult GoogleLogin()
    {
        var properties = new AuthenticationProperties
        {
            RedirectUri = Url.Action(nameof(GoogleCallback))
        };
        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }

    [HttpGet("google-callback")]
    public async Task<IActionResult> GoogleCallback()
    {
        var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
        
        if (!result.Succeeded)
        {
            return Redirect("/login?error=auth_failed");
        }

        var googleUserInfo = await _googleAuthService.ProcessGoogleCallbackAsync(result.Principal!);
        if (googleUserInfo == null)
        {
            return Redirect("/login?error=invalid_user_info");
        }

        // Check if user exists, if not create new user
        var existingUser = await _userService.GetUserByGoogleIdAsync(googleUserInfo.GoogleId);
        if (existingUser == null)
        {
            existingUser = await _userService.CreateUserFromGoogleAsync(googleUserInfo);
        }
        else
        {
            await _userService.UpdateLastLoginAsync(existingUser.Id);
        }

        // Create claims for the user session
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, existingUser.Id),
            new Claim(ClaimTypes.Name, $"{existingUser.FirstName} {existingUser.LastName}"),
            new Claim(ClaimTypes.Email, existingUser.Email),
            new Claim("picture", existingUser.ProfilePictureUrl ?? ""),
            new Claim("google_id", existingUser.GoogleId)
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddDays(30)
        };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);

        return Redirect("/");
    }

    [HttpGet("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Redirect("/login");
    }

    [HttpGet("user")]
    [Authorize]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var user = await _userService.GetUserByIdAsync(userId);
        if (user == null)
        {
            return NotFound();
        }

        return Ok(new
        {
            id = user.Id,
            email = user.Email,
            firstName = user.FirstName,
            lastName = user.LastName,
            profilePictureUrl = user.ProfilePictureUrl,
            preferences = user.Preferences
        });
    }
}
