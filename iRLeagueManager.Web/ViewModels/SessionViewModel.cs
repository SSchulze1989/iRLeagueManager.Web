using iRLeagueApiCore.Communication.Models;
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

        public void SetModel(GetSessionModel model)
        {
            this.model = model;
            OnPropertyChanged(null);
        }
    }
}