namespace iRLeagueDatabaseCore.Models;

public class ResultRowBase
{
    public double StartPosition { get; set; }
    public double FinishPosition { get; set; }
    public string CarNumber { get; set; }
    public int ClassId { get; set; }
    public string Car { get; set; }
    public string CarClass { get; set; }
    public double CompletedLaps { get; set; }
    public double LeadLaps { get; set; }
    public int FastLapNr { get; set; }
    public double Incidents { get; set; }
    public int Status { get; set; }
    public TimeSpan QualifyingTime { get; set; }
    public TimeSpan Interval { get; set; }
    public TimeSpan AvgLapTime { get; set; }
    public TimeSpan FastestLapTime { get; set; }
    public double PositionChange { get; set; }
    public string IRacingId { get; set; }
    public int SimSessionType { get; set; }
    public int OldIRating { get; set; }
    public int NewIRating { get; set; }
    public int SeasonStartIRating { get; set; }
    public string License { get; set; }
    public double OldSafetyRating { get; set; }
    public double NewSafetyRating { get; set; }
    public int OldCpi { get; set; }
    public int NewCpi { get; set; }
    public int ClubId { get; set; }
    public string ClubName { get; set; }
    public int CarId { get; set; }
    public double CompletedPct { get; set; }
    public DateTime? QualifyingTimeAt { get; set; }
    public int Division { get; set; }
    public int OldLicenseLevel { get; set; }
    public int NewLicenseLevel { get; set; }
    public int NumPitStops { get; set; }
    public string PittedLaps { get; set; }
    public int NumOfftrackLaps { get; set; }
    public string OfftrackLaps { get; set; }
    public int NumContactLaps { get; set; }
    public string ContactLaps { get; set; }
    public double RacePoints { get; set; }
}
