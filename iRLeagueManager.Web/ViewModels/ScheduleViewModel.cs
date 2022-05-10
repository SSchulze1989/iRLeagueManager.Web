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
            base(loggerFactory, apiService)
        {
            sessions = new ObservableCollection<SessionViewModel>();
            model = new GetScheduleModel();
        }

        public long ScheduleId { get => model.ScheduleId; set => SetProp(model.ScheduleId, value => model.ScheduleId = value, value); }

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

            var result = await ApiService.CurrentLeague.Schedules().WithId(ScheduleId).Sessions().Get(cancellationToken);
            if (result.Success == false)
            {
                return;
            }

            var sessions = result.Content;
            Sessions = new ObservableCollection<SessionViewModel>(sessions.Select(x =>
                new SessionViewModel(LoggerFactory, ApiService, x)));
        }
    }
}