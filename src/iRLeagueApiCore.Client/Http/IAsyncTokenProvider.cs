namespace iRLeagueApiCore.Client.Http;

public interface IAsyncTokenProvider
{
    event EventHandler TokenChanged;
    event EventHandler TokenExpired;

    Task<string> GetIdTokenAsync();
    Task<string> GetAccessTokenAsync();
}
