using iRLeagueApiCore.Client.Http;
using iRLeagueApiCore.Client.QueryBuilder;

namespace iRLeagueApiCore.Client.Endpoints.Protests;

internal sealed class ProtestByIdEndpoint : DeleteEndpoint, IProtestByIdEndpoint
{
    public ProtestByIdEndpoint(HttpClientWrapper httpClient, RouteBuilder routeBuilder, long protestId) : 
        base(httpClient, routeBuilder)
    {
        RouteBuilder.AddParameter(protestId);
    }
}
