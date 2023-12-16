using iRLeagueApiCore.Common.Enums;
using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Common.Models.Results;
using iRLeagueManager.Web.Data;
using iRLeagueManager.Web.Extensions;

namespace iRLeagueManager.Web.ViewModels;

public sealed class AddBonusViewModel : LeagueViewModelBase<AddBonusViewModel, AddBonusModel>
{
    public AddBonusViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService)
        : this(loggerFactory, apiService, new())
    {
    }

    public AddBonusViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService, AddBonusModel model) 
        : base(loggerFactory, apiService, model)
    {
    }

    public long AddBonusId => model.AddBonusId;
    public MemberInfoModel? Member => model.Member;
    public TeamInfoModel? Team => model.Team;

    private ResultModel? sessionResult;
    public ResultModel? SessionResult { get => sessionResult; set => sessionResult = value; }

    public ResultRowModel? ResultRow 
    { 
        get => sessionResult?.ResultRows.FirstOrDefault(x => x.ScoredResultRowId == model.ResultRowId); 
        set => SetP(model.ResultRowId, value => model.ResultRowId = value ?? 0, value?.ScoredResultRowId); 
    }

    public string Reason { get => model.Reason; set => SetP(model.Reason, value => model.Reason = value, value); }
    public double BonusPoints { get => model.BonusPoints; set => SetP(model.BonusPoints, value => model.BonusPoints = value, value); }

    public async Task<StatusResult> SaveChanges(CancellationToken cancellationToken = default)
    {
        if (CurrentLeague is null)
        {
            return LeagueNullResult();
        }

        try
        {
            Loading = true;
            var result = await CurrentLeague
                .Bonuses()
                .WithId(AddBonusId)
                .Put(model, cancellationToken);
            if (result.Success && result.Content is not null)
            {
                SetModel(result.Content);
            }
            return result.ToStatusResult();
        }
        finally
        {
            Loading = false;
        }
    }

    public async Task<StatusResult> SaveNew(CancellationToken cancellationToken = default)
    {
        if (CurrentLeague is null)
        {
            return LeagueNullResult();
        }

        try
        {
            Loading = true;
            if (SessionResult is null)
            {
                return StatusResult.FailedResult("Session result null", "Session result is required but was null", Array.Empty<object>());
            }
            if (ResultRow is null)
            {
                return StatusResult.FailedResult("Result row null", "Result row is required but was null", Array.Empty<object>());
            }
            var result = await CurrentLeague
                .SessionResults()
                .WithId(SessionResult.SessionResultId)
                .Rows()
                .WithId(ResultRow.ScoredResultRowId)
                .Bonuses()
                .Post(model, cancellationToken);
            if (result.Success && result.Content is not null)
            {
                SetModel(result.Content);
            }
            return result.ToStatusResult();
        }
        finally
        {
            Loading = false;
        }
    }

    public async Task<StatusResult> Delete(CancellationToken cancellationToken = default)
    {
        if (CurrentLeague is null)
        {
            return LeagueNullResult();
        }

        try
        {
            Loading = true;
            var result = await CurrentLeague
                .Bonuses()
                .WithId(AddBonusId)
                .Delete(cancellationToken);
            return result.ToStatusResult();
        }
        finally
        {
            Loading = false;
        }
    }
}
