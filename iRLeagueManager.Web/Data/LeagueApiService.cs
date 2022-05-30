﻿using iRLeagueApiCore.Client;
using iRLeagueApiCore.Client.Endpoints.Leagues;
using iRLeagueApiCore.Client.Endpoints.Seasons;
using iRLeagueApiCore.Communication.Models;
using System.Collections.ObjectModel;

namespace iRLeagueManager.Web.Data;

public class LeagueApiService
{
    //private readonly ILogger<LeagueApiService> logger;

    public LeagueApiService(ILeagueApiClient apiClient, SharedStateService sharedState)
    {
        //this.logger = logger;
        Client = apiClient;
        Shared = sharedState;
    }

    public ILeagueApiClient Client { get; }
    public SharedStateService Shared { get; }
    public ILeagueByNameEndpoint? CurrentLeague { get; private set; }
    public ISeasonByIdEndpoint? CurrentSeason { get; private set; }

    public async Task SetCurrentLeagueAsync(string leagueName)
    {
        if (CurrentLeague?.Name == leagueName)
        {
            return;
        }

        CurrentLeague = Client
            .Leagues()
            .WithName(leagueName);
        Shared.LeagueName = leagueName;
        CurrentSeason = null;
        Shared.SeasonId = 0;
        Shared.SeasonName = string.Empty;
        var result = await CurrentLeague.Seasons().Get();
        if (result.Success)
        {
            var seasons = result.Content;
            Shared.SeasonList = new ObservableCollection<SeasonModel>(seasons);
        }
    }

    public async Task SetCurrentSeasonAsync(string leagueName, long seasonId)
    {
        if (seasonId == CurrentSeason?.Id)
        {
            return;
        }

        // update season id if changed
        await SetCurrentLeagueAsync(leagueName);
        CurrentSeason = Client
            .Leagues()
            .WithName(leagueName)
            .Seasons()
            .WithId(seasonId);

        // get name and update state
        var result = await CurrentSeason.Get();
        if (result.Success)
        {
            var season = result.Content;
            Shared.SeasonId = seasonId;
            Shared.SeasonName = season.SeasonName;
        }
    }
}

public static class LeagueApiServiceExtensions
{
    public static void AddLeagueApiService(this IServiceCollection services)
    {
        services.AddScoped<SharedStateService>();
        services.AddScoped<LeagueApiService>();
    }
}
