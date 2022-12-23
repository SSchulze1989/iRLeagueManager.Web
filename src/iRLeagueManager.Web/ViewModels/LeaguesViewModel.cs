using iRLeagueManager.Web.Data;

namespace iRLeagueManager.Web.ViewModels
{
    public class LeaguesViewModel : LeagueViewModelBase<LeaguesViewModel>
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
                Loading = true;
                Status = string.Empty;
                var result = await ApiService.Client.Leagues().Get();
                Status = result.Status;
                if (result.Success && result.Content is not null)
                {
                    var leagueModels = result.Content;
                    Leagues = new ObservableCollection<LeagueViewModel>(
                        leagueModels.Select(x => new LeagueViewModel(LoggerFactory, ApiService, x))
                    );
                }
                Loading = false;
            }
        }
    }
}
