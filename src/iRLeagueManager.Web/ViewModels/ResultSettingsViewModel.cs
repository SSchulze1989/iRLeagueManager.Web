﻿using iRLeagueApiCore.Common.Models;
using iRLeagueManager.Web.Data;
using iRLeagueManager.Web.Extensions;
using System;
using System.ComponentModel.Design;

namespace iRLeagueManager.Web.ViewModels;

public sealed class ResultSettingsViewModel : LeagueViewModelBase<ResultSettingsViewModel>
{
    public ResultSettingsViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService) :
        base(loggerFactory, apiService)
    {
        championships = [];
        currentChampSeasons = [];
        resultConfigs = [];
    }

    private List<ChampionshipViewModel> championships;
    public List<ChampionshipViewModel> Championships { get => championships; set => Set(ref championships, value); }

    private List<ChampSeasonViewModel> currentChampSeasons;
    public List<ChampSeasonViewModel> CurrentChampSeasons { get => currentChampSeasons; set => Set(ref currentChampSeasons, value); }

    private List<ResultConfigViewModel> resultConfigs;
    public List<ResultConfigViewModel> ResultsConfigs { get => resultConfigs; set => Set(ref resultConfigs, value); }

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
                .Get(cancellationToken).ConfigureAwait(false);
            if (getChampionships.Success == false || getChampionships.Content is null)
            {
                return getChampionships.ToStatusResult();
            }
            Championships = getChampionships.Content.Select(x => new ChampionshipViewModel(LoggerFactory, ApiService, x)).ToList();

            var getChampSeasons = await ApiService.CurrentSeason
                .ChampSeasons()
                .Get(cancellationToken).ConfigureAwait(false);
            if (getChampSeasons.Success == false || getChampSeasons.Content is null)
            {
                return getChampSeasons.ToStatusResult();
            }
            CurrentChampSeasons = getChampSeasons.Content.Select(x => new ChampSeasonViewModel(LoggerFactory, ApiService, x)).ToList();
            foreach(var championship in Championships)
            {
                championship.CurrentChampSeason = CurrentChampSeasons.FirstOrDefault(x => x.ChampionshipId == championship.ChampionshipId);
            }
            Championships = Championships.OrderBy(x => x.CurrentChampSeason?.Index ?? -1).ToList();
            
            return StatusResult.SuccessResult();
        }
        finally
        {
            Loading = false;
        }
    }

    public async Task<StatusResult> AddChampionship(PutChampSeasonModel champSeason, bool noPointConfigs = false, CancellationToken cancellationToken = default)
    {
        if (CurrentLeague is null) return LeagueNullResult();
        if (CurrentSeason is null) return SeasonNullResult();
        try
        {
            Loading = true;
            // Post a new championship
            var postChampionship = new PostChampionshipModel() { Name = champSeason.ChampionshipName, DisplayName = champSeason.ChampionshipDisplayName };
            var postChampionshipResult = await CurrentLeague.Championships()
                .Post(postChampionship, cancellationToken).ConfigureAwait(false);
            if (postChampionshipResult.Success == false || postChampionshipResult.Content is null)
            {
                return postChampionshipResult.ToStatusResult();
            }
            Championships.Add(new(LoggerFactory, ApiService, postChampionshipResult.Content));
            // Post a champ season for the new championship and the current season
            var postChampSeasonResult = await CurrentSeason.Championships()
                .WithId(postChampionshipResult.Content.ChampionshipId)
                .Post(champSeason, cancellationToken).ConfigureAwait(false);
            if (postChampSeasonResult.Success == false || postChampSeasonResult.Content is null)
            {
                return postChampionshipResult.ToStatusResult();
            }

            if (noPointConfigs == false)
            {
                // Create empty result config
                var resultConfigTemplate = new PostResultConfigModel()
                {
                    Name = "Default Points",
                };
                var postResultConfigResult = await CurrentLeague
                    .ChampSeasons()
                    .WithId(postChampSeasonResult.Content.ChampSeasonId)
                    .ResultConfigs()
                    .Post(resultConfigTemplate, cancellationToken).ConfigureAwait(false);
                if (postResultConfigResult.Success == false || postResultConfigResult.Content is null)
                {
                    return postResultConfigResult.ToStatusResult();
                }
                champSeason.ResultConfigs = new[] { new ResultConfigInfoModel() { ResultConfigId = postResultConfigResult.Content.ResultConfigId } };
                champSeason.DefaultResultConfig = new() { ResultConfigId = postResultConfigResult.Content.ResultConfigId };
            }

            // Update the champseason with data from model
            var putChampSeasonResult = await CurrentLeague.ChampSeasons()
                .WithId(postChampSeasonResult.Content.ChampSeasonId)
                .Put(champSeason, cancellationToken).ConfigureAwait(false);
            if (putChampSeasonResult.Success == false || putChampSeasonResult.Content is null)
            {
                return putChampSeasonResult.ToStatusResult();
            }
            return await LoadFromCurrentSeasonAsync(cancellationToken);
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
                .Delete(cancellationToken).ConfigureAwait(false);
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
