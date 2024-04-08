namespace iRLeagueApiCore.Services.ResultService.Models;

internal sealed class ResultRowCalculationResult : ResultRowCalculationData
{
    public ResultRowCalculationResult()
    {
    }

    public ResultRowCalculationResult(ResultRowCalculationData data)
    {
        AddPenalties = data.AddPenalties;
        ScoredResultRowId = data.ScoredResultRowId;
        MemberId = data.MemberId;
        Firstname = data.Firstname;
        Lastname = data.Lastname;
        TeamId = data.TeamId;
        TeamName = data.TeamName;
        TeamColor = data.TeamColor;
        StartPosition = data.StartPosition;
        FinishPosition = data.FinishPosition;
        CarNumber = data.CarNumber;
        ClassId = data.ClassId;
        ClubId = data.ClubId;
        ClubName = data.ClubName;
        Car = data.Car;
        CarClass = data.CarClass;
        CompletedLaps = data.CompletedLaps;
        LeadLaps = data.LeadLaps;
        FastLapNr = data.FastLapNr;
        Incidents = data.Incidents;
        Status = data.Status;
        QualifyingTime = data.QualifyingTime;
        Interval = data.Interval;
        AvgLapTime = data.AvgLapTime;
        FastestLapTime = data.FastestLapTime;
        PositionChange = data.PositionChange;
        OldIrating = data.OldIrating;
        NewIrating = data.NewIrating;
        SeasonStartIrating = data.SeasonStartIrating;
        License = data.License;
        NewCpi = data.NewCpi;
        OldCpi = data.OldCpi;
        OldSafetyRating = data.OldSafetyRating;
        NewSafetyRating = data.NewSafetyRating;
        CarId = data.CarId;
        CompletedPct = data.CompletedPct;
        Division = data.Division;
        OldLicenseLevel = data.OldLicenseLevel;
        NewLicenseLevel = data.NewLicenseLevel;
        RacePoints = data.RacePoints;
        BonusPoints = data.BonusPoints;
        PenaltyPoints = data.PenaltyPoints;
        TotalPoints = data.TotalPoints;
        FinalPosition = data.FinalPosition;
        FinalPositionChange = data.FinalPositionChange;
        PointsEligible = data.PointsEligible;
    }

    public bool IsScored { get; set; }
    public ICollection<long> ScoredMemberResultRowIds { get; set; } = Array.Empty<long>();
}
