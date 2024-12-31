using iRLeagueApiCore.Common.Enums;
using iRLeagueApiCore.Common.Models;
using iRLeagueManager.Web.Components;
using iRLeagueManager.Web.Data;
using iRLeagueManager.Web.Shared;
using iRLeagueManager.Web.ViewModels;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Net;

namespace iRLeagueManager.Web.Pages.Settings;

public partial class Templates : LeagueComponentBase
{
    [Inject] private LeagueViewModel Vm { get; set; } = default!;
    [Inject] private IServiceProvider Provider { get; set; } = default!;
    [Inject] IDialogService DialogService { get; set; } = default!;

    private string Status { get; set; } = string.Empty;
    private string Message { get; set; } = string.Empty;
    private IEnumerable<object> Errors { get; set; } = Array.Empty<object>();
    private new bool Loading { get; set; } = false;

    private enum TemplateType
    {
        StandardIracing,
        StandardCustom,
        DriverTeamIracing,
        DriverTeamCuston,
    }

    private Dictionary<TemplateType, (string title, string description)> TemplatesAvailable = new()
    {
        { TemplateType.StandardIracing, ("Standard: iRacing points", "Use points uploaded from iRacing results directly without calculating points on the server" )},
        { TemplateType.StandardCustom, ("Standard: custom points", "Use custom points settings for calculating points on the server" )},
        { TemplateType.DriverTeamIracing, ("Driver + Teams: iRacing points", "Driver &amp; Team championships with points uploaded from iRacing" )},
        { TemplateType.DriverTeamCuston, ("Driver + Teams: custom points", "Driver &amp; Team championships with custom points settings" )},
    };

    [Parameter]
    [SupplyParameterFromQuery]
    public string ReturnUrl { get; set; } = string.Empty;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender == false)
        {
            return;
        }
        await Vm.LoadCurrent(CancellationToken);
        //var result = await ApplyDefaultTemplate(CancellationToken);
    }

    private async Task OnTemplateSelect(TemplateType template)
    {
        var (title, description) = TemplatesAvailable[template];
        Loading = true;
        var parameters = new DialogParameters<ConfirmDialog>()
        {
            { x => x.Text, $"Apply template <b><u>{title}</u></b> to your league?<br/>You can only apply a template once but of course change all the applied settings later."},
            { x => x.AllowMarkup, true},
            { x => x.ButtonTypes, ButtonTypes.YesNo},
        };
        var result = await DialogService.Show<ConfirmDialog>("Apply Template", parameters).Result;
        if (result?.Canceled == false)
        {
            await SubmitTemplateSelection(template);
        }
        Loading = false;
    }

    private async Task SubmitTemplateSelection(TemplateType template)
    {
        var result = await ApplyTemplate(template);
        Status = result.Status;
        Message = result.Message;
        Errors = result.Errors;
        await InvokeAsync(StateHasChanged);
        if (result.IsSuccess == false)
        {
            return;
        }
        await Vm.InitializeLeague(CancellationToken);
        if (string.IsNullOrEmpty(ReturnUrl) == false)
        {
            NavigateTo(WebUtility.UrlDecode(ReturnUrl));
        }
    }

    private async Task<StatusResult> ApplyTemplate(TemplateType template) => template switch
    {
        TemplateType.StandardIracing => await ApplyDefaultTemplate(CancellationToken),
        TemplateType.StandardCustom => await ApplyCustomPointsTemplate(CancellationToken),
        TemplateType.DriverTeamIracing => await ApplyDriverTeamIracingTemplate(CancellationToken),
        TemplateType.DriverTeamCuston => await ApplyDriverTeamCustomTemplate(CancellationToken),
        _ => TemplateFailure($"Unknown template: {template}")
    };

    private async Task<StatusResult> CreateFirstSeason(CancellationToken cancellationToken = default)
    {
        if (LeagueName is null)
        {
            return TemplateFailure("League was null");
        }

        var newSeason = new PostSeasonModel()
        {
            SeasonName = "Season 1",
            Finished = false,
            HideComments = false,
        };
        var result = await Vm.AddSeason(newSeason, cancellationToken);
        if (result.IsSuccess == false)
        {
            return result;
        }
        var season = Vm.Seasons.FirstOrDefault();
        if (season is null)
        {
            return TemplateFailure("Created season was null");
        }
        await ApiService.SetCurrentSeasonAsync(LeagueName, season.SeasonId);
        return result;
    }

    private async Task<StatusResult> CreateChampionship(ResultSettingsViewModel resultSettings, string name, ResultKind resultKind, CancellationToken cancellationToken = default)
    {
        var championship = new PutChampSeasonModel()
        {
            ChampionshipName = name,
            ChampionshipDisplayName = name,
            ResultKind = resultKind,
            StandingConfig = new()
            {
                Name = name,
                ResultKind = resultKind,
                WeeksCounted = 0,
            },
        };
        return await resultSettings.AddChampionship(championship, noPointConfigs: true, cancellationToken: cancellationToken);
    }

    private async Task<StatusResult> ApplyDefaultTemplate(CancellationToken cancellationToken = default)
    {
        StatusResult result = await CreateFirstSeason(cancellationToken);
        if (result.IsSuccess == false)
        {
            return result;
        }

        var resultSettingsVm = Provider.GetRequiredService<ResultSettingsViewModel>();
        result = await CreateChampionship(resultSettingsVm, "Championship", ResultKind.Member, cancellationToken);
        if (result.IsSuccess == false)
        {
            return result;
        }

        var champSeason = resultSettingsVm.CurrentChampSeasons.FirstOrDefault();
        if (champSeason is null)
        {
            return TemplateFailure("Created champseason could not be loaded");
        }

        var champSeasonVm = Provider.GetRequiredService<ChampSeasonViewModel>();
        champSeasonVm.SetModel(champSeason.GetModel());
        var resultConfig = new ResultConfigModel()
        {
            Name = "iRacing Points",
            DisplayName = "iRacing Points",
            Scorings = new[]
                                    {
                new ScoringModel()
                {
                    Name = "Race",
                }
            },
        };
        result = await champSeasonVm.AddResultConfig(resultConfig, cancellationToken);
        if (result.IsSuccess == false)
        {
            return result;
        }

        return StatusResult.SuccessResult();
    }

    private async Task<StatusResult> ApplyCustomPointsTemplate(CancellationToken cancellationToken = default)
    {
        StatusResult result = await CreateFirstSeason();
        if (result.IsSuccess == false)
        {
            return result;
        }

        var resultSettingsVm = Provider.GetRequiredService<ResultSettingsViewModel>();
        result = await CreateChampionship(resultSettingsVm, "Championship", ResultKind.Member, cancellationToken);
        if (result.IsSuccess == false)
        {
            return result;
        }

        var champSeason = resultSettingsVm.CurrentChampSeasons.FirstOrDefault();
        if (champSeason is null)
        {
            return TemplateFailure("Created champseason could not be loaded");
        }
        var champSeasonVm = Provider.GetRequiredService<ChampSeasonViewModel>();
        champSeasonVm.SetModel(champSeason.GetModel());
        var resultConfig = new ResultConfigModel()
        {
            Name = "Custom Points",
            DisplayName = "Custom Points",
            Scorings = new[]
                                    {
                new ScoringModel()
                {
                    Name = "Race",
                    PointRule = new PointRuleModel()
                    {
                        PointsPerPlace = new[] { 10, 9, 8, 7, 6, 5, 4, 3, 2, 1 },
                        PointsSortOptions = new[] { SortOptions.FinPosAsc },
                        FinalSortOptions = new[] { SortOptions.TotalPtsAsc, SortOptions.PenPtsDesc },
                    },
                }
            },
        };
        result = await champSeasonVm.AddResultConfig(resultConfig, cancellationToken);
        if (result.IsSuccess == false)
        {
            return result;
        }

        return StatusResult.SuccessResult();
    }

    private async Task<StatusResult> ApplyDriverTeamIracingTemplate(CancellationToken cancellationToken = default)
    {
        const string driverChampionshipName = "Drivers";
        const string teamChampionshipName = "Teams";

        StatusResult result = await CreateFirstSeason(cancellationToken);
        if (result.IsSuccess == false)
        {
            return result;
        }

        var resultSettingsVm = Provider.GetRequiredService<ResultSettingsViewModel>();
        result = await CreateChampionship(resultSettingsVm, driverChampionshipName, ResultKind.Member, cancellationToken);
        if (result.IsSuccess == false)
        {
            return result;
        }
        result = await CreateChampionship(resultSettingsVm, teamChampionshipName, ResultKind.Team, cancellationToken);
        if (result.IsSuccess == false)
        {
            return result;
        }

        var champSeasonVm = Provider.GetRequiredService<ChampSeasonViewModel>();
        var driverChampSeason = resultSettingsVm.CurrentChampSeasons.FirstOrDefault(x => x.ChampionshipName == driverChampionshipName);
        var teamChampSeason = resultSettingsVm.CurrentChampSeasons.FirstOrDefault(x => x.ChampionshipName == teamChampionshipName);
        if (driverChampSeason is null || teamChampSeason is null)
        {
            return TemplateFailure("Created champseason could not be loaded");
        }
        champSeasonVm.SetModel(driverChampSeason.GetModel());

        var driverResultConfig = new ResultConfigModel()
        {
            Name = "iRacing Points",
            DisplayName = "iRacing Points",
            Scorings = new[]
                                    {
                new ScoringModel()
                {
                    Name = "Race",
                }
            },
        };
        result = await champSeasonVm.AddResultConfig(driverResultConfig, cancellationToken);
        if (result.IsSuccess == false)
        {
            return result;
        }

        var driverResultConfigInfo = champSeasonVm.ResultConfigs.FirstOrDefault();
        if (driverResultConfigInfo is null)
        {
            return TemplateFailure("Created result config could not be loaded");
        }

        champSeasonVm.SetModel(teamChampSeason.GetModel());
        var teamResultConfig = new ResultConfigModel()
        {
            Name = "Sum of Driver points",
            DisplayName = "Sum of Driver points",
            SourceResultConfig = driverResultConfigInfo,
            ResultsPerTeam = 2,
            Scorings = new[]
                                    {
                new ScoringModel()
                {
                    Name = "Race",
                }
            },
        };
        result = await champSeasonVm.AddResultConfig(teamResultConfig, cancellationToken);

        return result;
    }

    private async Task<StatusResult> ApplyDriverTeamCustomTemplate(CancellationToken cancellationToken = default)
    {
        const string driverChampionshipName = "Drivers";
        const string teamChampionshipName = "Teams";

        StatusResult result = await CreateFirstSeason(cancellationToken);
        if (result.IsSuccess == false)
        {
            return result;
        }

        var resultSettingsVm = Provider.GetRequiredService<ResultSettingsViewModel>();
        result = await CreateChampionship(resultSettingsVm, driverChampionshipName, ResultKind.Member, cancellationToken);
        if (result.IsSuccess == false)
        {
            return result;
        }
        result = await CreateChampionship(resultSettingsVm, teamChampionshipName, ResultKind.Team, cancellationToken);
        if (result.IsSuccess == false)
        {
            return result;
        }

        var champSeasonVm = Provider.GetRequiredService<ChampSeasonViewModel>();
        var driverChampSeason = resultSettingsVm.CurrentChampSeasons.FirstOrDefault(x => x.ChampionshipName == driverChampionshipName);
        var teamChampSeason = resultSettingsVm.CurrentChampSeasons.FirstOrDefault(x => x.ChampionshipName == teamChampionshipName);
        if (driverChampSeason is null || teamChampSeason is null)
        {
            return TemplateFailure("Created champseason could not be loaded");
        }
        champSeasonVm.SetModel(driverChampSeason.GetModel());

        var driverResultConfig = new ResultConfigModel()
        {
            Name = "Custom Points",
            DisplayName = "Custom Points",
            Scorings = new[]
                                    {
                new ScoringModel()
                {
                    Name = "Race",
                    PointRule = new PointRuleModel()
                    {
                        PointsPerPlace = new[] { 10, 9, 8, 7, 6, 5, 4, 3, 2, 1 },
                        PointsSortOptions = new[] { SortOptions.FinPosAsc },
                        FinalSortOptions = new[] { SortOptions.TotalPtsAsc, SortOptions.PenPtsDesc },
                    },
                }
            },
        };
        result = await champSeasonVm.AddResultConfig(driverResultConfig, cancellationToken);
        if (result.IsSuccess == false)
        {
            return result;
        }

        var driverResultConfigInfo = champSeasonVm.ResultConfigs.FirstOrDefault();
        if (driverResultConfigInfo is null)
        {
            return TemplateFailure("Created result config could not be loaded");
        }

        champSeasonVm.SetModel(teamChampSeason.GetModel());
        var teamResultConfig = new ResultConfigModel()
        {
            Name = "Sum of Driver points",
            DisplayName = "Sum of Driver points",
            SourceResultConfig = driverResultConfigInfo,
            ResultsPerTeam = 2,
            Scorings = new[]
                                    {
                new ScoringModel()
                {
                    Name = "Race",
                }
            },
        };
        result = await champSeasonVm.AddResultConfig(teamResultConfig, cancellationToken);

        return result;
    }

    private StatusResult TemplateFailure(string reason)
    {
        return StatusResult.FailedResult("Error", "Template failed", new[] { reason });
    }
}
