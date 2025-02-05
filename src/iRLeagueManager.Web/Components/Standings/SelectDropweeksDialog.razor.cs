using iRLeagueApiCore.Common.Models.Standings;
using iRLeagueApiCore.Common.Models;
using iRLeagueManager.Web.Data;
using iRLeagueManager.Web.Exceptions;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using iRLeagueManager.Web.Shared;
using iRLeagueManager.Web.Extensions;

namespace iRLeagueManager.Web.Components;

public partial class SelectDropweeksDialog : UtilityComponentBase
{
    [Inject] LeagueApiService ApiService { get; init; } = default!;
    [CascadingParameter] IMudDialogInstance MudDialog { get; set; } = default!;

    [Parameter] public IEnumerable<EventModel> Events { get; set; } = default!;
    [Parameter] public StandingsModel Standing { get; set; } = default!;
    [Parameter] public StandingRowModel StandingRow { get; set; } = default!;

    private IEnumerable<DropweekOverrideModel> dropweekOverrides = [];

    private enum DropMode
    {
        None,
        AlwaysKeep,
        AlwaysDrop,
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        BlazorParameterNullException.ThrowIfNull(this, MudDialog, cascading: true);
        BlazorParameterNullException.ThrowIfNull(this, Events);
        BlazorParameterNullException.ThrowIfNull(this, Standing);
        BlazorParameterNullException.ThrowIfNull(this, StandingRow);
    }

    protected override async void OnAfterRender(bool firstRender)
    {
        base.OnAfterRender(firstRender);
        if (firstRender == false)
        {
            return;
        }
        await LoadDropweekOverrides();
    }

    private async Task<StatusResult> LoadDropweekOverrides()
    {
        if (ApiService.CurrentLeague is null)
        {
            return StatusResult.FailedResult("League Null", $"{nameof(LeagueApiService)}.{nameof(LeagueApiService.CurrentLeague)} was null", []);
        }

        using var cts = new CancellationTokenSource();
        try
        {
            Loading = true;
            var requests = StandingRow.ResultRows
            .NotNull()
                .Select(row =>
                {
                    return ApiService.CurrentLeague.Standings()
                        .WithId(Standing.StandingId)
                        .ResultRows()
                        .WithId(row.ResultRowId)
                        .DropweekOverride()
                        .Get(cts.Token);
                })
            .ToList();

            // wait for parallel requests to finish
            await Task.WhenAny(Task.WhenAll(requests), Task.Delay(5000));

            if (requests.Any(x => x.IsCompleted == false))
            {
                cts.Cancel();
                return StatusResult.FailedResult("Timed out", "Operation timed out", []);
            }
            if (requests.Any(x => x.IsFaulted))
            {
                return StatusResult.FailedResult("Error", "Operation failed", requests.Where(x => x.IsFaulted).Select(x => x.Exception?.Message ?? ""));
            }

            var overrideResults = requests.Select(x => x.Result);
            if (overrideResults.Any(x => x.Success == false && x.HttpStatusCode != System.Net.HttpStatusCode.NotFound))
            {
                return overrideResults.First(x => x.Success == false && x.HttpStatusCode != System.Net.HttpStatusCode.NotFound).ToStatusResult();
            }

            dropweekOverrides = overrideResults
                .Select(x => x.Content)
                .NotNull()
                .ToList();
            return StatusResult.SuccessResult();
        }
        finally
        {
            Loading = false;
        }
    }

    private async Task<StatusResult> DeleteDropweekOverride(long scoredResultRowId)
    {
        if (ApiService.CurrentLeague is null)
        {
            return StatusResult.FailedResult("League Null", $"{nameof(LeagueApiService)}.{nameof(LeagueApiService.CurrentLeague)} was null", []);
        }

        try
        {
            Loading = true;
            var result = await ApiService.CurrentLeague
                .Standings()
                .WithId(Standing.StandingId)
                .ResultRows()
                .WithId(scoredResultRowId)
                .DropweekOverride()
                .Delete(CancellationToken);

            return result.ToStatusResult();
        }
        finally
        {
            Loading = false;
        }
    }

    private async Task<StatusResult<DropweekOverrideModel>> SaveDropweekOverride(long scoredResultRowId, PutDropweekOverrideModel model)
    {
        if (ApiService.CurrentLeague is null)
        {
            return StatusResult<DropweekOverrideModel>.FailedResult("League Null", null, $"{nameof(LeagueApiService)}.{nameof(LeagueApiService.CurrentLeague)} was null", []);
        }

        try
        {
            Loading = true;
            var result = await ApiService.CurrentLeague
                .Standings()
                .WithId(Standing.StandingId)
                .ResultRows()
                .WithId(scoredResultRowId)
                .DropweekOverride()
                .Put(model, CancellationToken);

            return result.ToContentStatusResult();
        }
        finally
        {
            Loading = false;
        }
    }

    private async Task SetDropMode(long scoredResultRowId, DropMode mode)
    {
        switch (mode)
        {
            case DropMode.None:
                await DeleteDropweekOverride(scoredResultRowId);
                break;
            case DropMode.AlwaysKeep:
            case DropMode.AlwaysDrop:
                var shouldDrop = mode == DropMode.AlwaysDrop;
                await SaveDropweekOverride(scoredResultRowId, new()
                {
                    ShouldDrop = shouldDrop,
                });
                break;
        }

        await LoadDropweekOverrides();
        await InvokeAsync(StateHasChanged);
    }

    private async Task<StatusResult> CalculateStandings(long eventId)
    {
        if (ApiService.CurrentLeague is null)
        {
            return LeagueNullResult();
        }

        try
        {
            Loading = true;
            var result = await ApiService.CurrentLeague
                .Events()
                .WithId(eventId)
                .Standings()
                .Calculate()
                .Post(CancellationToken);
            return result.ToStatusResult();
        }
        finally
        {
            Loading = false;
        }
    }

    private async Task Close()
    {
        var latestEvent = Events.LastOrDefault(x => x.HasResult);
        if (latestEvent is not null)
        {
            await CalculateStandings(latestEvent.Id);
        }
        var firstEvent = Events.FirstOrDefault(x => x.HasResult);
        if (firstEvent is not null)
        {
            await CalculateStandings(firstEvent.Id);
        }

        MudDialog.Close();
    }
}
