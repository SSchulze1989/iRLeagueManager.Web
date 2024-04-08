namespace iRLeagueApiCore.Services.ResultService.Models;

internal sealed class EventCalculationConfiguration
{
    public long LeagueId { get; set; }
    public long EventId { get; set; }
    /// <summary>
    /// Id of existing result data (if result has been calculated before)
    /// </summary>
    public long? ResultId { get; set; }
    public long? ChampSeasonId { get; set; }
    public long? ResultConfigId { get; set; }
    public long? SourceResultConfigId { get; set; }

    public string DisplayName { get; set; } = string.Empty;
    public IEnumerable<SessionCalculationConfiguration> SessionResultConfigurations { get; set; } = Array.Empty<SessionCalculationConfiguration>();
}
