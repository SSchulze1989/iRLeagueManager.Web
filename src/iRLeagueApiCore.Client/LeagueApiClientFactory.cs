using iRLeagueApiCore.Client.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace iRLeagueApiCore.Client;

internal sealed class LeagueApiClientFactory
{
    private readonly ILoggerFactory loggerFactory;
    private readonly IHttpClientFactory httpClientFactory;
    private readonly ITokenStore tokenStore;
    private readonly HttpClientWrapperFactory clientWrapperFactory;

    private readonly string baseAddress;

    public LeagueApiClientFactory(string baseAddress, ILoggerFactory loggerFactory, IHttpClientFactory httpClientFactory, 
        ITokenStore tokenStore, HttpClientWrapperFactory clientWrapperFactory)
    {
        this.baseAddress = baseAddress;
        this.loggerFactory = loggerFactory;
        this.httpClientFactory = httpClientFactory;
        this.tokenStore = tokenStore;
        this.clientWrapperFactory = clientWrapperFactory;
    }

    public ILeagueApiClient CreateClient()
    {
        var httpClient = httpClientFactory.CreateClient();
        httpClient.BaseAddress = new Uri(baseAddress);
        var client = new LeagueApiClient(loggerFactory.CreateLogger<LeagueApiClient>(), httpClient, clientWrapperFactory, tokenStore);

        return client;
    }
}
