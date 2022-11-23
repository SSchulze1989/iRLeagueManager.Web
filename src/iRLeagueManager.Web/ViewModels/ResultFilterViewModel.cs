using iRLeagueApiCore.Common.Enums;
using iRLeagueApiCore.Common.Models;
using iRLeagueManager.Web.Data;

namespace iRLeagueManager.Web.ViewModels
{
    public class ResultFilterViewModel : LeagueViewModelBase<ResultFilterViewModel, ResultFilterModel>
    {
        public ResultFilterViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService, ResultFilterModel model) : 
            base(loggerFactory, apiService, model)
        {
        }

        public long LeagueId => model.LeagueId;
        public long ResultsFilterId => model.ResultsFilterId;
        public string ColumnPropertyName { get => model.ColumnPropertyName; set => SetP(model.ColumnPropertyName, value => model.ColumnPropertyName = value, value); } 
        public ComparatorType Comparator { get => model.Comparator; set => SetP(model.Comparator, value => model.Comparator = value, value); }
        public bool Include { get => model.Include; set => SetP(model.Include, value => model.Include = value, value); }
        public ICollection<string> FilterValues { get => model.FilterValues; set => SetP(model.FilterValues, value => model.FilterValues = value, value); }
    }
}
