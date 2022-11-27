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
        public long ResultsFilterId => model.FilterOptionId;
        public string ColumnPropertyName { get => model.ColumnPropertyName; set => SetP(model.ColumnPropertyName, value => model.ColumnPropertyName = value, value); } 
        public ComparatorType Comparator { get => model.Comparator; set => SetP(model.Comparator, value => model.Comparator = value, value); }
        public FilterType FilterType { get => model.FilterType; set => SetP(model.FilterType, value => model.FilterType = value, value); }
        public ICollection<string> FilterValues { get => model.FilterValues; set => SetP(model.FilterValues, value => model.FilterValues = value, value); }
    }
}
