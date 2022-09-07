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

        private Expression<Func<ResultRowModel, IComparable>> orderByPropertySelector;
        public Expression<Func<ResultRowModel, IComparable>> OrderByPropertySelector
        {
            get => orderByPropertySelector;
            set
            {
                if (Set(ref orderByPropertySelector, value))
                {
                    OnPropertyChanged(nameof(ResultRows));
                    return;
                }
                OrderDescending = !OrderDescending;
            }
        }
        public bool OrderDescending { get; private set; }

        public long SeasonId => model.SeasonId;
        public long ScoringId => model.ScoringId;
        public string ScoringName => model.ScoringName;
        public IEnumerable<ResultRowModel> ResultRows
        {
            get
            {
                if (OrderDescending)
                {
                    return model.ResultRows.OrderByDescending(OrderByPropertySelector.Compile());
                }
                return model.ResultRows.OrderBy(OrderByPropertySelector.Compile());
            }
        }
    }
}