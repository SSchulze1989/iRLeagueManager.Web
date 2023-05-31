using iRLeagueApiCore.Common.Models;
using iRLeagueManager.Web.Data;

namespace iRLeagueManager.Web.ViewModels;

public sealed class SessionResultViewModel : LeagueViewModelBase<SessionResultViewModel, ResultModel>
{
    public SessionResultViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService) :
        this(loggerFactory, apiService, new ResultModel())
    {
    }

    public SessionResultViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService, ResultModel model) :
        base(loggerFactory, apiService, model)
    {
    }

    public EventResultViewModel? EventResult { get; set; }
    public long SeasonId => model.SeasonId;
    public string SessionName => model.SessionName;
    public int? SessionNr => model.SessionNr;
    public IEnumerable<ResultRowModel> ResultRows => model.ResultRows;
}
