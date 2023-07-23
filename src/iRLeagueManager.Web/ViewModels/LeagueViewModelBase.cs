﻿using iRLeagueApiCore.Client.Endpoints.Leagues;
using iRLeagueApiCore.Client.Endpoints.Seasons;
using iRLeagueManager.Web.Data;
using iRLeagueManager.Web.Shared;
using MvvmBlazor.ViewModel;
using System.Runtime.CompilerServices;

namespace iRLeagueManager.Web.ViewModels;

public abstract class LeagueViewModelBase<T> : ViewModelBase, IModelState
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

    private bool hasChanged = false;

    public bool HasChanged
    {
        get => hasChanged;
        protected set => Set(ref hasChanged, value);
    }

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
            HasChanged = true;
            OnPropertyChanged(propertyName);
            return true;
        }
        return false;
    }

    protected static StatusResult LeagueNullResult() =>
        StatusResult.FailedResult("League Null", $"{nameof(LeagueApiService)}.{nameof(LeagueApiService.CurrentLeague)} was null", Array.Empty<object>());

    protected static StatusResult SeasonNullResult() =>
        StatusResult.FailedResult("Season Null", $"{nameof(LeagueApiService)}.{nameof(LeagueApiService.CurrentSeason)} was null", Array.Empty<object>());
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
        HasChanged = false;
    }

    public virtual TModel CopyModel()
    {
        return ModelHelper.CopyModel(model)!;
    }

    public virtual void ResetChangedState()
    {
        HasChanged = false;
    }
}
