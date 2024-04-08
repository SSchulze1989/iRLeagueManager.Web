using System.Security.Claims;

namespace iRLeagueApiCore.Server.Extensions;

public static class PrincipalExtensions
{
    public static string? GetUserId(this ClaimsPrincipal principal)
    {
        return principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }
}
