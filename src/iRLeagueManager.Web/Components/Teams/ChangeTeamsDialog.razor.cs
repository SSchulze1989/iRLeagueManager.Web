using iRLeagueApiCore.Common.Models;
using iRLeagueManager.Web.Data;
using iRLeagueManager.Web.Exceptions;
using iRLeagueManager.Web.Extensions;
using iRLeagueManager.Web.Pages;
using iRLeagueManager.Web.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MudBlazor;

namespace iRLeagueManager.Web.Components;

public partial class ChangeTeamsDialog : UtilityComponentBase
{
    [Inject]
    private LeagueApiService ApiService { get; set; } = default!;


    [CascadingParameter]
    IMudDialogInstance ModalInstance { get; set; } = default!;
    [Parameter]
    public MemberInfoModel Member { get; set; } = default!;
    [Parameter]
    public SeasonModel? InitialSeason { get; set; }

    private bool selectWholeSeason = false;

    private TeamInfoModel? newTeam;

    private TeamInfoModel? currentTeam;

    public IEnumerable<TeamInfoModel> Teams { get; set; } = [];

    private SeasonModel season = default!;
    private SeasonModel Season
    {
        get => season;
        set
        {
            if (season != value)
            {
                season = value;
                InvokeAsync(SeasonChanged);
            }
        }
    }

    private enum ChangeTeamState
    {
        NoAction = 0,
        ToChange,
        Changing,
        Changed,
    };

    private class SelectResultTeam
    {
        public long EventId { get; set; }
        public bool Selected { get; set; } = false;
        public TeamInfoModel? Team { get; set; }
        public int RaceNr { get; set; }
        public DateTime Date { get; set; }
        public string TrackName { get; set; } = string.Empty;
        public string ConfigName { get; set; } = string.Empty;
        public ChangeTeamState State { get; set; }
    }

    private IEnumerable<SelectResultTeam> memberTeamResults = [];

    private bool CanSubmit => memberTeamResults.Any(x => (x.Selected || selectWholeSeason) && x.Team?.TeamId != newTeam?.TeamId);

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        BlazorParameterNullException.ThrowIfNull(this, ModalInstance, cascading: true);
        BlazorParameterNullException.ThrowIfNull(this, Member);

        season = InitialSeason ?? ApiService.Shared.SeasonList
            .FirstOrDefault(x => x.SeasonId == ApiService.Shared.SeasonId)
            ?? new();
    }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender == false)
        {
            return;
        }
        await LoadTeams();
        await LoadSeasonResults(season.SeasonId);
    }

    private async Task SeasonChanged()
    {
        await LoadSeasonResults(Season.SeasonId);
    }

    private async Task<StatusResult> LoadSeasonResults(long seasonId)
    {
        if (ApiService.CurrentLeague is null)
        {
            return LeagueNullResult();
        }

        try
        {
            Loading = true;

            var request = ApiService.CurrentLeague.Seasons()
                .WithId(seasonId)
                .Results();
            var result = await request.Get(CancellationToken);

            if (result.Success && result.Content is IEnumerable<SeasonEventResultModel> eventResults)
            {
                memberTeamResults = eventResults
                    .Select((x, i) => MapToMemberTeamResult(x, i))
                    .NotNull()
                    .ToList();
                newTeam = currentTeam = memberTeamResults.LastOrDefault()
                    ?.Team;
                await InvokeAsync(StateHasChanged);
            }

            return result.ToStatusResult();
        }
        finally
        {
            Loading = false;
        }
    }

    private async Task<StatusResult> LoadTeams()
    {
        if (ApiService.CurrentLeague is null)
        {
            return LeagueNullResult();
        }

        try
        {
            Loading = true;
            var result = await ApiService.CurrentLeague.Teams().Get();
            if (result.Success && result.Content is IEnumerable<TeamModel> teams)
            {
                Teams = teams.Select(x => new TeamInfoModel()
                {
                    Name = x.Name,
                    TeamColor = x.TeamColor,
                    TeamId = x.TeamId,
                });
            }
            return result.ToStatusResult();
        }
        finally
        {
            Loading = false;
        }
    }

    private async Task<StatusResult> ChangeTeam(SelectResultTeam selectResult)
    {
        if (ApiService.CurrentLeague is null)
        {
            return LeagueNullResult();
        }

        try
        {
            Loading = true;

            // Todo: Modify result rows for event
            // 1. try fetching raw event result
            var rawResultRequest = await ApiService.CurrentLeague
                .Events()
                .WithId(selectResult.EventId)
                .Results()
                .Raw()
                .Get(CancellationToken);
            if (rawResultRequest.Success == false ||  rawResultRequest.Content is not RawEventResultModel rawEventResult)
            {
                return rawResultRequest.ToStatusResult();
            }

            // 2. Get member result rows that need team updated
            var rows = rawEventResult.SessionResults
                .SelectMany(x => x.ResultRows)
                .Where(x => x.MemberId == Member.MemberId)
                .ToList();

            // 3. update team and send update request
            foreach (var (row, index) in rows.WithIndex())
            {
                row.TeamId = newTeam?.TeamId;
                var modRowRequest = await ApiService.CurrentLeague
                    .Results()
                    .ModifyResultRow(row.ResultRowId)
                    .Put(row, CancellationToken);
            }

            return StatusResult.SuccessResult();
        }
        finally
        {
            Loading = false;
        }
    }

    private async Task<StatusResult> TriggerCalculation(long eventId)
    {
        if (ApiService.CurrentLeague is null)
        {
            return LeagueNullResult();
        }

        try
        {
            Loading = false;
            var request = await ApiService.CurrentLeague
                .Events()
                .WithId(eventId)
                .Results()
                .Calculate()
                .Post(CancellationToken);
            return request.ToStatusResult();
        }
        finally
        {
            Loading = true;
        }
    }

    private SelectResultTeam? MapToMemberTeamResult(SeasonEventResultModel? seasonEventResult, int index)
    {
        var eventResult = seasonEventResult?.EventResults.FirstOrDefault();
        if (eventResult == null)
        {
            return null;
        }

        var row = eventResult.SessionResults
            .SelectMany(x => x.ResultRows)
            .FirstOrDefault(x => x.MemberId == Member.MemberId);
        if (row == null)
        {
            return null;
        }

        var team = Teams.FirstOrDefault(x => x.TeamId == row.TeamId);
        return new()
        {
            EventId = eventResult.EventId,
            RaceNr = index + 1,
            Team = team,
            Date = eventResult.Date,
            TrackName = eventResult.TrackName,
            ConfigName = eventResult.ConfigName,
        };
    }

    private async Task<IEnumerable<TeamInfoModel?>> SearchTeam(string term, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(term))
        {
            return await Task.FromResult(Teams);
        }

        var separatedTerms = term.Split(' ', ',')
            .NotNull();
        return Teams
            .Where(team => separatedTerms.Any(x => team.Name.Contains(x, StringComparison.OrdinalIgnoreCase)))
            .Cast<TeamInfoModel?>();
    }

    private async Task Submit()
    {
        IEnumerable<SelectResultTeam> updateResults = updateResults = memberTeamResults
            .Where(x => (x.Selected || selectWholeSeason) && x.Team?.TeamId != newTeam?.TeamId);
        foreach (var result in updateResults)
        {
            result.State = ChangeTeamState.ToChange;
        }
        foreach (var result in updateResults)
        {
            result.State = ChangeTeamState.Changing;
            await InvokeAsync(StateHasChanged);
            await ChangeTeam(result);
            await Task.Delay(200);
            result.State = ChangeTeamState.Changed;
            await InvokeAsync(StateHasChanged);
            await TriggerCalculation(result.EventId);
        }
        await Task.Delay(2000);
        await LoadSeasonResults(Season.SeasonId);
    }
}
