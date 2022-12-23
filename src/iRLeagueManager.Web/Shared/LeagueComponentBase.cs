using iRLeagueApiCore.Common;
using iRLeagueManager.Web.Data;
using iRLeagueManager.Web.Extensions;
using iRLeagueManager.Web.ViewModels;
using Microsoft.AspNetCore.Components;
using MvvmBlazor.Components;
using System.ComponentModel;

namespace iRLeagueManager.Web.Shared;

public abstract partial class LeagueComponentBase : MvvmComponentBase
{
    [Inject]
    public SharedStateService Shared { get; set; } = default!;
    [Inject]
    public LeagueApiService ApiService { get; set; } = default!;
    [Inject]
    public NavigationManager NavigationManager { get; set; } = default!;

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
        if (EventId != null)
        {
            EventList.Selected = EventList.EventList.FirstOrDefault(x => x.EventId == EventId);
            return;
        }
        if (EventList.Selected == null)
        {
            EventList.Selected = EventList.EventList.LastOrDefault(x => x.HasResult);
            EventId = EventList.Selected?.EventId;
        }
    }

    protected async Task LoadEventList(long seasonId)
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
        Shared.StateChanged -= SharedStateChanged;
        EventList.PropertyChanged -= OnEventListPropertyChanged;
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
        if (LeagueName != null)
        {
            var leagueRoleNames = roleNames
                .Select(x => LeagueRoles.GetLeagueRoleName(LeagueName, x))
                .NotNull();
            roles = roles.Concat(leagueRoleNames);
        }
        return string.Join(',', roles);
    }
}
