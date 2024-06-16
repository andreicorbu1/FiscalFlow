using FiscalFlow.Application.Core.Abstractions.Authentication;
using FiscalFlow.Contracts.Authentication;
using FiscalFlow.Domain.Entities;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System.Security.Claims;
using System.Security.Cryptography;
using RegisterRequest = FiscalFlow.Contracts.Authentication.RegisterRequest;
using ResetPasswordRequest = FiscalFlow.Contracts.Authentication.ResetPasswordRequest;

namespace FiscalFlow.API.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _accountService;
    private readonly IJwtService _jwtService;
    private readonly UserManager<AppUser> _userManager;

    public UserController(IJwtService jwtService,
        UserManager<AppUser> userManager,
        IUserService accountService)
    {
        _jwtService = jwtService;
        _userManager = userManager;
        _accountService = accountService;
    }

    [HttpPut("revoke-refresh-token")]
    [Authorize]
    public async Task<IActionResult> RevokeTokenAsync()
    {
        var userId = Utils.Utils.ExtractUserIdFromClaims(User);
        var user = await _userManager.FindByIdAsync(userId);
        user!.RefreshToken = null;
        await _userManager.UpdateAsync(user);
        return Ok();
    }

    [HttpPost("refresh-user-token")]
    public async Task<IActionResult> RefreshTokenAsync(string refreshToken)
    {
        Request.Headers.TryGetValue("Authorization", out StringValues headerValues);
        string? token = headerValues.FirstOrDefault()?.Substring(7);
        if (string.IsNullOrEmpty(token))
        {
            return Unauthorized("Please login again!");
        }
        var principal = _jwtService.GetPrincipalFromExpiredToken(token);
        var email = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)!.Value;
        var savedRefreshToken = await _accountService.GetSavedRefreshToken(email);
        if (!savedRefreshToken.IsSuccess)
        {
            return Unauthorized("Please login again!");
        }
        if (savedRefreshToken != refreshToken)
        {
            return Unauthorized("Please login again!");
        }
        var user = await _userManager.FindByEmailAsync(email!);
        var newJwtToken = _jwtService.CreateJwt(user!);
        var newRefreshToken = GenerateRefreshToken();
        user!.RefreshToken = newRefreshToken;
        await _userManager.UpdateAsync(user);
        return Ok(new TokenResponse(newJwtToken, newRefreshToken, user.FirstName!, user.LastName!));
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync(LoginRequest loginRequest)
    {
        var user = await _accountService.FindByEmailAsync(loginRequest.Email);
        if (user == null) return Unauthorized("Invalid username!");

        if (!user.EmailConfirmed) return Unauthorized("Please confirm your email!");

        var result = await _accountService.CheckPasswordSignInAsync(user, loginRequest.Password, true);

        if (!result.Succeeded) return Unauthorized("Invalid password!");

        var token = _jwtService.CreateJwt(user);
        var refreshToken = GenerateRefreshToken();
        user.RefreshToken = refreshToken;
        await _userManager.UpdateAsync(user);
        return Ok(new TokenResponse(token, refreshToken, user.FirstName!, user.LastName!));
    }

    [HttpPut("confirm-email")]
    public async Task<IActionResult> ConfirmEmailAsync(ConfirmEmailRequest confirmEmailRequest)
    {
        var user = await _accountService.FindByEmailAsync(confirmEmailRequest.Email!);
        if (user == null) return Unauthorized("This email has not been registered yet!");
        if (user.EmailConfirmed) return BadRequest("Your email has already been confirmed!");

        try
        {
            var result = await _accountService.ConfirmEmailAsync(user, confirmEmailRequest.Token!);
            if (result.Succeeded)
                return Ok(new
                {
                    title = "Email confirmed",
                    message = "Your email address has been confirmed! You can login now."
                });

            return BadRequest("Invalid token. Please try again!");
        }
        catch (Exception)
        {
            return BadRequest("Invalid token. Please try again!");
        }
    }

    [HttpPost("resend-email-confirmation-link/{email}")]
    public async Task<IActionResult> ResendEmailConfirmationAsync(string email)
    {
        if (string.IsNullOrEmpty(email)) return BadRequest("Invalid email");
        var user = await _accountService.FindByEmailAsync(email);

        if (user == null) return Unauthorized("This email has not been registered yet!");
        if (user.EmailConfirmed) return BadRequest("Your email has already been confirmed!");

        try
        {
            if (await _accountService.SendConfirmEmailAsync(user))
                return Ok(new
                {
                    title = "Confirmation link sent",
                    message = "New confirmation link has been sent. Please check your inbox."
                });
            return BadRequest("Failed to send email.Please contact admin!");
        }
        catch (Exception)
        {
            return BadRequest("Failed to send email. Please contact admin!");
        }
    }

    [HttpPost("forgot-password/{email}")]
    public async Task<IActionResult> ForgotPasswordAsync(string email)
    {
        if (string.IsNullOrEmpty(email)) return BadRequest("Invalid email");
        var user = await _accountService.FindByEmailAsync(email);

        if (user == null) return Unauthorized("This email has not been registered yet!");
        if (!user.EmailConfirmed) return BadRequest("Please confirm your email first!");
        try
        {
            if (await _accountService.SendForgotPasswordEmailAsync(user))
                return Ok(new
                {
                    title = "Forgot password email sent",
                    message = "Please check your email inbox."
                });

            return BadRequest("Failed to send email. Please contact admin!");
        }
        catch (Exception)
        {
            return BadRequest("Failed to send email. Please contact admin!");
        }
    }

    [HttpPut("reset-password")]
    public async Task<IActionResult> ResetPasswordAsync(ResetPasswordRequest resetPasswordRequest)
    {
        var user = await _accountService.FindByEmailAsync(resetPasswordRequest.Email);
        if (user is null) return Unauthorized("This email address has not been registered yet!");
        if (!user.EmailConfirmed) return BadRequest("Please confirm your email first!");
        try
        {
            var result = await _accountService.ResetPasswordAsync(user, resetPasswordRequest);
            if (!result.Succeeded)
                return BadRequest("Invalid token! Please try again!");
            return Ok(new
            {
                title = "Password reset success",
                message = "Your password has been reset"
            });
        }
        catch (Exception)
        {
            return BadRequest("Invalid token! Please try again!");
        }
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync(RegisterRequest registerRequest)
    {
        if (await _accountService.CheckEmailExistsAlready(registerRequest.Email!))
            return BadRequest(
                $"An existing account with the email {registerRequest.Email} already exists! Please try another one!");

        var userToAdd = new AppUser
        {
            UserName = registerRequest.Email!.ToLower(),
            FirstName = registerRequest.FirstName,
            LastName = registerRequest.LastName,
            Email = registerRequest.Email.ToLower(),
            EmailConfirmed = false
        };

        var result = await _userManager.CreateAsync(userToAdd, registerRequest.Password!);

        if (!result.Succeeded) return BadRequest(result.Errors);

        try
        {
            if (await _accountService.SendConfirmEmailAsync(userToAdd)) return Created();

            return BadRequest("Failed to send email. Please contact admin!");
        }
        catch (Exception)
        {
            return BadRequest("Failed to send email. Please contact admin!");
        }
    }

    [HttpPost("external-login")]
    public async Task<IActionResult> ExternalLoginAsync([FromBody] ExternalAuthRequest externalAuth)
    {
        UserLoginInfo info;
        dynamic payload = null;
        if (externalAuth.Provider == "Facebook")
        {
            info = new UserLoginInfo(externalAuth.Provider, externalAuth.IdToken, externalAuth.Provider);
            payload = externalAuth;
        }
        else
        {
            payload = await _jwtService.VerifyGoogleTokenAsync(externalAuth);
            info = new UserLoginInfo(externalAuth.Provider!, payload!.Subject, externalAuth.Provider);
        }
        var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
        if (user == null)
        {
            user = await _userManager.FindByEmailAsync(payload.Email);
            if (user == null)
            {
                if (payload is GoogleJsonWebSignature.Payload)
                {
                    user = new AppUser
                    {
                        Email = payload.Email,
                        UserName = payload.Email,
                        FirstName = payload.GivenName ?? string.Empty,
                        LastName = payload.FamilyName ?? string.Empty,
                        EmailConfirmed = true
                    };
                }
                else
                {
                    user = new AppUser
                    {
                        Email = payload.Email,
                        UserName = payload.Email,
                        FirstName = payload.FirstName ?? string.Empty,
                        LastName = payload.LastName ?? string.Empty,
                        EmailConfirmed = true
                    };
                }
                await _userManager.CreateAsync(user);
            }

            await _userManager.AddLoginAsync(user, info);
        }

        var token = _jwtService.CreateJwt(user);
        var refreshToken = GenerateRefreshToken();
        user.RefreshToken = refreshToken;
        await _userManager.UpdateAsync(user);
        return Ok(new TokenResponse(token, refreshToken, user.FirstName!, user.LastName!));
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}