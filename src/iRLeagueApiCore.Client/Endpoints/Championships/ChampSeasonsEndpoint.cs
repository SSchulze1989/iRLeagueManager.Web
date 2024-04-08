using iRLeagueApiCore.Client.Http;
using iRLeagueApiCore.Client.QueryBuilder;
using iRLeagueApiCore.Common.Models;

namespace iRLeagueApiCore.Client.Endpoints.Championships;
internal class ChampSeasonsEndpoint : GetAllEndpoint<ChampSeasonModel>, IChampSeasonsEndpoint, ISeasonChampSeasonsEndpoint, IChampionshipChampSeasonsEndpoint
{
    public ChampSeasonsEndpoint(HttpClientWrapper httpClientWrapper, RouteBuilder routeBuilder) : 
        base(httpClientWrapper, routeBuilder)
    {
        RouteBuilder.AddEndpoint("ChampSeasons");
    }

    IChampSeasonByIdEndpoint IWithIdEndpoint<IChampSeasonByIdEndpoint, long>.WithId(long id)
    {
        return new ChampSeasonByIdEndpoint(HttpClientWrapper, RouteBuilder, id);
    }
}
