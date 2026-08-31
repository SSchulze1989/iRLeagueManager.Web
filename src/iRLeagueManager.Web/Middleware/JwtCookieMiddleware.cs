using Microsoft.Extensions.Primitives;

namespace iRLeagueManager.Web.Middleware;

/// <summary>
/// Reads the JWT stored in the <see cref="CookieName"/> HttpOnly cookie and, if present,
/// exposes it as a standard <c>Authorization: Bearer</c> header so that downstream
/// authentication handlers (e.g. JWT bearer authentication) can pick it up.
/// </summary>
public sealed class JwtCookieMiddleware
{
    public const string CookieName = "X-Access-Token";

    private readonly RequestDelegate next;
    private readonly ILogger<JwtCookieMiddleware> logger;

    public JwtCookieMiddleware(RequestDelegate next, ILogger<JwtCookieMiddleware> logger)
    {
        this.next = next;
        this.logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Headers.ContainsKey("Authorization") == false &&
            context.Request.Cookies.TryGetValue(CookieName, out var token) &&
            string.IsNullOrWhiteSpace(token) == false)
        {
            logger.LogDebug("Found {CookieName} cookie, setting Authorization header for request {Path}",
                CookieName, context.Request.Path);
            context.Request.Headers.Authorization = new StringValues("Bearer " + token);
        }

        await next(context);
    }
}

public static class JwtCookieMiddlewareExtensions
{
    /// <summary>
    /// Adds middleware that extracts the JWT stored in the <see cref="JwtCookieMiddleware.CookieName"/>
    /// cookie and forwards it as an <c>Authorization: Bearer</c> header.
    /// </summary>
    public static IApplicationBuilder UseJwtCookieMiddleware(this IApplicationBuilder app)
    {
        return app.UseMiddleware<JwtCookieMiddleware>();
    }
}
