using iRLeagueManager.Web.Data;
using System.Collections.ObjectModel;

namespace iRLeagueManager.Web.ViewModels
{
    public class ResultConfigSettingsViewModel : LeagueViewModelBase<ResultConfigSettingsViewModel>
    {
        public ResultConfigSettingsViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService) : 
            base(loggerFactory, apiService)
        {
            resultConfigs = new ObservableCollection<ResultConfigViewModel>();
            selected = new ResultConfigViewModel(loggerFactory, apiService);
        }

        private ObservableCollection<ResultConfigViewModel> resultConfigs;
        public ObservableCollection<ResultConfigViewModel> ResultsConfigs { get => resultConfigs; set => Set(ref resultConfigs, value); }

        private ResultConfigViewModel selected;
        public ResultConfigViewModel Selected { get => selected; set => Set(ref selected, value); }

        public async Task LoadFromLeagueAsync()
        {
            if (ApiService.CurrentLeague == null)
            {
                return;
            }

            var resultConfigsEndpoint = ApiService.CurrentLeague.ResultConfigs();
            var resultConfigsResult = await resultConfigsEndpoint.Get();
            if (resultConfigsResult.Success == false)
            {
                return;
            }

            ResultsConfigs = new ObservableCollection<ResultConfigViewModel>(
                resultConfigsResult.Content.Select(x => new ResultConfigViewModel(LoggerFactory, ApiService, x)));
        }
    }
}
