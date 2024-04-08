namespace iRLeagueApiCore.Common.Models.Standings;

[DataContract]
public class StandingRowModel
{
    [DataMember]
    public long StandingRowId { get; set; }
    [DataMember]
    public string Firstname { get; set; } = string.Empty;
    [DataMember]
    public string Lastname { get; set; } = string.Empty;
    [DataMember]
    public string TeamName { get; set; } = string.Empty;
    [DataMember]
    public string TeamColor { get; set; } = string.Empty;
    [DataMember]
    public long? MemberId { get; set; }
    [DataMember]
    public long? TeamId { get; set; }
    [DataMember]
    public int Position { get; set; }
    [DataMember]
    public int LastPosition { get; set; }
    [DataMember]
    public int ClassId { get; set; }
    [DataMember]
    public string CarClass { get; set; } = string.Empty;
    [DataMember]
    public int RacePoints { get; set; }
    [DataMember]
    public int RacePointsChange { get; set; }
    [DataMember]
    public int PenaltyPoints { get; set; }
    [DataMember]
    public int PenaltyPointsChange { get; set; }
    [DataMember]
    public int TotalPoints { get; set; }
    [DataMember]
    public int TotalPointsChange { get; set; }
    [DataMember]
    public int Races { get; set; }
    [DataMember]
    public int RacesCounted { get; set; }
    [DataMember]
    public int RacesScored { get; set; }
    [DataMember]
    public int RacesInPoints { get; set; }
    [DataMember]
    public int DroppedResultCount { get; set; }
    [DataMember]
    public int ClubId { get; set; }
    [DataMember]
    public string ClubName { get; set; } = string.Empty;
    [DataMember]
    public int CompletedLaps { get; set; }
    [DataMember]
    public int CompletedLapsChange { get; set; }
    [DataMember]
    public int LeadLaps { get; set; }
    [DataMember]
    public int LeadLapsChange { get; set; }
    [DataMember]
    public int FastestLaps { get; set; }
    [DataMember]
    public int FastestLapsChange { get; set; }
    [DataMember]
    public int PolePositions { get; set; }
    [DataMember]
    public int PolePositionsChange { get; set; }
    [DataMember]
    public int Wins { get; set; }
    [DataMember]
    public int WinsChange { get; set; }
    [DataMember]
    public int Top3 { get; set; }
    [DataMember]
    public int Top5 { get; set; }
    [DataMember]
    public int Top10 { get; set; }
    [DataMember]
    public int Incidents { get; set; }
    [DataMember]
    public int IncidentsChange { get; set; }
    [DataMember]
    public int PositionChange { get; set; }
    [DataMember]
    public int StartIrating { get; set; }
    [DataMember]
    public int LastIrating { get; set; }
    [DataMember]
    public IEnumerable<StandingResultRowModel?> ResultRows { get; set; } = Array.Empty<StandingResultRowModel>();
}
