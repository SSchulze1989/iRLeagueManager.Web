using iRLeagueApiCore.Common.Enums;
using iRLeagueApiCore.Services.ResultService.Calculation;

namespace iRLeagueApiCore.Services.ResultService.Models;

internal sealed class SessionCalculationConfiguration
{
    public long LeagueId { get; set; }
    public long? SessionId { get; set; }
    public int? SessionNr { get; set; }
    public long? ScoringId { get; set; }
    /// <summary>
    /// Id of existing session result data (if result has been calculated before)
    /// </summary>
    public long? SessionResultId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int MaxResultsPerGroup { get; set; }
    public bool UseResultSetTeam { get; set; }
    public bool UpdateTeamOnRecalculation { get; set; }
    public ResultKind ResultKind { get; set; }
    public bool IsCombinedResult { get; set; }
    public SessionType SessionType { get; set; }
    /// <summary>
    /// Use the points coming from a selected external source in a combined result configuration
    /// </summary>
    public bool UseExternalSourcePoints { get; set; }

    public PointRule<ResultRowCalculationResult> PointRule { get; set; } = PointRule<ResultRowCalculationResult>.Default();
}
