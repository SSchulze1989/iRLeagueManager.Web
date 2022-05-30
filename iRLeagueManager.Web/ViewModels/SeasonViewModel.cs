using iRLeagueApiCore.Client;
using iRLeagueApiCore.Client.Results;
using iRLeagueApiCore.Communication.Models;
using iRLeagueManager.Web.Data;
using MvvmBlazor.ViewModel;
using System.Runtime.CompilerServices;

namespace iRLeagueManager.Web.ViewModels;

public class SeasonViewModel : ViewModelBase
{
    private readonly ILoggerFactory loggerFactory;
    private readonly ILogger<SeasonViewModel> logger;
    private readonly LeagueApiService apiService;
    private SeasonModel model;

    public SeasonViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService) :
        this(loggerFactory, apiService, new SeasonModel())
    { }

    public SeasonViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService, SeasonModel model)
    {
        this.loggerFactory = loggerFactory;
        logger = loggerFactory.CreateLogger<SeasonViewModel>();
        this.apiService = apiService;
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

    public string SeasonName
    {
        get => model.SeasonName;
        set
        {
            SetProperty(model.SeasonName, x => model.SeasonName = x, value);
        }
    }

    public long? MainScoringId
    {
        get => model.MainScoringId;
        set
        {
            model.MainScoringId = value;
            OnPropertyChanged();
        }
    }

    public bool HideComments
    {
        get => HideComments;
        set
        {
            model.HideComments = value;
            OnPropertyChanged();
        }
    }

    public bool Finished
    {
        get => model.Finished;
        set
        {
            model.Finished = value;
            OnPropertyChanged();
        }
    }

    public IEnumerable<long> ScheduleIds
    {
        get => model.ScheduleIds;
    }

    public void SetModel(SeasonModel model)
    {
        model ??= new SeasonModel();
        this.model = model;
        OnPropertyChanged();
    }

    public async Task<bool> SaveCurrentModel()
    {
        if (model.SeasonId == 0 || apiService.CurrentLeague == null)
        {
            return false;
        }
        var result = await apiService.CurrentLeague
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
}
