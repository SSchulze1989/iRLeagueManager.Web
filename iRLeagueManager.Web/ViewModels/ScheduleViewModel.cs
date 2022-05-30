using iRLeagueApiCore.Communication.Models;
using iRLeagueManager.Web.Data;
using MvvmBlazor.ViewModel;
using System.Collections.ObjectModel;

namespace iRLeagueManager.Web.ViewModels
{
    public class ScheduleViewModel : LeagueViewModelBase<ScheduleViewModel>
    {
        private GetScheduleModel model;

        public ScheduleViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService) : 
            this(loggerFactory, apiService, new GetScheduleModel())
        {
        }

        public ScheduleViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService, GetScheduleModel model) : 
            base(loggerFactory, apiService)
        {
            sessions = new ObservableCollection<SessionViewModel>();
            this.model = model;
        }

        public long ScheduleId { get => model.ScheduleId; set => SetP(model.ScheduleId, value => model.ScheduleId = value, value); }
        public string Name { get => model.Name; set => SetP(model.Name, value => model.Name = value, value); }

        private ObservableCollection<SessionViewModel> sessions;
        public ObservableCollection<SessionViewModel> Sessions { get => sessions; set => Set(ref sessions, value); }

        public async Task SetModel(GetScheduleModel model)
        {
            this.model = model;
            OnPropertyChanged(null);
            await LoadSessions();
        }

        public async Task LoadSessions(CancellationToken cancellationToken = default)
        {
            if (ApiService.CurrentLeague == null) return;
            Loading = true;

            //await Task.Delay(500);

            var result = await ApiService.CurrentLeague.Schedules().WithId(ScheduleId).Sessions().Get(cancellationToken);
            if (result.Success == false)
            {
                return;
            }

            var sessions = result.Content;
            Sessions = new ObservableCollection<SessionViewModel>(sessions.Select(x =>
                new SessionViewModel(LoggerFactory, ApiService, x)));

            Loading = false;
        }
    }
}