using iRLeagueApiCore.Client.Http;
using iRLeagueApiCore.Client.QueryBuilder;
using iRLeagueApiCore.Common.Models;

namespace iRLeagueApiCore.Client.Endpoints.Championships;
internal sealed class ChampionshipsEndpoint : PostGetAllEndpoint<ChampionshipModel, PostChampionshipModel>, IChampionshipsEndpoint, ISeasonChampionshipsEndpoint
{
    public ChampionshipsEndpoint(HttpClientWrapper httpClientWrapper, RouteBuilder routeBuilder) : 
        base(httpClientWrapper, routeBuilder)
    {
        RouteBuilder.AddEndpoint("Championships");
    }

    IChampionshipByIdEndpoint IWithIdEndpoint<IChampionshipByIdEndpoint, long>.WithId(long id)
    {
        return new ChampionshipByIdEndpoint(HttpClientWrapper, RouteBuilder, id);
    }

    ISeasonChampionshipByIdEndpoint IWithIdEndpoint<ISeasonChampionshipByIdEndpoint, long>.WithId(long id)
    {
        return new ChampionshipByIdEndpoint(HttpClientWrapper, RouteBuilder, id);
    }
}
