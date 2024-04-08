using iRLeagueApiCore.Client.Http;
using iRLeagueApiCore.Client.QueryBuilder;
using iRLeagueApiCore.Common.Models;

namespace iRLeagueApiCore.Client.Endpoints.Leagues;

internal sealed class LeagueByIdEndpoint : UpdateEndpoint<LeagueModel, PutLeagueModel>, ILeagueByIdEndpoint
{
    public LeagueByIdEndpoint(HttpClientWrapper httpClientWrapper, RouteBuilder routeBuilder, long leagueId) :
        base(httpClientWrapper, routeBuilder)
    {
        RouteBuilder.AddParameter(leagueId);
    }

    IPostEndpoint<LeagueModel> ILeagueByIdEndpoint.Initialize()
    {
        return new PostEndpoint<LeagueModel>(HttpClientWrapper, RouteBuilder);
    }
}
