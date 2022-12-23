﻿using iRLeagueApiCore.Client.Http;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace iRLeagueManager.Web.Shared;

public class JwtAuthenticationStateProvicer : AuthenticationStateProvider, IDisposable
{
    private readonly JwtSecurityTokenHandler tokenHandler = new();
    private readonly IAsyncTokenProvider tokenStore;
    private string lastToken = string.Empty;

    public JwtAuthenticationStateProvicer(IAsyncTokenProvider tokenStore)
    {
        this.tokenStore = tokenStore;
        tokenStore.TokenChanged += TokenStore_TokenChanged;
    }

    private void TokenStore_TokenChanged(object? sender, EventArgs e)
    {
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    private async Task<ClaimsPrincipal> GetTokenUser()
    {
        var token = await tokenStore.GetTokenAsync();
        if (string.IsNullOrEmpty(token))
        {
            return GetAnonymous();
        }
        var jwtSecurityToken = tokenHandler.ReadJwtToken(token);
        var identity = new ClaimsIdentity(jwtSecurityToken.Claims, "bearer");
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

    void IDisposable.Dispose()
    {
        tokenStore.TokenChanged -= TokenStore_TokenChanged;
    }
}
