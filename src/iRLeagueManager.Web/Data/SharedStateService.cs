using iRLeagueApiCore.Common.Models;
using iRLeagueManager.Web.ViewModels;
using System.Runtime.CompilerServices;

namespace iRLeagueManager.Web.Data;

public sealed class SharedStateService
{
    private bool loggedIn;
    public bool LoggedIn { get => loggedIn; set => Set(ref loggedIn, value); }

    private string? username;
    public string? Username { get => username; set => Set(ref username, value); }

    private string? leagueName;
    public string? LeagueName { get => leagueName; set => Set(ref leagueName, value); }

    private long seasonId;
    public long SeasonId { get => seasonId; set => Set(ref seasonId, value); }

    private string? seasonName;
    public string? SeasonName { get => seasonName; set => Set(ref seasonName, value); }

    private bool seasonFinished;
    public bool SeasonFinished { get => seasonFinished; set => Set(ref seasonFinished, value); }

    private ObservableCollection<SeasonModel> seasonList;
    public ObservableCollection<SeasonModel> SeasonList { get => seasonList; set => Set(ref seasonList, value); }

    public event EventHandler? StateChanged;

    private TimeZoneInfo localTimeZone;
    public TimeZoneInfo LocalTimeZone { get => localTimeZone; set => Set(ref localTimeZone, value); }

    private LeagueModel? leagueInfo;
    public LeagueModel? LeagueInfo { get => leagueInfo; set => Set(ref leagueInfo, value); }

    public SharedStateService()
    {
        seasonList = new ObservableCollection<SeasonModel>();
        localTimeZone = TimeZoneInfo.Local;
    }

    private void Set<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if ((field != null && field.Equals(value) == false) || (field == null && value != null))
        {
            field = value;
            OnStateHasChanged(propertyName);
        }
    }

    private void OnStateHasChanged([CallerMemberName] string? propertyName = null)
    {
        StateChanged?.Invoke(this, EventArgs.Empty);
    }
}
