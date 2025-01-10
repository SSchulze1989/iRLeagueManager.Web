using iRLeagueApiCore.Common.Enums;
using iRLeagueApiCore.Common.Models;
using iRLeagueManager.Web.Data;

namespace iRLeagueManager.Web.ViewModels;

public sealed class RawResultRowViewModel : LeagueViewModelBase<RawResultRowViewModel, RawResultRowModel>
{
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