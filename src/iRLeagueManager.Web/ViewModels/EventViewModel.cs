using iRLeagueApiCore.Common.Enums;
using iRLeagueApiCore.Common.Models;
using iRLeagueManager.Web.Data;
using MvvmBlazor.ViewModel;
using System.Collections.ObjectModel;

namespace iRLeagueManager.Web.ViewModels
{
    public class EventViewModel : LeagueViewModelBase<EventViewModel>
    {
        private EventModel model;

        public EventViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService) : 
            base(loggerFactory, apiService)
        {
            this.model = new EventModel();
        }

        public EventViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService, EventModel model) :
            this(loggerFactory, apiService)
        {
            SetModel(model);
        }

        public long EventId { get => model.Id; }
        public string Name { get => model.Name; set => SetP(model.Name, value => model.Name = value, value); }
        public DateTime Date 
        { 
            get => model.Date.GetValueOrDefault(); 
            set => SetP(model.Date, value => model.Date = value.GetValueOrDefault().Add(model.Date.GetValueOrDefault().TimeOfDay), value); 
        }
        public long? TrackId 
        { 
            get => model.TrackId; 
            set => SetP(model.TrackId, value => model.TrackId = value, value); 
        }

        public string TrackIdString
        {
            get => TrackId?.ToString() ?? string.Empty;
            set => TrackId = long.TryParse(value, out long trackId) ? trackId : null;
        }

        public DateTime StartTime 
        { 
            get => model.Date.GetValueOrDefault();
            set => SetP(model.Date.GetValueOrDefault().TimeOfDay, value => model.Date = model.Date.GetValueOrDefault().Date.Add(value), value.TimeOfDay); 
        }

        public DateTime Duration
        {
            get => DateTime.MinValue.Add(model.Duration);
            set => SetP(model.Duration, value => model.Duration = value, value.TimeOfDay);
        }

        public ObservableCollection<SessionViewModel> Sessions { get; private set; } = new ObservableCollection<SessionViewModel>();

        public bool HasPractice
        {
            get => Practice != null;
            set
            {
                if (value && Practice == null)
                {
                    var practice = new SessionModel()
                    {
                        Name = "Practice",
                        SessionType = SessionType.Practice
                    };
                    model.Sessions.Add(practice);
                    Sessions.Add(new SessionViewModel(LoggerFactory, ApiService, practice));
                    return;
                }
                if (value == false && Practice != null)
                {
                    model.Sessions.Remove(Practice.GetModel());
                    Sessions.Remove(Practice);
                }
            }
        }
        public SessionViewModel? Practice
        {
            get => Sessions.FirstOrDefault(x => x.SessionType == SessionType.Practice);
        }

        public SessionViewModel? Qualifying
        {
            get => Sessions.FirstOrDefault(x => x.SessionType == SessionType.Qualifying);
        }

        public SessionViewModel? Race
        {
            get => Sessions.FirstOrDefault(x => x.SessionType == SessionType.Race);
        }

        public bool HasResult => model.HasResult;

        public int Laps => Race?.Laps ?? Qualifying?.Laps ?? Practice?.Laps ?? 0;

        public async Task Load(long eventId, CancellationToken cancellationToken = default)
        {
            if (ApiService.CurrentLeague == null)
            {
                return;
            }

            var result = await ApiService.CurrentLeague.Events().WithId(eventId).Get(cancellationToken);
            if (result.Success == false)
            {
                return;
            }

            SetModel(result.Content);
        }

        public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            if (model == null)
                return true;
            if (ApiService.CurrentLeague == null)
                return false;
            try
            {
                Loading = true;
                var result = await ApiService.CurrentLeague
                    .Events()
                    .WithId(model.Id)
                    .Put(model, cancellationToken);
                return result.Success;
            }
            finally
            {
                Loading = false;
            }
        }

        public void SetModel(EventModel model)
        {
            _ = model ?? throw new ArgumentNullException(nameof(model));
            this.model = model;
            Sessions = new ObservableCollection<SessionViewModel>(model.Sessions.Select(x => new SessionViewModel(LoggerFactory, ApiService, x)));
            OnPropertyChanged();
        }

        public EventModel GetModel()
        {
            return model;
        }
    }
}