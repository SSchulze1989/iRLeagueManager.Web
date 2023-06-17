using iRLeagueApiCore.Common.Models;
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

    public async Task<StatusResult> AddPenalty(PenaltyModel penalty, CancellationToken cancellationToken = default)
    {
        if (CurrentLeague is null)
        {
            return LeagueNullResult();
        }
        if (penalty.MemberId == default)
        {
            return StatusResult.FailedResult("MemberId missing", "Member id is required on PenaltyModel but was default", Array.Empty<object>());
        }

        try
        {
            Loading = true;
            var route = $"ScoredSessionResults/{SessionResultId}/Drivers/{penalty.MemberId}/Penalties";
            var result = await CurrentLeague
                .CustomEndpoint<PenaltyModel>(route)
                .Post(penalty, cancellationToken);
            return result.ToStatusResult();
        }
        finally
        {
            Loading = false;
        }
    }
}
