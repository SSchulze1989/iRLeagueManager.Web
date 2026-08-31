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
        if (cookieUser is not null)
        {
            return cookieUser;
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
