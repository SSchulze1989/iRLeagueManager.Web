using iRLeagueApiCore.Common.Models;
using iRLeagueManager.Web.Data;
using iRLeagueManager.Web.Extensions;
using System.Threading.Tasks;

namespace iRLeagueManager.Web.ViewModels;

public sealed class RawSessionResultViewModel : LeagueViewModelBase<RawSessionResultViewModel, RawSessionResultModel>
{
    public RawSessionResultViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService, RawSessionResultModel model)
        : base(loggerFactory, apiService, model)
    {
    }

    private RawSessionResultModel modelCopy = new();

    public long SessionId { get => model.SessionId; }

    private ObservableCollection<RawResultRowViewModel> resultRows = [];
    public ObservableCollection<RawResultRowViewModel> ResultRows { get => resultRows; set => Set(ref resultRows, value); }

    protected override void SetModel(RawSessionResultModel model)
    {
        base.SetModel(model);
        SetRows(model.ResultRows);
        modelCopy = CopyModel();
    }

    private void SetRows(IEnumerable<RawResultRowModel> rows)
    {
        ResultRows = new (rows
            .Select(x => new RawResultRowViewModel(LoggerFactory, ApiService, x)
            {
                ParentViewModel = this,
            })
            .ToList());
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
                .Put(eventResult.Content, cancellationToken).ConfigureAwait(false);
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

    private void RefreshFinishedPositions()
    {
        for (int i = 0; i < ResultRows.Count; i++)
        {
            var row = ResultRows[i];
            row.FinishPosition = i + 1;
        }
    }

    private async Task<StatusResult<MemberModel>> GetMemberIracingIdAsync(long memberId, CancellationToken cancellationToken)
    {
        if (CurrentLeague is null)
        {
            return LeagueNullResult<MemberModel>(new());
        }
        try
        {
            var membersResult = await CurrentLeague
                .Members()
                .WithId(memberId)
                .Get(cancellationToken)
                .ConfigureAwait(false);
            return membersResult.ToContentStatusResult();
        }
        finally
        {
            Loading = false;
        }
    }

    public async Task AddRow(RawResultRowModel row)
    {
        // get the members iracing id for the new row
        var member = await GetMemberIracingIdAsync(row.MemberId, CancellationToken.None);
        if (member.IsSuccess && member.Content is not null)
        {
            row.IRacingId = member.Content.IRacingId;
        }

        model.ResultRows = [.. model.ResultRows, row];
        ResultRows.Add(new RawResultRowViewModel(LoggerFactory, ApiService, row)
        {
            ParentViewModel = this,
        });
        RefreshFinishedPositions();
        Changed();
    }

    public void RemoveRow(RawResultRowViewModel row)
    {
        ResultRows.Remove(row);
        model.ResultRows = ResultRows.Select(x => x.GetModel());
        RefreshFinishedPositions();
        Changed();
    }
}
