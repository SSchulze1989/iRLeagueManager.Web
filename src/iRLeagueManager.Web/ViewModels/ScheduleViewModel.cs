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

        public long ScheduleId { get => model.ScheduleId; set => SetP(model.ScheduleId, value => model.ScheduleId = value, value); }
        public string Name { get => model.Name; set => SetP(model.Name, value => model.Name = value, value); }

        public int MaxRaceCount => Events.Count > 0 ? Events.Max(x => x.RaceCount) : 0;

        private ObservableCollection<EventViewModel> events;
        public ObservableCollection<EventViewModel> Events { get => events; set => Set(ref events, value); }

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
}