using iRLeagueManager.Web.Exceptions;
using iRLeagueManager.Web.ViewModels;
using Microsoft.AspNetCore.Components;
using System.ComponentModel;

namespace iRLeagueManager.Web.Shared;

public abstract partial class LeagueComponentBase : UtilityComponentBase
{
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

    protected virtual void RedirectUrl()
    {
    }

    protected override void OnParametersSet()
    {
        BlazorParameterNullException.ThrowIfNull(this, EventList, cascading: true);
        if (SeasonId == null && EventId == null && Shared.SeasonId != 0)
        {
            SeasonId = Shared.SeasonId;
        }
        if (HasRendered)
        {
            if (EventId != Event?.EventId && EventId != null)
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
                .Get().ConfigureAwait(false);
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
        base.OnInitialized();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            EventList.PropertyChanged -= OnEventListPropertyChanged;
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
        return GetRoleString(Shared.LeagueName, roleNames);
    }

    /// <summary>
    /// Get a link to a resource in the current league
    /// </summary>
    /// <param name="relativeUrl">part of the url relative to the link. E.g "Results" for "/{LeagueName}/Results"</param>
    /// <returns></returns>
    protected string GetLeagueLink(string relativeUrl)
    {
        return $"/{Shared.LeagueName}/{relativeUrl}";
    }
}
