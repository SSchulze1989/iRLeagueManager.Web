using iRLeagueApiCore.Client.Results;
using iRLeagueManager.Web.Data;
using iRLeagueManager.Web.Middleware;
using System.Net.Http.Json;

namespace iRLeagueManager.Web.Endpoints;

/// <summary>
/// Minimal API endpoints implementing cookie based authentication on top of the
/// iRLeagueManager API. Successful logins store a first-party JWT in an HttpOnly,
/// Secure, SameSite=Strict cookie which is later translated into a bearer token by
/// <see cref="JwtCookieMiddleware"/>.
/// </summary>
public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/auth");

        group.MapPost("/login", LoginAsync);
        group.MapPost("/logout", Logout);
        group.MapPost("/refresh", RefreshAsync);
        group.MapGet("/status", GetStatus);

        return endpoints;
    }

    private static async Task<IResult> LoginAsync(
        LoginRequest request,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        IAuthenticationService authenticationService,
        HttpContext httpContext,
        ILoggerFactory loggerFactory,
        CancellationToken cancellationToken)
    {
        var logger = loggerFactory.CreateLogger("AuthEndpoints");

        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        {
            return Results.BadRequest(new { message = "Username and password are required." });
        }

        logger.LogInformation("Login attempt for user {UserName}", request.Username);

        // Authenticate directly against the iRLeagueManager API kernel. A dedicated
        // HttpClient is used here (rather than the Blazor-circuit bound ILeagueApiClient)
        // since minimal API endpoints run outside of a Blazor circuit.
        var apiBaseAddress = configuration["APIServer"];
        if (string.IsNullOrWhiteSpace(apiBaseAddress))
        {
            logger.LogError("APIServer configuration value is missing.");
            return Results.Problem("Server is not configured correctly.", statusCode: StatusCodes.Status500InternalServerError);
        }

        var httpClient = httpClientFactory.CreateClient();
        httpClient.BaseAddress = new Uri(apiBaseAddress);

        HttpResponseMessage response;
        try
        {
            response = await httpClient.PostAsJsonAsync(
                "Authenticate/Login",
                new { username = request.Username, password = request.Password },
                cancellationToken);
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Login request to API failed for user {UserName}", request.Username);
            return Results.Problem("Could not reach authentication server.", statusCode: StatusCodes.Status502BadGateway);
        }

        if (response.IsSuccessStatusCode == false)
        {
            logger.LogWarning("Login failed for user {UserName}: {StatusCode}", request.Username, response.StatusCode);
            return Results.Unauthorized();
        }

        LoginResponse loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>(cancellationToken: cancellationToken);
        if (string.IsNullOrEmpty(loginResponse.AccessToken))
        {
            logger.LogWarning("Login response for user {UserName} did not contain an access token", request.Username);
            return Results.Unauthorized();
        }

        var principal = await authenticationService.CreateClaimsFromApiResponseAsync(loginResponse);
        var jwt = await authenticationService.CreateJwtTokenAsync(principal);
        SetAccessTokenCookie(httpContext, jwt);

        logger.LogInformation("Login succeeded for user {UserName}", request.Username);
        return Results.Ok(new { success = true });
    }

    private static IResult Logout(HttpContext httpContext, ILoggerFactory loggerFactory)
    {
        var logger = loggerFactory.CreateLogger("AuthEndpoints");
        httpContext.Response.Cookies.Delete(JwtCookieMiddleware.CookieName, GetCookieOptions());
        logger.LogInformation("User logged out");
        return Results.Ok(new { success = true });
    }

    private static async Task<IResult> RefreshAsync(
        HttpContext httpContext,
        IAuthenticationService authenticationService,
        ILoggerFactory loggerFactory)
    {
        var logger = loggerFactory.CreateLogger("AuthEndpoints");

        if (httpContext.Request.Cookies.TryGetValue(JwtCookieMiddleware.CookieName, out var token) == false ||
            string.IsNullOrWhiteSpace(token))
        {
            logger.LogInformation("Token refresh failed: no token cookie present");
            return Results.Unauthorized();
        }

        var principal = await authenticationService.ValidateTokenAsync(token, validateLifetime: false);
        if (principal is null)
        {
            logger.LogInformation("Token refresh failed: token invalid or expired");
            return Results.Unauthorized();
        }

        var jwt = await authenticationService.CreateJwtTokenAsync(principal);
        SetAccessTokenCookie(httpContext, jwt);

        logger.LogInformation("Token refreshed for user {UserName}", principal.Identity?.Name);
        return Results.Ok(new { success = true });
    }

    private static IResult GetStatus(HttpContext httpContext)
    {
        var isAuthenticated = httpContext.User.Identity?.IsAuthenticated ?? false;
        return Results.Ok(new
        {
            isAuthenticated,
            userName = httpContext.User.Identity?.Name,
        });
    }

    private static void SetAccessTokenCookie(HttpContext httpContext, string jwt)
    {
        httpContext.Response.Cookies.Append(JwtCookieMiddleware.CookieName, jwt, GetCookieOptions());
    }

    private static CookieOptions GetCookieOptions()
    {
        return new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Path = "/",
        };
    }
}

public sealed record LoginRequest(string Username, string Password);
