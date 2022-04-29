using iRLeagueApiCore.Client.Http;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.Extensions.Logging;
using System;

namespace iRleagueManager.Web.Server.Data
{
    public class AsyncTokenStore : ITokenStore
    {
        private readonly ILogger<AsyncTokenStore> logger;
        private readonly ILocalStorageService localStorage;

        private const string tokenKey = "LeagueApiToken";

        public AsyncTokenStore(ILogger<AsyncTokenStore> logger, ILocalStorageService localStorage)
        {
            this.logger = logger;
            this.localStorage = localStorage;
        }

        public async Task ClearTokenAsync()
        {
            logger.LogDebug("Clear token in local browser store");
            await localStorage.RemoveItemAsync(tokenKey);
        }

        public async Task<string> GetTokenAsync()
        {
            logger.LogDebug("Reading token from local browser store");
            try
            {
                var token = await localStorage.GetItemAsStringAsync(tokenKey);
                logger.LogDebug("Retrieved token: {Token}", token);
                return token;
            }
            catch (InvalidOperationException ex)
            {
                logger.LogError("Could not read from local browser storage: {Exception}", ex);
                return string.Empty;
            }
        }

        public async Task SetTokenAsync(string token)
        {
            logger.LogDebug("Set token to local browser store: {Token}", token);
            await localStorage.SetItemAsStringAsync(tokenKey, token);
        }
    }
}
