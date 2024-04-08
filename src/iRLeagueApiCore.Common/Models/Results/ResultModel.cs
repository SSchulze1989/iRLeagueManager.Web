namespace iRLeagueApiCore.Common.Models;

/// <summary>
/// Get a complete scored result from the database
/// </summary>
[DataContract]
public class ResultModel
{
    /// <summary>
    /// Id of the league
    /// </summary>
    public long LeagueId { get; set; }
    /// <summary>
    /// Id of the season the result belongs to
    /// </summary>
    public long SeasonId { get; set; }
    /// <summary>
    /// Id of the unique session result
    /// </summary>
    [DataMember]
    public long SessionResultId { get; set; }
    /// <summary>
    /// Name of the season the result belongs to
    /// </summary>
    [DataMember]
    public string SeasonName { get; set; } = string.Empty;
    /// <summary>
    /// Id of the schedule the result belongs to
    /// </summary>
    [DataMember]
    public long ScheduleId { get; set; }
    /// <summary>
    /// Name of the schedule the result belongs to
    /// </summary>
    [DataMember]
    public string ScheduleName { get; set; } = string.Empty;
    /// <summary>
    /// Id of the event the result belongs to
    /// </summary>
    [DataMember]
    public long EventId { get; set; }
    /// <summary>
    /// Name of the event the result belongs to
    /// </summary>
    [DataMember]
    public string EventName { get; set; } = string.Empty;
    /// <summary>
    /// Id of the session the result belongs to
    /// </summary>
    [DataMember]
    public long? SessionId { get; set; }
    /// <summary>
    /// Name of the session the result belongs to
    /// </summary>
    [DataMember]
    public string SessionName { get; set; } = string.Empty;
    /// <summary>
    /// Number of the session order
    /// </summary>
    [DataMember]
    public int? SessionNr { get; set; }
    /// <summary>
    /// Id of the scoring for this result
    /// </summary>
    [DataMember]
    public long? ScoringId { get; set; }
    /// <summary>
    /// Name of the scoring for this result
    /// </summary>
    [DataMember]
    public string ScoringName { get; set; } = string.Empty;
    /// <summary>
    /// List of entries 
    /// </summary>
    [DataMember(IsRequired = true)]
    public IEnumerable<ResultRowModel> ResultRows { get; set; } = Array.Empty<ResultRowModel>();

    [DataMember]
    public TimeSpan? FastestLapTime { get; set; }
    [DataMember]
    public MemberInfoModel? FastestLapDriver { get; set; }
    [DataMember]
    public TimeSpan? PoleLapTime { get; set; }
    [DataMember]
    public MemberInfoModel? PoleLapDriver { get; set; }
    [DataMember]
    public IEnumerable<MemberInfoModel>? CleanestDrivers { get; set; }

    #region version
    /// <summary>
    /// Date of creation
    /// </summary>
    [DataMember]
    public DateTime? CreatedOn { get; set; }
    /// <summary>
    /// Date of last modification
    /// </summary>
    [DataMember]
    public DateTime? LastModifiedOn { get; set; }
    #endregion
}
