using dotFitness.Common.Authorization;
using dotFitness.Common.Results;
using dotFitness.Modules.Users.Application.DTOs;
using dotFitness.Modules.Users.Application.Services;
using dotFitness.Modules.Users.Application.Settings;
using dotFitness.Modules.Users.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace dotFitness.Modules.Users.Application.Commands;

public class LoginWithGoogleCommandHandler : IRequestHandler<LoginWithGoogleCommand, Result<LoginResponseDto>>
{
    private readonly IIdentityService _identityService;
    private readonly IGoogleAuthService _googleAuthService;
    private readonly AdminSettings _adminSettings;
    private readonly ILogger<LoginWithGoogleCommandHandler> _logger;

    public LoginWithGoogleCommandHandler(
        IIdentityService identityService,
        IGoogleAuthService googleAuthService,
        IOptions<AdminSettings> adminSettings,
        ILogger<LoginWithGoogleCommandHandler> logger)
    {
        _identityService = identityService;
        _googleAuthService = googleAuthService;
        _adminSettings = adminSettings.Value;
        _logger = logger;
    }

    public async Task<Result<LoginResponseDto>> Handle(LoginWithGoogleCommand request, CancellationToken cancellationToken)
    {
        // 1. Verify Google token
        var googleUser = await _googleAuthService.GetUserInfoAsync(request.Request.GoogleToken, cancellationToken);
        if (googleUser == null)
        {
            return Result.Failure<LoginResponseDto>("Failed to get user information from Google");
        }

        // 2. Find or create user
        var user = await _identityService.FindByEmailAsync(googleUser.Email, cancellationToken);
        if (user == null)
        {
            user = new ApplicationUser
            {
                UserName = googleUser.Email,
                Email = googleUser.Email,
                DisplayName = googleUser.Name,
                GoogleId = googleUser.Id,
                ProfilePicture = googleUser.ProfilePicture,
                EmailConfirmed = true
            };

            var createResult = await _identityService.CreateAsync(user, cancellationToken);
            if (createResult.IsFailure)
            {
                return Result.Failure<LoginResponseDto>(createResult.Error ?? "Failed to create user");
            }

            // Assign default User role
            var addRoleResult = await _identityService.AddToRoleAsync(user, Roles.User, cancellationToken);
            if (addRoleResult.IsFailure)
            {
                _logger.LogWarning("Failed to add User role to new user: {Error}", addRoleResult.Error);
            }
            
            // Check if user should be admin based on email whitelist (UM-008 requirement)
            if (_adminSettings.AdminEmails.Contains(googleUser.Email, StringComparer.OrdinalIgnoreCase))
            {
                var addAdminRoleResult = await _identityService.AddToRoleAsync(user, Roles.Admin, cancellationToken);
                if (addAdminRoleResult.IsSuccess)
                {
                    _logger.LogInformation("Admin role assigned to user: {Email}", googleUser.Email);
                }
            }
        }

        // 3. Sign in and get tokens using Identity's token generation
        await _identityService.SignInAsync(user, isPersistent: false, cancellationToken);

        // 4. Generate access and refresh tokens
        var tokens = await GenerateTokensAsync(user, cancellationToken);

        var rolesResult = await _identityService.GetRolesAsync(user, cancellationToken);
        var roles = rolesResult.IsSuccess ? rolesResult.Value!.ToList() : new List<string>();

        var response = new LoginResponseDto
        {
            UserId = user.Id,
            Email = user.Email!,
            DisplayName = user.DisplayName,
            AccessToken = tokens.AccessToken,
            RefreshToken = tokens.RefreshToken,
            ExpiresIn = tokens.ExpiresIn,
            Roles = roles,
            ProfilePicture = user.ProfilePicture,
            Gender = user.Gender?.ToString(),
            DateOfBirth = user.DateOfBirth,
            UnitPreference = user.UnitPreference.ToString(),
            IsOnboarded = user.IsOnboarded
        };

        _logger.LogInformation("User {Email} logged in successfully", user.Email);
        return Result.Success(response);
    }

    private async Task<(string AccessToken, string RefreshToken, int ExpiresIn)> GenerateTokensAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        // Use Identity's built-in token generation
        var accessToken = await _identityService.GenerateUserTokenAsync(user, "access_token", cancellationToken);
        var refreshToken = await _identityService.GenerateUserTokenAsync(user, "refresh_token", cancellationToken);
        
        return (accessToken, refreshToken, 3600); // 1 hour
    }
}

