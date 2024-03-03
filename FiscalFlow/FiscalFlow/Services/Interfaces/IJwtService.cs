using FiscalFlow.Dto.Request;
using FiscalFlow.Model;
using Google.Apis.Auth;

namespace FiscalFlow.Services.Interfaces;
public interface IJwtService
{
    string CreateJwt(AppUser user);
    Task<GoogleJsonWebSignature.Payload> VerifyGoogleTokenAsync(ExternalAuthRequest externalAuthRequest);
}
