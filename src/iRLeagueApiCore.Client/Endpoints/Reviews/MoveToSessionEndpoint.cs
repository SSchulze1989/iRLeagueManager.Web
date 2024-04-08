using iRLeagueApiCore.Client.Http;
using iRLeagueApiCore.Client.QueryBuilder;
using iRLeagueApiCore.Common.Models.Reviews;

namespace iRLeagueApiCore.Client.Endpoints.Reviews;

internal sealed class MoveToSessionEndpoint : PostEndpoint<ReviewModel>, IPostEndpoint<ReviewModel>
{
    public MoveToSessionEndpoint(HttpClientWrapper httpClient, RouteBuilder routeBuilder, long sessionId) :
        base(httpClient, routeBuilder)
    {
        RouteBuilder.AddEndpoint("MoveToSession");
        RouteBuilder.AddParameter(sessionId);
    }
}
