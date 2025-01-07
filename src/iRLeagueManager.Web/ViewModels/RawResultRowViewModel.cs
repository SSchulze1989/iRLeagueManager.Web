using iRLeagueApiCore.Common.Models;
using iRLeagueManager.Web.Data;

namespace iRLeagueManager.Web.ViewModels;

public sealed class RawResultRowViewModel : LeagueViewModelBase<RawResultRowViewModel, RawResultRowModel>
{
    public RawResultRowViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService, RawResultRowModel model) : base(loggerFactory, apiService, model)
    {
    }

    private RawResultRowModel modelCopy;

    public int FinishPosition { get => (int)model.FinishPosition; set => SetP(model.FinishPosition, value => model.FinishPosition = value, value);}
    public int StartPosition { get => (int)model.StartPosition; set => SetP(model.StartPosition, value => model.StartPosition = value, value);}
    public long MemberId { get => model.MemberId; set => SetP(model.MemberId, value => model.MemberId = value, value); }
    public int CompletedLaps { get => (int)model.CompletedLaps; set => SetP(model.CompletedLaps, value => model.CompletedLaps = value, value); }
    public double RacePoints { get => model.RacePoints; set => SetP(model.RacePoints, value => model.RacePoints = value, value); }
    public TimeSpan Interval { get => model.Interval; set => SetP(model.Interval, value => model.Interval = value, value); }
    public TimeSpan FastestLapTime {  get => model.FastestLapTime; set => SetP(model.FastestLapTime, value => model.FastestLapTime = value, value); }
    public TimeSpan AvgLapTime { get => model.AvgLapTime; set => SetP(model.AvgLapTime, value => model.AvgLapTime = value, value); }
    public int Incidents { get => (int)model.Incidents; set => SetP(model.Incidents, value => model.Incidents = value, value); }

    protected override void SetModel(RawResultRowModel model)
    {
        base.SetModel(model);
        modelCopy = CopyModel();
    }

    public void Reset()
    {
        SetModel(modelCopy);
    }

    public void ApplyChanges()
    {
        modelCopy = CopyModel();
    }
}