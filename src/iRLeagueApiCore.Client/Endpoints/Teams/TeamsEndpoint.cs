using iRLeagueApiCore.Client.Http;
using iRLeagueApiCore.Client.QueryBuilder;
using iRLeagueApiCore.Common.Models;

namespace iRLeagueApiCore.Client.Endpoints.Teams;

internal sealed class TeamsEndpoint : PostGetAllEndpoint<TeamModel, PostTeamModel>, ITeamsEndpoint
{
    public TeamsEndpoint(HttpClientWrapper httpClientWrapper, RouteBuilder routeBuilder) : 
        base(httpClientWrapper, routeBuilder)
    {
        RouteBuilder.AddEndpoint("Teams");
    }

    ITeamByIdEndpoint IWithIdEndpoint<ITeamByIdEndpoint, long>.WithId(long id)
    {
        return new TeamByIdEndpoint(HttpClientWrapper, RouteBuilder, id);
    }
}
