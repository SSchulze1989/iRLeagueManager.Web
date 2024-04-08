using iRLeagueApiCore.Client.Endpoints.Protests;
using iRLeagueApiCore.Client.Endpoints.Reviews;
using iRLeagueApiCore.Client.Http;
using iRLeagueApiCore.Client.QueryBuilder;
using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Common.Models.Reviews;

namespace iRLeagueApiCore.Client.Endpoints.Sessions;

internal sealed class SessionByIdEndpoint : EndpointBase, ISessionByIdEndpoint
{
    public SessionByIdEndpoint(HttpClientWrapper httpClient, RouteBuilder routeBuilder, long sessionId) :
        base(httpClient, routeBuilder)
    {
        RouteBuilder.AddParameter(sessionId);
    }

    public IPostEndpoint<ReviewModel, PostReviewModel> Reviews()
    {
        return new ReviewsEndpoint(HttpClientWrapper, RouteBuilder);
    }

    public IPostEndpoint<ProtestModel, PostProtestModel> Protests()
    {
        return new ProtestsEndpoint(HttpClientWrapper, RouteBuilder);
    }
}
