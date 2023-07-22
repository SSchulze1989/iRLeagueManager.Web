using iRLeagueApiCore.Common.Models;
using iRLeagueManager.Web.Data;

namespace iRLeagueManager.Web.ViewModels;

public sealed class AutoPenaltiesCollectionViewModel : ViewModelCollection<AutoPenaltyConfigViewModel, AutoPenaltyConfiguration, AutoPenaltiesCollectionViewModel>
{
    public AutoPenaltiesCollectionViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService) 
        : base(loggerFactory, apiService)
    {
    }

    public override AutoPenaltyConfigViewModel CreateInstance(AutoPenaltyConfiguration model)
    {
        return new(LoggerFactory, ApiService, model);
    }
}
