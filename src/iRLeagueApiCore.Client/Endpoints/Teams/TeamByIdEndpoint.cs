using iRLeagueApiCore.Client.Http;
using iRLeagueApiCore.Client.QueryBuilder;
using iRLeagueApiCore.Common.Models;

namespace iRLeagueApiCore.Client.Endpoints.Teams;

internal sealed class TeamByIdEndpoint : UpdateEndpoint<TeamModel, PutTeamModel>, ITeamByIdEndpoint
{
    public TeamByIdEndpoint(HttpClientWrapper httpClientWrapper, RouteBuilder routeBuilder, long teamId) : 
        base(httpClientWrapper, routeBuilder)
    {
        RouteBuilder.AddParameter(teamId);
    }
}
