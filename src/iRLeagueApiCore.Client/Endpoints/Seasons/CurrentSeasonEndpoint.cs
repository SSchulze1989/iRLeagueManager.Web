using iRLeagueApiCore.Client.Http;
using iRLeagueApiCore.Client.QueryBuilder;
using iRLeagueApiCore.Common.Models;

namespace iRLeagueApiCore.Client.Endpoints.Seasons;

internal sealed class CurrentSeasonEndpoint : GetEndpoint<SeasonModel>
{
    public CurrentSeasonEndpoint(HttpClientWrapper httpClient, RouteBuilder routeBuilder) :
        base(httpClient, routeBuilder)
    {
        RouteBuilder.AddEndpoint("Current");
    }
}
