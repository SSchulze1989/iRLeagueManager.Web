namespace iRLeagueManager.Web.Data;

internal sealed class JwtCookieMiddleware
{
    private readonly RequestDelegate next;

    public JwtCookieMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    public Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Headers.ContainsKey("Authorization") &&
            context.Request.Cookies.TryGetValue(CookieTokenStore.AccessTokenCookieName, out var accessToken) &&
            !string.IsNullOrWhiteSpace(accessToken))
        {
            context.Request.Headers.Authorization = "Bearer " + accessToken;
        }

        return next(context);
    }
}
