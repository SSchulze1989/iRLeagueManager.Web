using iRLeagueApiCore.Communication.Enums;
using iRLeagueApiCore.Communication.Models;
using iRLeagueManager.Web.Data;
using MvvmBlazor.ViewModel;

namespace iRLeagueManager.Web.ViewModels
{
    public class SessionViewModel : LeagueViewModelBase<SessionViewModel>
    {
        private SessionModel model;

        public SessionViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService) : 
            base(loggerFactory, apiService)
        {
            this.model = new SessionModel();
        }

        public SessionViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService, SessionModel model) :
            base(loggerFactory, apiService)
        {
            this.model = model;
        }

        public long SessionId { get => model.SessionId; set => SetP(model.SessionId, value => model.SessionId = value, value); }
        public string Name { get => model.Name; set => SetP(model.Name, value => model.Name = value, value); }
        public DateTime Date 
        { 
            get => model.Date.GetValueOrDefault().Date; 
            set => SetP(model.Date, value => model.Date = value.GetValueOrDefault().Add(model.Date.GetValueOrDefault().TimeOfDay), value); 
        }
        public long? TrackId { get => model.TrackId; set => SetP(model.TrackId, value => model.TrackId = value, value); }

        public TimeSpan SessionStart 
        { 
            get => model.Date.GetValueOrDefault().TimeOfDay;
            set => SetP(model.Date.GetValueOrDefault().TimeOfDay, value => model.Date = model.Date.GetValueOrDefault().Date.Add(value), value); 
        }

        public TimeSpan Duration
        {
            get => model.Duration;
            set => SetP(model.Duration, value => model.Duration = value, value);
        }

        public SubSessionModel? Practice
        {
            get => model.SubSessions.FirstOrDefault(x => x.SessionType == SimSessionType.OpenPractice);
        }

        public SubSessionModel? Qualifying
        {
            get => model.SubSessions.FirstOrDefault(x => x.SessionType == SimSessionType.LoneQualifying || x.SessionType == SimSessionType.OpenQualifying);
        }

        public SubSessionModel? Race
        {
            get => model.SubSessions.FirstOrDefault(x => x.SessionType == SimSessionType.Race);
        }

        public bool HasResult => model.HasResult;

        public void SetModel(SessionModel model)
        {
            this.model = model;
            OnPropertyChanged(null);
        }
    }
}