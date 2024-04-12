using System.Security.Claims;
using Ardalis.Result;

namespace FiscalFlow.API.Utils;

internal static class Utils
{
    internal static Result<string> ExtractUserIdFromClaims(ClaimsPrincipal user)
    {
        var ownerId = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        if (ownerId is null) return Result.Unauthorized();

        return Result.Success(ownerId.Value);
    }
}