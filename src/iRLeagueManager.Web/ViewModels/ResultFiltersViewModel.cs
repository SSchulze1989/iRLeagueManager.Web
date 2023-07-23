using iRLeagueApiCore.Common.Models;
using iRLeagueManager.Web.Data;
using System.Collections.Specialized;

namespace iRLeagueManager.Web.ViewModels;

public sealed class ResultFiltersViewModel : ViewModelCollection<ResultFilterViewModel, ResultFilterModel, ResultFiltersViewModel>
{
    public ResultFiltersViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService) :
        base(loggerFactory, apiService)
    {
    }

    public override ResultFilterViewModel CreateInstance(ResultFilterModel model)
    {
        return new ResultFilterViewModel(LoggerFactory, ApiService, model);
    }
}
