namespace iRLeagueApiCore.Services.ResultService.Models;

internal sealed class EventCalculationData
{
    public long LeagueId { get; set; }
    public long EventId { get; set; }
    public IEnumerable<AddPenaltyCalculationData> AddPenalties { get; set; } = Array.Empty<AddPenaltyCalculationData>();
    public IEnumerable<SessionCalculationData> SessionResults { get; set; } = Array.Empty<SessionCalculationData>();
}
