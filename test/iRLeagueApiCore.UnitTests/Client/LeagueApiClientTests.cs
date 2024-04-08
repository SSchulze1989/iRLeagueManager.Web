using iRLeagueApiCore.Client;
using iRLeagueApiCore.Client.Http;
using Microsoft.AspNetCore.Identity.Test;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;

namespace iRLeagueApiCore.UnitTests.Client;

public sealed class LeagueApiClientTests
{
    private const string baseUrl = "https://example.com/api";
    private const string testToken = "aslkgjwuipoht2io3ro2pqhuishgiag";

    private ILogger<LeagueApiClient> Logger { get; } = new Mock<ILogger<LeagueApiClient>>().Object;

    private static HttpClientWrapperFactory ClientWrapperFactory { get; }

    static LeagueApiClientTests()
    {
        var mockLoggerFactory = new Mock<ILoggerFactory>();
        mockLoggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>()))
            .Returns(Mock.Of<ILogger>());
        ClientWrapperFactory = new(mockLoggerFactory.Object, null);
    }

    [Fact]
    public async Task ShouldSetAuthenticationToken()
    {
        var messageHandler = MockHelpers.TestMessageHandler(x => new HttpResponseMessage()
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(JsonConvert.SerializeObject(new
            {
                idToken = testToken,
                accessToken = testToken,
                expiration = DateTime.UtcNow.AddDays(1),
            })),
        });

        string token = string.Empty;
        var mockTokenStore = new Mock<ITokenStore>();
        mockTokenStore.Setup(x => x.SetAccessTokenAsync(It.IsAny<string>()))
            .Callback<string>(x => token = x);

        var httpClient = new HttpClient(messageHandler);
        httpClient.BaseAddress = new Uri(baseUrl);

        var apiClient = new LeagueApiClient(Logger, httpClient, ClientWrapperFactory, mockTokenStore.Object);

        var result = await apiClient.LogIn("testUser", "testPassword");

        result.Success.Should().BeTrue();
        token.Should().Be(testToken);
    }

    [Fact]
    public async Task ShouldSendAuthenticatedRequest()
    {
        AuthenticationHeaderValue? authHeader = default;
        var messageHandler = MockHelpers.TestMessageHandler(x =>
        {
            authHeader = x.Headers.Authorization;
            return new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(default)),
            };
        });
        var mockTokenStore = new Mock<ITokenStore>();
        mockTokenStore.Setup(x => x.GetAccessTokenAsync())
            .ReturnsAsync(testToken);
        var httpClient = new HttpClient(messageHandler);
        httpClient.BaseAddress = new Uri(baseUrl);

        var apiClient = new LeagueApiClient(Logger, httpClient, ClientWrapperFactory, mockTokenStore.Object);
        await apiClient.Leagues().Get();

        Assert.Equal("bearer", authHeader?.Scheme, ignoreCase: true);
        Assert.Equal(testToken, authHeader?.Parameter);
    }

    [Fact]
    public async Task ShouldNotSendAuthenticatedRequestAfterLogOut()
    {
        AuthenticationHeaderValue? authHeader = default;
        var messageHandler = MockHelpers.TestMessageHandler(x =>
        {
            authHeader = x.Headers.Authorization;
            return new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(default)),
            };
        });

        string token = testToken;
        var mockTokenStore = new Mock<ITokenStore>();
        mockTokenStore.Setup(x => x.SetAccessTokenAsync(It.IsAny<string>()))
            .Callback<string>(x => token = x);
        mockTokenStore.Setup(x => x.ClearTokensAsync())
            .Callback(() => token = string.Empty);
        mockTokenStore.Setup(x => x.GetAccessTokenAsync())
            .ReturnsAsync(testToken);
        var httpClient = new HttpClient(messageHandler);
        httpClient.BaseAddress = new Uri(baseUrl);

        var apiClient = new LeagueApiClient(Logger, httpClient, ClientWrapperFactory, mockTokenStore.Object);
        await apiClient.LogOut();
        await apiClient.Leagues().Get();

        Assert.True(string.IsNullOrEmpty(token));
    }
}
