using iRLeagueApiCore.Client.Http;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace iRLeagueManager.Web.Shared;

internal sealed class JwtAuthenticationStateProvicer : AuthenticationStateProvider, IDisposable
{
    private readonly JwtSecurityTokenHandler tokenHandler = new();
    private readonly IAsyncTokenProvider tokenStore;
    private readonly ClaimsPrincipal? cookieUser;

    public JwtAuthenticationStateProvicer(IAsyncTokenProvider tokenStore, IHttpContextAccessor httpContextAccessor)
    {
        this.tokenStore = tokenStore;
        tokenStore.TokenChanged += TokenStore_TokenChanged;

        // HttpContext is only reliably available while the initial HTTP request that
        // establishes the Blazor circuit is still being processed, so it is captured here
        // in the constructor rather than read lazily later on. By this point
        // JwtCookieMiddleware has already translated the "X-Access-Token" HttpOnly cookie
        // into an "Authorization: Bearer" header (if present) and the JWT bearer
        // authentication handler has populated HttpContext.User accordingly.
        var httpContextUser = httpContextAccessor.HttpContext?.User;
        if (httpContextUser?.Identity?.IsAuthenticated == true)
        {
            cookieUser = httpContextUser;
        }
    }

    private void TokenStore_TokenChanged(object? sender, EventArgs e)
    {
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    private async Task<ClaimsPrincipal> GetTokenUser()
    {
        // Prefer the cookie based login state established by the "/api/auth/*" endpoints.
        // The principal is captured once per circuit (see constructor), so its expiration is
        // re-checked here on every read: once the JWT backing the cookie expires, the user is
        // treated as anonymous for the remainder of the circuit, forcing a fresh login/redirect
        // instead of continuing to show the stale authenticated state. The cookie based
        // mechanism takes priority: if it was present but has expired, the user is anonymous
        // regardless of any leftover state in the legacy token store. There is currently no
        // code path that writes a fresh token into the legacy token store without also
        // re-issuing the cookie (e.g. via "/api/auth/refresh"), so this cannot mask a valid
        // refreshed session; if such a path is added in the future, this fallback will need to
        // be revisited.
        if (cookieUser is not null)
        {
            return IsExpired(cookieUser) ? GetAnonymous() : cookieUser;
        }

        // Fall back to the legacy browser-storage based token flow for backwards
        // compatibility with any code path that still populates it directly.
        var idToken = await tokenStore.GetIdTokenAsync();
        if (string.IsNullOrEmpty(idToken))
        {
            return GetAnonymous();
        }
        var accessToken = await tokenStore.GetAccessTokenAsync();
        if (string.IsNullOrEmpty(accessToken))
        {
            return GetAnonymous();
        }
        var jwtSecurityToken = tokenHandler.ReadJwtToken(accessToken);
        var identity = new ClaimsIdentity(jwtSecurityToken.Claims, "bearer");
        return new ClaimsPrincipal(identity);
    }

    private static ClaimsPrincipal GetAnonymous()
    {
        return new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
                new Claim(ClaimTypes.Sid, "0"),
                new Claim(ClaimTypes.Name, "Anonymous"),
                new Claim(ClaimTypes.Role, "Anonymous")
            }, null));
    }

    private static bool IsExpired(ClaimsPrincipal principal)
    {
        var expClaim = principal.FindFirst("exp")?.Value;
        if (long.TryParse(expClaim, out var expSeconds) == false)
        {
            // The JWTs issued by AuthenticationService.CreateJwtTokenAsync always include an
            // "exp" claim; if it is missing the token cannot be trusted, so treat it as expired.
            return true;
        }
        var expires = DateTimeOffset.FromUnixTimeSeconds(expSeconds);
        return expires <= DateTimeOffset.UtcNow;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var user = await GetTokenUser();
        return new AuthenticationState(user);
    }

    void IDisposable.Dispose()
    {
        tokenStore.TokenChanged -= TokenStore_TokenChanged;
    }
}
