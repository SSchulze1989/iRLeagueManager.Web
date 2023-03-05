using iRLeagueApiCore.Common.Models;
using iRLeagueManager.Web.Data;
using iRLeagueManager.Web.Extensions;
using System.Collections.Concurrent;

namespace iRLeagueManager.Web.ViewModels;

public sealed class ChampSeasonViewModel : LeagueViewModelBase<ChampSeasonViewModel, ChampSeasonModel>
{
    public ChampSeasonViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService) :
        this(loggerFactory, apiService, new())
    { }

    public ChampSeasonViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService, ChampSeasonModel model) : 
        base(loggerFactory, apiService, model)
    {
        resultConfigViewModels = new List<ResultConfigViewModel>();
    }

    public long ChampSeasonId => model.ChampSeasonId;
    public long ChampionshipId => model.ChampionshipId;
    public long SeasonId => model.SeasonId;
    public string ChampionshipName { get => model.ChampionshipName; set => SetP(model.ChampionshipName, value => model.ChampionshipName = value, value); }
    public string ChampionshipDisplayName { get => model.ChampionshipDisplayName; set => SetP(model.ChampionshipDisplayName, value => model.ChampionshipDisplayName = value, value); }
    public string SeasonName => model.SeasonName;

    private StandingConfigurationViewModel? standingConfig;
    public StandingConfigurationViewModel? StandingConfig { get => standingConfig; private set => Set(ref standingConfig, value); }

    public ICollection<ResultConfigInfoModel> ResultConfigs => model.ResultConfigs;

    private ICollection<ResultConfigViewModel> resultConfigViewModels;
    public ICollection<ResultConfigViewModel> ResultConfigViewModels { get => resultConfigViewModels; set => Set(ref resultConfigViewModels, value); }

    private StandingConfigurationViewModel? standingConfigViewModel;
    public StandingConfigurationViewModel? StandingConfigViewModel { get => standingConfigViewModel; set => Set(ref standingConfigViewModel, value); }

    public async Task<StatusResult> SaveChangesAsync(CancellationToken cancellationToken)
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
                .Put(model, cancellationToken);
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
            List<ResultConfigModel> configModels = new();
            foreach(var configInfo in model.ResultConfigs)
            {
                var result = await ApiService.CurrentLeague
                    .ResultConfigs()
                    .WithId(configInfo.ResultConfigId)
                    .Get(cancellationToken);
                if (result.Success == false || result.Content is null)
                { 
                    return result.ToStatusResult();
                }
                configModels.Add(result.Content);
            }
            ResultConfigViewModels = configModels.Select(x => new ResultConfigViewModel(LoggerFactory, ApiService, x)).ToList();
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
            var result = await CurrentLeague.ResultConfigs()
                .Post(config, cancellationToken);
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
                .Delete(cancellationToken);
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

    public override void SetModel(ChampSeasonModel model)
    {
        base.SetModel(model);
        StandingConfig = model.StandingConfig == null ? null : new(LoggerFactory, ApiService, model.StandingConfig);
    }
}