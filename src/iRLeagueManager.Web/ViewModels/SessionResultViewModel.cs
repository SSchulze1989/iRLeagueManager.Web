using iRLeagueApiCore.Common.Models;
using iRLeagueManager.Web.Data;
using System.Linq.Expressions;

namespace iRLeagueManager.Web.ViewModels
{
    public class SessionResultViewModel : LeagueViewModelBase<SessionResultViewModel>
    {
        private ResultModel model;

        public SessionResultViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService) : 
            this(loggerFactory, apiService, new ResultModel())
        {
        }

        public SessionResultViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService, ResultModel model) :
            base(loggerFactory, apiService)
        {
            this.model = model;
            orderByPropertySelector = x => x.FinalPosition;
        }

        public long SeasonId => model.SeasonId;
        public string SessionName => model.SessionName;

        private Expression<Func<ResultRowModel, IComparable>> orderByPropertySelector;
        public Expression<Func<ResultRowModel, IComparable>> OrderByPropertySelector
        {
            get => orderByPropertySelector;
            set
            {
                if (Set(ref orderByPropertySelector, value, new SamePropertyEqualityComparer<ResultRowModel>()))
                {
                    OnPropertyChanged(nameof(ResultRows));
                    return;
                }
                OrderDescending = !OrderDescending;
            }
        }
        public bool OrderDescending { get; private set; }
        
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
