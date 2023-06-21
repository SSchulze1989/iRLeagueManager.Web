using iRLeagueApiCore.Common.Models;
using iRLeagueManager.Web.Data;
using iRLeagueManager.Web.Extensions;

namespace iRLeagueManager.Web.ViewModels;

public sealed class LeaguesViewModel : LeagueViewModelBase<LeaguesViewModel>
{
    private string _status;
    public string Status { get => _status; set => Set(ref _status, value); }

    public LeaguesViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService) :
        base(loggerFactory, apiService)
    {
        _status = string.Empty;
        leagues = new ObservableCollection<LeagueViewModel>();
    }

    private ObservableCollection<LeagueViewModel> leagues;
    public ObservableCollection<LeagueViewModel> Leagues { get => leagues; set => Set(ref leagues, value); }

    //public override void OnInitialized()
    //{
    //    apiClient = RootServiceProvider.GetRequiredService<ILeagueApiClient>();
    //}

    //public override async Task OnAfterRenderAsync(bool firstRender)
    public override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            Status = string.Empty;
            var result = await LoadLeagues();
            Status = result.Status;
        }
    }

    public async Task<StatusResult> LoadLeagues(CancellationToken cancellationToken = default)
    {
        try
        {
            Loading = true;
            var result = await ApiService.Client
                .Leagues()
                .Get(cancellationToken);
            if (result.Success && result.Content is not null)
            {
                var leagueModels = result.Content;
                Leagues = new ObservableCollection<LeagueViewModel>(
                    leagueModels.Select(x => new LeagueViewModel(LoggerFactory, ApiService, x))
                );
            }
            return result.ToStatusResult();
        }
        finally
        {
            Loading = false;
        }
    }

    public async Task<StatusResult<LeagueModel>> AddLeague(PostLeagueModel model, CancellationToken cancellationToken = default)
    {
        try
        {
            Loading = true;
            var result = await ApiService.Client
                .Leagues()
                .Post(model, cancellationToken);
            return result.ToContentStatusResult();
        }
        finally
        {
            Loading = false;
        }
    }
}
