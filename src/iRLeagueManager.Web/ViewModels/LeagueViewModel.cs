﻿using iRLeagueApiCore.Common.Enums;
using iRLeagueApiCore.Common.Models;
using iRLeagueManager.Web.Data;
using iRLeagueManager.Web.Extensions;

namespace iRLeagueManager.Web.ViewModels;

public sealed class LeagueViewModel : LeagueViewModelBase<LeagueViewModel, LeagueModel>
{
    public LeagueViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService) :
        this(loggerFactory, apiService, new())
    {
    }

    public LeagueViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService, LeagueModel model) :
        base(loggerFactory, apiService, model)
    {
        seasons = new ObservableCollection<SeasonViewModel>();
    }

    public long LeagueId => model.Id;
    public string LeagueName { get => model.Name; set => SetP(model.Name, value => model.Name = value, value); }
    public string NameFull { get => model.NameFull; set => SetP(model.NameFull, value => model.NameFull = value, value); }
    public string Description { get => model.Description; set => SetP(model.Description, value => model.Description = value, value); }

    private ObservableCollection<SeasonViewModel> seasons;
    public ObservableCollection<SeasonViewModel> Seasons { get => seasons; set => Set(ref seasons, value); }

    private SeasonViewModel? currentSeason;
    public new SeasonViewModel? CurrentSeason { get => currentSeason; private set => Set(ref currentSeason, value); }

    public bool IsInitialized => model.IsInitialized;
    public bool EnableProtests { get => model.EnableProtests; set => SetP(model.EnableProtests, value => model.EnableProtests = value, value); }
    public TimeSpan ProtestCoolDownPeriod { get => model.ProtestCoolDownPeriod; set => SetP(model.ProtestCoolDownPeriod, value => model.ProtestCoolDownPeriod = value, value); }
    public int CoolDownHrs { get => (int)ProtestCoolDownPeriod.TotalHours; set => SetP((int)ProtestCoolDownPeriod.TotalHours, value => model.ProtestCoolDownPeriod = SetHours(model.ProtestCoolDownPeriod, value), value); }
    public int CoolDownMinutes { get => ProtestCoolDownPeriod.Minutes; set => SetP(ProtestCoolDownPeriod.Minutes, value => model.ProtestCoolDownPeriod = SetMinutes(model.ProtestCoolDownPeriod, value), value); }
    public TimeSpan ProtestsClosedAfter { get => model.ProtestsClosedAfter; set => SetP(model.ProtestsClosedAfter, value => model.ProtestsClosedAfter = value, value); }
    public int ProtestsClosedHrs { get => (int)model.ProtestsClosedAfter.TotalHours; set => SetP((int)model.ProtestsClosedAfter.TotalHours, value => model.ProtestsClosedAfter = SetHours(model.ProtestsClosedAfter, value), value); }
    public int ProtestsClosedMinutes { get => model.ProtestsClosedAfter.Minutes; set => SetP(model.ProtestsClosedAfter.Minutes, value => model.ProtestsClosedAfter = SetMinutes(model.ProtestsClosedAfter, value), value); }
    public ProtestPublicSetting ProtestPublic { get => model.ProtestsPublic; set => SetP(model.ProtestsPublic, value => model.ProtestsPublic = value, value); }
    public LeaguePublicSetting LeaguePublic { get => model.LeaguePublic; set => SetP(model.LeaguePublic, value => model.LeaguePublic = value, value); }

    private TimeSpan SetHours(TimeSpan time, int hours)
    {
        time = TimeSpan.FromMinutes(time.Minutes);
        time = time.Add(TimeSpan.FromHours(hours));
        return time;
    }

    private TimeSpan SetMinutes(TimeSpan time, int minutes)
    {
        time = time.Subtract(TimeSpan.FromMinutes(time.Minutes));
        time = time.Add(TimeSpan.FromMinutes(minutes));
        return time;
    }

    public async Task<StatusResult> LoadCurrent(CancellationToken cancellationToken = default)
    {
        if (CurrentLeague is null)
        {
            return LeagueNullResult();
        }

        try
        {
            Loading = true;
            var request = ApiService.Client.Leagues()
                .WithName(CurrentLeague.Name)
                .Get(cancellationToken).ConfigureAwait(false);
            var result = await request;
            if (result.Success && result.Content is LeagueModel leagueModel)
            {
                SetModel(leagueModel);
            }
            return result.ToStatusResult();
        }
        finally
        {
            Loading = false;
        }
    }

    public async Task<StatusResult> LoadSeasons(CancellationToken cancellationToken = default)
    {
        if (CurrentLeague is null)
        {
            return LeagueNullResult();
        }

        try
        {
            Loading = true;
            var request = CurrentLeague.Seasons()
                .Get(cancellationToken).ConfigureAwait(false);
            var result = await request;
            if (result.Success && result.Content is IEnumerable<SeasonModel> seasonModels)
            {
                Seasons = new(seasonModels.Select(x => new SeasonViewModel(LoggerFactory, ApiService, x)));
                ApiService.Shared.SeasonList = new(Seasons.Select(x => x.GetModel()));
            }
            var currentSeason = await CurrentLeague.Seasons().Current().Get().ConfigureAwait(false);
            if (currentSeason.Success && currentSeason.Content is SeasonModel season)
            {
                CurrentSeason = Seasons.FirstOrDefault(x => x.SeasonId == season.SeasonId);
            }
            if (CurrentSeason is null)
            {
                CurrentSeason = Seasons.LastOrDefault();
            }
            return result.ToStatusResult();
        }
        finally
        {
            Loading = false;
        }
    }

    public async Task<StatusResult> AddSeason(PostSeasonModel season, CancellationToken cancellationToken = default)
    {
        if (CurrentLeague is null)
        {
            return LeagueNullResult();
        }

        try
        {
            Loading = true;
            var request = CurrentLeague.Seasons()
                .Post(season, cancellationToken).ConfigureAwait(false);
            var result = await request;
            if (result.Success == false || result.Content is not SeasonModel newSeason)
            {
                return result.ToStatusResult();
            }
            var scheduleRequest = CurrentLeague.Seasons()
                .WithId(newSeason.SeasonId)
                .Schedules()
                .Post(new() { Name = "Schedule" }, cancellationToken).ConfigureAwait(false);
            await scheduleRequest;
            var activateSeasons = await ActivateChampSeasons(base.CurrentSeason?.Id, result.Content.SeasonId, cancellationToken);
            if (activateSeasons.IsSuccess == false) return activateSeasons;
            return await LoadSeasons(cancellationToken);
        }
        finally
        {
            Loading = false;
        }
    }

    public async Task<StatusResult> ActivateChampSeasons(long? previousSeasonId, long? seasonId = null, CancellationToken cancellationToken = default)
    {
        if (CurrentLeague is null) return LeagueNullResult();
        seasonId ??= base.CurrentSeason?.Id;
        if (seasonId == null) return SeasonNullResult();
        // if previous season is same as current or previous season is null => success (none action taken)
        if (previousSeasonId == null || previousSeasonId == seasonId) return StatusResult.SuccessResult();

        try
        {
            Loading = true;
            // get active champ seasons from previous season
            var champSeasonsResult = await CurrentLeague.Seasons()
                .WithId(previousSeasonId.Value)
                .ChampSeasons()
                .Get(cancellationToken).ConfigureAwait(false);
            if (champSeasonsResult.Success == false || champSeasonsResult.Content is null)
            {
                return champSeasonsResult.ToStatusResult();
            }
            foreach (var champSeason in champSeasonsResult.Content)
            {
                var activateChampSeasonResult = await CurrentLeague.Seasons()
                    .WithId(seasonId.Value)
                    .Championships()
                    .WithId(champSeason.ChampionshipId)
                    .Post(new(), cancellationToken).ConfigureAwait(false);
                if (activateChampSeasonResult.Success == false || activateChampSeasonResult.Content is null)
                {
                    return activateChampSeasonResult.ToStatusResult();
                }
                var putChampSeason = activateChampSeasonResult.Content;
                putChampSeason.DefaultResultConfig = putChampSeason.ResultConfigs.FirstOrDefault(x => x.Name == champSeason.DefaultResultConfig?.Name);
                var setDefaultResultConfig = await CurrentLeague
                    .ChampSeasons()
                    .WithId(activateChampSeasonResult.Content.ChampSeasonId)
                    .Put(putChampSeason, cancellationToken).ConfigureAwait(false);
            }
            return StatusResult.SuccessResult();
        }
        finally
        {
            Loading = false;
        }
    }

    public async Task<StatusResult> DeleteSeason(SeasonModel season, CancellationToken cancellationToken = default)
    {
        if (CurrentLeague is null)
        {
            return LeagueNullResult();
        }

        try
        {
            Loading = true;
            var request = CurrentLeague.Seasons()
                .WithId(season.SeasonId)
                .Delete(cancellationToken).ConfigureAwait(false);
            var result = await request;
            if (result.Success)
            {
                return await LoadSeasons(cancellationToken);
            }
            return result.ToStatusResult();
        }
        finally
        {
            Loading = false;
        }
    }

    public async Task<StatusResult> SaveChangesAsync(CancellationToken cancellationToken)
    {
        try
        {
            Loading = true;
            var request = ApiService.Client.Leagues()
                .WithId(LeagueId)
                .Put(model, cancellationToken).ConfigureAwait(false);
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

    public async Task<StatusResult> InitializeLeague(CancellationToken cancellationToken = default)
    {
        try
        {
            Loading = true;
            var result = await ApiService.Client.Leagues()
                .WithId(LeagueId)
                .Initialize()
                .Post(cancellationToken).ConfigureAwait(false);
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
}