using iRLeagueApiCore.Client.Http;

namespace iRLeagueApiCore.UnitTests.Client.Services;

public class TestTokenStore : ITokenStore
{
    private readonly ITokenStore mockStore = Mock.Of<ITokenStore>();

    public bool IsLoggedIn => mockStore.IsLoggedIn;

    public DateTime AccessTokenExpires => mockStore.AccessTokenExpires;

    public event EventHandler TokenChanged
    {
        add
        {
            mockStore.TokenChanged += value;
        }

        remove
        {
            mockStore.TokenChanged -= value;
        }
    }

    public event EventHandler TokenExpired
    {
        add
        {
            mockStore.TokenExpired += value;
        }

        remove
        {
            mockStore.TokenExpired -= value;
        }
    }

    public Task ClearTokensAsync()
    {
        return mockStore.ClearTokensAsync();
    }

    public Task<string> GetAccessTokenAsync()
    {
        return mockStore.GetAccessTokenAsync();
    }

    public Task<string> GetIdTokenAsync()
    {
        return mockStore.GetIdTokenAsync();
    }

    public Task SetAccessTokenAsync(string token)
    {
        return mockStore.SetAccessTokenAsync(token);
    }

    public Task SetIdTokenAsync(string token)
    {
        return mockStore.SetIdTokenAsync(token);
    }
}