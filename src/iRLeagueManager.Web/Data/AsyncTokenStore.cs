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
            logger.LogDebug("Clear token in local browser store");
            IsLoggedIn = false;
            await localStore.DeleteAsync(tokenKey);
            await Task.FromResult(true);
            TokenChanged?.Invoke(this, EventArgs.Empty);
        }

        public async Task<string> GetTokenAsync()
        {
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
                    return token.Value ?? string.Empty;
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

        //private Task<AuthenticationState> AuthenticationTask(string token)
        //{
        //    var state = new AuthenticationState(CreatePrincipal(token));
        //    return Task.FromResult());
        //}

        //private ClaimsPrincipal CreatePrincipal(string token)
        //{
        //    // decode token
        //    var handler = new JwtSecurityTokenHandler();
        //    var jsonToken = handler.ReadToken(token);
        //    var tokenS = jsonToken as JwtSecurityToken;
        //    if (tokenS != null)
        //    {
        //        var claims = tokenS.Claims.ToList();
        //        claims.Add(new Claim("ApiToken", token));
        //        return new ClaimsPrincipal(new ClaimsIdentity(claims));
        //    }
        //    return new ClaimsPrincipal();
        //}

        public async Task SetTokenAsync(string token)
        {
            logger.LogDebug("Set token to local browser session: {Token}", token);
            await localStore.SetAsync(tokenKey, token);
            TokenChanged?.Invoke(this, EventArgs.Empty);            
        }
    }
}
