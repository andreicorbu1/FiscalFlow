using FiscalFlow.Dto.Dto;
using FiscalFlow.Model;
using FiscalFlow.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using FiscalFlow.Dto.Request;
using Mailjet.Client.Resources;
using Microsoft.EntityFrameworkCore;
using RegisterRequest = FiscalFlow.Dto.Request.RegisterRequest;

namespace FiscalFlow.Services;

public class AccountService : IAccountService
{
    private readonly IJwtService _jwtService;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly UserManager<AppUser> _userManager;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _config;

    public AccountService(IJwtService jwtService, 
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
        return await _userManager.Users.AnyAsync(user => user.Email == email.ToLower());
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

        var emailSend = new EmailSendDto(user.Email!, "Confirm your account", body);

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

        var emailSend = new EmailSendDto(user.Email!, "Reset your password", body);

        return await _emailService.SendEmailAsync(emailSend);
    }
}