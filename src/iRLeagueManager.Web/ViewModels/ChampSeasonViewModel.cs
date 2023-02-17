using iRLeagueApiCore.Common.Models;
using iRLeagueManager.Web.Data;
using iRLeagueManager.Web.Extensions;

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

    public async Task<StatusResult> SaveChangesAsync(CancellationToken cancellationToken)
    {
        if (ApiService.CurrentLeague is null)
        {
            return LeagueNullResult();
        }

        try
        {
            Loading = true;
            var result = await ApiService.CurrentLeague
                .ChampSeasons()
                .WithId(ChampSeasonId)
                .Put(model, cancellationToken);
            if (result.Success && result.Content is not null)
            {
                SetModel(result.Content);
            }
            return result.ToStatusResult();
        }
        finally
        {
            Loading = false;
        }
    }
}