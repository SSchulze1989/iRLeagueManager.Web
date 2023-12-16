using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Common.Models.Results;
using iRLeagueManager.Web.Data;
using iRLeagueManager.Web.Extensions;

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
    public long SessionResultId => model.SessionResultId;
    public long SeasonId => model.SeasonId;
    public string SessionName => model.SessionName;
    public int? SessionNr => model.SessionNr;
    public IEnumerable<ResultRowModel> ResultRows => model.ResultRows;

    public async Task<StatusResult<IEnumerable<AddBonusModel>>> GetBonuses(CancellationToken cancellationToken = default)
    {
        if (CurrentLeague is null)
        {
            return new StatusResult<IEnumerable<AddBonusModel>>(LeagueNullResult());
        }

        try
        {
            Loading = true;
            var result = await CurrentLeague
                .SessionResults()
                .WithId(SessionResultId)
                .Bonuses()
                .Get(cancellationToken);
            return result.ToContentStatusResult();
        }
        finally
        {
            Loading = false;
        }
    }
}
