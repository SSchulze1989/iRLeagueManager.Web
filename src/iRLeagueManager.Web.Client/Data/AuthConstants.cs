namespace iRLeagueManager.Web.Data;

/// <summary>
/// Claim types shared between the Server-side auth pipeline (which issues the cookie-based
/// JWT and the persisted <see cref="UserInfo"/>) and the Client-side (WebAssembly)
/// <c>PersistentAuthenticationStateProvider</c>, which reconstructs a <see
/// cref="System.Security.Claims.ClaimsPrincipal"/> from that persisted state.
/// </summary>
public static class AuthConstants
{
    /// <summary>
    /// Claim type holding the iRLeagueManager external API's id token, used to hydrate
    /// <see cref="iRLeagueApiCore.Client.Http.ITokenStore"/> so calls to the external API
    /// remain authenticated after a cookie based login.
    /// </summary>
    public const string ApiIdTokenClaimType = "irl_api_id_token";
}
