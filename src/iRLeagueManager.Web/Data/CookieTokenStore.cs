using iRLeagueApiCore.Client.Http;

namespace iRLeagueManager.Web.Data;

internal sealed class CookieTokenStore : ITokenStore
{
    internal const string AccessTokenCookieName = "X-Access-Token";
    internal const string IdTokenCookieName = "X-Id-Token";

    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly BrowserProtectedStorageTokenStore fallbackTokenStore;

    public CookieTokenStore(
        IHttpContextAccessor httpContextAccessor,
        BrowserProtectedStorageTokenStore fallbackTokenStore)
    {
        this.httpContextAccessor = httpContextAccessor;
        this.fallbackTokenStore = fallbackTokenStore;
        fallbackTokenStore.TokenChanged += (_, e) => TokenChanged?.Invoke(this, e);
        fallbackTokenStore.TokenExpired += (_, e) => TokenExpired?.Invoke(this, e);
    }

    public event EventHandler? TokenChanged;
    public event EventHandler? TokenExpired;

    public bool IsLoggedIn => GetAccessToken() is not null || fallbackTokenStore.IsLoggedIn;
    public DateTime IdTokenExpires => fallbackTokenStore.IdTokenExpires;
    public DateTime AccessTokenExpires => fallbackTokenStore.AccessTokenExpires;

    public Task ClearTokensAsync() => fallbackTokenStore.ClearTokensAsync();

    public Task<string> GetIdTokenAsync() =>
        GetCookieTokenAsync(IdTokenCookieName, fallbackTokenStore.GetIdTokenAsync);

    public Task<string> GetAccessTokenAsync() =>
        GetCookieTokenAsync(AccessTokenCookieName, fallbackTokenStore.GetAccessTokenAsync);

    public Task SetIdTokenAsync(string token) => fallbackTokenStore.SetIdTokenAsync(token);

    public Task SetAccessTokenAsync(string token) => fallbackTokenStore.SetAccessTokenAsync(token);

    private async Task<string> GetCookieTokenAsync(string cookieName, Func<Task<string>> getFallbackToken)
    {
        return GetCookieToken(cookieName) ?? await getFallbackToken();
    }

    private string? GetAccessToken() => GetCookieToken(AccessTokenCookieName);

    private string? GetCookieToken(string cookieName)
    {
        var request = httpContextAccessor.HttpContext?.Request;
        if (request is null)
        {
            return null;
        }

        if (cookieName == AccessTokenCookieName &&
            request.Headers.Authorization.FirstOrDefault() is { } authorizationHeader &&
            authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            return authorizationHeader["Bearer ".Length..];
        }

        return request.Cookies[cookieName];
    }
}
