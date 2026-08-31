using iRLeagueManager.Web.Data;

namespace iRLeagueManager.Web.ViewModels;

public sealed class EventListViewModel : LeagueViewModelBase<EventListViewModel>
{
    public EventListViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService) :
        base(loggerFactory, apiService)
    {
    }

    public long? LoadedSeasonId { get; set; }

    private List<EventViewModel> eventList = new List<EventViewModel>();
    public List<EventViewModel> EventList { get => eventList; set => Set(ref eventList, value); }

    private EventViewModel? selected;
    public EventViewModel? Selected
    {
        get => selected;
        set
        {
            var old = selected;
            Set(ref selected, value);
            if (selected?.EventId != old?.EventId)
            {
                EventChanged?.Invoke(this, new EventChangedEventArgs(old, value));
            }
        }
    }

    public event EventHandler<EventChangedEventArgs>? EventChanged;

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
            var result = await sessionsEndpoint.Get().ConfigureAwait(false);
            if (result.Success == false || result.Content is null)
            {
                EventList.Clear();
                return;
            }

            var sessions = result.Content;
            EventList = sessions.Select(x => new EventViewModel(LoggerFactory, ApiService, x)).ToList();
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
                .Get(cancellationToken).ConfigureAwait(false);
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

    public class EventChangedEventArgs : EventArgs
    {
        public EventViewModel? Old;
        public EventViewModel? New;

        public EventChangedEventArgs(EventViewModel? old, EventViewModel? @new)
        {
            Old = old;
            New = @new;
        }
    }
}
