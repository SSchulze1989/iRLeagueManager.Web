using iRLeagueApiCore.Common.Models;
using iRLeagueManager.Web.Data;
using iRLeagueManager.Web.Extensions;
using System;
using System.Diagnostics.Contracts;

namespace iRLeagueManager.Web.ViewModels;

public sealed class ChampionshipViewModel : LeagueViewModelBase<ChampionshipViewModel, ChampionshipModel>
{
    public ChampionshipViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService) :
        this(loggerFactory, apiService, new())
    { }

    public ChampionshipViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService, ChampionshipModel model) : 
        base(loggerFactory, apiService, model)
    { }

    public long ChampionshipId => model.ChampionshipId;
    public string Name { get => model.Name; set => SetP(model.Name, value => model.Name = value, value); }
    public string DisplayName { get => model.DisplayName; set => SetP(model.DisplayName, value => model.DisplayName = value, value); }
    public IReadOnlyCollection<ChampSeasonInfoModel> Seasons => model.Seasons;
    public bool IsActive { get => isActive; private set => Set(ref isActive, value); }
    private bool isActive;

    public async Task<StatusResult> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        if (ApiService.CurrentLeague is null)
        {
            return LeagueNullResult();
        }

        try
        {
            Loading = true;
            var request = ApiService.CurrentLeague
                .Championships()
                .WithId(ChampionshipId)
                .Put(model, cancellationToken);
            var result = await request;
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

    private bool IsChampionshipActive()
    {
        return Seasons.Any(x => x.SeasonId == ApiService.CurrentSeason?.Id);
    }

    public async Task<StatusResult> ActivateForCurrentSeasonAsync(CancellationToken cancellationToken = default)
    {
        if (ApiService.CurrentLeague is null)
        {
            return LeagueNullResult();
        }
        if (ApiService.CurrentSeason is null)
        {
            return SeasonNullResult();
        }
        if (IsChampionshipActive())
        {
            isActive = true;
            return StatusResult.SuccessResult("Championship already active for current season");
        }

        try
        {
            Loading = true;
            var previousSeasonId = Seasons.LastOrDefault()?.ChampSeasonId;
            ChampSeasonModel? previousSeason = default;
            if (previousSeasonId != null)
            {
                var previousSeasonResult = await ApiService.CurrentLeague
                    .ChampSeasons()
                    .WithId(previousSeasonId.GetValueOrDefault())
                    .Get(cancellationToken);
                if (previousSeasonResult.Success && previousSeasonResult.Content is not null)
                {
                    previousSeason = previousSeasonResult.Content;
                }
            }
            var result = await ApiService.CurrentSeason
                .Championships()
                .WithId(ChampionshipId)
                .Post(CreateChampSeason(previousSeason));
            if (result.Success == false)
            {
                return result.ToStatusResult();
            }
            var getChampionshipResult = await ApiService.CurrentLeague
                .Championships()
                .WithId(ChampionshipId)
                .Get(cancellationToken);
            if (getChampionshipResult.Success && getChampionshipResult.Content is not null)
            {
                SetModel(getChampionshipResult.Content);
            }
            IsActive = IsChampionshipActive();
            return getChampionshipResult.ToStatusResult();
        }
        finally
        {
            Loading = false;
        }
    }

    public async Task<StatusResult> DeactivateForCurrentSeasonAsync(CancellationToken cancellationToken = default)
    {
        IsActive = false;

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
            var champSeason = Seasons.FirstOrDefault(x => x.SeasonId == ApiService.CurrentSeason.Id);
            if (champSeason is null)
            {
                return StatusResult.SuccessResult();
            }
            var result = await ApiService.CurrentLeague
                .ChampSeasons()
                .WithId(champSeason.ChampSeasonId)
                .Delete();
            if (result.Success == false)
            {
                return result.ToStatusResult();
            }
            var getChampionshipResult = await ApiService.CurrentLeague
                .Championships()
                .WithId(ChampionshipId)
                .Get();
            if (getChampionshipResult.Success && getChampionshipResult.Content is not null)
            {
                SetModel(getChampionshipResult.Content);
            }
            return getChampionshipResult.ToStatusResult();
        }
        finally
        {
            Loading = false;
        }
    }

    private ChampSeasonModel CreateChampSeason(ChampSeasonModel? previousChampSeason)
    {
        // make copies of previous settings
        var standingConfig = ModelHelper.CopyModel(previousChampSeason?.StandingConfig);
        var resultConfigs = previousChampSeason?.ResultConfigs
            .Select(x => ModelHelper.CopyModel(x))
            .NotNull()
            .ToList() ?? new();
        var champSeason = new ChampSeasonModel()
        {
            StandingConfig = standingConfig,
            ResultConfigs = resultConfigs,
        };
        return champSeason;
    }

    public override void SetModel(ChampionshipModel model)
    {
        base.SetModel(model);
        IsActive = IsChampionshipActive();
    }
}