namespace iRLeagueApiCore.Common.Models;

/// <summary>
/// Get a scored result row from the database
/// </summary>
[DataContract]
public class ResultRowModel
{
    [DataMember]
    public long ScoredResultRowId { get; set; }
    /// <summary>
    /// First name of the driver
    /// </summary>
    [DataMember]
    public string Firstname { get; set; } = string.Empty;
    /// <summary>
    /// Last name of the driver
    /// </summary>
    [DataMember]
    public string Lastname { get; set; } = string.Empty;
    /// <summary>
    /// Team name of the drivers team (or team result)
    /// </summary>
    [DataMember]
    public string TeamName { get; set; } = string.Empty;
    /// <summary>
    /// Hex code for team color
    /// </summary>
    [DataMember]
    public string TeamColor { get; set; } = string.Empty;
    /// <summary>
    /// Posiion at start of race session (equal to qually result when using attached qualifying)
    /// </summary>
    [DataMember]
    public double StartPosition { get; set; }
    /// <summary>
    /// Finish position in the race results (iracing penalties are applied - league penalties are not)
    /// </summary>
    [DataMember]
    public double FinishPosition { get; set; }
    /// <summary>
    /// Iracing id of the member
    /// </summary>
    [DataMember]
    public long? MemberId { get; set; }
    /// <summary>
    /// Car number in the session
    /// </summary>
    [DataMember]
    public string CarNumber { get; set; } = string.Empty;
    /// <summary>
    /// Class id in the session (in multiclass sessions)
    /// </summary>
    [DataMember]
    public int ClassId { get; set; }
    /// <summary>
    /// Id of the iracing member club
    /// </summary>
    [DataMember]
    public int ClubId { get; set; }
    /// <summary>
    /// Name of the iracing member club
    /// </summary>
    [DataMember]
    public string ClubName { get; set; } = string.Empty;
    /// <summary>
    /// Name of the car (e.g: "Skip Barber RT200")
    /// </summary>
    [DataMember]
    public string Car { get; set; } = string.Empty;
    /// <summary>
    /// Name of the car class (in multiclass sessions)
    /// </summary>
    [DataMember]
    public string CarClass { get; set; } = string.Empty;
    /// <summary>
    /// Number of completed laps in the main session (only includes laps from one session type e.g: race)
    /// </summary>
    [DataMember]
    public double CompletedLaps { get; set; }
    /// <summary>
    /// Number of laps lead by this driver (only race)
    /// </summary>
    [DataMember]
    public double LeadLaps { get; set; }
    /// <summary>
    /// Number of the fastest laps of this driver (applicable to all session types)
    /// </summary>
    [DataMember]
    public int FastLapNr { get; set; }
    /// <summary>
    /// Number of incidents in the session (only main session)
    /// </summary>
    [DataMember]
    public double Incidents { get; set; }
    /// <summary>
    /// Driver status at the end of the race (checkered flag)
    /// </summary>
    [DataMember]
    public RaceStatus Status { get; set; }
    /// <summary>
    /// Time set in qualifying (only available with attached qualy)
    /// </summary>
    [DataMember]
    public TimeSpan QualifyingTime { get; set; }
    /// <summary>
    /// Interval to the leading driver 
    /// </summary>
    [DataMember]
    public Interval Interval { get; set; } = new();
    /// <summary>
    /// Average lap time in the main session
    /// </summary>
    [DataMember]
    public TimeSpan AvgLapTime { get; set; }
    /// <summary>
    /// Fastest lap time in the main session
    /// </summary>
    [DataMember]
    public TimeSpan FastestLapTime { get; set; }
    /// <summary>
    /// Position change StartPos -> FinPos during the main session
    /// </summary>
    [DataMember]
    public double PositionChange { get; set; }
    /// <summary>
    /// Irating before the event
    /// </summary>
    [DataMember]
    public int OldIrating { get; set; }
    /// <summary>
    /// Irating after completing the event
    /// </summary>
    [DataMember]
    public int NewIrating { get; set; }
    /// <summary>
    /// Irating at the start of the season
    /// </summary>
    [DataMember]
    public int SeasonStartIrating { get; set; }
    /// <summary>
    /// License class of the driver
    /// </summary>
    [DataMember]
    public string License { get; set; } = string.Empty;
    /// <summary>
    /// Driver safety rating before the event
    /// </summary>
    [DataMember]
    public double OldSafetyRating { get; set; }
    /// <summary>
    /// Driver safety rating after completing the event
    /// </summary>
    [DataMember]
    public double NewSafetyRating { get; set; }
    /// <summary>
    /// Driver/Team car id
    /// </summary>
    [DataMember]
    public int CarId { get; set; }
    /// <summary>
    /// Completed race distance 
    /// When omited the value is calculated based on driver-laps/session-laps
    /// </summary>
    [DataMember]
    public double? CompletedPct { get; set; }
    /// <summary>
    /// Iracing division of the driver
    /// </summary>
    [DataMember]
    public int Division { get; set; }
    /// <summary>
    /// Driver License level before the event
    /// </summary>
    [DataMember]
    public int OldLicenseLevel { get; set; }
    /// <summary>
    /// Driver license level after completing the event
    /// </summary>
    [DataMember]
    public int NewLicenseLevel { get; set; }
    /// <summary>
    /// [optional] Id of the team the driver was part in this event
    /// omit for no team
    /// </summary>
    [DataMember]
    public long? TeamId { get; set; }
    /// <summary>
    /// Points gained from result in the race
    /// </summary>
    [DataMember]
    public double RacePoints { get; set; }
    /// <summary>
    /// Points gained from bonus condition (will be added to race points)
    /// </summary>
    [DataMember]
    public double BonusPoints { get; set; }
    /// <summary>
    /// Points deducted as penalty (Value is positive but points will be deducted from race points)
    /// </summary>
    [DataMember]
    public double PenaltyPoints { get; set; }
    /// <summary>
    /// Total scored points -> sum of: (RacePoints + BonusPoints - PenaltyPoints)
    /// </summary>
    [DataMember]
    public double TotalPoints { get; set; }
    /// <summary>
    /// Final position after all scoring rules and penalties are applied
    /// </summary>
    [DataMember]
    public int FinalPosition { get; set; }
    /// <summary>
    /// Position change StartPosition -> FinalPosition
    /// </summary>
    [DataMember]
    public double FinalPositionChange { get; set; }
    [DataMember]
    public IEnumerable<ResultRowModel> TeamResultRows { get; set; } = Array.Empty<ResultRowModel>();
}
