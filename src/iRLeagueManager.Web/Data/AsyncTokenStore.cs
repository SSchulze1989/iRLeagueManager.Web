using iRLeagueApiCore.Client.Http;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.Extensions.Logging;
using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace iRLeagueManager.Web.Data
{
    public class AsyncTokenStore : ITokenStore
    {
        private readonly ILogger<AsyncTokenStore> logger;
        private readonly ProtectedLocalStorage localStore;

        private const string tokenKey = "LeagueApiToken";

        private string inMemoryToken = string.Empty;

        public event EventHandler? TokenChanged;

        public bool IsLoggedIn { get; private set; }
        public DateTime Expiration { get; private set; }

        public AsyncTokenStore(ILogger<AsyncTokenStore> logger, ProtectedLocalStorage localStorage)
        {
            this.logger = logger;
            this.localStore = localStorage;
        }

        public async Task ClearTokenAsync()
        {
            var tokenValue = inMemoryToken;

            logger.LogDebug("Clear token in local browser store");
            IsLoggedIn = false;
            inMemoryToken = string.Empty;
            await localStore.DeleteAsync(tokenKey);
            await Task.FromResult(true);
            if (inMemoryToken != tokenValue)
            {
                TokenChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public async Task<string> GetTokenAsync()
        {
            if (string.IsNullOrEmpty(inMemoryToken) == false)
            {
                return inMemoryToken;
            }

            logger.LogDebug("Reading token from local browser store");
            try
            {
                //if (contextAccessor.HttpContext?.Session.IsAvailable == true)
                //{
                //    string token = contextAccessor.HttpContext?.Session.GetString(tokenKey) ?? string.Empty;
                //    return await Task.FromResult(token);
                //}
                var token = await localStore.GetAsync<string>(tokenKey);
                if (token.Success)
                {
                    if (IsLoggedIn == false)
                    {
                        // set expiration date
                        var handler = new JwtSecurityTokenHandler();
                        var jsonToken = handler.ReadJwtToken(token.Value);
                        if (jsonToken.Claims.Any(x => x.Type == "exp"))
                        {
                            var expSeconds = Convert.ToInt64(jsonToken.Claims.First(x => x.Type == "exp").Value);
                            Expiration = new DateTime(1970, 1, 1).AddSeconds(expSeconds);
                        }
                    }

                    // check if token is still valid
                    if (Expiration < DateTime.UtcNow.AddMinutes(5))
                    {
                        await ClearTokenAsync();
                        logger.LogInformation("Token read from token store has expired");
                        return string.Empty;
                    }
                    IsLoggedIn = true;
                    return inMemoryToken = token.Value ?? string.Empty;
                }
                IsLoggedIn = false;
                return string.Empty;
            }
            catch (Exception ex)
            {
                logger.LogError("Could not read from local browser session: {Exception}", ex);
                return string.Empty;
            }
        }

        public async Task SetTokenAsync(string token)
        {
            var oldToken = inMemoryToken;
            logger.LogDebug("Set token to local browser session: {Token}", token);
            await localStore.SetAsync(tokenKey, token);
            inMemoryToken = token;

            if (inMemoryToken != oldToken)
            {
                TokenChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
