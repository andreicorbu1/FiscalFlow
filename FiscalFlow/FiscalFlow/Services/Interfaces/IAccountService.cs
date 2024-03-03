using FiscalFlow.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;

namespace FiscalFlow.Services.Interfaces;

public interface IAccountService
{
    Task<AppUser?> FindByEmailAsync(string email);
    Task<SignInResult> CheckPasswordSignInAsync(AppUser user, string password, bool lockoutOnFailure);
    Task<IdentityResult> ConfirmEmailAsync(AppUser user, string token);
    Task<IdentityResult> ResetPasswordAsync(AppUser user, ResetPasswordRequest resetPasswordRequest);
    Task<bool> CheckEmailExistsAlready(string email);
    Task<bool> SendConfirmEmailAsync(AppUser user);
    Task<bool> SendForgotPasswordEmailAsync(AppUser user);
}