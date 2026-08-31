using iRLeagueApiCore.Client.Results;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace iRLeagueManager.Web.Data;

/// <summary>
/// Provides creation and validation of first-party JWTs that are stored in an
/// HttpOnly cookie and used to authenticate requests to this application.
/// </summary>
public interface IAuthenticationService
{
    /// <summary>
    /// Creates a signed JWT containing the claims of <paramref name="principal"/>.
    /// </summary>
    Task<string> CreateJwtTokenAsync(ClaimsPrincipal principal);

    /// <summary>
    /// Validates a JWT previously created by <see cref="CreateJwtTokenAsync(ClaimsPrincipal)"/>
    /// and returns the corresponding <see cref="ClaimsPrincipal"/> or <see langword="null"/> if the
    /// token is invalid or expired.
    /// </summary>
    /// <param name="validateLifetime">
    /// When <see langword="false"/>, an already expired token is still accepted as long as its
    /// signature, issuer and audience are valid. Used to allow refreshing an expired session.
    /// </param>
    Task<ClaimsPrincipal?> ValidateTokenAsync(string token, bool validateLifetime = true);

    /// <summary>
    /// Builds a <see cref="ClaimsPrincipal"/> from the tokens contained in a login response
    /// received from the iRLeagueManager API.
    /// </summary>
    Task<ClaimsPrincipal> CreateClaimsFromApiResponseAsync(LoginResponse loginResponse);
}

internal sealed class AuthenticationService : IAuthenticationService
{
    private const string DefaultExpirationMinutes = "60";

    private readonly ILogger<AuthenticationService> logger;
    private readonly IConfiguration configuration;
    private readonly JwtSecurityTokenHandler tokenHandler = new();

    public AuthenticationService(ILogger<AuthenticationService> logger, IConfiguration configuration)
    {
        this.logger = logger;
        this.configuration = configuration;
    }

    public Task<string> CreateJwtTokenAsync(ClaimsPrincipal principal)
    {
        var jwtSection = configuration.GetSection("Jwt");
        var issuer = jwtSection["Issuer"];
        var audience = jwtSection["Audience"];
        var expirationMinutes = double.TryParse(jwtSection["ExpirationMinutes"], NumberStyles.Number, CultureInfo.InvariantCulture, out var parsedMinutes)
            ? parsedMinutes
            : double.Parse(DefaultExpirationMinutes, CultureInfo.InvariantCulture);
        var signingCredentials = GetSigningCredentials(jwtSection);

        var expires = DateTime.UtcNow.AddMinutes(expirationMinutes);
        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: principal.Claims,
            expires: expires,
            signingCredentials: signingCredentials);

        var jwt = tokenHandler.WriteToken(token);
        logger.LogDebug("Created JWT for user {UserName} expiring at {Expires:o}",
            principal.Identity?.Name, expires);
        return Task.FromResult(jwt);
    }

    public Task<ClaimsPrincipal?> ValidateTokenAsync(string token, bool validateLifetime = true)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return Task.FromResult<ClaimsPrincipal?>(null);
        }

        var jwtSection = configuration.GetSection("Jwt");
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSection["Issuer"],
            ValidateAudience = true,
            ValidAudience = jwtSection["Audience"],
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = GetSigningKey(jwtSection),
            ValidateLifetime = validateLifetime,
            ClockSkew = TimeSpan.FromMinutes(1),
        };

        try
        {
            var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
            return Task.FromResult<ClaimsPrincipal?>(principal);
        }
        catch (Exception ex)
        {
            logger.LogInformation(ex, "Token validation failed");
            return Task.FromResult<ClaimsPrincipal?>(null);
        }
    }

    public Task<ClaimsPrincipal> CreateClaimsFromApiResponseAsync(LoginResponse loginResponse)
    {
        ArgumentNullException.ThrowIfNull(loginResponse);

        // The access token issued by the iRLeagueManager API already contains
        // the claims describing the authenticated user (id, name, roles, ...).
        var apiToken = tokenHandler.ReadJwtToken(loginResponse.AccessToken);
        var identity = new ClaimsIdentity(apiToken.Claims, authenticationType: JwtBearerDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);
        logger.LogDebug("Created claims principal for user {UserName} from API login response",
            principal.Identity?.Name);
        return Task.FromResult(principal);
    }

    private SymmetricSecurityKey GetSigningKey(IConfigurationSection jwtSection)
    {
        var secret = jwtSection["Secret"];
        if (string.IsNullOrWhiteSpace(secret))
        {
            throw new InvalidOperationException("Jwt:Secret configuration value is missing.");
        }
        return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
    }

    private SigningCredentials GetSigningCredentials(IConfigurationSection jwtSection)
    {
        return new SigningCredentials(GetSigningKey(jwtSection), SecurityAlgorithms.HmacSha256);
    }
}
