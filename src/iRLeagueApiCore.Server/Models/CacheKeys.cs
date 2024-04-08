namespace iRLeagueApiCore.Server.Models;

internal static class CacheKeys
{
    public static string GetLeagueNameKey(string leagueName) => $"leagueName_{leagueName}";
}
