namespace iRLeagueManager.Web.Data;

/// <summary>
/// Minimal, JSON-serializable snapshot of the current user's authentication state, persisted
/// by the Server (via <c>PersistentComponentState</c>) during the initial static/server
/// render and read back by the Client (WebAssembly) project's
/// <c>PersistentAuthenticationStateProvider</c> so that pages rendered with
/// <c>@rendermode InteractiveAuto</c> see the same authenticated user once WebAssembly takes
/// over, without needing their own copy of the cookie/JWT based auth pipeline.
/// </summary>
public sealed class UserInfo
{
    /// <summary>
    /// Key used to persist/restore this payload via <c>PersistentComponentState</c>.
    /// </summary>
    public const string PersistenceKey = "iRLeagueManager.UserInfo";

    public required string UserId { get; init; }
    public required string Name { get; init; }
    public string[] Roles { get; init; } = [];

    /// <summary>
    /// The external API's id token (see <see cref="AuthConstants.ApiIdTokenClaimType"/>), so
    /// the Client project can hydrate its own <c>ITokenStore</c> without waiting on
    /// JS-interop-based cookie reads.
    /// </summary>
    public string? ApiIdToken { get; init; }
}
