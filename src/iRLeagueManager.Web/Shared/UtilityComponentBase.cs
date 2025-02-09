using iRLeagueApiCore.Common;
using iRLeagueManager.Web.Data;
using iRLeagueManager.Web.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;
using MudBlazor;
using MvvmBlazor.Components;
using System.Net;
using System.Threading;

namespace iRLeagueManager.Web.Shared;

/// <summary>
/// Base class for components that require access to enhanced utility functions but do not need the overhead of initializing a full <see cref="LeagueComponentBase"/>
/// </summary>
public class UtilityComponentBase : MvvmComponentBase
{
    [Inject]
    public NavigationManager NavigationManager { get; set; } = default!;
    [Inject]
    protected IJSRuntime JsRuntime { get; set; } = default!;
    [Inject]
    protected SharedStateService Shared { get; set; } = default!;
    [Inject]
    public LeagueApiService ApiService { get; set; } = default!;

    private readonly CancellationTokenSource cancellationTokenSource = new();
    protected CancellationToken CancellationToken => cancellationTokenSource.Token;

    private IDisposable? locationChangingHandler;

    private bool loading;
    public bool Loading
    {
        get => loading;
        protected set
        {
            if (loading != value)
            {
                loading = value;
                Shared.LoadingCount += loading ? 1 : -1;
            }
        }
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        NavigationManager.LocationChanged += OnLocationChanged;
        Shared.StateChanged += SharedStateChanged;
    }

    protected virtual void SharedStateChanged(object? sender, EventArgs e)
    {
        InvokeAsync(StateHasChanged);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing == false)
        {
            locationChangingHandler?.Dispose();
            cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
            Shared.StateChanged -= SharedStateChanged;
            NavigationManager.LocationChanged -= OnLocationChanged;
            Loading = false;
        }

        base.Dispose(disposing);
    }

    public string GetRoleString(string? leagueName, params string[] roleNames)
    {
        IEnumerable<string> roles = new[] { "Admin" };
        if (string.IsNullOrEmpty(leagueName) == false)
        {
            var roleValues = roleNames.Select(x => LeagueRoles.GetRoleValue(x));
            roleValues = roleValues.Concat(roleValues.SelectMany(LeagueRoles.ImplicitRoleOf)).Distinct();
            var leagueRoleNames = roleValues
                .Select(x => LeagueRoles.GetLeagueRoleName(leagueName, x))
                .NotNull();
            roles = roles.Concat(leagueRoleNames);
        }
        return string.Join(',', roles);
    }

    protected async Task ScrollToElementId(string id)
    {
        await JsRuntime.InvokeVoidAsync("scrollToElementId", id);
    }

    protected async Task ScrollToElement(ElementReference reference)
    {
        await ScrollToElementId(reference.Id);
    }

    protected async Task EnableTooltips()
    {
        await JsRuntime.InvokeVoidAsync("enableTooltips", "right");
    }

    public static string GetFlagEmoji(string? countryCode)
    {
        if (string.IsNullOrEmpty(countryCode))
        {
            return string.Empty;
        }

        var characters = countryCode
            .ToUpper()
            .Select(x => char.ConvertFromUtf32(x + 0x1F1A5));
        return string.Concat(characters);
    }

    protected void NavigateTo(string url, bool replace = false, string? returnUrl = null)
    {
        if (string.IsNullOrEmpty(returnUrl) == false)
        {
            var queryParameters = new Dictionary<string, object?> { { "returnUrl", returnUrl } };
            url = NavigationManager.GetUriWithQueryParameters(url, queryParameters);
        }
        NavigationManager.NavigateTo(url, replace: replace);
    }

    protected void NavigateToRelative(string relativeUrl, bool replace = false)
    {
        var absoluteUrl = NavigationManager.ToAbsoluteUri(relativeUrl).ToString();
        NavigateTo(absoluteUrl, replace: replace);
    }

    protected void ForceNavigateTo(string url, bool fullReload = false)
    {
        if (fullReload)
        {
            NavigationManager.NavigateTo(url, forceLoad: true);
            return;
        }

        NavigationManager.NavigateTo("/");
        NavigationManager.NavigateTo(url);
    }

    /// <summary>
    /// Activate tracking for location changing.
    /// Override <see cref="OnLocationChanging(LocationChangingContext)"/> to perform an action when the location changes
    /// </summary>
    protected void RegisterLocationChangingHandler()
    {
        locationChangingHandler = NavigationManager.RegisterLocationChangingHandler(OnLocationChanging);
    }

    /// <summary>
    /// Perform action when location is changing (e.g. cancel location on certain conditions)
    /// Requires call to <see cref="RegisterLocationChangingHandler()"/> once to initialize location change tracking
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    protected virtual async ValueTask OnLocationChanging(LocationChangingContext context)
    {
        await Task.CompletedTask;
    }

    /// <summary>
    /// Perform action after location has changed
    /// </summary>
    /// <param name="e"></param>
    protected virtual void OnLocationChanged(object? sender, LocationChangedEventArgs e)
    {
    }

    /// <summary>
    /// Get the current url as HTML encoded
    /// </summary>
    /// <returns></returns>
    protected string GetCurrentUrlEncoded()
    {
        return WebUtility.UrlEncode(new Uri(NavigationManager.Uri).PathAndQuery);
    }

    /// <summary>
    /// Get the url from returnUrl= parameter. If returnUrl is not set -> return the current url as HTML encoded instead
    /// </summary>
    /// <returns></returns>
    protected string GetReturnUrl()
    {
        var returnUrl = NavigationManager.QueryParameter<string>("returnUrl");
        if (string.IsNullOrEmpty(returnUrl))
        {
            return GetCurrentUrlEncoded();
        }
        return WebUtility.UrlEncode(returnUrl);
    }

    protected static StatusResult LeagueNullResult() =>
        StatusResult.FailedResult("League Null", $"{nameof(LeagueApiService)}.{nameof(LeagueApiService.CurrentLeague)} was null", []);

    protected static StatusResult SeasonNullResult() =>
        StatusResult.FailedResult("Season Null", $"{nameof(LeagueApiService)}.{nameof(LeagueApiService.CurrentSeason)} was null", []);

    protected static StatusResult<T> LeagueNullResult<T>(T? content = default) =>
        StatusResult<T>.FailedResult("League Null", content, $"{nameof(LeagueApiService)}.{nameof(LeagueApiService.CurrentSeason)}", []);
}
