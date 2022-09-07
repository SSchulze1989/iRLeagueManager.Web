using iRLeagueApiCore.Common.Enums;
using iRLeagueApiCore.Common.Models;
using iRLeagueManager.Web.Data;
using MvvmBlazor.ViewModel;

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
            base(loggerFactory, apiService)
        {
            this.model = model;
        }

        public long EventId { get => model.Id; }
        public string Name { get => model.Name; set => SetP(model.Name, value => model.Name = value, value); }
        public DateTime Date 
        { 
            get => model.Date.GetValueOrDefault().Date; 
            set => SetP(model.Date, value => model.Date = value.GetValueOrDefault().Add(model.Date.GetValueOrDefault().TimeOfDay), value); 
        }
        public long? TrackId { get => model.TrackId; set => SetP(model.TrackId, value => model.TrackId = value, value); }

        public TimeSpan StartTime 
        { 
            get => model.Date.GetValueOrDefault().TimeOfDay;
            set => SetP(model.Date.GetValueOrDefault().TimeOfDay, value => model.Date = model.Date.GetValueOrDefault().Date.Add(value), value); 
        }

        public TimeSpan Duration
        {
            get => model.Duration;
            set => SetP(model.Duration, value => model.Duration = value, value);
        }

        public ICollection<SessionModel> Sessions { get => model.Sessions; }

        public SessionModel? Practice
        {
            get => model.Sessions.FirstOrDefault(x => x.SessionType == SessionType.Practice);
        }

        public SessionModel? Qualifying
        {
            get => model.Sessions.FirstOrDefault(x => x.SessionType == SessionType.Qualifying);
        }

        public SessionModel? Race
        {
            get => model.Sessions.FirstOrDefault(x => x.SessionType == SessionType.Race);
        }

        public bool HasResult => model.HasResult;

        public int Laps => Race?.Laps ?? Qualifying?.Laps ?? Practice?.Laps ?? 0;

        public void SetModel(EventModel model)
        {
            this.model = model;
            OnPropertyChanged(null);
        }
    }
}