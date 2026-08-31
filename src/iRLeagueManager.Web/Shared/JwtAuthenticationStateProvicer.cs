using iRLeagueApiCore.Client.Http;
using iRLeagueManager.Web.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace iRLeagueManager.Web.Shared;

/// <summary>
/// Server-only authentication state provider. It depends on <see cref="IHttpContextAccessor"/>
/// to read the cookie-authenticated <see cref="System.Security.Claims.ClaimsPrincipal"/>
/// populated by <c>JwtCookieMiddleware</c>/the JWT bearer handler, which is only available
/// while handling the HTTP request that starts a Blazor Server circuit. It also falls back to
/// <see cref="IAsyncTokenProvider"/> (backed by <c>BrowserProtectedStorageTokenStore</c>,
/// itself Server-only since it depends on <c>ProtectedLocalStorage</c> JS interop that
/// requires an active circuit). Because of these dependencies, this provider must keep
/// running under an <c>InteractiveServer</c> render mode; it is not WebAssembly-compatible.
/// It additionally persists a minimal snapshot of the authenticated user (see
/// <see cref="UserInfo"/>) via <see cref="PersistentComponentState"/> so that the Client
/// (WebAssembly) project's <c>PersistentAuthenticationStateProvider</c> can pick up the same
/// authenticated user for <c>InteractiveAuto</c> pages once WebAssembly takes over.
/// </summary>
internal sealed class JwtAuthenticationStateProvicer : AuthenticationStateProvider, IDisposable
{
    private readonly JwtSecurityTokenHandler tokenHandler = new();
    private readonly IAsyncTokenProvider tokenStore;
    private readonly ClaimsPrincipal? cookieUser;
    private readonly PersistentComponentState persistentComponentState;
    private readonly PersistingComponentStateSubscription persistingSubscription;

    public JwtAuthenticationStateProvicer(IAsyncTokenProvider tokenStore, IHttpContextAccessor httpContextAccessor, PersistentComponentState persistentComponentState)
    {
        this.tokenStore = tokenStore;
        this.persistentComponentState = persistentComponentState;
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

        // Persist a minimal snapshot of the authenticated user (id, name, roles, external API
        // id token) so that the Client (WebAssembly) project's PersistentAuthenticationStateProvider
        // can reconstruct an equivalent ClaimsPrincipal once InteractiveAuto pages switch over
        // to WebAssembly, without needing its own copy of the cookie/JWT auth pipeline.
        persistingSubscription = persistentComponentState.RegisterOnPersisting(PersistUserInfoAsync, RenderMode.InteractiveWebAssembly);
    }

    private Task PersistUserInfoAsync()
    {
        if (cookieUser?.Identity?.IsAuthenticated != true || IsExpired(cookieUser))
        {
            return Task.CompletedTask;
        }

        var userInfo = new UserInfo
        {
            UserId = cookieUser.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? cookieUser.FindFirst(ClaimTypes.Sid)?.Value ?? string.Empty,
            Name = cookieUser.Identity.Name ?? string.Empty,
            Roles = cookieUser.FindAll(ClaimTypes.Role).Select(c => c.Value).ToArray(),
            ApiIdToken = cookieUser.FindFirst(AuthConstants.ApiIdTokenClaimType)?.Value,
        };
        persistentComponentState.PersistAsJson(UserInfo.PersistenceKey, userInfo);
        return Task.CompletedTask;
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
        persistingSubscription.Dispose();
    }
}
