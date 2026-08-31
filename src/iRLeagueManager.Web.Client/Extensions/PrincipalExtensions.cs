using System.Security.Claims;

namespace iRLeagueManager.Web.Extensions;

public static class PrincipalExtensions
{
    public static string? GetUserId(this ClaimsPrincipal principal)
    {
        return principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }
}
