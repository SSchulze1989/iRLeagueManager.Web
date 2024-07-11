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
    [CascadingParameter] MudDialogInstance MudDialog { get; set; } = default!;

    [Parameter] public IEnumerable<EventModel> Events { get; set; } = default!;
    [Parameter] public StandingsModel Standing { get; set; } = default!;
    [Parameter] public StandingRowModel StandingRow { get; set; } = default!;

    private bool loading = false;
    private IEnumerable<DropweekOverrideModel> dropweekOverrides = [];

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
            return StatusResult.FailedResult("League Null", $"{nameof(LeagueApiService)}.{nameof(LeagueApiService.CurrentLeague)} was null", Array.Empty<object>());
        }

        using var cts = new CancellationTokenSource();
        try
        {
            loading = true;
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
            loading = false;
        }
    }

    //private async Task ToggleDropweek(bool isScored, DropweekOverrideModel dropweekOverride) 
    //{ 

    //}
}
