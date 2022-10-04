using iRLeagueApiCore.Client;
using iRLeagueApiCore.Client.Results;
using iRLeagueApiCore.Common.Models;
using iRLeagueManager.Web.Data;
using MvvmBlazor.ViewModel;
using System.Runtime.CompilerServices;

namespace iRLeagueManager.Web.ViewModels;

public class SeasonViewModel : LeagueViewModelBase<SeasonViewModel>
{
    private SeasonModel model;

    public SeasonViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService) :
        this(loggerFactory, apiService, new SeasonModel())
    { }

    public SeasonViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService, SeasonModel model) :
        base (loggerFactory, apiService)
    {
        this.model = model;
    }

    private void SetProperty<T>(T get, Action<T> set, T value, [CallerMemberName] string? propertyName = null) where T : IEquatable<T>
    {
        if (get.Equals(value) == false)
        {
            set.Invoke(value);
            OnPropertyChanged(propertyName);
        }
    }

    public long SeasonId => model.SeasonId;
    public long LeagueId => model.LeagueId;
    public DateTime? SeasonStart => model.SeasonStart;
    public DateTime? SeasonEnd => SeasonEnd;
    public string? SeasonName { get => model.SeasonName; set => Set(model, x => x.SeasonName, value); }
    public long? MainScoringId { get => model.MainScoringId; set => Set(model, x => x.MainScoringId, value); }
    public bool HideComments { get => model.HideComments; set => Set(model, x => x.HideComments, value); }
    public bool Finished { get => model.Finished; set => Set(model, x => x.Finished, value); }
    public IEnumerable<long> ScheduleIds => model.ScheduleIds;

    public void SetModel(SeasonModel model)
    {
        model ??= new SeasonModel();
        this.model = model;
        OnPropertyChanged();
    }

    public async Task Load(long seasonId)
    {
        try
        {
            Loading = true;
            if (ApiService.CurrentLeague == null)
            {
                return;
            }
            await ApiService.SetCurrentSeasonAsync(ApiService.CurrentLeague.Name, seasonId);
            if (ApiService.CurrentSeason == null)
            {
                return;
            }
            var result = await ApiService.CurrentSeason.Get();
            if (result.Success)
            {
                SetModel(result.Content);
            }
        }
        finally
        {
            Loading = false;
        }
    }

    public async Task<bool> SaveCurrentModel()
    {
        try
        {
            Loading = true;
            await Task.Delay(1000);
            if (model.SeasonId == 0 || ApiService.CurrentLeague == null)
            {
                return false;
            }
            var result = await ApiService.CurrentLeague
                .Seasons()
                .WithId(model.SeasonId)
                .Put(model);
            if (result.Success)
            {
                model = result.Content;
                return true;
            }
            return false;
        }
        finally
        {
            Loading = false;
        }
    }
}
