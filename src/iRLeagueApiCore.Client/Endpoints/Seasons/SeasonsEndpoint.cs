using iRLeagueApiCore.Client.Http;
using iRLeagueApiCore.Client.QueryBuilder;
using iRLeagueApiCore.Common.Models;

namespace iRLeagueApiCore.Client.Endpoints.Seasons;

internal class SeasonsEndpoint : PostGetAllEndpoint<SeasonModel, PostSeasonModel>, ISeasonsEndpoint
{
    public SeasonsEndpoint(HttpClientWrapper httpClientWrapper, RouteBuilder routeBuilder) :
        base(httpClientWrapper, routeBuilder)
    {
        RouteBuilder.AddEndpoint("Seasons");
    }

    IGetEndpoint<SeasonModel> ISeasonsEndpoint.Current()
    {
        return new CurrentSeasonEndpoint(HttpClientWrapper, RouteBuilder);
    }

    ISeasonByIdEndpoint IWithIdEndpoint<ISeasonByIdEndpoint, long>.WithId(long id)
    {
        return new SeasonByIdEndpoint(HttpClientWrapper, RouteBuilder, id);
    }
}
