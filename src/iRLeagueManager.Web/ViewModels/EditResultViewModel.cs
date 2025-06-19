using iRLeagueApiCore.Common.Models;
using iRLeagueManager.Web.Data;
using iRLeagueManager.Web.Extensions;

namespace iRLeagueManager.Web.ViewModels;

public sealed class EditResultViewModel : LeagueViewModelBase<EditResultViewModel, RawEventResultModel>
{
    public EditResultViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService, EventListViewModel eventList)
        : base(loggerFactory, apiService, new())
    {
        this.eventList = eventList;
    }

    private readonly EventListViewModel eventList;

    private List<TeamInfoModel> teams = [];
    public List<TeamInfoModel> Teams { get => teams; set => Set(ref teams, value); }

    private List<MemberInfoModel> members = [];
    public List<MemberInfoModel> Members { get => members; set => Set(ref members, value); }

    private List<RawSessionResultViewModel> sessionResults = [];
    public List<RawSessionResultViewModel> SessionResults { get => sessionResults; set => Set(ref sessionResults, value); }

    public async Task<StatusResult> Load(long eventId, CancellationToken cancellationToken = default)
    {
        if (CurrentLeague is null)
        {
            return LeagueNullResult();
        }

        try
        {
            Loading = true;
            var request = CurrentLeague
                .Events()
                .WithId(eventId)
                .Results()
                .Raw();
            var result = await request.Get(cancellationToken).ConfigureAwait(false);
            if (!result.Success || result.Content is not RawEventResultModel model)
            {
                return result.ToStatusResult();
            }
            SetModel(model);

            var loadMembers = await CurrentLeague
                .Members()
                .Get(cancellationToken).ConfigureAwait(false);
            if (!loadMembers.Success || loadMembers.Content is null)
            {
                return loadMembers.ToStatusResult();
            }
            Members = loadMembers.Content.Select(x => (MemberInfoModel)x).ToList();

            var loadTeams = await CurrentLeague
                .Teams()
                .Get(cancellationToken).ConfigureAwait(false);
            if (!loadTeams.Success || loadTeams.Content is null)
            {
                return loadTeams.ToStatusResult();
            }
            Teams = loadTeams.Content
                .Select(team => new TeamInfoModel()
                {
                    TeamId = team.TeamId,
                    Name = team.Name,
                    TeamColor = team.TeamColor,
                })
                .ToList();

            return StatusResult.SuccessResult();
        }
        finally
        {
            Loading = false;
        }
    }

    public async Task<StatusResult> SaveAsync(CancellationToken cancellationToken)
    {
        if (CurrentLeague is null)
        {
            return LeagueNullResult();
        }

        try
        {
            Loading = true;
            var result = await CurrentLeague.Events()
                .WithId(model.EventId)
                .Results()
                .Raw()
                .Put(model, cancellationToken).ConfigureAwait(false);
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

    protected override void SetModel(RawEventResultModel model)
    {
        base.SetModel(model);
        SessionResults = model.SessionResults
            .Select(x => new RawSessionResultViewModel(LoggerFactory, ApiService, x)
            {
                ParentViewModel = this,
            })
            .ToList();
    }

    public async Task<StatusResult> CalculateResults(CancellationToken cancellationToken)
    {
        try
        {
            Loading = true;
            ResultsPageViewModel resultsPageVm = new(LoggerFactory, ApiService, eventList)
            {
                SelectedEvent = eventList.Selected
            };
            return await resultsPageVm.TriggerCalculation(cancellationToken);
        }
        finally
        {
            Loading = false;
        }
    }
}
