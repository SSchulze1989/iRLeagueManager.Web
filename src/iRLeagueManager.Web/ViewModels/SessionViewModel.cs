using iRLeagueApiCore.Common.Enums;
using iRLeagueApiCore.Common.Models;
using iRLeagueManager.Web.Data;

namespace iRLeagueManager.Web.ViewModels;

public sealed class SessionViewModel : LeagueViewModelBase<SessionViewModel>
{
    private SessionModel model;

    public SessionViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService) :
        base(loggerFactory, apiService)
    {
        model = new SessionModel();
    }

    public SessionViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService, SessionModel model)
        : base(loggerFactory, apiService)
    {
        this.model = model;
    }

    public string Name { get => model.Name; set => SetP(model.Name, value => model.Name = value, value); }
    public long SessionId { get => model.SessionId; set => SetP(model.SessionId, value => model.SessionId = value, value); }
    public int SessionNr { get => model.SessionNr; set => SetP(model.SessionNr, value => model.SessionNr = value, value); }
    public SessionType SessionType { get => model.SessionType; set => SetP(model.SessionType, value => model.SessionType = value, value); }
    public DateTime Duration
    {
        get => DateTime.MinValue.Add(model.Duration);
        set => SetP(model.Duration, value => model.Duration = value, value.TimeOfDay);
    }
    public int Laps { get => model.Laps; set => SetP(model.Laps, value => model.Laps = value, value); }

    public string LapsString
    {
        get => Laps.ToString();
        set => Laps = int.TryParse(value, out int laps) ? laps : 0;
    }

    public SessionModel GetModel()
    {
        return model;
    }
}
