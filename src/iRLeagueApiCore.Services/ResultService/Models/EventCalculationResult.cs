namespace iRLeagueApiCore.Services.ResultService.Models;

internal sealed class EventCalculationResult
{
    public EventCalculationResult() { }

    public EventCalculationResult(EventCalculationData data)
    {
        LeagueId = data.LeagueId;
        EventId = data.EventId;
    }

    public long LeagueId { get; set; }
    public long EventId { get; set; }
    public long? ResultId { get; set; }
    public long? ChampSeasonId { get; set; }
    public long? ResultConfigId { get; set; }
    public string Name { get; set; } = string.Empty;
    public IEnumerable<SessionCalculationResult> SessionResults { get; set; } = Array.Empty<SessionCalculationResult>();
}
