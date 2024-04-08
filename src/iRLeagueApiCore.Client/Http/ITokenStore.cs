namespace iRLeagueApiCore.Client.Http;

public interface ITokenStore : IAsyncTokenProvider
{
    bool IsLoggedIn { get; }
    DateTime AccessTokenExpires { get; }
    Task SetIdTokenAsync(string token);
    Task SetAccessTokenAsync(string token);
    Task ClearTokensAsync();
}
