using iRLeagueApiCore.Client.Endpoints;
using iRLeagueApiCore.Common.Models;

public interface IChampSeasonByIdEndpoint : IUpdateEndpoint<ChampSeasonModel, PutChampSeasonModel>
{
    public IPostEndpoint<ResultConfigModel, PostResultConfigModel> ResultConfigs();
}