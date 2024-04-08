using iRLeagueDatabaseCore;

namespace iRLeagueApiCore.Server.Models;

public class RequestLeagueProvider : ILeagueProvider
{
    public long LeagueId { get; private set; } = 0;

    public bool HasLeagueName => string.IsNullOrEmpty(LeagueName) == false;

    public string LeagueName { get; private set; } = string.Empty;

    public void SetLeague(long leagueId, string? leagueName = null)
    {
        LeagueId = leagueId;
        LeagueName = leagueName ?? string.Empty;
    }
}
