using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace iRLeagueManager.Web.Data;

/// <summary>
/// Client-only (WebAssembly) authentication state provider. It has no access to the
/// HttpOnly auth cookie or <c>HttpContext</c> (both Server-only), so instead it reads the
/// <see cref="UserInfo"/> snapshot persisted by the Server's
/// <c>JwtAuthenticationStateProvicer</c> via <see cref="PersistentComponentState"/> during
/// the initial (static/server) render, and reconstructs an equivalent
/// <see cref="ClaimsPrincipal"/> from it. If no such state was persisted (user was not
/// authenticated, or WebAssembly started up outside of that render, e.g. a full page
/// reload served purely by the WASM host), the user is treated as anonymous - normal cookie
/// based authorization checks still apply server-side for any subsequent HTTP/API calls.
/// </summary>
public sealed class PersistentAuthenticationStateProvider : AuthenticationStateProvider
{
    private static readonly Task<AuthenticationState> defaultUnauthenticatedTask =
        Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));

    private readonly Task<AuthenticationState> authenticationStateTask = defaultUnauthenticatedTask;

    /// <summary>
    /// The external API id token persisted alongside the user's identity, if any (see
    /// <see cref="AuthConstants.ApiIdTokenClaimType"/>). Exposed so it can be used to hydrate
    /// the Client project's <c>ITokenStore</c> on startup without waiting for a JS-interop
    /// round trip.
    /// </summary>
    public string? ApiIdToken { get; }

    public PersistentAuthenticationStateProvider(PersistentComponentState state)
    {
        if (state.TryTakeFromJson<UserInfo>(UserInfo.PersistenceKey, out var userInfo) == false || userInfo is null)
        {
            return;
        }

        ApiIdToken = userInfo.ApiIdToken;

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userInfo.UserId),
            new(ClaimTypes.Name, userInfo.Name),
        };
        claims.AddRange(userInfo.Roles.Select(role => new Claim(ClaimTypes.Role, role)));
        if (string.IsNullOrEmpty(userInfo.ApiIdToken) == false)
        {
            claims.Add(new Claim(AuthConstants.ApiIdTokenClaimType, userInfo.ApiIdToken));
        }

        var identity = new ClaimsIdentity(claims, authenticationType: nameof(PersistentAuthenticationStateProvider));
        authenticationStateTask = Task.FromResult(new AuthenticationState(new ClaimsPrincipal(identity)));
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync() => authenticationStateTask;
}
