using iRLeagueApiCore.Communication.Models;
using iRLeagueManager.Web.Data;
using System.Linq.Expressions;

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
            orderByPropertySelector = x => x.FinalPosition;
        }

        private Func<ResultRowModel, IComparable> orderByPropertySelector;
        public Func<ResultRowModel, IComparable> OrderByPropertySelector
        {
            get => orderByPropertySelector;
            set
            {
                if (Set(ref orderByPropertySelector, value))
                {
                    OnPropertyChanged(nameof(ResultRows));
                }
            }
        }

        public long SeasonId => model.SeasonId;
        public long ScoringId => model.ScoringId;
        public string ScoringName => model.ScoringName;
        public IEnumerable<ResultRowModel> ResultRows => model.ResultRows.OrderBy(OrderByPropertySelector);
    }
}