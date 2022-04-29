using iRLeagueApiCore.Client;
using iRleagueManager.Web.Extensions;
using Microsoft.Extensions.Logging;
using MvvmBlazor.ViewModel;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace iRleagueManager.Web.ViewModels
{
    public class LeaguesViewModel : ViewModelBase
    {
        private readonly ILoggerFactory loggerFactory;
        private readonly ILogger<LeaguesViewModel> logger;
        private readonly ILeagueApiClient apiClient;

        public LeaguesViewModel(ILoggerFactory loggerFactory, ILeagueApiClient apiClient)
        {
            this.loggerFactory = loggerFactory;
            logger = loggerFactory.CreateLogger<LeaguesViewModel>();
            this.apiClient = apiClient;
        }

        public ObservableCollection<LeagueViewModel> Leagues { get; set; }

        public override async Task OnInitializedAsync()
        {
            var leagueModels = (await apiClient.Leagues().Get()).EnsureSuccess();
            Leagues = new ObservableCollection<LeagueViewModel>(
                leagueModels.Select(x => new LeagueViewModel(loggerFactory.CreateLogger<LeagueViewModel>(), apiClient, x))
            );
        }
    }
}
