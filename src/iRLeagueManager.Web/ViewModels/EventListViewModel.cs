using iRLeagueManager.Web.Data;
using System.Collections.ObjectModel;

namespace iRLeagueManager.Web.ViewModels
{
    public class EventListViewModel : LeagueViewModelBase<EventListViewModel>
    {
        public EventListViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService) : 
            base(loggerFactory, apiService)
        {
        }

        public long? LoadedSeasonId { get; set; }

        private ObservableCollection<EventViewModel> eventList = new();
        public ObservableCollection<EventViewModel> EventList { get => eventList; set => Set(ref eventList, value); }

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
                if (result.Success == false)
                {
                    EventList.Clear();
                    return;
                }

                var sessions = result.Content;
                EventList = new ObservableCollection<EventViewModel>(sessions.Select(x => new EventViewModel(LoggerFactory, ApiService, x)));
            }
            finally
            {
                Loading = false;
            }
        }
    }
}
