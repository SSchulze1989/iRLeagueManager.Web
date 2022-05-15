using iRLeagueManager.Web.Data;
using System.Collections.ObjectModel;

namespace iRLeagueManager.Web.ViewModels
{
    public class ResultsPageViewModel : LeagueViewModelBase<ResultsPageViewModel>
    {
        public ResultsPageViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService) : 
            base(loggerFactory, apiService)
        {
            sessionList = new ObservableCollection<SessionViewModel>();
            results = new ObservableCollection<ResultViewModel>();
        }

        private ObservableCollection<SessionViewModel> sessionList;
        public ObservableCollection<SessionViewModel> SessionList { get => sessionList; set => Set(ref sessionList, value); }

        private long? selectedSessionId;
        public long? SelectedSessionId { get => selectedSessionId; set { if (Set(ref selectedSessionId, value)) _ = OnSelectedSessionChanged(value); } }

        //public SessionViewModel? Session => SessionList.SingleOrDefault(x => x.SessionId == selectedSessionId);

        private ObservableCollection<ResultViewModel> results;
        public ObservableCollection<ResultViewModel> Results { get => results; set => Set(ref results, value); }

        public event Action<long?> SelectedSessionChanged;

        public async Task LoadSessionListAsync()
        {
            if (ApiService.CurrentSeason == null)
            {
                SessionList.Clear();
                return;
            }

            var sessionsEndpoint = ApiService.CurrentSeason.Sessions();
            var result = await sessionsEndpoint.Get();
            if (result.Success == false)
            {
                SessionList.Clear();
                return;
            }

            var sessions = result.Content;
            SessionList = new ObservableCollection<SessionViewModel>(sessions.Select(x => new SessionViewModel(LoggerFactory, ApiService, x)));
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
                var sessionEndpoint = ApiService.CurrentLeague.Sessions().WithId(sessionId);
                selectedSessionId = sessionId;

                var requestResult = await sessionEndpoint.Results().Get();
                if (requestResult.Success == false)
                {
                    Results.Clear();
                    return;
                }

                var results = requestResult.Content;
                Results = new ObservableCollection<ResultViewModel>(results.Select(x => new ResultViewModel(LoggerFactory, ApiService, x)));
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
