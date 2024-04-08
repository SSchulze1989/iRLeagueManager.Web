namespace iRLeagueApiCore.Common.Models.Standings;

public class StandingsModel
{
    public long LeagueId { get; set; }
    public long SeasonId { get; set; }
    public long StandingId { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsTeamStanding { get; set; }
    public IEnumerable<StandingRowModel> StandingRows { get; set; } = Array.Empty<StandingRowModel>();
}
