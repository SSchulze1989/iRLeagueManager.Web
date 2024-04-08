namespace iRLeagueApiCore.Common.Models;

/// <summary>
/// Schema for creating a new scoring
/// </summary>
[DataContract]
public class PostScoringModel
{
    /// <summary>
    /// Kind of scoring
    /// </summary>
    [DataMember]
    [EnumDataType(typeof(ResultKind))]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ResultKind ScoringKind { get; set; }
    /// <summary>
    /// Name of the scoring - shown in results
    /// </summary>
    [DataMember]
    public string Name { get; set; } = string.Empty;
    [DataMember]
    public int Index { get; set; }
    /// <summary>
    /// Maximum number of results to use for calculating scorin group (e.g: Team scoring)
    /// </summary>
    [DataMember]
    public int MaxResultsPerGroup { get; set; }
    /// <summary>
    /// External source for results if TakeResultsFromExtSource is true
    /// </summary>
    [DataMember]
    public long? ExtScoringSourceId { get; set; }
    /// <summary>
    /// Use teams information available from uploaded result set
    /// </summary>
    [DataMember]
    public bool UseResultSetTeam { get; set; }
    /// <summary>
    /// Update teams information on recalculation - this will overwrite the previous team in a scored row when a recalculation is triggered
    /// If you do not want the team to change after the result has been uploaded first (e.g.: team change during the runnin season) set to false
    /// </summary>
    [DataMember]
    public bool UpdateTeamOnRecalculation { get; set; }
    /// <summary>
    /// Show this result on the result page
    /// If false the results calculated from this scoring wont be 
    /// </summary>
    [DataMember]
    public bool ShowResults { get; set; }
    /// <summary>
    /// True to combine result of all other scorings in this config
    /// </summary>
    [DataMember]
    public bool IsCombinedResult { get; set; }
    /// <summary>
    /// Use the points from the source configuration (when calculating a combined result)
    /// </summary>
    [DataMember]
    public bool UseSourcePoints { get; set; }
    /// <summary>
    /// Point rule to determine point for this scoring
    /// </summary>
    [DataMember]
    public PointRuleModel? PointRule { get; set; }
}
