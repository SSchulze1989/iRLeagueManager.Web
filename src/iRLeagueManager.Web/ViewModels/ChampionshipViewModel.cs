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

    public async Task<StatusResult> Load(long championshipId, CancellationToken cancellationToken = default)
    {
        if (CurrentLeague is null)
        {
            return LeagueNullResult();
        }

        try
        {
            Loading = true;
            var result = await CurrentLeague.Championships()
                .WithId(championshipId)
                .Get(cancellationToken);
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

    public async Task<StatusResult> ActivateForSeasonAsync(long? seasonId = null, CancellationToken cancellationToken = default)
    {
        if (ApiService.CurrentLeague is null)
        {
            return LeagueNullResult();
        }
        if (ApiService.CurrentSeason is null && seasonId == null)
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
            var season = seasonId == null ? CurrentSeason! : ApiService.CurrentLeague.Seasons()
                .WithId(seasonId.Value);
            var result = await season
                .Championships()
                .WithId(ChampionshipId)
                .Post(new(), cancellationToken);
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
            var champSeason = Seasons.FirstOrDefault(x => x.SeasonId == CurrentSeason.Id);
            if (champSeason is null)
            {
                return StatusResult.SuccessResult();
            }
            var result = await CurrentLeague
                .ChampSeasons()
                .WithId(champSeason.ChampSeasonId)
                .Delete(cancellationToken);
            if (result.Success == false)
            {
                return result.ToStatusResult();
            }
            var getChampionshipResult = await CurrentLeague
                .Championships()
                .WithId(ChampionshipId)
                .Get(cancellationToken);
            if (getChampionshipResult.Success && getChampionshipResult.Content is not null)
            {
                SetModel(getChampionshipResult.Content);
            }
            return result.ToStatusResult();
        }
        finally
        {
            Loading = false;
        }
    }

    public override void SetModel(ChampionshipModel model)
    {
        base.SetModel(model);
        IsActive = IsChampionshipActive();
    }
}