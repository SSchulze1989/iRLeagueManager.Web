using iRLeagueApiCore.Client.Endpoints.Results;
using iRLeagueApiCore.Client.Http;
using iRLeagueApiCore.Client.QueryBuilder;
using iRLeagueApiCore.Common.Models;

namespace iRLeagueApiCore.Client.Endpoints.Championships;
internal class ChampSeasonByIdEndpoint : UpdateEndpoint<ChampSeasonModel, PutChampSeasonModel>, IChampSeasonByIdEndpoint
{
    public ChampSeasonByIdEndpoint(HttpClientWrapper httpClientWrapper, RouteBuilder routeBuilder, long id) : 
        base(httpClientWrapper, routeBuilder)
    {
        RouteBuilder.AddParameter(id);
    }

    IPostEndpoint<ResultConfigModel, PostResultConfigModel> IChampSeasonByIdEndpoint.ResultConfigs()
    {
        return new ResultConfigsEndpoint(HttpClientWrapper, RouteBuilder);
    }
}
