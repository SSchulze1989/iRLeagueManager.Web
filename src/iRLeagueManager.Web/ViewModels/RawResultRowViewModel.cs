using iRLeagueApiCore.Common.Enums;
using iRLeagueApiCore.Common.Models;
using iRLeagueManager.Web.Data;

namespace iRLeagueManager.Web.ViewModels;

public sealed class RawResultRowViewModel : LeagueViewModelBase<RawResultRowViewModel, RawResultRowModel>
{
    public RawResultRowViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService) : this(loggerFactory, apiService, new())
    {
    }

    public RawResultRowViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService, RawResultRowModel model) : base(loggerFactory, apiService, model)
    {
    }

    private RawResultRowModel modelCopy = new();

    public int FinishPosition { get => (int)model.FinishPosition; set => SetP(model.FinishPosition, value => model.FinishPosition = value, value);}
    public int StartPosition { get => (int)model.StartPosition; set => SetP(model.StartPosition, value => model.StartPosition = value, value);}
    public long MemberId { get => model.MemberId; set => SetP(model.MemberId, value => model.MemberId = value, value); }
    public string CarNumber { get => model.CarNumber; set => SetP(model.CarNumber, value => model.CarNumber = value, value); }
    public int CompletedLaps { get => (int)model.CompletedLaps; set => SetP(model.CompletedLaps, value => model.CompletedLaps = value, value); }
    public int LeadLaps { get => (int)model.LeadLaps; set => SetP(model.LeadLaps, value => model.LeadLaps = value, value); }
    public double RacePoints { get => model.RacePoints; set => SetP(model.RacePoints, value => model.RacePoints = value, value); }
    public TimeSpan Interval { get => model.Interval; set => SetP(model.Interval, value => model.Interval = value, value); }
    public TimeSpan FastestLapTime {  get => model.FastestLapTime; set => SetP(model.FastestLapTime, value => model.FastestLapTime = value, value); }
    public int FastLapNr { get => model.FastLapNr; set => SetP(model.FastLapNr, value => model.FastLapNr = value, value); }
    public TimeSpan AvgLapTime { get => model.AvgLapTime; set => SetP(model.AvgLapTime, value => model.AvgLapTime = value, value); }
    public int Incidents { get => (int)model.Incidents; set => SetP(model.Incidents, value => model.Incidents = value, value); }
    public RaceStatus Status { get => (RaceStatus)model.Status; set => SetP(model.Status, value => model.Status = value, (int)value); }
    public int ClassId { get => model.ClassId; set => SetP(model.ClassId, value => model.ClassId = value, value); }
    public long ResultRowId { get => model.ResultRowId; set => SetP(model.ResultRowId, value => model.ResultRowId = value, value); }
    public double PositionChange { get => model.PositionChange; set => SetP(model.PositionChange, value => model.PositionChange = value, value); }
    public string IRacingId { get => model.IRacingId; set => SetP(model.IRacingId, value => model.IRacingId = value, value); }
    public int SimSessionType { get => model.SimSessionType; set => SetP(model.SimSessionType, value => model.SimSessionType = value, value); }
    public int OldIRating { get => model.OldIRating; set => SetP(model.OldIRating, value => model.OldIRating = value, value); }
    public int NewIRating { get => model.NewIRating; set => SetP(model.NewIRating, value => model.NewIRating = value, value); }
    public int SeasonStartIRating { get => model.SeasonStartIRating; set => SetP(model.SeasonStartIRating, value => model.SeasonStartIRating = value, value); }
    public string License { get => model.License; set => SetP(model.License, value => model.License = value, value); }
    public double OldSafetyRating { get => model.OldSafetyRating; set => SetP(model.OldSafetyRating, value => model.OldSafetyRating = value, value); }
    public double NewSafetyRating { get => model.NewSafetyRating; set => SetP(model.NewSafetyRating, value => model.NewSafetyRating = value, value); }
    public int OldCpi { get => model.OldCpi; set => SetP(model.OldCpi, value => model.OldCpi = value, value); }
    public int NewCpi { get => model.NewCpi; set => SetP(model.NewCpi, value => model.NewCpi = value, value); }
    public int ClubId { get => model.ClubId; set => SetP(model.ClubId, value => model.ClubId = value, value); }
    public string ClubName { get => model.ClubName; set => SetP(model.ClubName, value => model.ClubName = value, value); }
    public int CarId { get => model.CarId; set => SetP(model.CarId, value => model.CarId = value, value); }
    public double CompletedPct { get => model.CompletedPct; set => SetP(model.CompletedPct, value => model.CompletedPct = value, value); }
    public DateTime? QualifyingTimeAt { get => model.QualifyingTimeAt; set => SetP(model.QualifyingTimeAt, value => model.QualifyingTimeAt = value, value); }
    public int Division { get => model.Division; set => SetP(model.Division, value => model.Division = value, value); }
    public int OldLicenseLevel { get => model.OldLicenseLevel; set => SetP(model.OldLicenseLevel, value => model.OldLicenseLevel = value, value); }
    public int NewLicenseLevel { get => model.NewLicenseLevel; set => SetP(model.NewLicenseLevel, value => model.NewLicenseLevel = value, value); }
    public int NumPitStops { get => model.NumPitStops; set => SetP(model.NumPitStops, value => model.NumPitStops = value, value); }
    public string PittedLaps { get => model.PittedLaps; set => SetP(model.PittedLaps, value => model.PittedLaps = value, value); }
    public int NumOfftrackLaps { get => model.NumOfftrackLaps; set => SetP(model.NumOfftrackLaps, value => model.NumOfftrackLaps = value, value); }
    public string OfftrackLaps { get => model.OfftrackLaps; set => SetP(model.OfftrackLaps, value => model.OfftrackLaps = value, value); }
    public int NumContactLaps { get => model.NumContactLaps; set => SetP(model.NumContactLaps, value => model.NumContactLaps = value, value); }
    public string ContactLaps { get => model.ContactLaps; set => SetP(model.ContactLaps, value => model.ContactLaps = value, value); }
    public long? TeamId { get => model.TeamId; set => SetP(model.TeamId, value => model.TeamId = value, value); }
    public bool PointsEligible { get => model.PointsEligible; set => SetP(model.PointsEligible, value => model.PointsEligible = value, value); }
    public string? CountryCode { get => model.CountryCode; set => SetP(model.CountryCode, value => model.CountryCode = value, value); }
    public string Car { get => model.Car; set => SetP(model.Car, value => model.Car = value, value); }
    public string CarClass { get => model.CarClass; set => SetP(model.CarClass, value => model.CarClass = value, value); }
    public TimeSpan QualifyingTime { get => model.QualifyingTime; set => SetP(model.QualifyingTime, value => model.QualifyingTime = value, value); }
    protected override void SetModel(RawResultRowModel model)
    {
        base.SetModel(model);
        modelCopy = CopyModel();
    }

    public void Reset()
    {
        ModelHelper.CopyModelProperties(modelCopy, model);
        ResetChangedState();
    }

    public void ApplyChanges()
    {
        modelCopy = CopyModel();
    }
}