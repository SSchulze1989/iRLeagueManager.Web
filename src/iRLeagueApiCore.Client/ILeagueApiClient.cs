using iRLeagueApiCore.Client.Endpoints;
using iRLeagueApiCore.Client.Endpoints.Leagues;
using iRLeagueApiCore.Client.Endpoints.Register;
using iRLeagueApiCore.Client.Endpoints.Seasons;
using iRLeagueApiCore.Client.Endpoints.Tracks;
using iRLeagueApiCore.Client.Endpoints.Users;
using iRLeagueApiCore.Client.Results;
using iRLeagueApiCore.Common.Models.Users;

namespace iRLeagueApiCore.Client;

public interface ILeagueApiClient
{
    bool IsLoggedIn { get; }
    Uri? BaseAddress { get; }
    ILeagueByNameEndpoint? CurrentLeague { get; }
    ISeasonByIdEndpoint? CurrentSeason { get; }
    ILeaguesEndpoint Leagues();
    IUsersEndpoint Users();
    ITracksEndpoint Tracks();
    ICustomEndpoint<T> CustomEndpoint<T>(string route);
    Task<ClientActionResult<LoginResponse>> LogIn(string username, string password, CancellationToken cancellationToken = default);
    Task Reauthorize(CancellationToken cancellationToken = default);
    Task CheckLogin(CancellationToken cancellationToken = default);
    Task LogOut();
    IAuthenticateEndpoint Authenticate();
    void SetCurrentLeague(string leagueName);
    void SetCurrentSeason(string leagueName, long seasonId);
}
