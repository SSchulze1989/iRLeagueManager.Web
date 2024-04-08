using iRLeagueApiCore.Client.Http;
using iRLeagueApiCore.Client.QueryBuilder;

namespace iRLeagueApiCore.Client.Endpoints;
internal sealed class GetLatestEndpoint<T> : GetEndpoint<T>
{
    public GetLatestEndpoint(HttpClientWrapper httpClient, RouteBuilder routeBuilder) : base(httpClient, routeBuilder)
    {
        RouteBuilder.AddEndpoint("Latest");
    }
}
