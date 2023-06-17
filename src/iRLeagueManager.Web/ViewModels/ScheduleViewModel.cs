using iRLeagueApiCore.Common.Models;
using iRLeagueManager.Web.Data;
using iRLeagueManager.Web.Extensions;

namespace iRLeagueManager.Web.ViewModels;

public sealed class ScheduleViewModel : LeagueViewModelBase<ScheduleViewModel>
{
    private ScheduleModel model;

    public ScheduleViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService) :
        this(loggerFactory, apiService, new ScheduleModel())
    {
    }

    public ScheduleViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService, ScheduleModel model) :
        base(loggerFactory, apiService)
    {
        events = new ObservableCollection<EventViewModel>();
        this.model = model;
    }

    public long ScheduleId { get => model.ScheduleId; set => SetP(model.ScheduleId, value => model.ScheduleId = value, value); }
    public string Name { get => model.Name; set => SetP(model.Name, value => model.Name = value, value); }

    public int MaxRaceCount => Events.Count > 0 ? Events.Max(x => x.RaceCount) : 0;

    private ObservableCollection<EventViewModel> events;
    public ObservableCollection<EventViewModel> Events { get => events; set => Set(ref events, value); }

    private IEnumerable<ResultConfigInfoModel> defaultResultConfigs = Array.Empty<ResultConfigInfoModel>();
    public IEnumerable<ResultConfigInfoModel> DefaultResultConfigs { get => defaultResultConfigs; set => Set(ref defaultResultConfigs, value); }

    public async Task SetModel(ScheduleModel model)
    {
        this.model = model;
        OnPropertyChanged();
        await LoadEvents();
    }

    public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        if (ApiService.CurrentLeague == null)
        {
            return false;
        }
        try
        {
            Loading = true;
            var result = await ApiService.CurrentLeague.Schedules().WithId(ScheduleId).Put(model, cancellationToken);

            if (result.Success)
            {
                return true;
            }
            return false;
        }
        finally
        {
            Loading = false;
        }
    }

    public async Task Reload(CancellationToken cancellationToken = default)
    {
        if (ApiService.CurrentLeague == null)
        {
            return;
        }
        if (ScheduleId == 0)
        {
            return;
        }

        var result = await ApiService.CurrentLeague.Schedules().WithId(ScheduleId).Get(cancellationToken);
        if (result.Success == false || result.Content is null)
        {
            return;
        }
        await SetModel(result.Content);
    }

    public async Task<StatusResult> AddEvent(EventViewModel @event, CancellationToken cancellationToken = default)
    {
        if (ApiService.CurrentLeague == null)
        {
            return LeagueNullResult();
        }

        try
        {
            Loading = true;
            var request = ApiService.CurrentLeague.Schedules()
                .WithId(ScheduleId)
                .Events()
                .Post(@event.GetModel(), cancellationToken);
            var result = await request;
            if (result.Success)
            {
                await LoadEvents(cancellationToken);
            }

            return result.ToStatusResult();
        }
        finally
        {
            Loading = false;
        }
    }

    public async Task<StatusResult> LoadDefaultResultConfig(CancellationToken cancellationToken = default)
    {
        if (CurrentLeague is null)
        {
            return LeagueNullResult();
        }
        if (CurrentSeason is null)
        {
            return SeasonNullResult();
        }

        try
        {
            Loading = true;
            var result = await CurrentSeason
                .ResultsConfigs()
                .Get(cancellationToken);
            if (result.Success && result.Content is not null)
            {
                DefaultResultConfigs = result.Content
                    .Where(x => x.IsDefaultConfig)
                    .Select(x => new ResultConfigInfoModel()
                    {
                        LeagueId = x.LeagueId,
                        ChampionshipName = x.ChampionshipName,
                        ChampSeasonId = x.ChampSeasonId,
                        IsDefaultConfig = x.IsDefaultConfig,
                        DisplayName = x.DisplayName,
                        Name = x.Name,
                        ResultConfigId = x.ResultConfigId,
                    });
            }
            return result.ToStatusResult();
        }
        finally
        {
            Loading = false;
        }
    }

    public async Task<StatusResult> RemoveEvent(EventViewModel @event, CancellationToken cancellationToken = default)
    {
        if (ApiService.CurrentLeague == null)
        {
            return LeagueNullResult();
        }

        var request = ApiService.CurrentLeague.Events()
            .WithId(@event.EventId)
            .Delete(cancellationToken);
        var result = await request;
        if (result.Success)
        {
            await LoadEvents(cancellationToken);
        }

        return result.ToStatusResult();
    }

    public async Task LoadEvents(CancellationToken cancellationToken = default)
    {
        if (ApiService.CurrentLeague == null) return;
        Loading = true;

        //await Task.Delay(500);

        var result = await ApiService.CurrentLeague.Schedules().WithId(ScheduleId).Events().Get(cancellationToken);
        if (result.Success == false || result.Content is null)
        {
            return;
        }

        var sessions = result.Content;
        Events = new ObservableCollection<EventViewModel>(sessions.Select(x =>
            new EventViewModel(LoggerFactory, ApiService, x)));

        Loading = false;
    }
}
