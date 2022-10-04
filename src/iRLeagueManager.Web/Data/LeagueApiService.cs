using iRLeagueApiCore.Client;
using iRLeagueApiCore.Client.Endpoints.Leagues;
using iRLeagueApiCore.Client.Endpoints.Seasons;
using iRLeagueApiCore.Common.Models;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace iRLeagueManager.Web.Data;

public class LeagueApiService
{
    //private readonly ILogger<LeagueApiService> logger;

    public LeagueApiService(ILeagueApiClient apiClient, SharedStateService sharedState, JsonSerializerOptions jsonOptions)
    {
        //this.logger = logger;
        Client = apiClient;
        Shared = sharedState;
        JsonOptions = jsonOptions;
    }

    public ILeagueApiClient Client { get; }
    public SharedStateService Shared { get; }
    public JsonSerializerOptions JsonOptions { get; }
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
            .Get();
        if (eventRequestResult.Success == false)
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
