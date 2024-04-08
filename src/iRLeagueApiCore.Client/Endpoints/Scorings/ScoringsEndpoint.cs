using iRLeagueApiCore.Client.Http;
using iRLeagueApiCore.Client.QueryBuilder;
using iRLeagueApiCore.Common.Models;

namespace iRLeagueApiCore.Client.Endpoints.Scorings;

internal class ScoringsEndpoint : PostGetAllEndpoint<ScoringModel, PostScoringModel>, IScoringsEndpoint
{
    public ScoringsEndpoint(HttpClientWrapper httpClientWrapper, RouteBuilder routeBuilder) :
        base(httpClientWrapper, routeBuilder)
    {
        RouteBuilder.AddEndpoint("Scorings");
    }

    IScoringByIdEndpoint IWithIdEndpoint<IScoringByIdEndpoint, long>.WithId(long id)
    {
        return new ScoringByIdEndpoint(HttpClientWrapper, RouteBuilder, id);
    }
}
