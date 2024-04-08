using iRLeagueApiCore.Common.Enums;

namespace iRLeagueApiCore.Services.ResultService.Models;

internal sealed class StandingCalculationConfiguration
{
    public long LeagueId { get; set; }
    public long SeasonId { get; set; }
    public long EventId { get; set; }
    public long? ChampSeasonId { get; set; }
    public long? StandingConfigId { get; set; }
    public IEnumerable<long> ResultConfigs { get; set; } = Array.Empty<long>();
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public int WeeksCounted { get; set; }
    public bool UseCombinedResult { get; set; }
    public ResultKind ResultKind { get; set; }
    public ICollection<SortOptions> SortOptions { get; set; } = new List<SortOptions>();
}
