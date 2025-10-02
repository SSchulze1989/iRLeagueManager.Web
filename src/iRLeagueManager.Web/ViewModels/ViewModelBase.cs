using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace iRLeagueManager.Web.ViewModels;

public class ViewModelBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected bool Set<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        return Set(ref field, value, EqualityComparer<T>.Default, propertyName);
    }

    protected bool Set<T>(ref T field, T value, IEqualityComparer<T> equalityComparer, [CallerMemberName] string? propertyName = null)
    {
        ArgumentNullException.ThrowIfNull(equalityComparer, "equalityComparer");
        if (!equalityComparer.Equals(field, value))
        {
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        return false;
    }

    public virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        ArgumentNullException.ThrowIfNull(propertyName, "propertyName");
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public virtual Task OnAfterRenderAsync(bool firstRender)
    {
        return Task.CompletedTask;
    }
}
