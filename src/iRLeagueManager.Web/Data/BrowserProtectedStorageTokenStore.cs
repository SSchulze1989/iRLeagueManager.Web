using iRLeagueApiCore.Client.Http;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;

namespace iRLeagueManager.Web.Data;

internal sealed class BrowserProtectedStorageTokenStore : ITokenStore
{
    private readonly ILogger<BrowserProtectedStorageTokenStore> logger;
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

    public BrowserProtectedStorageTokenStore(ILogger<BrowserProtectedStorageTokenStore> logger, ProtectedLocalStorage localStorage)
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
        inMemoryAccessToken = string.Empty;
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
            var token = await localStore.GetAsync<string>(tokenKey);
            if (token.Success)
            {
                return await ValidateToken(token.Value);
            }
            IsLoggedIn = false;
            return string.Empty;
        }
        catch (InvalidOperationException ex)
        {
            logger.LogDebug("Could not read from local browser session: {Exception}", ex);
            return string.Empty;
        }
        catch (Exception ex) when (ex is CryptographicException or AntiforgeryValidationException)
        {
            logger.LogWarning("{Exception}", ex);
            logger.LogWarning("Deleting locally stored identity token");
            await localStore.DeleteAsync(tokenKey);
            return string.Empty;
        }
    }

    private async Task<string> ValidateToken(string? token)
    {
        if (IsLoggedIn == false)
        {
            // set expiration date
            IdTokenExpires = ExtractExpirationDate(token);
        }

        // check if token is still valid
        if (IdTokenExpires < DateTime.UtcNow.AddMinutes(5))
        {
            await ClearTokensAsync();
            logger.LogInformation("Token read from token store has expired");
            return string.Empty;
        }
        IsLoggedIn = true;
        return inMemoryIdToken = token ?? string.Empty;
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

    private static DateTime ExtractExpirationDate(string? token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadJwtToken(token);
        if (jsonToken.Claims.Any(x => x.Type == "exp"))
        {
            var expSeconds = Convert.ToInt64(jsonToken.Claims.First(x => x.Type == "exp").Value);
            return new DateTime(1970, 1, 1).AddSeconds(expSeconds);
        }
        return DateTime.MinValue;
    }
}
