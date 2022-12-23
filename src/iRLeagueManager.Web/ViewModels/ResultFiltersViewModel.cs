using iRLeagueApiCore.Common.Models;
using iRLeagueManager.Web.Data;

namespace iRLeagueManager.Web.ViewModels;

public class ResultFiltersViewModel : LeagueViewModelBase<ResultFiltersViewModel>
{
    public ResultFiltersViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService) :
        base(loggerFactory, apiService)
    {
        filters = new();
    }

    private ObservableCollection<ResultFilterViewModel> filters;
    public ObservableCollection<ResultFilterViewModel> Filters { get => filters; set => SetP(filters, value => filters = value, value); }

    public void SetModel(IEnumerable<ResultFilterModel> model)
    {
        Filters = new(model.Select(x => new ResultFilterViewModel(LoggerFactory, ApiService, x)));
    }

    public IEnumerable<ResultFilterModel> GetModel()
    {
        return Filters.Select(x => x.GetModel());
    }

    public void Add()
    {
        Filters.Add(new ResultFilterViewModel(LoggerFactory, ApiService));
    }

    public void Remove(ResultFilterViewModel filter)
    {
        Filters.Remove(filter);
    }
}
