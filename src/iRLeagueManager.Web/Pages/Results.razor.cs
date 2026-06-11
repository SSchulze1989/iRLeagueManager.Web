using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Common.Models.Rosters;
using iRLeagueManager.Web.Components;
using iRLeagueManager.Web.Data;
using iRLeagueManager.Web.Data.CsvExporter;
using iRLeagueManager.Web.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using System.Text;

namespace iRLeagueManager.Web.Pages;

public partial class Results
{
    [CascadingParameter]
    public SharedStateService SharedState { get; set; } = default!;
    [CascadingParameter]
    public Breakpoint Breakpoint { get; set; }
    private const string resultTabParam = "resultTab";
    [Parameter]
    [SupplyParameterFromQuery(Name = resultTabParam)]
    public int SelectedTabIndexParam { get; set; } = 0;

    private int selectedTabIndex;
    private int SelectedTabIndex
    {
        get => selectedTabIndex;
        set
        {
            if (selectedTabIndex != value)
            {
                selectedTabIndex = value;
                NavigationManager.NavigateTo(GetTabLink(value), replace: true);
            }
        }
    }

    protected override async Task OnInitializedAsync()
    {
        await Task.FromResult(true);
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        selectedTabIndex = SelectedTabIndexParam;
    }

    protected override async Task OnEventChangedAsync(EventViewModel? @event)
    {
        if (@event?.EventId != null)
        {
            await vm.LoadFromEventAsync(@event.EventId, CancellationToken);
            var resultCount = vm.Results.Count();
            if (SelectedTabIndex < 0)
            {
                selectedTabIndex = 0;
            }
            if (resultCount > 0 && selectedTabIndex >= resultCount)
            {
                selectedTabIndex = resultCount - 1;
            }
            var navUri = $"/{LeagueName}/Results/Events/{@event.EventId}";
            bool replace = NavigationManager.Uri.Contains($"/Events/") == false || NavigationManager.Uri.Contains(navUri);
            navUri = $"{navUri}?{resultTabParam}={SelectedTabIndex}";
            NavigateTo(navUri, replace: replace);
        }
    }

    private string GetTabLink(int index)
    {
        var url = NavigationManager.GetUriWithQueryParameter(resultTabParam, index);
        return url;
    }

    private async Task UploadButtonClick()
    {
        var @event = vm.SelectedEvent?.GetModel();
        if (@event is null)
        {
            return;
        }
        var parameters = new DialogParameters<UploadResultDialog>()
        {
            { x => x.Event, @event },
        };
        var result = await DialogService.Show<UploadResultDialog>("Upload json result", parameters).Result;
        if (result?.Canceled == false)
        {
            await Task.Delay(2000);
            await vm.LoadFromEventAsync(@event.Id, CancellationToken);
        }
    }

    private async Task FetchResultsClick()
    {
        var @event = vm.SelectedEvent?.GetModel();
        if (@event is null)
        {
            return;
        }
        var parameters = new DialogParameters<FetchResultsDialog>()
        {
            { x => x.Event, @event },
        };
        var result = await DialogService.Show<FetchResultsDialog>("Fetch results from iracing subsession", parameters).Result;
        if (result?.Canceled == false)
        {
            await Task.Delay(2000);
            await vm.LoadFromEventAsync(@event.Id, CancellationToken);
        }
    }

    private async Task TriggerCalculation()
    {
        if (vm.Loading)
        {
            return;
        }
        await vm.TriggerCalculation(CancellationToken);
    }

    private async Task ChangeTabOrder()
    {
        var settingsViewModel = ServiceProvider.GetRequiredService<ResultSettingsViewModel>();
        await settingsViewModel.LoadFromCurrentSeasonAsync(CancellationToken);
        var parameters = new DialogParameters<ReorderChampionshipsDialog>() {
            { x => x.Value, settingsViewModel.CurrentChampSeasons.ToList() }
        };
        var result = await DialogService.Show<ReorderChampionshipsDialog>("Change tab order", parameters).Result;
        if (result?.Canceled == false && Event is not null)
        {
            await vm.LoadFromEventAsync(Event.EventId, CancellationToken);
        }
    }

    private async Task DeleteResultsClick()
    {
        var @event = vm.SelectedEvent;
        if (@event is null)
        {
            return;
        }
        var dialogText = $"""
            Really clear results for event ""{@event.Name} - {@event.Date.GetValueOrDefault().ToString(@"dd.MM.yyyy")}""?<br/>
            This will also remove all added penalties and bonus points.
            """;
        var parameters = new DialogParameters<ConfirmDialog>()
        {
            { x => x.Text, dialogText },
            { x => x.ButtonTypes, ButtonTypes.YesNo },
            { x => x.AllowMarkup, true },
        };
        var result = await DialogService.Show<ConfirmDialog>("Clear Results", parameters).Result;
        if (result?.Canceled == false)
        {
            await vm.DeleteEventResults(CancellationToken);
        }
    }


    private string GetLatestResultLink()
    {
        return $"{NavigationManager.BaseUri}{LeagueName}/Results";
    }

    private string GetCurrentSeasonLatestResultLink()
    {
        if (Shared.SeasonName == null)
        {
            return GetLatestResultLink();
        }
        return $"{NavigationManager.BaseUri}{LeagueName}/Results/Seasons/{Shared.SeasonId}";
    }

    private string GetCurrentResultLink()
    {
        if (Event is null)
        {
            return GetCurrentSeasonLatestResultLink();
        }
        return $"{NavigationManager.BaseUri}{LeagueName}/Results/Events/{Event.EventId}";
    }

    private async Task ExportResults()
    {
        if (vm.Results is null || vm.Results.Count == 0)
        {
            return;
        }

        var currentResult = vm.Results.ElementAtOrDefault(SelectedTabIndex);
        if (currentResult is null)
        {
            return;
        }

        var supportedExporters = ServiceProvider.GetServices<IResultsCsvGenerator>().ToDictionary(k => k.GetName(), v => v);
        var parameters = new DialogParameters<RadioSelectPromptDialog<string>>()
        {
            {x => x.Label, "CSV Exporter"},
            {x => x.Value, supportedExporters.Keys.FirstOrDefault()},
            {x => x.Items, supportedExporters.Keys},
        };

        var result = await DialogService.Show<RadioSelectPromptDialog<string>>("Select CSV Exporter", parameters).Result;
        if (result?.Canceled != false || result.Data is not string exporter)
        {
            return;
        }

        var selectedExporter = supportedExporters.GetValueOrDefault(exporter);
        if (selectedExporter is null)
        {
            return;
        }

        using var zipStream = new MemoryStream();
        using (var archive = new System.IO.Compression.ZipArchive(zipStream, System.IO.Compression.ZipArchiveMode.Create, true))
        {
            foreach (var sessionResult in currentResult.SessionResults)
            {
                var csv = selectedExporter.ExportCsv(sessionResult.GetModel());
                var filenameBuilder = new StringBuilder();
                filenameBuilder.Append($"event_{vm.SelectedEvent?.EventId}_");
                if (vm.Results.Count > 1)
                {
                    filenameBuilder.Append($"{currentResult.Name.Replace(" ", "-")}_");
                }
                if (currentResult.SessionResults.Count > 1)
                {
                    filenameBuilder.Append($"{sessionResult.SessionName.Replace(" ", "-")}_");
                }
                filenameBuilder.Append("results.csv");
                var filename = filenameBuilder.ToString();
                var entry = archive.CreateEntry(filename);
                using var entryStream = entry.Open();
                using var writer = new StreamWriter(entryStream);
                writer.Write(csv);
            }
        }

        zipStream.Position = 0;
        using var zipStreamRef = new DotNetStreamReference(stream: zipStream);
        await JS.InvokeVoidAsync("downloadFileFromStream", $"event_{vm.SelectedEvent?.EventId}_grid.zip", zipStreamRef);
    }

    private async Task ExportGrid()
    {
        if (vm.Rosters is null || vm.Rosters.Count == 0)
        {
            return;
        }
        var supportedGridExporters = ServiceProvider.GetServices<IGridCsvExporter>().ToDictionary(k => k.GetName(), v => v);
        var supportedRosterExporters = ServiceProvider.GetServices<IRosterListCsvExporter>().ToDictionary(k => k.GetName(), v => v);
        var supportedTeamExporters = ServiceProvider.GetServices<ITeamListCsvGenerator>().ToDictionary(k => k.GetName(), v => v);
        var supportedExporters = supportedGridExporters.Keys
            .Concat(supportedRosterExporters.Keys)
            .Concat(supportedTeamExporters.Keys)
            .Distinct()
            .Select(key => new
            {
                Grid = supportedGridExporters.GetValueOrDefault(key)!,
                Roster = supportedRosterExporters.GetValueOrDefault(key)!,
                Team = supportedTeamExporters.GetValueOrDefault(key)!
            })
            .Where(x => x.Grid != null && x.Roster != null && x.Team != null)
            .ToDictionary(k => k.Grid.GetName(), v => v);
        var parameters = new DialogParameters<RadioSelectPromptDialog<string>>()
        {
            {x => x.Label, "CSV Exporter"},
            {x => x.Value, supportedExporters.Keys.FirstOrDefault()},
            {x => x.Items, supportedExporters.Keys},
        };

        var result = await DialogService.Show<RadioSelectPromptDialog<string>>("Select CSV Exporter", parameters).Result;

        if (result?.Canceled != false || result.Data is not string exporter)
        {
            return;
        }

        var selectedExporters = supportedExporters.GetValueOrDefault(exporter);
        if (selectedExporters is null)
        {
            return;
        }

        using var zipStream = new MemoryStream();
        using (var archive = new System.IO.Compression.ZipArchive(zipStream, System.IO.Compression.ZipArchiveMode.Create, true))
        {
            // Export Teams
            {
                var teams = vm.Rosters
                    .SelectMany(x => x.RosterEntries)
                    .Where(x => x.TeamId != null)
                    .Select(x => new TeamModel()
                    {
                        Name = x.TeamName,
                        TeamColor = x.TeamColor,
                        TeamId = x.TeamId!.Value,
                    })
                    .DistinctBy(x => x.TeamId)
                    .ToList();
                var csv = selectedExporters.Team.ExportCsv(teams);
                var entry = archive.CreateEntry($"event_{vm.SelectedEvent?.EventId}_teams.csv");
                using var entryStream = entry.Open();
                using var writer = new StreamWriter(entryStream);
                writer.Write(csv);
            }

            // Export Rosters
            {
                var rosterInfos = vm.Rosters.Select(x => new RosterInfoModel()
                {
                    Description = x.Description,
                    RosterId = x.RosterId,
                    EntryCount = x.RosterEntries.Count(),
                    Name = x.Name,
                });
                var csv = selectedExporters.Roster.ExportCsv(rosterInfos);
                var entry = archive.CreateEntry($"event_{vm.SelectedEvent?.EventId}_rosters.csv");
                using var entryStream = entry.Open();
                using var writer = new StreamWriter(entryStream);
                writer.Write(csv);
            }

            // Export Grid
            {
                var csv = selectedExporters.Grid.ExportCsv(vm.Rosters);
                var entry = archive.CreateEntry($"event_{vm.SelectedEvent?.EventId}_drivers.csv");
                using var entryStream = entry.Open();
                using var writer = new StreamWriter(entryStream);
                writer.Write(csv);
            }
        }

        zipStream.Position = 0;
        using var zipStreamRef = new DotNetStreamReference(stream: zipStream);
        await JS.InvokeVoidAsync("downloadFileFromStream", $"event_{vm.SelectedEvent?.EventId}_grid.zip", zipStreamRef);
    }
}
