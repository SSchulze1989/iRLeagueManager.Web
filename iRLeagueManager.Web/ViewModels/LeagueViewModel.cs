using iRLeagueApiCore.Communication.Models;
using iRLeagueApiCore.Client;
using System.Threading.Tasks;
using MvvmBlazor;
using MvvmBlazor.ViewModel;
using iRLeagueManager.Web.Extensions;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using Microsoft.AspNetCore.Identity;

namespace iRLeagueManager.Web.ViewModels;

public partial class LeagueViewModel : ViewModelBase
{
    private readonly ILogger<LeagueViewModel> logger;
    private readonly ILeagueApiClient apiClient;

    public LeagueViewModel(ILogger<LeagueViewModel> logger, ILeagueApiClient apiClient, LeagueModel model)
    {
        this.logger = logger;
        this.apiClient = apiClient;
        _model = model;
        _seasons = new ObservableCollection<SeasonViewModel>();
    }

    private readonly LeagueModel _model;

    public long LeagueId => _model.Id;
    public string LeagueName
    {
        get => _model.Name;
        set
        {
            _model.Name = value;
            OnPropertyChanged();
        }
    }

    public string NameFull
    {
        get => _model.NameFull;
        set
        {
            _model.NameFull = value;
            OnPropertyChanged();
        }
    }

    private ObservableCollection<SeasonViewModel> _seasons;
    public ObservableCollection<SeasonViewModel> Seasons
    {
        get => _seasons;
        set => Set(ref _seasons, value);
    }

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set => Set(ref _isLoading, value);
    }

    public override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            IsLoading = true;
            try
            {
                Seasons = new ObservableCollection<SeasonViewModel>();
                await Task.FromResult(true);
            }
            catch (ActionResultException<LeagueModel> ex)
            {
                logger.LogError(ex.ActionResult.Message);
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}