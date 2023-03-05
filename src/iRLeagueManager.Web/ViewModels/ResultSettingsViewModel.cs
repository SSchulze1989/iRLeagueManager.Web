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

    public async Task<StatusResult> AddChampionship(PutChampSeasonModel champSeason, CancellationToken cancellationToken = default)
    {
        if (CurrentLeague is null) return LeagueNullResult();
        if (CurrentSeason is null) return SeasonNullResult();
        try
        {
            Loading = true;
            // Post a new championship
            var postChampionship = new PostChampionshipModel() { Name = champSeason.ChampionshipName, DisplayName = champSeason.ChampionshipDisplayName };
            var postChampionshipResult = await CurrentLeague.Championships()
                .Post(postChampionship, cancellationToken);
            if (postChampionshipResult.Success == false || postChampionshipResult.Content is null)
            {
                return postChampionshipResult.ToStatusResult();
            }
            // Post a champ season for the new championship and the current season
            var postChampSeasonResult = await CurrentSeason.Championships()
                .WithId(postChampionshipResult.Content.ChampionshipId)
                .Post(champSeason);
            if (postChampSeasonResult.Success == false || postChampSeasonResult.Content is null)
            {
                return postChampionshipResult.ToStatusResult();
            }
            // Update the champseason with data from model
            var putChampSeasonResult = await CurrentLeague.ChampSeasons()
                .WithId(postChampSeasonResult.Content.ChampSeasonId)
                .Put(champSeason);
            return putChampSeasonResult.ToStatusResult();
        }
        finally 
        { 
            Loading = false; 
        }
    }

    public async Task<StatusResult> DeleteChampionship(ChampionshipModel championship, CancellationToken cancellationToken = default)
    {
        if (CurrentLeague is null) return LeagueNullResult();
        try
        {
            Loading = true;
            var result = await CurrentLeague.Championships()
                .WithId(championship.ChampionshipId)
                .Delete(cancellationToken);
            if (result.Success == false)
            {
                return result.ToStatusResult();
            }
            return await LoadFromCurrentSeasonAsync(cancellationToken);
        }
        finally
        {
            Loading = false;
        }
    }
}
