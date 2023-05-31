using iRLeagueApiCore.Common;
using iRLeagueManager.Web.Data;
using iRLeagueManager.Web.Extensions;
using iRLeagueManager.Web.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;
using MvvmBlazor.Components;
using System.ComponentModel;
using System.Net;

namespace iRLeagueManager.Web.Shared;

public abstract partial class LeagueComponentBase : MvvmComponentBase
{
    [Inject]
    public SharedStateService Shared { get; set; } = default!;
    [Inject]
    public LeagueApiService ApiService { get; set; } = default!;
    [Inject]
    public NavigationManager NavigationManager { get; set; } = default!;

    [Inject]
    protected IJSRuntime JsRuntime { get; set; } = default!;

    private EventListViewModel eventList = default!;

    [CascadingParameter]
    public EventListViewModel EventList
    {
        get => eventList;
        set
        {
            if (eventList != null)
            {
                eventList.PropertyChanged -= OnEventListPropertyChanged;
            }
            eventList = value;
            if (eventList != null)
            {
                eventList.PropertyChanged += OnEventListPropertyChanged;
            }
        }
    }
    [Parameter]
    public string? LeagueName { get; set; }
    [Parameter]
    public long? SeasonId { get; set; }
    [Parameter]
    public long? EventId { get; set; }

    protected bool ParametersSet { get; set; } = false;
    protected bool HasRendered { get; set; } = false;
    protected EventViewModel? Event => EventList?.Selected;
    private readonly CancellationTokenSource cancellationTokenSource = new();
    protected CancellationToken CancellationToken => cancellationTokenSource.Token;

    private IDisposable? locationChangingHandler;

    protected virtual void SharedStateChanged(object? sender, EventArgs e)
    {
        InvokeAsync(StateHasChanged);
    }

    protected virtual void RedirectUrl()
    {
    }

    protected override void OnParametersSet()
    {
        if (SeasonId == null && EventId == null && Shared.SeasonId != 0)
        {
            SeasonId = Shared.SeasonId;
        }
        if (HasRendered)
        {
            if (EventId != Event?.EventId)
            {
                EventList.Selected = EventList.EventList.FirstOrDefault(x => x.EventId == EventId);
            }
        }
        ParametersSet = true;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender == false || ParametersSet == false)
        {
            return;
        }

        if (LeagueName == null)
        {
            return;
        }
        await ApiService.SetCurrentLeagueAsync(LeagueName);

        if (ApiService.CurrentLeague == null)
        {
            return;
        }
        if (EventId != null)
        {
            await ApiService.SetCurrentSeasonByEventId(LeagueName, EventId.Value);
        }
        if (ApiService.CurrentSeason == null && SeasonId != null)
        {
            await ApiService.SetCurrentSeasonAsync(ApiService.CurrentLeague.Name, SeasonId.Value);
        }
        if (ApiService.CurrentSeason == null)
        {
            var currentSeason = await ApiService.CurrentLeague.Seasons()
                .Current()
                .Get();
            if (currentSeason.Success && currentSeason.Content is not null)
            {
                await ApiService.SetCurrentSeasonAsync(ApiService.CurrentLeague.Name, currentSeason.Content.SeasonId);
            }
        }
        if (ApiService.CurrentSeason == null)
        {
            var lastSeason = Shared.SeasonList.LastOrDefault();
            if (lastSeason != null)
            {
                await ApiService.SetCurrentSeasonAsync(ApiService.CurrentLeague.Name, lastSeason.SeasonId);
            }
        }
        if (ApiService.CurrentSeason == null)
        {
            return;
        }
        SeasonId = ApiService.CurrentSeason.Id;

        HasRendered = true;
        await LoadEventList(ApiService.CurrentSeason.Id);
        if (EventId != null && EventList.Selected?.EventId != EventId)
        {
            EventList.Selected = EventList.EventList.FirstOrDefault(x => x.EventId == EventId);
            await InvokeAsync(StateHasChanged);
            return;
        }
        if (EventList.Selected == null)
        {
            EventList.Selected = EventList.EventList.LastOrDefault(x => x.HasResult)
                ?? EventList.EventList.FirstOrDefault();
            EventId = EventList.Selected?.EventId;
            await InvokeAsync(StateHasChanged);
        }
    }

    public async Task LoadEventList(long seasonId)
    {
        await EventList.LoadEventListAsync(seasonId);
    }

    protected virtual async Task OnEventChangedAsync(EventViewModel? @event)
    {
        await Task.CompletedTask;
    }

    protected override void OnInitialized()
    {
        Shared.StateChanged += SharedStateChanged;
        base.OnInitialized();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing == false)
        {
            Shared.StateChanged -= SharedStateChanged;
            EventList.PropertyChanged -= OnEventListPropertyChanged;
            locationChangingHandler?.Dispose();
            cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
        }

        base.Dispose(disposing);
    }

    private void OnEventListPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(EventList.Selected):
                _ = OnEventChangedAsync(EventList.Selected);
                break;
        }
    }

    protected string GetRoleString(params string[] roleNames)
    {
        IEnumerable<string> roles = new[] { "Admin" };
        if (Shared.LeagueName != null)
        {
            var roleValues = roleNames.Select(x => LeagueRoles.GetRoleValue(x));
            roleValues = roleValues.Concat(roleValues.SelectMany(LeagueRoles.ImplicitRoleOf)).Distinct();
            var leagueRoleNames = roleValues
                .Select(x => LeagueRoles.GetLeagueRoleName(Shared.LeagueName, x))
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
}
