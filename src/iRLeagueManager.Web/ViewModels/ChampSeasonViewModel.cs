﻿using iRLeagueApiCore.Common.Enums;
using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Common.Models.Rosters;
using iRLeagueManager.Web.Data;
using iRLeagueManager.Web.Extensions;
using iRLeagueManager.Web.Shared;

namespace iRLeagueManager.Web.ViewModels;

public sealed class ChampSeasonViewModel : LeagueViewModelBase<ChampSeasonViewModel, ChampSeasonModel>
{
    public ChampSeasonViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService) :
        this(loggerFactory, apiService, new())
    { }

    public ChampSeasonViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService, ChampSeasonModel model) : 
        base(loggerFactory, apiService, model)
    {
        resultConfigViewModels = [];
    }

    public long ChampSeasonId => model.ChampSeasonId;
    public long ChampionshipId => model.ChampionshipId;
    public long SeasonId => model.SeasonId;
    public int Index { get => model.Index; set => SetP(model.Index, value => model.Index = value, value); }
    public string ChampionshipName { get => model.ChampionshipName; set => SetP(model.ChampionshipName, value => model.ChampionshipName = value, value); }
    public string ChampionshipDisplayName { get => model.ChampionshipDisplayName; set => SetP(model.ChampionshipDisplayName, value => model.ChampionshipDisplayName = value, value); }
    public string SeasonName => model.SeasonName;
    public RosterInfoModel? Roster { get => model.Roster; set => SetP(model.Roster, value => model.Roster = value, value); }
    public ResultKind ResultKind { get => model.ResultKind; set => SetP(model.ResultKind, value => model.ResultKind = value, value); }
    public IEnumerable<ResultFilterModel> Filters { get => model.Filters; set => SetP(model.Filters, value => model.Filters = value.ToList(), value); }
    public IEnumerable<FilterConditionModel> FilterConditions { get => model.Filters.Select(x => x.Condition); set => SetFilterConditions(value); }

    private StandingConfigurationViewModel? standingConfig;
    public StandingConfigurationViewModel? StandingConfig 
    { 
        get => standingConfig;
        private set
        {
            if (standingConfig is not null)
            {
                standingConfig.PropertyChanged -= ChildViewModel_PropertyChanged;
            }
            Set(ref standingConfig, value);
            if (standingConfig is not null)
            {
                standingConfig.PropertyChanged += ChildViewModel_PropertyChanged;
            }
        }
    }

    private void ChildViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (sender is IModelState childState == false)
        {
            return;
        }
        switch (e.PropertyName)
        {
            case nameof(HasChanges):
                HasChanges |= childState.HasChanges;
                break;
        }
    }

    public ICollection<ResultConfigInfoModel> ResultConfigs => model.ResultConfigs;
    public ResultConfigInfoModel? DefaultResultConfig { get => model.DefaultResultConfig; set => SetP(model.DefaultResultConfig, value => model.DefaultResultConfig = value, value); }

    private ICollection<ResultConfigViewModel> resultConfigViewModels;
    public ICollection<ResultConfigViewModel> ResultConfigViewModels { get => resultConfigViewModels; set => Set(ref resultConfigViewModels, value); }

    private ResultConfigViewModel NewResultConfigViewModel(ResultConfigModel model)
    {
        var viewModel = new ResultConfigViewModel(LoggerFactory, ApiService, model)
        {
            ParentViewModel = this,
        };
        return viewModel;
    }

    private void SetFilterConditions(IEnumerable<FilterConditionModel> conditions)
    {
        var updatedFilters = Filters.ToList();
        for (int i=0; i<Math.Max(Filters.Count(), conditions.Count()); i++)
        {
            var filter = Filters.ElementAtOrDefault(i);
            var condition = conditions.ElementAtOrDefault(i);
            if (condition is null)
            {
                updatedFilters.Remove(filter!);
                continue;
            }
            if (filter is null)
            {
                filter = new();
                updatedFilters.Add(filter);
            }
            filter.Condition = condition;
        }
        Filters = updatedFilters;
    }

    public async Task<StatusResult> Load(long championshipId, CancellationToken cancellationToken = default)
    {
        if (CurrentLeague is null)
        {
            return LeagueNullResult();
        }
        if (CurrentSeason is null)
        {
            return SeasonNullResult();
        }

        try
        {
            Loading = true;
            var result = await CurrentSeason.Championships()
                .WithId(championshipId)
                .Get(cancellationToken).ConfigureAwait(false);
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

    public async Task<StatusResult> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        if (ApiService.CurrentLeague is null)
        {
            return LeagueNullResult();
        }

        try
        {
            Loading = true;
            var result = await ApiService.CurrentLeague
                .ChampSeasons()
                .WithId(ChampSeasonId)
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

    public async Task<StatusResult> LoadResultConfigs(CancellationToken cancellationToken = default)
    {
        if (ApiService.CurrentLeague is null)
        {
            return LeagueNullResult();
        }

        try
        {
            Loading = true;
            List<ResultConfigModel> configModels = [];
            foreach(var configInfo in model.ResultConfigs)
            {
                var result = await ApiService.CurrentLeague
                    .ResultConfigs()
                    .WithId(configInfo.ResultConfigId)
                    .Get(cancellationToken).ConfigureAwait(false);
                if (result.Success == false || result.Content is null)
                { 
                    return result.ToStatusResult();
                }
                configModels.Add(result.Content);
            }
            ResultConfigViewModels = configModels.Select(NewResultConfigViewModel).ToList();
            return StatusResult.SuccessResult();
        }
        finally
        {
            Loading = false;
        }
    }

    public async Task<StatusResult> AddResultConfig(ResultConfigModel config, CancellationToken cancellationToken = default)
    {
        if (CurrentLeague is null)
        {
            return LeagueNullResult();
        }

        try
        {
            Loading = true;
            var result = await CurrentLeague.ChampSeasons()
                .WithId(model.ChampSeasonId)
                .ResultConfigs()
                .Post(config, cancellationToken).ConfigureAwait(false);
            if (result.Success == false || result.Content is null)
            {
                return result.ToStatusResult();
            }
            config = result.Content;
            model.ResultConfigs.Add(new ResultConfigInfoModel()
            {
                Name = config.Name,
                DisplayName = config.DisplayName,
                LeagueId = config.LeagueId,
                ResultConfigId = config.ResultConfigId,
            });
            ResultConfigViewModels.Add(new(LoggerFactory, ApiService, result.Content));
            return StatusResult.SuccessResult();
        }
        finally
        {
            Loading = false;
        }
    }

    public async Task<StatusResult> DeleteResultConfig(ResultConfigModel config, CancellationToken cancellationToken = default)
    {
        if (CurrentLeague is null)
        {
            return StatusResult.SuccessResult();
        }

        try
        {
            Loading = true;
            var result = await CurrentLeague.ResultConfigs()
                .WithId(config.ResultConfigId)
                .Delete(cancellationToken).ConfigureAwait(false);
            if (result.Success)
            {
                return await LoadResultConfigs(cancellationToken);
            }
            return result.ToStatusResult();
        }
        finally
        { 
            Loading = false; 
        }
    }

    protected override void SetModel(ChampSeasonModel model)
    {
        base.SetModel(model);
        StandingConfig = model.StandingConfig == null ? null : new(LoggerFactory, ApiService, model.StandingConfig);
        if (StandingConfig != null)
        {
            StandingConfig.ParentViewModel = this;
        }
        ResetChangedState();
    }
}