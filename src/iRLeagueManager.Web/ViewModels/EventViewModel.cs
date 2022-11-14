using iRLeagueApiCore.Common.Enums;
using iRLeagueApiCore.Common.Models;
using iRLeagueManager.Web.Data;
using MvvmBlazor.ViewModel;
using System.Collections.ObjectModel;

namespace iRLeagueManager.Web.ViewModels
{
    public class EventViewModel : LeagueViewModelBase<EventViewModel, EventModel>
    {
        public EventViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService) : 
            base(loggerFactory, apiService, new EventModel())
        {
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
            set
            {
                if (SetP(model.TrackId, value => model.TrackId = value, value))
                {
                    OnPropertyChanged(nameof(TrackName));
                    OnPropertyChanged(nameof(ConfigName));
                }
            }
        }

        public string TrackIdString
        {
            get => TrackId?.ToString() ?? string.Empty;
            set => TrackId = long.TryParse(value, out long trackId) ? trackId : null;
        }

        public string TrackName => model.TrackName;

        public string ConfigName => model.ConfigName;

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

        public IEnumerable<SessionViewModel> Races => Sessions.Where(x => x.SessionType == SessionType.Race);

        public int RaceCount => Races.Count();

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
            if (result.Success == false || result.Content is null)
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
                ReorderSesssionNrs();
                Loading = true;
                var result = await ApiService.CurrentLeague
                    .Events()
                    .WithId(model.Id)
                    .Put(model, cancellationToken);
                if (result.Success == false || result.Content is null)
                {
                    return false;
                }
                SetModel(result.Content);
                return result.Success;
            }
            finally
            {
                Loading = false;
            }
        }

        private void ReorderSesssionNrs()
        {
            // find practice session and move first:
            int currentIndex = 1;
            var practice = Sessions.FirstOrDefault(x => x.SessionType == SessionType.Practice);
            if (practice != null)
            {
                practice.SessionNr = currentIndex++;
            }
            // find qualifying and move up (behind practice)
            var qualifying = Sessions.FirstOrDefault(x => x.SessionType == SessionType.Qualifying);
            if (qualifying != null)
            {
                qualifying.SessionNr = currentIndex++;
            }
            // sort those sessions that have a number and make them ascending +1 each
            foreach(var session in Sessions.Except(new[] {practice, qualifying}).Where(x => x?.SessionNr != 0).OrderBy(x => x!.SessionNr))
            {
                session!.SessionNr = currentIndex++;
            }
            // cover the rest
            foreach(var session in Sessions.Where(x => x?.SessionNr == 0))
            {
                session!.SessionNr = currentIndex++;
            }
        }

        private void SortSessions()
        {
            Sessions = new ObservableCollection<SessionViewModel>(Sessions.OrderBy(x => x.SessionNr));
        }

        public SessionViewModel AddSession(string name, SessionType sessionType, bool onlyOnce=false)
        {
            SessionModel sessionModel = default!;
            bool reorderNumbers = false;
            if (onlyOnce)
            {   
                // if onlyOnce is true check if a session of that type exists inside model
                sessionModel = model.Sessions
                    .FirstOrDefault(x => x.SessionType == sessionType)!;
            }
            if (sessionModel == null)
            {
                // find the first session model in model.Sessions that is not currently part of a SesionViewModel
                // and that matches the requested type
                sessionModel = model.Sessions
                    .Where(x => Sessions.Any(y => y.GetModel() == x) == false)
                    .Where(x => x.SessionType == sessionType)
                    .FirstOrDefault()!;
            }
            if (sessionModel == null)
            {
                // When no suitable model exists, create new
                sessionModel = new SessionModel()
                {
                    Name = name,
                    SessionType = sessionType,
                };
                model.Sessions.Add(sessionModel);
                reorderNumbers = true;
            }

            var session = new SessionViewModel(LoggerFactory, ApiService, sessionModel);
            Sessions.Add(session);
            if (reorderNumbers)
            {
                ReorderSesssionNrs();
            }
            SortSessions();
            return session;
        }

        public bool CanMoveSessionUp(SessionViewModel session)
        {
            var sessionIndex = Sessions.IndexOf(session);
            return sessionIndex > 0 && Sessions.ElementAt(sessionIndex - 1).SessionType == SessionType.Race;
        }

        public bool CanMoveSessionDown(SessionViewModel session)
        {
            var sessionIndex = Sessions.IndexOf(session);
            return sessionIndex < Sessions.Count -1 && Sessions.ElementAt(sessionIndex + 1).SessionType == SessionType.Race;
        }

        public void MoveSessionUp(SessionViewModel session)
        {
            // find previous session that is no a practice or qualy
            var sessionIndex = Sessions.IndexOf(session);
            if (sessionIndex <= 0)
            {
                return;
            }
            var prevSession = Sessions.ElementAt(sessionIndex - 1);
            if (prevSession.SessionType != SessionType.Race)
            {
                return;
            }
            // swap sessions
            var tmp = session.SessionNr;
            session.SessionNr = prevSession.SessionNr;
            prevSession.SessionNr = tmp;
            SortSessions();
        }

        public void MoveSessionDown(SessionViewModel session)
        {
            // find next session that is no a practice or qualy
            var sessionIndex = Sessions.IndexOf(session);
            if (sessionIndex >= Sessions.Count)
            {
                return;
            }
            var nextSession = Sessions.ElementAt(sessionIndex + 1);
            if (nextSession.SessionType == SessionType.Practice || nextSession.SessionType == SessionType.Qualifying)
            {
                return;
            }
            // swap sessions
            var tmp = session.SessionNr;
            session.SessionNr = nextSession.SessionNr;
            nextSession.SessionNr = tmp;
            SortSessions();
        }

        public override void SetModel(EventModel model)
        {
            _ = model ?? throw new ArgumentNullException(nameof(model));
            this.model = model;
            Sessions = new ObservableCollection<SessionViewModel>(model.Sessions.Select(x => new SessionViewModel(LoggerFactory, ApiService, x)));
            SortSessions();
            OnPropertyChanged();
        }
    }
}