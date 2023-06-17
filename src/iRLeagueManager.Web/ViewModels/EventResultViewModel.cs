using iRLeagueApiCore.Common.Models;
using iRLeagueManager.Web.Data;
using iRLeagueManager.Web.Extensions;

namespace iRLeagueManager.Web.ViewModels;

public sealed class EventResultViewModel : LeagueViewModelBase<EventResultViewModel, EventResultModel>
{
    public EventResultViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService) :
        this(loggerFactory, apiService, new EventResultModel())
    {
    }

    public EventResultViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService, EventResultModel model) :
        base(loggerFactory, apiService, model)
    {
        sessionResults = new ObservableCollection<SessionResultViewModel>(model.SessionResults.Select(x => new SessionResultViewModel(loggerFactory, ApiService, x) { EventResult = this }));
    }

    public long ResultId => model.ResultId;
    public long SeasonId => model.SeasonId;
    public long EventId => model.EventId;
    public string Name => model.DisplayName;
    public int StrengthOfField => model.StrengthOfField;

    private ObservableCollection<SessionResultViewModel> sessionResults;
    public ObservableCollection<SessionResultViewModel> SessionResults { get => sessionResults; set => Set(ref sessionResults, value); }

    public async Task<StatusResult<IEnumerable<PenaltyModel>>> GetPenalties(CancellationToken cancellationToken = default)
    {
        if (CurrentLeague is null)
        {
            return new StatusResult<IEnumerable<PenaltyModel>>(LeagueNullResult());
        }

        try
        {
            Loading = true;
            // FIXME: Add missing endpoint
            //var result = await CurrentLeague.Results()
            //    .WithId(ResultId)
            //    .Penalties()
            //    .Get(cancellationToken);
            var route = $"Results/{ResultId}/Penalties";
            var result = await CurrentLeague
                .CustomEndpoint<IEnumerable<PenaltyModel>>(route)
                .Get(cancellationToken);
            return result.ToContentStatusResult();
        }
        finally 
        { 
            Loading = false; 
        }
    }
}
