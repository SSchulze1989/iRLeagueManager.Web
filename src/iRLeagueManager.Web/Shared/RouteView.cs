﻿using iRLeagueApiCore.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System.Net;

namespace iRLeagueManager.Web.Shared;

internal sealed class AppRouteView : RouteView
{
    [Inject]
    public NavigationManager NavigationManager { get; set; } = default!;
    [Inject]
    public ILeagueApiClient ApiClient { get; set; } = default!;

    protected override void Render(RenderTreeBuilder builder)
    {
        var authorize = Attribute.GetCustomAttribute(RouteData.PageType, typeof(AuthorizeAttribute)) != null;
        if (authorize && ApiClient.IsLoggedIn == false)
        {
            var returnUrl = WebUtility.UrlEncode(new Uri(NavigationManager.Uri).PathAndQuery);
            NavigationManager.NavigateTo($"members/login?returnUrl={returnUrl}");
        }
        else
        {
            base.Render(builder);
        }
    }
}
