﻿using iRLeagueApiCore.Communication.Models;
using iRLeagueManager.Web.Data;
using MvvmBlazor.ViewModel;

namespace iRLeagueManager.Web.ViewModels
{
    public class SessionViewModel : LeagueViewModelBase<SessionViewModel>
    {
        private GetSessionModel model;

        public SessionViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService) : 
            base(loggerFactory, apiService)
        {
            this.model = new GetSessionModel();
        }

        public SessionViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService, GetSessionModel model) :
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
        public int Laps { get => model.Laps; set => SetP(model.Laps, value => model.Laps = value, value); }
        public TimeSpan SessionStart 
        { 
            get => model.Date.GetValueOrDefault().TimeOfDay;
            set => SetP(model.Date.GetValueOrDefault().TimeOfDay, value => model.Date = model.Date.GetValueOrDefault().Date.Add(value), value); 
        }

        public void SetModel(GetSessionModel model)
        {
            this.model = model;
            OnPropertyChanged(null);
        }
    }
}