using dotFitness.Common.Authorization;
using dotFitness.Common.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using dotFitness.Modules.Users.Application.Commands;
using dotFitness.Modules.Users.Application.DTOs;
using dotFitness.Modules.Users.Application.Services;
using dotFitness.Modules.Users.Domain.Entities;

namespace dotFitness.Modules.Users.Infrastructure.Handlers;

public class LoginWithGoogleCommandHandler : IRequestHandler<LoginWithGoogleCommand, Result<LoginResponseDto>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IGoogleAuthService _googleAuthService;
    private readonly ILogger<LoginWithGoogleCommandHandler> _logger;

    public LoginWithGoogleCommandHandler(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IGoogleAuthService googleAuthService,
        ILogger<LoginWithGoogleCommandHandler> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _googleAuthService = googleAuthService;
        _logger = logger;
    }

    public async Task<Result<LoginResponseDto>> Handle(LoginWithGoogleCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Verify Google token
            var googleUser = await _googleAuthService.GetUserInfoAsync(request.Request.GoogleToken, cancellationToken);
            if (googleUser == null)
            {
                return Result.Failure<LoginResponseDto>("Failed to get user information from Google");
            }

            // 2. Find or create user
            var user = await _userManager.FindByEmailAsync(googleUser.Email);
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

                var createResult = await _userManager.CreateAsync(user);
                if (!createResult.Succeeded)
                {
                    var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                    return Result.Failure<LoginResponseDto>($"Failed to create user: {errors}");
                }

                await _userManager.AddToRoleAsync(user, Roles.User);
            }

            // 3. Sign in and get tokens using Identity's token generation
            await _signInManager.SignInAsync(user, isPersistent: false);

            // 4. Generate access and refresh tokens
            var tokens = await GenerateTokensAsync(user);

            var response = new LoginResponseDto
            {
                UserId = user.Id,
                Email = user.Email!,
                DisplayName = user.DisplayName,
                AccessToken = tokens.AccessToken,
                RefreshToken = tokens.RefreshToken,
                ExpiresIn = tokens.ExpiresIn,
                Roles = (await _userManager.GetRolesAsync(user)).ToList(),
                ProfilePicture = user.ProfilePicture,
                Gender = user.Gender?.ToString(),
                DateOfBirth = user.DateOfBirth,
                UnitPreference = user.UnitPreference.ToString(),
                IsOnboarded = user.IsOnboarded
            };

            _logger.LogInformation("User {Email} logged in successfully", user.Email);
            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred during Google login");
            return Result.Failure<LoginResponseDto>($"Login failed: {ex.Message}");
        }
    }

    private async Task<(string AccessToken, string RefreshToken, int ExpiresIn)> GenerateTokensAsync(ApplicationUser user)
    {
        // Use Identity's built-in token generation
        var accessToken = await _userManager.GenerateUserTokenAsync(user, "Default", "access_token");
        var refreshToken = await _userManager.GenerateUserTokenAsync(user, "Default", "refresh_token");
        
        return (accessToken, refreshToken, 3600); // 1 hour
    }
}