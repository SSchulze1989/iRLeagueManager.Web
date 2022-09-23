using iRLeagueApiCore.Client;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using Blazored.LocalStorage;
using System.Threading.Tasks;
using iRLeagueApiCore.Client.Http;
using System.Text.Json;

namespace iRLeagueManager.Web.Server.Data
{
    public class LeagueApiClientFactory
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
            baseAddress = configuration["APIServer"];
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
}
