using iRLeagueApiCore.Common.Models;
using iRLeagueManager.Web.Data;

namespace iRLeagueManager.Web.ViewModels;

public sealed class ChampSeasonViewModel : LeagueViewModelBase<ChampSeasonViewModel, ChampSeasonModel>
{
    public ChampSeasonViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService) :
        this(loggerFactory, apiService, new())
    { }

    public ChampSeasonViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService, ChampSeasonModel model) : 
        base(loggerFactory, apiService, model)
    {
    }

    public long ChampSeasonId => model.ChampSeasonId;
    public long ChampionshipId => model.ChampionshipId;
    public long SeasonId => model.SeasonId;
    public string ChampionshipName => model.ChampionshipName;
    public string SeasonName => model.SeasonName;

    public StandingConfigModel? StandingConfig { get => model.StandingConfig; set => SetP(model.StandingConfig, value => model.StandingConfig = value, value); }

    public ICollection<ResultConfigInfoModel> ResultConfigs => model.ResultConfigs;
}