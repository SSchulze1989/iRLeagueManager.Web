using iRLeagueApiCore.Client.Endpoints;
using iRLeagueApiCore.Client.Endpoints.Leagues;
using iRLeagueApiCore.Client.Endpoints.Register;
using iRLeagueApiCore.Client.Endpoints.Seasons;
using iRLeagueApiCore.Client.Endpoints.Tracks;
using iRLeagueApiCore.Client.Endpoints.Users;
using iRLeagueApiCore.Client.Http;
using iRLeagueApiCore.Client.QueryBuilder;
using iRLeagueApiCore.Client.Results;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;

namespace iRLeagueApiCore.Client;

public sealed class LeagueApiClient : ILeagueApiClient
{
    private readonly ILogger<LeagueApiClient> logger;
    private readonly HttpClientWrapper httpClientWrapper;
    private readonly ITokenStore tokenStore;
    private bool isAuthorizing;

    private string? CurrentLeagueName { get; set; }

    public Uri? BaseAddress => httpClientWrapper.BaseAddress;

    public LeagueApiClient(ILogger<LeagueApiClient> logger, HttpClient httpClient, HttpClientWrapperFactory clientWrapperFactory, ITokenStore tokenStore)
    {
        this.logger = logger;
        httpClientWrapper = clientWrapperFactory.Create(httpClient, tokenStore, this);
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        this.tokenStore = tokenStore;
        tokenStore.TokenChanged += TokenStore_TokenChanged;
        //tokenStore.TokenExpired += TokenStore_TokenExpired;
        TokenStore_TokenChanged(this, EventArgs.Empty);
    }

    ///// <summary>
    ///// Automatically reauthorize if token has expired
    ///// </summary>
    ///// <param name="sender"></param>
    ///// <param name="e"></param>
    //private async void TokenStore_TokenExpired(object? sender, EventArgs e)
    //{
    //    await Reauthorize();
    //}

    /// <summary>
    /// Reauthorize (if required) if the token has changed (e.g. when the token has been read from browser storage)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void TokenStore_TokenChanged(object? sender, EventArgs e)
    {
        var idToken = await tokenStore.GetIdTokenAsync();
        var accessToken = await tokenStore.GetAccessTokenAsync();
        if (string.IsNullOrEmpty(idToken) == false && string.IsNullOrEmpty(accessToken))
        {
            try
            {
                await Reauthorize();
            }
            catch (Exception ex) when (
                ex is InvalidOperationException || 
                ex is ApiServiceUnavailableException ||
                ex is HttpRequestException)
            {
                // do not throw any non system critical Exceptions in async void
                // this will silently fail the reathorization when the service is unavailable
                // TODO: find a way to inform the client consumer about the service state
            }
        }
    }

    public bool IsLoggedIn => tokenStore.IsLoggedIn;

    public ILeagueByNameEndpoint? CurrentLeague { get; private set; }
    public ISeasonByIdEndpoint? CurrentSeason { get; private set; }

    public ILeaguesEndpoint Leagues()
    {
        return new LeaguesEndpoint(httpClientWrapper, new RouteBuilder());
    }

    public IUsersEndpoint Users()
    {
        return new UsersEndpoint(httpClientWrapper, new RouteBuilder());
    }

    public ITracksEndpoint Tracks()
    {
        return new TracksEndpoint(httpClientWrapper, new RouteBuilder());
    }

    /// <summary>
    /// Login with username and password
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Response with id and access token</returns>
    public async Task<ClientActionResult<LoginResponse>> LogIn(string username, string password, CancellationToken cancellationToken = default)
    {
        // request to login endpoint
        await LogOut();

        logger.LogInformation("Log in for {User} ...", username);
        var requestUrl = "Authenticate/Login";
        var body = new
        {
            username = username,
            password = password
        };
        
        var result = await httpClientWrapper.PostAsClientActionResult<LoginResponse>(requestUrl, body, cancellationToken);

        if (result.Success)
        {
            logger.LogInformation("Log in successful!");
            // set authorization header
            string idToken = result.Content.IdToken;
            string accessToken = result.Content.AccessToken;
            await tokenStore.SetIdTokenAsync(idToken);
            await tokenStore.SetAccessTokenAsync(accessToken);
            return result;
        }

        logger.LogError("Login failed: {Status}", result.Status);
        return result;
    }

    /// <summary>
    /// Get access token using a valid idToken
    /// </summary>
    /// <param name="idToken"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Response containing access token</returns>
    public async Task Reauthorize(CancellationToken cancellationToken = default)
    {
        try
        {
            isAuthorizing = true;

            logger.LogInformation("Request access token using id token");

            var requestUrl = "Authenticate/authorize";
            var idToken = await tokenStore.GetIdTokenAsync();
            var body = new
            {
                idToken
            };
            var options = new HttpRequestOptions();
            options.TryAdd("SkipAuth", true);
            var request = httpClientWrapper.CreateRequest(HttpMethod.Post, requestUrl, body, options);
            var response = await httpClientWrapper.SendRequest(request, cancellationToken);
            var result = await httpClientWrapper.ConvertToClientActionResult<AuthorizeResponse>(response, cancellationToken);

            if (result.Success)
            {
                string token = result.Content.AccessToken;
                await tokenStore.SetAccessTokenAsync(token);
                return;
            }

            logger.LogError("Access request failed: {Status}", result.Status);
            await LogOut();
            return;
        }
        finally
        {
            isAuthorizing = false;
        }
    }

    /// <summary>
    /// Check the current login state and reauthorize if required
    /// </summary>
    /// <returns></returns>
    public async Task CheckLogin(CancellationToken cancellationToken = default)
    {
        var idToken = await tokenStore.GetIdTokenAsync();
        if (string.IsNullOrWhiteSpace(idToken))
        {
            // user not logged in
            return;
        }

        if (isAuthorizing)
        {
            // wait until authorizing finished
            var timeout = 5000;
            await Task.WhenAny(Task.Run(() => { while (isAuthorizing) ; }), Task.Delay(timeout, cancellationToken));
        }
        // user logged in but has no access token -> get access through reauthorization
        var accessToken = await tokenStore.GetAccessTokenAsync();
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            await Reauthorize(cancellationToken);
        }

        if (DateTime.UtcNow >= tokenStore.AccessTokenExpires)
        {
            // access token has expired -> reauthoriza
            await Reauthorize(cancellationToken);
        }

        // Do nothing because login is valid
    }

    public async Task LogOut()
    {
        await tokenStore.ClearTokensAsync();
        logger.LogInformation("User logged out");
    }

    public void SetCurrentLeague(string leagueName)
    {
        CurrentLeagueName = leagueName;
        if (string.IsNullOrEmpty(CurrentLeagueName))
        {
            CurrentLeague = null;
            return;
        }

        CurrentLeague = Leagues().WithName(leagueName);
    }

    public void SetCurrentSeason(string leagueName, long seasonId)
    {
        SetCurrentLeague(leagueName);
        if (CurrentLeague == null)
        {
            throw new InvalidOperationException("Could not set current season: current league was null");
        }
        CurrentSeason = CurrentLeague.Seasons().WithId(seasonId);
    }

    ICustomEndpoint<T> ILeagueApiClient.CustomEndpoint<T>(string route)
    {
        return new CustomEndpoint<T>(httpClientWrapper, new(), route);
    }

    IAuthenticateEndpoint ILeagueApiClient.Authenticate()
    {
        return new AuthenticateEndpoint(httpClientWrapper, new());
    }
}
