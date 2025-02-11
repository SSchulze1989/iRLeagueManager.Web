using iRLeagueApiCore.Common.Models;
using iRLeagueManager.Web.Data;
using iRLeagueManager.Web.Extensions;

namespace iRLeagueManager.Web.ViewModels;

public sealed class RawSessionResultViewModel : LeagueViewModelBase<RawSessionResultViewModel, RawSessionResultModel>
{
    public RawSessionResultViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService, RawSessionResultModel model) 
        : base(loggerFactory, apiService, model)
    {
    }

    private RawSessionResultModel modelCopy = new();

    public long SessionId { get => model.SessionId; }

    private List<RawResultRowViewModel> resultRows = [];
    public List<RawResultRowViewModel> ResultRows { get => resultRows; set => Set(ref resultRows, value); }

    protected override void SetModel(RawSessionResultModel model)
    {
        base.SetModel(model);
        ResultRows = model.ResultRows
            .Select(x => new RawResultRowViewModel(LoggerFactory, ApiService, x))
            .ToList();
        modelCopy = CopyModel();
    }

    public void Reset()
    {
        ModelHelper.CopyModelProperties(modelCopy, model);
        SetModel(model, changed: false);
    }

    public async Task<StatusResult> SaveAsync(long eventId, CancellationToken cancellationToken)
    {
        if (CurrentLeague is null)
        {
            return LeagueNullResult();
        }

        try
        {
            Loading = true;
            var eventResult = await CurrentLeague
                .Events()
                .WithId(eventId)
                .Results()
                .Raw()
                .Get(cancellationToken).ConfigureAwait(false);
            if (!eventResult.Success || eventResult.Content is null)
            {
                return eventResult.ToStatusResult();
            }

            var sessionResults = eventResult.Content.SessionResults.ToList();
            var currentSessionResult = sessionResults.FirstOrDefault(x => x.SessionId == SessionId);
            if (currentSessionResult is null)
            {
                return StatusResult.FailedResult("Failed", "SessionId not found in event result", []);
            }

            var currentSessionResultIndex = sessionResults.IndexOf(currentSessionResult);
            sessionResults[currentSessionResultIndex] = model;
            eventResult.Content.SessionResults = sessionResults;

            var putResult = await CurrentLeague
                .Events()
                .WithId(eventId)
                .Results()
                .Raw()
                .Put(eventResult.Content, cancellationToken);
            if (!putResult.Success || putResult.Content is null)
            {
                return putResult.ToStatusResult();
            }
            currentSessionResult = putResult.Content.SessionResults.FirstOrDefault(x => x.SessionId == SessionId);
            if (currentSessionResult is null)
            {
                return StatusResult.FailedResult("Failed", "SessionId not found in event result", []);
            }
            ModelHelper.CopyModelProperties(currentSessionResult, model);
            ResetChangedState();

            return StatusResult.SuccessResult();
        }
        finally
        {
            Loading = false;
        }
    }
}
