using iRLeagueApiCore.Common.Models;
using iRLeagueManager.Web.Data;
using iRLeagueManager.Web.Extensions;
using System;

namespace iRLeagueManager.Web.ViewModels;

public sealed class ResultSettingsViewModel : LeagueViewModelBase<ResultSettingsViewModel>
{
    public ResultSettingsViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService) :
        base(loggerFactory, apiService)
    {
        championships = new();
        currentChampSeasons = new();
        resultConfigs = new();
    }

    private ObservableCollection<ChampionshipViewModel> championships;
    public ObservableCollection<ChampionshipViewModel> Championships { get => championships; set => Set(ref championships, value); }

    private ObservableCollection<ChampSeasonViewModel> currentChampSeasons;
    public ObservableCollection<ChampSeasonViewModel> CurrentChampSeasons { get => currentChampSeasons; set => Set(ref currentChampSeasons, value); }

    private ObservableCollection<ResultConfigViewModel> resultConfigs;
    public ObservableCollection<ResultConfigViewModel> ResultsConfigs { get => resultConfigs; set => Set(ref resultConfigs, value); }

    private ResultConfigViewModel? selected;
    public ResultConfigViewModel? Selected { get => selected; set => Set(ref selected, value); }

    public async Task<StatusResult> LoadFromCurrentSeasonAsync(CancellationToken cancellationToken = default)
    {
        if (ApiService.CurrentLeague is null)
        {
            return LeagueNullResult();
        }
        if (ApiService.CurrentSeason is null)
        {
            return SeasonNullResult();
        }

        try
        {
            Loading = true;

            var getChampionships = await ApiService.CurrentLeague
                .Championships()
                .Get(cancellationToken);
            if (getChampionships.Success == false || getChampionships.Content is null)
            {
                return getChampionships.ToStatusResult();
            }
            Championships = new(getChampionships.Content.Select(x => new ChampionshipViewModel(LoggerFactory, ApiService, x)));

            var getChampSeasons = await ApiService.CurrentSeason
                .ChampSeasons()
                .Get(cancellationToken);
            if (getChampSeasons.Success == false || getChampSeasons.Content is null)
            {
                return getChampSeasons.ToStatusResult();
            }
            CurrentChampSeasons = new(getChampSeasons.Content.Select(x => new ChampSeasonViewModel(LoggerFactory, ApiService, x)));
            
            return StatusResult.SuccessResult();
        }
        finally
        {
            Loading = false;
        }
    }

    public async Task<StatusResult> AddConfiguration(ResultConfigModel? newConfig = null)
    {
        if (ApiService.CurrentLeague is null)
        {
            return LeagueNullResult();
        }

        try
        {
            Loading = true;
            newConfig ??= new() { Name = "New Config", DisplayName = "New Config" };
            var request = ApiService.CurrentLeague.ResultConfigs()
                .Post(newConfig);
            var result = await request;

            if (result.Success == true && result.Content is not null)
            {
                ResultsConfigs.Add(new ResultConfigViewModel(LoggerFactory, ApiService, result.Content));
            }

            return result.ToStatusResult();
        }
        finally
        {
            Loading = false;
        }
    }

    public async Task<StatusResult> DeleteConfiguration(ResultConfigViewModel config)
    {
        if (ApiService.CurrentLeague is null)
        {
            return LeagueNullResult();
        }

        try
        {
            Loading = true;
            var request = ApiService.CurrentLeague.ResultConfigs()
                .WithId(config.ResultConfigId)
                .Delete();
            var result = await request;

            if (result.Success)
            {
                ResultsConfigs.Remove(config);
            }

            return result.ToStatusResult();
        }
        finally
        {
            Loading = false;
        }
    }
}
