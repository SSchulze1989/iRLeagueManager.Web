using iRLeagueApiCore.Client.Results;
using System.Net.Http.Json;

namespace iRLeagueManager.Web.Data;

internal static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/api/auth/login", Login);
        endpoints.MapPost("/api/auth/logout", Logout);
        return endpoints;
    }

    private static async Task<IResult> Login(
        LoginRequest request,
        IHttpClientFactory httpClientFactory,
        HttpContext context,
        ILoggerFactory loggerFactory,
        CancellationToken cancellationToken)
    {
        var logger = loggerFactory.CreateLogger("AuthEndpoints");
        logger.LogInformation("Login requested for {UserName}", request.Username);

        using var response = await httpClientFactory.CreateClient("AuthApi").PostAsJsonAsync(
            "Authenticate/Login",
            request,
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadFromJsonAsync<AuthResponse>(cancellationToken: cancellationToken);
            logger.LogWarning("Login failed for {UserName} with status {StatusCode}", request.Username, response.StatusCode);
            return Results.Json(
                new AuthResponse(false, error?.Status),
                statusCode: (int)response.StatusCode);
        }

        var login = await response.Content.ReadFromJsonAsync<LoginResponse>(cancellationToken: cancellationToken);
        if (string.IsNullOrWhiteSpace(login.IdToken) || string.IsNullOrWhiteSpace(login.AccessToken))
        {
            logger.LogError("Authentication API returned an incomplete token response for {UserName}", request.Username);
            return Results.Problem(statusCode: StatusCodes.Status502BadGateway);
        }

        context.Response.Cookies.Append(CookieTokenStore.IdTokenCookieName, login.IdToken, CreateCookieOptions(login.Expires));
        context.Response.Cookies.Append(CookieTokenStore.AccessTokenCookieName, login.AccessToken, CreateCookieOptions(login.Expires));
        logger.LogInformation("Login succeeded for {UserName}", request.Username);
        return Results.Ok(new AuthResponse(true));
    }

    private static IResult Logout(HttpContext context, ILoggerFactory loggerFactory)
    {
        context.Response.Cookies.Delete(CookieTokenStore.IdTokenCookieName);
        context.Response.Cookies.Delete(CookieTokenStore.AccessTokenCookieName);
        loggerFactory.CreateLogger("AuthEndpoints").LogInformation("Logout requested");
        return Results.Ok();
    }

    private static CookieOptions CreateCookieOptions(DateTime expires) => new()
    {
        HttpOnly = true,
        Secure = true,
        SameSite = SameSiteMode.Lax,
        Expires = new DateTimeOffset(expires)
    };

    internal sealed record LoginRequest(string Username, string Password);
    internal sealed record AuthResponse(bool Success, string? Status = null);
}
