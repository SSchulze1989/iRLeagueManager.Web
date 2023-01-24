using iRLeagueApiCore.Client.Http;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.IdentityModel.Tokens.Jwt;

namespace iRLeagueManager.Web.Data;

internal sealed class AsyncTokenStore : ITokenStore
{
    private readonly ILogger<AsyncTokenStore> logger;
    private readonly ProtectedLocalStorage localStore;

    private const string tokenKey = "idToken";

    private string inMemoryIdToken = string.Empty;
    private string inMemoryAccessToken = string.Empty;

    public event EventHandler? TokenChanged;
    public event EventHandler? TokenExpired;

    public bool IsLoggedIn { get; private set; }
    private bool AccessTokenExpired { get; set; } = false;
    public DateTime IdTokenExpires { get; private set; }
    public DateTime AccessTokenExpires { get; private set; }

    public AsyncTokenStore(ILogger<AsyncTokenStore> logger, ProtectedLocalStorage localStorage)
    {
        this.logger = logger;
        this.localStore = localStorage;
    }

    public async Task ClearTokensAsync()
    {
        var tokenValue = inMemoryIdToken;

        logger.LogDebug("Clear token in local browser store");
        IsLoggedIn = false;
        inMemoryIdToken = string.Empty;
        await localStore.DeleteAsync(tokenKey);
        await Task.FromResult(true);
        if (inMemoryIdToken != tokenValue)
        {
            TokenChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public async Task<string> GetIdTokenAsync()
    {
        if (string.IsNullOrEmpty(inMemoryIdToken) == false)
        {
            return inMemoryIdToken;
        }

        logger.LogDebug("Reading token from local browser store");
        try
        {
            //if (contextAccessor.HttpContext?.Session.IsAvailable == true)
            //{
            //    string token = contextAccessor.HttpContext?.Session.GetString(tokenKey) ?? string.Empty;
            //    return await Task.FromResult(token);
            //}
            var token = await localStore.GetAsync<string>(tokenKey);
            if (token.Success)
            {
                if (IsLoggedIn == false)
                {
                    // set expiration date
                    var handler = new JwtSecurityTokenHandler();
                    var jsonToken = handler.ReadJwtToken(token.Value);
                    if (jsonToken.Claims.Any(x => x.Type == "exp"))
                    {
                        var expSeconds = Convert.ToInt64(jsonToken.Claims.First(x => x.Type == "exp").Value);
                        IdTokenExpires = new DateTime(1970, 1, 1).AddSeconds(expSeconds);
                    }
                }

                // check if token is still valid
                if (IdTokenExpires < DateTime.UtcNow.AddMinutes(5))
                {
                    await ClearTokensAsync();
                    logger.LogInformation("Token read from token store has expired");
                    return string.Empty;
                }
                IsLoggedIn = true;
                return inMemoryIdToken = token.Value ?? string.Empty;
            }
            IsLoggedIn = false;
            return string.Empty;
        }
        catch (Exception ex)
        {
            logger.LogError("Could not read from local browser session: {Exception}", ex);
            return string.Empty;
        }
    }

    public async Task SetIdTokenAsync(string token)
    {
        var oldToken = inMemoryIdToken;
        logger.LogDebug("Set token to local browser session: {Token}", token);
        await localStore.SetAsync(tokenKey, token);
        inMemoryIdToken = token;

        if (inMemoryIdToken != oldToken)
        {
            TokenChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public async Task SetAccessTokenAsync(string token)
    {
        inMemoryAccessToken = token;
        AccessTokenExpired = false;

        if (string.IsNullOrEmpty(inMemoryAccessToken) == false)
        {
            // set expiration date
            var jwtToken = new JwtSecurityTokenHandler().ReadToken(inMemoryAccessToken);
            AccessTokenExpires = jwtToken.ValidTo;
        }

        TokenChanged?.Invoke(this, EventArgs.Empty);
        await Task.CompletedTask;
    }

    public async Task<string> GetAccessTokenAsync()
    {
        if (AccessTokenExpires <= DateTime.UtcNow && string.IsNullOrEmpty(inMemoryAccessToken) == false && AccessTokenExpired == false)
        {
            AccessTokenExpired = true;
            TokenExpired?.Invoke(this, EventArgs.Empty);
        }
        return await Task.FromResult(inMemoryAccessToken);
    }
}
