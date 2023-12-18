using iRLeagueApiCore.Common.Enums;
using iRLeagueApiCore.Common.Models;
using iRLeagueManager.Web.Data;
using iRLeagueManager.Web.Extensions;

namespace iRLeagueManager.Web.ViewModels;

public sealed class AddPenaltyViewModel : LeagueViewModelBase<AddPenaltyViewModel, PenaltyModel>
{
    public AddPenaltyViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService)
        : this(loggerFactory, apiService, new())
    {
    }

    public AddPenaltyViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService, PenaltyModel model) 
        : base(loggerFactory, apiService, model)
    {
    }

    public long AddPenaltyId => model.AddPenaltyId ?? 0;
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
    public PenaltyType Type { get => model.Type; set => SetP(model.Type, value => model.Type = value, value); }
    public int Points { get => -model.Points; set => SetP(model.Points, value => model.Points = value, -value); }
    public int Positions { get => model.Positions; set => SetP(model.Positions, value => model.Positions = value, value); }
    public TimeSpan Time { get => model.Time; set => SetP(model.Time, value => model.Time = value, value); }
    public int TimeSeconds { get => (int)model.Time.TotalSeconds; set => SetP(TimeSeconds, value => model.Time = TimeSpan.FromSeconds(value), value); }

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
                .Penalties()
                .WithId(AddPenaltyId)
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
            var route = $"ScoredSessionResults/{SessionResult.SessionResultId}/Rows/{ResultRow.ScoredResultRowId}/Penalties";
            var endpoint = CurrentLeague
                .CustomEndpoint<PenaltyModel>(route);
            var result = await endpoint
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
                .Penalties()
                .WithId(AddPenaltyId)
                .Delete(cancellationToken);
            return result.ToStatusResult();
        }
        finally
        {
            Loading = false;
        }
    }
}
