using iRLeagueApiCore.Common.Models;
using iRLeagueManager.Web.Data;
using MvvmBlazor.ViewModel;
using System.Collections.ObjectModel;

namespace iRLeagueManager.Web.ViewModels
{
    public class ScheduleViewModel : LeagueViewModelBase<ScheduleViewModel>
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

        public long ScheduleId { get => model.ScheduleId; set => Set(model, x => x.ScheduleId, value); }
        public string Name { get => model.Name; set => Set(model, x => model.Name, value); }

        private ObservableCollection<EventViewModel> events;
        public ObservableCollection<EventViewModel> Events { get => events; set => Set(ref events, value); }

        public async Task SetModel(ScheduleModel model)
        {
            this.model = model;
            OnPropertyChanged(null);
            await LoadEvents();
        }

        public async Task LoadEvents(CancellationToken cancellationToken = default)
        {
            if (ApiService.CurrentLeague == null) return;
            Loading = true;

            //await Task.Delay(500);

            var result = await ApiService.CurrentLeague.Schedules().WithId(ScheduleId).Events().Get(cancellationToken);
            if (result.Success == false)
            {
                return;
            }

            var sessions = result.Content;
            Events = new ObservableCollection<EventViewModel>(sessions.Select(x =>
                new EventViewModel(LoggerFactory, ApiService, x)));

            Loading = false;
        }
    }
}