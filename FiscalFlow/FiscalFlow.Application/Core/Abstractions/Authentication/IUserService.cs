using FiscalFlow.Contracts.Authentication;
using FiscalFlow.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace FiscalFlow.Application.Core.Abstractions.Authentication;

public interface IUserService
{
    Task<AppUser?> FindByEmailAsync(string email);
    Task<SignInResult> CheckPasswordSignInAsync(AppUser user, string password, bool lockoutOnFailure);
    Task<IdentityResult> ConfirmEmailAsync(AppUser user, string token);
    Task<IdentityResult> ResetPasswordAsync(AppUser user, ResetPasswordRequest resetPasswordRequest);
    Task<bool> CheckEmailExistsAlready(string email);
    Task<bool> SendConfirmEmailAsync(AppUser user);
    Task<bool> SendForgotPasswordEmailAsync(AppUser user);
    bool CheckUserExists(string accountId);
}