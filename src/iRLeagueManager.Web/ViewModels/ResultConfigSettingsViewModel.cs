using iRLeagueApiCore.Common.Models;
using iRLeagueManager.Web.Data;
using iRLeagueManager.Web.Extensions;

namespace iRLeagueManager.Web.ViewModels;

public sealed class ResultConfigSettingsViewModel : LeagueViewModelBase<ResultConfigSettingsViewModel>
{
    public ResultConfigSettingsViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService) :
        base(loggerFactory, apiService)
    {
        resultConfigs = new ObservableCollection<ResultConfigViewModel>();
    }

    private ObservableCollection<ResultConfigViewModel> resultConfigs;
    public ObservableCollection<ResultConfigViewModel> ResultsConfigs { get => resultConfigs; set => Set(ref resultConfigs, value); }

    private ResultConfigViewModel? selected;
    public ResultConfigViewModel? Selected { get => selected; set => Set(ref selected, value); }

    public async Task LoadFromLeagueAsync()
    {
        if (ApiService.CurrentLeague == null)
        {
            return;
        }

        var resultConfigsEndpoint = ApiService.CurrentLeague.ResultConfigs();
        var resultConfigsResult = await resultConfigsEndpoint.Get();
        if (resultConfigsResult.Success == false || resultConfigsResult.Content is null)
        {
            return;
        }

        ResultsConfigs = new ObservableCollection<ResultConfigViewModel>(
            resultConfigsResult.Content.Select(x => new ResultConfigViewModel(LoggerFactory, ApiService, x)));
    }

    public async Task<StatusResult> AddConfiguration()
    {
        if (ApiService.CurrentLeague is null)
        {
            return LeagueNullResult();
        }

        try
        {
            Loading = true;
            ResultConfigModel newConfig = new() { Name = "New Config", DisplayName = "New Config" };
            var request = ApiService.CurrentLeague.ResultConfigs()
                .Post(newConfig);
            var result = await request;

            if (result.Success == true && result.Content is not null)
            {
                ResultsConfigs.Add(new ResultConfigViewModel(LoggerFactory, ApiService, result.Content));
            }

            return result.ToStatusResult();
        }
        finally
        {
            Loading = false;
        }
    }

    public async Task<StatusResult> DeleteConfiguration(ResultConfigViewModel config)
    {
        if (ApiService.CurrentLeague is null)
        {
            return LeagueNullResult();
        }

        try
        {
            Loading = true;
            var request = ApiService.CurrentLeague.ResultConfigs()
                .WithId(config.ResultConfigId)
                .Delete();
            var result = await request;

            if (result.Success)
            {
                ResultsConfigs.Remove(config);
            }

            return result.ToStatusResult();
        }
        finally
        {
            Loading = false;
        }
    }
}
