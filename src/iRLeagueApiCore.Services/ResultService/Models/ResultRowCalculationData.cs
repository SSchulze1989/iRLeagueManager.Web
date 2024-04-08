using System.Diagnostics;

namespace iRLeagueApiCore.Services.ResultService.Models;

[DebuggerDisplay("RowId = {ScoredResultRowId}, MemberId = {MemberId}, " +
    "TeamId = {TeamId}, RacePoints = {RacePoints}, BonusPoints = {BonusPoints}, " +
    "PenaltyPoints = {PenaltyPoints}, TotalPoints = {TotalPoints}")]
internal class ResultRowCalculationData : IPointRow, IPenaltyRow
{
    public long? ScoredResultRowId { get; set; }
    public long? MemberId { get; set; }
    public string Firstname { get; set; } = string.Empty;
    public string Lastname { get; set; } = string.Empty;
    public long? TeamId { get; set; }
    public string TeamName { get; set; } = string.Empty;
    public string TeamColor { get; set; } = string.Empty;
    public double StartPosition { get; set; }
    public double FinishPosition { get; set; }
    public string CarNumber { get; set; } = string.Empty;
    public int ClassId { get; set; }
    public int ClubId { get; set; }
    public string ClubName { get; set; } = string.Empty;
    public string Car { get; set; } = string.Empty;
    public string CarClass { get; set; } = string.Empty;
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
    public int OldIrating { get; set; }
    public int NewIrating { get; set; }
    public int SeasonStartIrating { get; set; }
    public string License { get; set; } = string.Empty;
    public int NewCpi { get; set; }
    public int OldCpi { get; set; }
    public double OldSafetyRating { get; set; }
    public double NewSafetyRating { get; set; }
    public int CarId { get; set; }
    public double CompletedPct { get; set; }
    public int Division { get; set; }
    public int OldLicenseLevel { get; set; }
    public int NewLicenseLevel { get; set; }
    public double RacePoints { get; set; }
    public double BonusPoints { get; set; }
    public double PenaltyPoints { get; set; }
    public double TotalPoints { get; set; }
    public int FinalPosition { get; set; }
    public double FinalPositionChange { get; set; }
    public bool PointsEligible { get; set; }
    public IEnumerable<AddPenaltyCalculationData> AddPenalties { get; set; } = Array.Empty<AddPenaltyCalculationData>();
    public ICollection<ReviewPenaltyCalculationResult> ReviewPenalties { get; set; } = new List<ReviewPenaltyCalculationResult>();
}
