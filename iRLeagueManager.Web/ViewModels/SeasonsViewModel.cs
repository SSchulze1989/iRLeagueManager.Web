using iRLeagueApiCore.Client;
using iRLeagueManager.Web.Data;
using iRLeagueManager.Web.Extensions;
using iRLeagueManager.Web.ViewModels;
using Microsoft.AspNetCore.Components;
using MvvmBlazor.ViewModel;
using System.Collections.ObjectModel;

namespace iRLeagueManager.Web.ViewModels;

public class SeasonsViewModel : ViewModelBase
{
    private readonly ILoggerFactory loggerFactory;
    private readonly ILogger<SeasonsViewModel> logger;
    private readonly LeagueApiService apiService;

    private string status = string.Empty;
    public string Status { get => status; set => Set(ref status, value); }

    private bool loading = false;
    public bool Loading { get => loading; set => Set(ref loading, value); }

    private ObservableCollection<SeasonViewModel> seasons;
    public ObservableCollection<SeasonViewModel> Seasons { get => seasons; set => Set(ref seasons, value); }

    public SeasonsViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService)
    {
        this.loggerFactory = loggerFactory;
        logger = loggerFactory.CreateLogger<SeasonsViewModel>();
        this.apiService = apiService;
        seasons = new ObservableCollection<SeasonViewModel>();
    }

    public override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender == false) return;

        // load seasons
        if (apiService.CurrentLeague == null)
        {
            return;
        }
        var seasons = (await apiService.CurrentLeague.Seasons().Get()).EnsureSuccess();
        Seasons = new ObservableCollection<SeasonViewModel>(seasons.Select(x =>
            new SeasonViewModel(loggerFactory, apiService, x)
        ));
    }
}
