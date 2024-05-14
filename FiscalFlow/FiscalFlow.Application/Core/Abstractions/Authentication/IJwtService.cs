using FiscalFlow.Contracts.Authentication;
using FiscalFlow.Domain.Entities;
using Google.Apis.Auth;
using System.Security.Claims;

namespace FiscalFlow.Application.Core.Abstractions.Authentication;

public interface IJwtService
{
    string CreateJwt(AppUser user);
    Task<GoogleJsonWebSignature.Payload> VerifyGoogleTokenAsync(ExternalAuthRequest externalAuthRequest);
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}