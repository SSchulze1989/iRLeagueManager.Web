using Microsoft.JSInterop;

namespace iRLeagueManager.Web.Shared;

/// <summary>
/// Bridges Blazor Server components to the browser's <c>fetch</c> API so that calls to the
/// cookie based "/api/auth/*" endpoints are issued by the user's browser instead of the
/// server-side Blazor circuit. This is required because the HttpOnly "X-Access-Token" cookie
/// set by these endpoints must be received and stored by the user's browser - an
/// <see cref="HttpClient"/> call made from within a Blazor Server component would only reach
/// the server's own loopback connection and could never set a cookie for the user's browser.
/// </summary>
public sealed class AuthJsInterop
{
    private readonly IJSRuntime jsRuntime;

    public AuthJsInterop(IJSRuntime jsRuntime)
    {
        this.jsRuntime = jsRuntime;
    }

    public async Task<AuthJsResult> PostAsync(string url, object? payload = null)
    {
        return await jsRuntime.InvokeAsync<AuthJsResult>("authInterop.postJson", url, payload ?? new { });
    }
}

/// <summary>
/// Result of a browser-issued request to one of the "/api/auth/*" endpoints.
/// </summary>
public sealed record AuthJsResult(bool Success, int Status, AuthJsResponseBody? Body);

public sealed record AuthJsResponseBody(string? Message);
