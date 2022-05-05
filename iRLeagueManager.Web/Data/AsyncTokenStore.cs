using iRLeagueApiCore.Client.Http;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.Extensions.Logging;
using System;
using Microsoft.AspNetCore.Identity;

namespace iRleagueManager.Web.Data
{
    public class AsyncTokenStore : ITokenStore
    {
        private readonly ILogger<AsyncTokenStore> logger;
        private readonly IHttpContextAccessor contextAccessor;

        private const string tokenKey = "LeagueApiToken";

        public AsyncTokenStore(ILogger<AsyncTokenStore> logger, IHttpContextAccessor contextAccessor)
        {
            this.logger = logger;
            this.contextAccessor = contextAccessor;
        }

        public async Task ClearTokenAsync()
        {
            logger.LogDebug("Clear token in local browser store");
            contextAccessor.HttpContext?.Session.Remove(tokenKey);
            await Task.FromResult(true);
        }

        public async Task<string> GetTokenAsync()
        {
            logger.LogDebug("Reading token from local browser store");
            try
            {
                if (contextAccessor.HttpContext?.Session.IsAvailable == true)
                {
                    string token = contextAccessor.HttpContext?.Session.GetString(tokenKey) ?? string.Empty;
                    return await Task.FromResult(token);
                }
                return string.Empty;
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
            if (await GetTokenAsync() != token)
                contextAccessor.HttpContext?.Session?.SetString(tokenKey, token);
            await contextAccessor.HttpContext?.Session.CommitAsync();
        }
    }
}
