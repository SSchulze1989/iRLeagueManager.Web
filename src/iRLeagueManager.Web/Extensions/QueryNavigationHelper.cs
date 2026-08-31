using Microsoft.AspNetCore.Components;

namespace iRLeagueManager.Web.Extensions;

public static class QueryNavigationHelper
{
    public static string BuildUriWithMergedQuery(this NavigationManager navigationManager, string path, IReadOnlyDictionary<string, object?> queryParameters)
    {
        var navUrl = $"{path}?{navigationManager.QueryString()}";
        return navigationManager.GetUriWithQueryParameters(navUrl, queryParameters);
    }

    public static bool ShouldReplaceNavigation(this NavigationManager navigationManager, string navUrl)
    {
        if (navigationManager.Uri.Contains("/Events/") == false)
        {
            return true;
        }

        var urlWithoutParameters = navUrl.Split('?').First();
        return navigationManager.Uri.Contains(urlWithoutParameters);
    }
}
