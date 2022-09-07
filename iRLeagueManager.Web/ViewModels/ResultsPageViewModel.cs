using iRLeagueApiCore.Common.Models.Results;
using iRLeagueManager.Web.Data;
using System.Collections.ObjectModel;
using System.Linq;

namespace iRLeagueManager.Web.ViewModels
{
    public class ResultsPageViewModel : LeagueViewModelBase<ResultsPageViewModel>
    {
        public ResultsPageViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService) : 
            base(loggerFactory, apiService)
        {
            eventList = new ObservableCollection<EventViewModel>();
            results = new ObservableCollection<EventResultViewModel>();
        }

        private long? loadedSeasonId;
        public long? LoadedSeasonId { get => loadedSeasonId; set => Set(ref loadedSeasonId, value); }

        private ObservableCollection<EventViewModel> eventList;
        public ObservableCollection<EventViewModel> EventList { get => eventList; set => Set(ref eventList, value); }

        private long? selectedSessionId;
        public long? SelectedSessionId { get => selectedSessionId; set { if (Set(ref selectedSessionId, value)) _ = OnSelectedSessionChanged(value); } }

        public EventViewModel? Session => EventList.SingleOrDefault(x => x.EventId == selectedSessionId);

        private ObservableCollection<EventResultViewModel> results;
        public ObservableCollection<EventResultViewModel> Results { get => results; set => Set(ref results, value); }

        public event Action<long?>? SelectedSessionChanged;

        public async Task LoadSessionListAsync()
        {
            if (ApiService.CurrentSeason == null)
            {
                LoadedSeasonId = null;
                EventList.Clear();
                return;
            }
            LoadedSeasonId = ApiService.CurrentSeason.Id;

            var sessionsEndpoint = ApiService.CurrentSeason.Events();
            var result = await sessionsEndpoint.Get();
            if (result.Success == false)
            {
                EventList.Clear();
                return;
            }

            var sessions = result.Content;
            EventList = new ObservableCollection<EventViewModel>(sessions.Select(x => new EventViewModel(LoggerFactory, ApiService, x)));
            OnPropertyChanged(nameof(SelectedSessionId));
        }

        public async Task LoadFromSessionAsync(long sessionId)
        {
            if (ApiService.CurrentLeague == null)
            {
                Results.Clear();
                return;
            }

            try
            {
                Loading = true;
                var sessionEndpoint = ApiService.CurrentLeague.Events().WithId(sessionId);
                selectedSessionId = sessionId;

                var resultEndpoint = sessionEndpoint.Results();
                var requestResult = await resultEndpoint.Get();
                if (requestResult.Success == false)
                {
                    Results.Clear();
                    return;
                }

                var results = requestResult.Content;
                Results = new ObservableCollection<EventResultViewModel>(results.Select(x => new EventResultViewModel(LoggerFactory, ApiService, x)));
            }
            finally
            {
                Loading = false;
            }
        }

        private async Task OnSelectedSessionChanged(long? sessionId)
        {
            SelectedSessionChanged?.Invoke(sessionId);
            if (sessionId != null)
            {
                await LoadFromSessionAsync(sessionId.Value);
            }
        }
    }
}
