using iRLeagueApiCore.Client;
using iRleagueManager.Web.Server.Extensions;
using Microsoft.Extensions.Logging;
using MvvmBlazor.ViewModel;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace iRleagueManager.Web.Server.ViewModels
{
    public class LeaguesViewModel : ViewModelBase
    {
        private readonly ILoggerFactory loggerFactory;
        private readonly ILogger<LeaguesViewModel> logger;
        private readonly ILeagueApiClient apiClient;

        private string _status;
        public string Status { get => _status; set => Set(ref _status, value); }

        public LeaguesViewModel(ILoggerFactory loggerFactory, ILeagueApiClient apiClient)
        {
            this.loggerFactory = loggerFactory;
            logger = loggerFactory.CreateLogger<LeaguesViewModel>();
            this.apiClient = apiClient;
        }

        public ObservableCollection<LeagueViewModel> Leagues { get; set; }

        public override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                Status = string.Empty;
                var result = await apiClient.Leagues().Get();
                Status = result.Status;
                if (result.Success)
                {
                    var leagueModels = result.Content;
                    Leagues = new ObservableCollection<LeagueViewModel>(
                        leagueModels.Select(x => new LeagueViewModel(loggerFactory.CreateLogger<LeagueViewModel>(), apiClient, x))
                    );
                }
                StateHasChanged();
            }
        }
    }
}
