using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata;

namespace iRLeagueApiCore.Client.Http;
internal sealed class DefaultTokenStore : ITokenStore
{
    private string idToken = string.Empty;
    private string accessToken = string.Empty;

    public bool IsLoggedIn => throw new NotImplementedException();

    public DateTime AccessTokenExpires { get; private set; }
    public bool AccessTokenExpired { get; set; }

    public event EventHandler? TokenChanged;
    public event EventHandler? TokenExpired;

    public async Task ClearTokensAsync()
    {
        idToken = string.Empty;
        accessToken = string.Empty;
        TokenChanged?.Invoke(this, EventArgs.Empty);
        await Task.CompletedTask;
    }

    public async Task<string> GetAccessTokenAsync()
    {
        if (AccessTokenExpires <= DateTime.UtcNow && string.IsNullOrEmpty(accessToken) == false && AccessTokenExpired == false)
        {
            AccessTokenExpired = true;
            TokenExpired?.Invoke(this, EventArgs.Empty);
        }
        return await Task.FromResult(accessToken);
    }

    public async Task<string> GetIdTokenAsync()
    {
        return await Task.FromResult(idToken);
    }

    public async Task SetAccessTokenAsync(string token)
    {
        accessToken = token;
        AccessTokenExpired = false;
        if (string.IsNullOrEmpty(accessToken) == false)
        {
            // set expiration date
            var jwtToken = new JwtSecurityTokenHandler().ReadToken(accessToken);
            AccessTokenExpires = jwtToken.ValidTo;
        }
        TokenChanged?.Invoke(this, EventArgs.Empty);
        await Task.CompletedTask;
    }

    public async Task SetIdTokenAsync(string token)
    {
        if (token != idToken)
        {
            idToken = token;
            TokenChanged?.Invoke(this, EventArgs.Empty);
        }
        await Task.CompletedTask;
    }
}
