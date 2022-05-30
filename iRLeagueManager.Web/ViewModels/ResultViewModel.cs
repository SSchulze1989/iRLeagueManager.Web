using iRLeagueApiCore.Communication.Models;
using iRLeagueManager.Web.Data;

namespace iRLeagueManager.Web.ViewModels
{
    public class ResultViewModel : LeagueViewModelBase<ResultViewModel>
    {
        private ResultModel model;

        public ResultViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService) : 
            this(loggerFactory, apiService, new ResultModel())
        {
        }

        public ResultViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService, ResultModel model) :
            base(loggerFactory, apiService)
        {
            this.model = model;
        }

        public long SeasonId => model.SeasonId;
        public long ScoringId => model.ScoringId;
        public string ScoringName => model.ScoringName;
        public IEnumerable<ResultRowModel> ResultRows => model.ResultRows;
    }
}