

using iRLeagueApiCore.Client.Http;
using iRLeagueApiCore.Client.QueryBuilder;

namespace iRLeagueApiCore.Client.Endpoints.Sessions;

internal sealed class SessionsEndpoint : EndpointBase, ISessionsEndpoint
{
    public SessionsEndpoint(HttpClientWrapper httpClient, RouteBuilder routeBuilder) :
        base(httpClient, routeBuilder)
    {
        RouteBuilder.AddEndpoint("Sessions");
    }

    public ISessionByIdEndpoint WithId(long id)
    {
        return new SessionByIdEndpoint(HttpClientWrapper, RouteBuilder, id);
    }
}
