using iRLeagueApiCore.Common.Models.Rosters;
using iRLeagueManager.Web.Data;
using iRLeagueManager.Web.Extensions;

namespace iRLeagueManager.Web.ViewModels;

public sealed class ResultsPageViewModel : LeagueViewModelBase<ResultsPageViewModel>
{
    public ResultsPageViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService, EventListViewModel eventList) :
        base(loggerFactory, apiService)
    {
        this.eventList = eventList.EventList;
        results = [];
        rosters = [];
    }

    private long? loadedSeasonId;
    public long? LoadedSeasonId { get => loadedSeasonId; set => Set(ref loadedSeasonId, value); }

    private List<EventViewModel> eventList;
    public List<EventViewModel> EventList { get => eventList; set => Set(ref eventList, value); }

    private EventViewModel? selectedEvent;
    public EventViewModel? SelectedEvent
    {
        get => selectedEvent;
        set { if (Set(ref selectedEvent, value)) _ = OnSelectedSessionChanged(value); }
    }

    private ObservableCollection<EventResultViewModel> results;
    public ObservableCollection<EventResultViewModel> Results { get => results; set => Set(ref results, value); }

    public event Action<EventViewModel?>? SelectedSessionChanged;

    private List<RosterModel> rosters;
    public List<RosterModel> Rosters { get => rosters; set => Set(ref rosters, value); }

    public async Task LoadEventListAsync(CancellationToken cancellationToken)
    {
        if (ApiService.CurrentSeason == null)
        {
            LoadedSeasonId = null;
            EventList.Clear();
            return;
        }
        LoadedSeasonId = ApiService.CurrentSeason.Id;

        var sessionsEndpoint = ApiService.CurrentSeason.Events();
        var result = await sessionsEndpoint.Get(cancellationToken).ConfigureAwait(false);
        if (result.Success == false || result.Content is null)
        {
            EventList.Clear();
            return;
        }

        var sessions = result.Content;
        EventList = sessions.Select(x => new EventViewModel(LoggerFactory, ApiService, x)).ToList();
        OnPropertyChanged(nameof(SelectedEvent));
    }

    public async Task LoadFromEventAsync(long eventId, CancellationToken cancellationToken)
    {
        if (ApiService.CurrentLeague == null)
        {
            Results.Clear();
            return;
        }

        try
        {
            Loading = true;
            var eventEndpoint = ApiService.CurrentLeague.Events().WithId(eventId);
            selectedEvent = EventList.FirstOrDefault(x => x.EventId == eventId);

            if (selectedEvent == null)
            {
                // Load event list first if event is not in current event list  
                var eventRequest = await eventEndpoint.Get(cancellationToken).ConfigureAwait(false);
                if (eventRequest.Success == false || eventRequest.Content is null)
                {
                    return;
                }
                var @event = eventRequest.Content;
                if (ApiService.CurrentSeason == null || ApiService.CurrentSeason.Id != @event.SeasonId)
                {
                    await ApiService.SetCurrentSeasonAsync(ApiService.CurrentLeague.Name, @event.SeasonId);
                }
                await LoadEventListAsync(cancellationToken);
                selectedEvent = EventList.FirstOrDefault(x => x.EventId == eventId);
            }

            var resultEndpoint = eventEndpoint.Results();
            var requestResult = await resultEndpoint.Get(cancellationToken).ConfigureAwait(false);
            Results.Clear();
            if (requestResult.Success && requestResult.Content is not null)
            {
                var results = requestResult.Content;
                Results = new ObservableCollection<EventResultViewModel>(results.Select(x => new EventResultViewModel(LoggerFactory, ApiService, x)));
            }

            var rosters = await eventEndpoint.Rosters().Get(cancellationToken).ConfigureAwait(false);
            Rosters.Clear();
            if (rosters.Success && rosters.Content is not null)
            {
                Rosters = rosters.Content.ToList();
            }
        }
        finally
        {
            Loading = false;
        }
    }

    public async Task<StatusResult> TriggerCalculation(CancellationToken cancellationToken = default)
    {
        if (ApiService.CurrentLeague is null)
        {
            return LeagueNullResult();
        }

        if (SelectedEvent is null)
        {
            return new StatusResult(false, "No event selected");
        }
        try
        {
            Loading = true;
            var request = ApiService.CurrentLeague.Events()
                .WithId(SelectedEvent.EventId)
                .Results()
                .Calculate()
                .Post(cancellationToken).ConfigureAwait(false);
            var result = await request;

            if (result.Success)
            {
                await Task.Delay(2000, cancellationToken);
                await LoadFromEventAsync(SelectedEvent.EventId, cancellationToken);
            }

            return result.ToStatusResult();
        }
        finally
        {
            Loading = false;
        }
    }

    public async Task<StatusResult> DeleteEventResults(CancellationToken cancellationToken = default)
    {
        if (ApiService.CurrentLeague is null)
        {
            return LeagueNullResult();
        }
        if (SelectedEvent is null)
        {
            return new StatusResult(false, "Selected event was null");
        }

        try
        {
            Loading = true;
            var request = ApiService.CurrentLeague.Events()
                .WithId(SelectedEvent.EventId)
                .Results()
                .Delete(cancellationToken).ConfigureAwait(false);
            var result = await request;
            if (result.Success)
            {
                await LoadFromEventAsync(SelectedEvent.EventId, cancellationToken);
            }
            return result.ToStatusResult();
        }
        finally
        {
            Loading = false;
        }
    }

    private async Task OnSelectedSessionChanged(EventViewModel? @event, CancellationToken cancellationToken = default)
    {
        SelectedSessionChanged?.Invoke(@event);
        if (@event != null)
        {
            await LoadFromEventAsync(@event.EventId, cancellationToken);
        }
    }
}
