namespace iRLeagueApiCore.Common.Models;
public sealed class StandingResultRowModel
{
    /// <summary>
    /// Id of the Event
    /// </summary>
    [DataMember]
    public long EventId { get; set; }
    /// <summary>
    /// Date of event
    /// </summary>
    [DataMember]
    public DateTime Date { get; set; }
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
    /// Number of completed laps in the main session (only includes laps from one session type e.g: race)
    /// </summary>
    [DataMember]
    public double CompletedLaps { get; set; }
    /// <summary>
    /// Number of incidents in the session (only main session)
    /// </summary>
    [DataMember]
    public double Incidents { get; set; }
    /// <summary>
    /// Driver status at the end of the race (checkered flag)
    /// </summary>
    [DataMember]
    public int Status { get; set; }
    /// <summary>
    /// Irating before the event
    /// </summary>
    [DataMember]
    public int Irating { get; set; }
    /// <summary>
    /// Irating at the start of the season
    /// </summary>
    [DataMember]
    public int SeasonStartIrating { get; set; }
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
    [DataMember]
    public bool IsScored { get; set; }
}
