namespace iRLeagueApiCore.Services.ResultService.Models;

internal sealed class StandingCalculationResult
{
    public long LeagueId { get; set; }
    public long SeasonId { get; set; }
    public long EventId { get; set; }
    public long? ChampSeasonId { get; set; }
    public long? StandingConfigId { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsTeamStanding { get; set; }
    public IList<StandingRowCalculationResult> StandingRows { get; set; } = new List<StandingRowCalculationResult>();
}