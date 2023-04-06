using Blazored.LocalStorage;
using iRLeagueApiCore.Client;
using iRLeagueApiCore.Client.Http;
using System.Text.Json;

namespace iRLeagueManager.Web.Server.Data;

internal sealed class LeagueApiClientFactory
{
    private readonly ILoggerFactory loggerFactory;
    private readonly ILogger<LeagueApiClientFactory> logger;
    private readonly IHttpClientFactory httpClientFactory;
    private readonly ILocalStorageService localStorage;
    private readonly ITokenStore tokenStore;
    private readonly JsonSerializerOptions jsonOptions;

    private readonly string baseAddress;

    public LeagueApiClientFactory(IConfiguration configuration, ILoggerFactory loggerFactory, IHttpClientFactory httpClientFactory,
        ILocalStorageService localStorage, ITokenStore tokenStore, JsonSerializerOptions jsonOptions)
    {
        baseAddress = configuration["APIServer"] ?? string.Empty;
        this.loggerFactory = loggerFactory;
        logger = loggerFactory.CreateLogger<LeagueApiClientFactory>();
        this.httpClientFactory = httpClientFactory;
        this.localStorage = localStorage;
        this.tokenStore = tokenStore;
        this.jsonOptions = jsonOptions;
    }

    public ILeagueApiClient CreateClient()
    {
        var httpClient = httpClientFactory.CreateClient();
        httpClient.BaseAddress = new Uri(baseAddress);
        var client = new LeagueApiClient(loggerFactory.CreateLogger<LeagueApiClient>(), httpClient, tokenStore, jsonOptions);

        return client;
    }
}
