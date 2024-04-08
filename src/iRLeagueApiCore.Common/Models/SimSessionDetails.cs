namespace iRLeagueApiCore.Common.Models;

/// <summary>
/// Extended session information about the iracing subsession
/// </summary>
[DataContract]
public class SimSessionDetails
{
    /// <summary>
    /// Id of the iracing subsession
    /// </summary>
    [DataMember]
    public long IRSubsessionId { get; set; }
    /// <summary>
    /// Id of the iracing season
    /// </summary>
    [DataMember]
    public long IRSeasonId { get; set; }
    /// <summary>
    /// Name of the iracing season
    /// </summary>
    [DataMember]
    public string IRSeasonName { get; set; } = string.Empty;
    /// <summary>
    /// Year of the iracing season
    /// </summary>
    [DataMember]
    public int IRSeasonYear { get; set; }
    /// <summary>
    /// Quarter of the iracing season
    /// </summary>
    [DataMember]
    public int IRSeasonQuarter { get; set; }
    /// <summary>
    /// Race week in the iracing season
    /// </summary>
    [DataMember]
    public int IRRaceWeek { get; set; }
    /// <summary>
    /// Id of the iracing session
    /// </summary>
    [DataMember]
    public long IRSessionId { get; set; }
    /// <summary>
    /// License category of the iracing session
    /// </summary>
    [DataMember]
    public int LicenseCategory { get; set; }
    /// <summary>
    /// Name of the iracing session
    /// </summary>
    [DataMember]
    public string SessionName { get; set; } = string.Empty;
    /// <summary>
    /// Start time of the iracing session (timezone??)
    /// </summary>
    [DataMember]
    public DateTime? StartTime { get; set; }
    /// <summary>
    /// End time of the iracing session (timezone??)
    /// </summary>
    [DataMember]
    public DateTime? EndTime { get; set; }
    /// <summary>
    /// Corners per lap on the selected track configuration
    /// </summary>
    [DataMember]
    public int CornersPerLap { get; set; }
    /// <summary>
    /// Distance per lap on the selected track configuration
    /// </summary>
    [DataMember]
    public double KmDistPerLap { get; set; }
    /// <summary>
    /// Maximum number of weeks in the iracing session
    /// </summary>
    [DataMember]
    public int MaxWeeks { get; set; }
    /// <summary>
    /// SOF of the field in this event
    /// </summary>
    [DataMember]
    public int EventStrengthOfField { get; set; }
    /// <summary>
    /// Average number of laps in this event
    /// </summary>
    [DataMember]
    public long EventAverageLap { get; set; }
    /// <summary>
    /// Number of laps completed in this event
    /// </summary>
    [DataMember]
    public int EventLapsComplete { get; set; }
    /// <summary>
    /// Number of cautions in this event
    /// </summary>
    [DataMember]
    public int NumCautions { get; set; }
    /// <summary>
    /// Number of laps under caution in this event 
    /// </summary>
    [DataMember]
    public int NumCautionLaps { get; set; }
    /// <summary>
    /// Number of lead changes in this event
    /// </summary>
    [DataMember]
    public int NumLeadChanges { get; set; }
    /// <summary>
    /// Time of day setting in the iracing session
    /// </summary>
    [DataMember]
    public int TimeOfDay { get; set; }
    /// <summary>
    /// Damage model setting in the iracing session
    /// </summary>
    [DataMember]
    public int DamageModel { get; set; }
    /// <summary>
    /// Track id of the selected track configuration
    /// </summary>
    [DataMember]
    public int IRTrackId { get; set; }
    /// <summary>
    /// Name of the selected track (without config)
    /// </summary>
    [DataMember]
    public string TrackName { get; set; } = string.Empty;
    /// <summary>
    /// Name of the selected track configuration
    /// </summary>
    [DataMember]
    public string ConfigName { get; set; } = string.Empty;
    /// <summary>
    /// Category id of the selected track configuration
    /// </summary>
    [DataMember]
    public int TrackCategoryId { get; set; }
    /// <summary>
    /// Category name of the selected track configuration
    /// </summary>
    [DataMember]
    public string Category { get; set; } = string.Empty;
    /// <summary>
    /// Weather type in the iracing session
    /// </summary>
    [DataMember]
    public int WeatherType { get; set; }
    /// <summary>
    /// Selected temp unit
    /// </summary>
    [DataMember]
    public int TempUnits { get; set; }
    /// <summary>
    /// Environment temperate in the iracing session
    /// </summary>
    [DataMember]
    public int TempValue { get; set; }
    /// <summary>
    /// Relative humidity in the iracing session
    /// </summary>
    [DataMember]
    public int RelHumidity { get; set; }
    /// <summary>
    /// Fog level in the iracing session
    /// </summary>
    [DataMember]
    public int Fog { get; set; }
    /// <summary>
    /// Wind direction in the iracing session
    /// </summary>
    [DataMember]
    public int WindDir { get; set; }
    /// <summary>
    /// Selected wind units
    /// </summary>
    [DataMember]
    public int WindUnits { get; set; }
    /// <summary>
    /// Skies setting in the iracing session
    /// </summary>
    [DataMember]
    public int Skies { get; set; }
    /// <summary>
    /// ???
    /// </summary>
    [DataMember]
    public int WeatherVarInitial { get; set; }
    /// <summary>
    /// ???
    /// </summary>
    [DataMember]
    public int WeatherVarOngoing { get; set; }
    /// <summary>
    /// Start time of the simulation in UTC
    /// </summary>
    [DataMember]
    public DateTime? SimStartUtcTime { get; set; }
    /// <summary>
    /// UTC offset to local time
    /// </summary>
    [DataMember]
    public long SimStartUtcOffset { get; set; }
    /// <summary>
    /// Leave marbles between session transition setting
    /// </summary>
    [DataMember]
    public bool LeaveMarbles { get; set; }
    /// <summary>
    /// Setting for rubber in practice session
    /// </summary>
    [DataMember]
    public int PracticeRubber { get; set; }
    /// <summary>
    /// Setting for rubber in qualy session
    /// </summary>
    [DataMember]
    public int QualifyRubber { get; set; }
    /// <summary>
    /// Setting for rubber in warmub session
    /// </summary>
    [DataMember]
    public int WarmupRubber { get; set; }
    /// <summary>
    /// Setting for rubber in race session
    /// </summary>
    [DataMember]
    public int RaceRubber { get; set; }
    /// <summary>
    /// Compound setting in practice session
    /// </summary>
    [DataMember]
    public int PracticeGripCompound { get; set; }
    /// <summary>
    /// Compound setting in qualy session
    /// </summary>
    [DataMember]
    public int QualifyGripCompund { get; set; }
    /// <summary>
    /// Compound setting in warmup session
    /// </summary>
    [DataMember]
    public int WarmupGripCompound { get; set; }
    /// <summary>
    /// Compound setting in race session
    /// </summary>
    [DataMember]
    public int RaceGripCompound { get; set; }
}