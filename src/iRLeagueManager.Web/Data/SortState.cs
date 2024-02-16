using MudBlazor;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace iRLeagueManager.Web.Data;

public class SortState<TRow> : INotifyPropertyChanged
{
    private SortDirection direction = SortDirection.Ascending;
    public SortDirection Direction { get => direction; set => Set(ref direction, value); }

    private Func<TRow, IComparable>? sortFunc;
    public Func<TRow, IComparable>? SortFunc { get => sortFunc; set => Set(ref sortFunc, value); }

    public event PropertyChangedEventHandler? PropertyChanged;

    private bool Set<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        OnPropertyChanged(propertyName);
        return false;
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
