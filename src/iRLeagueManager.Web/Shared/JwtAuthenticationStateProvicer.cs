using iRLeagueApiCore.Client.Http;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace iRLeagueManager.Web.Shared
{
    public class JwtAuthenticationStateProvicer : AuthenticationStateProvider
    {
        private readonly JwtSecurityTokenHandler tokenHandler = new();
        private readonly ITokenStore tokenStore;
        private string lastToken = string.Empty;

        public JwtAuthenticationStateProvicer(ITokenStore tokenStore)
        {
            this.tokenStore = tokenStore;
        }

        private async Task<ClaimsPrincipal> GetTokenUser()
        {
            var token = await tokenStore.GetTokenAsync();
            if (string.IsNullOrEmpty(token))
            {
                return GetAnonymous();
            }
            var jwtSecurityToken = tokenHandler.ReadJwtToken(token);
            var identity = new ClaimsIdentity(jwtSecurityToken.Claims);
            return new ClaimsPrincipal(identity);
        }

        private static ClaimsPrincipal GetAnonymous()
        {
            return new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Sid, "0"),
                new Claim(ClaimTypes.Name, "Anonymous"),
                new Claim(ClaimTypes.Role, "Anonymous")
            }, null));
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var user = await GetTokenUser();
            return new AuthenticationState(user);
        }
    }
}
