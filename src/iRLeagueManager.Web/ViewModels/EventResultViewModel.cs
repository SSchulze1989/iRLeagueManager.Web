using iRLeagueApiCore.Common.Models;
using iRLeagueManager.Web.Data;
using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace iRLeagueManager.Web.ViewModels
{
    public class EventResultViewModel : LeagueViewModelBase<EventResultViewModel>
    {
        private EventResultModel model;

        public EventResultViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService) : 
            this(loggerFactory, apiService, new EventResultModel())
        {
        }

        public EventResultViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService, EventResultModel model) :
            base(loggerFactory, apiService)
        {
            this.model = model;
            sessionResults = new ObservableCollection<SessionResultViewModel>(model.SessionResults.Select(x => new SessionResultViewModel(loggerFactory, ApiService, x)));
        }

        public long ResultId => model.ResultId;
        public long SeasonId => model.SeasonId;
        public string Name => model.DisplayName;

        private ObservableCollection<SessionResultViewModel> sessionResults;
        public ObservableCollection<SessionResultViewModel> SessionResults { get => sessionResults; set => Set(ref sessionResults, value); }
    }
}