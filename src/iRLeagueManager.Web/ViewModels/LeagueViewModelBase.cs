using iRLeagueApiCore.Client.Endpoints.Leagues;
using iRLeagueApiCore.Client.Endpoints.Seasons;
using iRLeagueManager.Web.Data;
using iRLeagueManager.Web.Shared;
using Microsoft.AspNetCore.Components;
using MvvmBlazor.ViewModel;
using System.Runtime.CompilerServices;

namespace iRLeagueManager.Web.ViewModels;

public abstract class LeagueViewModelBase<T> : ViewModelBase, IModelState, IDisposable
    where T : LeagueViewModelBase<T>
{
    public LeagueViewModelBase(ILoggerFactory loggerFactory, LeagueApiService apiService)
    {
        LoggerFactory = loggerFactory;
        Logger = loggerFactory.CreateLogger<T>();
        this.ApiService = apiService;
    }

    protected ILoggerFactory LoggerFactory { get; }
    protected ILogger<T> Logger { get; }
    protected LeagueApiService ApiService { get; }
    protected CancellationTokenSource Cts { get; } = new();
    protected ILeagueByNameEndpoint? CurrentLeague => ApiService.CurrentLeague;
    protected ISeasonByIdEndpoint? CurrentSeason => ApiService.CurrentSeason;

    private bool loading;
    public bool Loading
    {
        get => loading;
        protected set => Set(ref loading, value);
    }

    private bool saving;
    public bool Saving
    {
        get => saving;
        protected set => Set(ref saving, value);
    }

    private bool hasChanges = false;

    public bool HasChanges
    {
        get => hasChanges;
        protected set => Set(ref hasChanges, value);
    }

    public EventHandler? HasChanged { get; set; }
    protected bool suppressHasChanged = false;
    private bool disposedValue;

    /// <summary>
    /// Set a value on a model property and call OnPropertyChanged() if value changed
    /// </summary>
    /// <typeparam name="TProperty"></typeparam>
    /// <param name="get">current value of property</param>
    /// <param name="set">action to update property value</param>
    /// <param name="value">new value to set</param>
    /// <param name="propertyName">Name of the property for OnPropertyChanged()</param>
    /// <returns></returns>
    protected bool SetP<TProperty>(TProperty get, Action<TProperty> set, TProperty value, [CallerMemberName] string? propertyName = null)
    {
        if (!EqualityComparer<TProperty>.Default.Equals(get, value))
        {
            set.Invoke(value);
            HasChanges = true;
            OnHasChanged();
            OnPropertyChanged(propertyName);
            return true;
        }
        return false;
    }

    protected virtual void OnHasChanged()
    {
        if (Loading == false)
        {
            HasChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    protected static StatusResult LeagueNullResult() =>
        StatusResult.FailedResult("League Null", $"{nameof(LeagueApiService)}.{nameof(LeagueApiService.CurrentLeague)} was null", Array.Empty<object>());

    protected static StatusResult SeasonNullResult() =>
        StatusResult.FailedResult("Season Null", $"{nameof(LeagueApiService)}.{nameof(LeagueApiService.CurrentSeason)} was null", Array.Empty<object>());

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in der Methode "Dispose(bool disposing)" ein.
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}

public abstract class LeagueViewModelBase<TViewModel, TModel> : LeagueViewModelBase<TViewModel>
    where TViewModel : LeagueViewModelBase<TViewModel, TModel>
    where TModel : class 
{
    protected TModel model = default!;

    public LeagueViewModelBase(ILoggerFactory loggerFactory, LeagueApiService apiService, TModel model) :
        base(loggerFactory, apiService)
    {
        SetModel(model);
    }

    public virtual TModel GetModel()
    {
        return model;
    }

    public virtual void SetModel(TModel model)
    {
        this.model = model;
        HasChanges = false;
    }

    public virtual TModel CopyModel()
    {
        return ModelHelper.CopyModel(model)!;
    }

    public virtual void ResetChangedState()
    {
        HasChanges = false;
    }
}
