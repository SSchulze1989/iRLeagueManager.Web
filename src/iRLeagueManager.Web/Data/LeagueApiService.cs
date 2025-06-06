﻿using iRLeagueApiCore.Client;
using iRLeagueApiCore.Client.Endpoints.Leagues;
using iRLeagueApiCore.Client.Endpoints.Seasons;
using iRLeagueApiCore.Common.Models;
using System.Text.Json;

namespace iRLeagueManager.Web.Data;

public sealed class LeagueApiService
{
    public LeagueApiService(ILeagueApiClient apiClient, SharedStateService sharedState, ClientLocalTimeProvider localTimeProvider)
    {
        Client = apiClient;
        Shared = sharedState;
        ClientTimeProvider= localTimeProvider;
    }

    public ILeagueApiClient Client { get; }
    public SharedStateService Shared { get; }
    public ClientLocalTimeProvider ClientTimeProvider { get; }
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
        Shared.SeasonFinished = true;
        var leagueResult = await CurrentLeague.Get().ConfigureAwait(false);
        if (leagueResult.Success && leagueResult.Content is not null)
        {
            Shared.LeagueInfo = leagueResult.Content;
        }
        var seasonResult = await CurrentLeague.Seasons().Get().ConfigureAwait(false);
        if (seasonResult.Success && seasonResult.Content is not null)
        {
            var seasons = seasonResult.Content;
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
        var result = await CurrentSeason.Get().ConfigureAwait(false);
        if (result.Success && result.Content is not null)
        {
            var season = result.Content;
            Shared.SeasonId = seasonId;
            Shared.SeasonName = season.SeasonName;
            Shared.SeasonFinished = season.Finished;
        }
    }

    public async Task SetCurrentSeasonByEventId(string leagueName, long eventId)
    {
        await SetCurrentLeagueAsync(leagueName);
        if (CurrentLeague == null)
        {
            return;
        }

        var eventRequestResult = await CurrentLeague
            .Events()
            .WithId(eventId)
            .Get().ConfigureAwait(false);
        if (eventRequestResult.Success == false || eventRequestResult.Content is null)
        {
            return;
        }
        var @event = eventRequestResult.Content;
        await SetCurrentSeasonAsync(leagueName, @event.SeasonId);
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
