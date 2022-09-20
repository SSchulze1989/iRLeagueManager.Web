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
                    AddSession("Practice", SessionType.Practice, onlyOnce: true);
                    return;
                }
                if (value == false && Practice != null)
                {
                    Sessions.Remove(Practice);
                }
            }
        }
        public SessionViewModel? Practice
        {
            get => Sessions.FirstOrDefault(x => x.SessionType == SessionType.Practice);
        }

        public bool HasQualifying
        {
            get => Qualifying != null;
            set 
            {
                if (value && Qualifying == null)
                {
                    AddSession("Qualifying", SessionType.Qualifying, onlyOnce: true);
                    return;
                }
                if (value == false && Qualifying != null)
                {
                    Sessions.Remove(Qualifying);
                }
            }
        }

        public SessionViewModel? Qualifying
        {
            get => Sessions.FirstOrDefault(x => x.SessionType == SessionType.Qualifying);
        }

        public bool HasRace
        {
            get => Race != null;
            set
            {
                if (value && Race == null)
                {
                    AddSession("Race", SessionType.Race, onlyOnce: true);
                    return;
                }
                if (value == false && Race != null)
                {
                    Sessions.Remove(Race);
                }
            }
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
                // Delete sessions that were removed from the SessionViewModel collection
                foreach(var sessionModel in model.Sessions.ToList())
                {
                    if (Sessions.Any(x => x.GetModel() == sessionModel) == false)
                    {
                        model.Sessions.Remove(sessionModel);
                    }
                }
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

        private SessionViewModel AddSession(string name, SessionType sessionType, bool onlyOnce=false)
        {
            // if onlyOnce is true check if a session of that type exists inside model
            SessionModel sessionModel = default!;
            if (onlyOnce)
            {
                sessionModel = model.Sessions
                    .FirstOrDefault(x => x.SessionType == sessionType)!;
            }
            if (sessionModel == null)
            {
                sessionModel = new SessionModel()
                {
                    Name = name,
                    SessionType = sessionType,
                };
                model.Sessions.Add(sessionModel);
            }

            var session = new SessionViewModel(LoggerFactory, ApiService, sessionModel);
            Sessions.Add(session);
            return session;
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