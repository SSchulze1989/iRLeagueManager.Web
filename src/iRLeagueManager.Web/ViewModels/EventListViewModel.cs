using iRLeagueManager.Web.Data;

namespace iRLeagueManager.Web.ViewModels;

public class EventListViewModel : LeagueViewModelBase<EventListViewModel>
{
    public EventListViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService) :
        base(loggerFactory, apiService)
    {
    }

    public long? LoadedSeasonId { get; set; }

    private ObservableCollection<EventViewModel> eventList = new();
    public ObservableCollection<EventViewModel> EventList { get => eventList; set => Set(ref eventList, value); }

    private EventViewModel? selected;
    public EventViewModel? Selected { get => selected; set => Set(ref selected, value); }

    public async Task LoadEventListAsync(long seasonId)
    {
        if (ApiService.CurrentLeague == null)
        {
            return;
        }

        try
        {
            Loading = true;
            await ApiService.SetCurrentSeasonAsync(ApiService.CurrentLeague.Name, seasonId);
            LoadedSeasonId = seasonId;
            if (ApiService.CurrentSeason == null)
            {
                LoadedSeasonId = null;
                EventList.Clear();
                return;
            }

            var sessionsEndpoint = ApiService.CurrentSeason.Events();
            var result = await sessionsEndpoint.Get();
            if (result.Success == false || result.Content is null)
            {
                EventList.Clear();
                return;
            }

            var sessions = result.Content;
            EventList = new ObservableCollection<EventViewModel>(sessions.Select(x => new EventViewModel(LoggerFactory, ApiService, x)));
            if (Selected != null)
            {
                Selected = EventList.FirstOrDefault(x => x.EventId == Selected.EventId);
            }
        }
        finally
        {
            Loading = false;
        }
    }

    public async Task LoadEventListFromEventAsync(long eventId, CancellationToken cancellationToken = default)
    {
        if (ApiService.CurrentLeague == null)
        {
            return;
        }

        try
        {
            Loading = true;
            var result = await ApiService.CurrentLeague
                .Events()
                .WithId(eventId)
                .Get(cancellationToken);
            if (result.Success == false || result.Content is null)
            {
                return;
            }
            var @event = result.Content;
            await LoadEventListAsync(@event.SeasonId);
        }
        finally
        {
            Loading = false;
        }
    }
}
