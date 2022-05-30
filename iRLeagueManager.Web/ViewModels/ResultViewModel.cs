using iRLeagueApiCore.Communication.Models;
using iRLeagueManager.Web.Data;

namespace iRLeagueManager.Web.ViewModels
{
    public class ResultViewModel : LeagueViewModelBase<ResultViewModel>
    {
        private GetResultModel model;

        public ResultViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService) : 
            this(loggerFactory, apiService, new GetResultModel())
        {
        }

        public ResultViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService, GetResultModel model) :
            base(loggerFactory, apiService)
        {
            this.model = model;
        }

        public long SeasonId => model.SeasonId;
        public long ScoringId => model.ScoringId;
        public string ScoringName => model.ScoringName;
        public IEnumerable<GetResultRowModel> ResultRows => model.ResultRows;
    }
}