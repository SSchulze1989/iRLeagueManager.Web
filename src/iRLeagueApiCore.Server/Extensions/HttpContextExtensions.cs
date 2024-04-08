using iRLeagueApiCore.Server.Models;

namespace iRLeagueApiCore.Server.Extensions;

internal static class HttpContextExtensions
{
    public static LeagueUser GetLeagueUser(this HttpContext context)
    {
        var leagueName = (string?)context.GetRouteValue("leagueName") ?? string.Empty;
        return new LeagueUser(leagueName, context.User);
    }
}
