using Ardalis.Result;
using FiscalFlow.Application.Core.Abstractions.Authentication;
using FiscalFlow.Application.Core.Abstractions.Emails;
using FiscalFlow.Contracts.Authentication;
using FiscalFlow.Contracts.Emails;
using FiscalFlow.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace FiscalFlow.Application.Services;

public class UserService : IUserService
{
    private readonly IConfiguration _config;
    private readonly IEmailService _emailService;
    private readonly IJwtService _jwtService;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly UserManager<AppUser> _userManager;

    public UserService(IJwtService jwtService,
        SignInManager<AppUser> signInManager,
        UserManager<AppUser> userManager,
        IEmailService emailService,
        IConfiguration config)
    {
        _jwtService = jwtService;
        _signInManager = signInManager;
        _userManager = userManager;
        _emailService = emailService;
        _config = config;
    }

    public async Task<AppUser?> FindByEmailAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email);
    }

    public async Task<SignInResult> CheckPasswordSignInAsync(AppUser user, string password, bool lockoutOnFailure)
    {
        return await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure);
    }

    public async Task<IdentityResult> ConfirmEmailAsync(AppUser user, string token)
    {
        var decodedTokenBytes = WebEncoders.Base64UrlDecode(token);
        var decodedToken = Encoding.UTF8.GetString(decodedTokenBytes);

        var result = await _userManager.ConfirmEmailAsync(user, decodedToken);
        return result;
    }

    public async Task<IdentityResult> ResetPasswordAsync(AppUser user, ResetPasswordRequest resetPasswordRequest)
    {
        var decodedTokenBytes = WebEncoders.Base64UrlDecode(resetPasswordRequest.ResetCode);
        var decodedToken = Encoding.UTF8.GetString(decodedTokenBytes);

        var result =
            await _userManager.ResetPasswordAsync(user, decodedToken, resetPasswordRequest.NewPassword);

        return result;
    }

    public async Task<bool> CheckEmailExistsAlready(string email)
    {
        return await _userManager.Users.AnyAsync(user => user.Email!.Equals(email));
    }

    public async Task<bool> SendConfirmEmailAsync(AppUser user)
    {
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
        var url = $"{_config["JWT:Audience"]}/{_config["Email:ConfirmEmailPath"]}?token={token}&email={user.Email}";

        var body = $"""
                    <p>Hello: {user.FirstName} {user.LastName}</p>
                    <p>Please confirm your email address by clicking on the following link.</p>
                    <p><a href="{url}">Click here</a></p>
                    <p>Thank you,</p>
                    <br>{_config["Email:ApplicationName"]}
                    """;

        var emailSend = new MailRequest(user.Email!, "Confirm your account", body);

        return await _emailService.SendEmailAsync(emailSend);
    }

    public async Task<bool> SendForgotPasswordEmailAsync(AppUser user)
    {
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
        var url = $"{_config["JWT:Audience"]}/{_config["Email:ResetPasswordPath"]}?token={token}&email={user.Email}";

        var body = $"""
                    <p>Hello: {user.FirstName} {user.LastName}</p>
                    <p>Please change your password by clicking on the following link.</p>
                    <p><a href="{url}">Click here</a></p>
                    <p>Thank you,</p>
                    <br>{_config["Email:ApplicationName"]}
                    """;

        var emailSend = new MailRequest(user.Email!, "Reset your password", body);

        return await _emailService.SendEmailAsync(emailSend);
    }

    public bool CheckUserExists(string accountId)
    {
        return _userManager.FindByIdAsync(accountId).Result != null;
    }

    public async Task<Result<string>> GetSavedRefreshToken(string? email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return Result.NotFound($"User with email {email} does not exist!");
        }
        return user.RefreshToken;
    }
}