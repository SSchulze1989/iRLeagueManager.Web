using iRLeagueManager.Web.Data;
using iRLeagueManager.Web.Extensions;

namespace iRLeagueManager.Web.ViewModels;

public sealed class SeasonsViewModel : LeagueViewModelBase<SeasonsViewModel>
{
    private string status = string.Empty;
    public string Status { get => status; set => Set(ref status, value); }

    private ObservableCollection<SeasonViewModel> seasons;
    public ObservableCollection<SeasonViewModel> Seasons { get => seasons; set => Set(ref seasons, value); }

    public SeasonsViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService)
        : base(loggerFactory, apiService)
    {
        seasons = new ObservableCollection<SeasonViewModel>();
    }

    public override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender == false) return;

        try
        {
            Loading = true;
            // load seasons
            if (ApiService.CurrentLeague == null)
            {
                return;
            }
            var seasons = (await ApiService.CurrentLeague.Seasons().Get()).EnsureSuccess();
            if (seasons == null)
            {
                return;
            }
            Seasons = new ObservableCollection<SeasonViewModel>(seasons.Select(x =>
                new SeasonViewModel(LoggerFactory, ApiService, x)
            ));
        }
        finally
        {
            Loading = false;
        }
    }
}
