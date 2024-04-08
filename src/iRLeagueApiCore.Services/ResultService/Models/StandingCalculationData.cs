namespace iRLeagueApiCore.Services.ResultService.Models;

internal sealed class StandingCalculationData
{
    public long LeagueId { get; set; }
    public long SeasonId { get; set; }
    public long EventId { get; set; }
    public IList<EventCalculationResult> PreviousEventResults { get; set; } = new List<EventCalculationResult>();
    public EventCalculationResult CurrentEventResult { get; set; } = default!;
}
