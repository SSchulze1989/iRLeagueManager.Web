using iRLeagueApiCore.Client.Http;
using iRLeagueApiCore.Client.QueryBuilder;

namespace iRLeagueApiCore.Client.Endpoints.Results;
internal class FetchEndpoint : EndpointBase, IFetchResultsEndpoint
{
    public FetchEndpoint(HttpClientWrapper httpClient, RouteBuilder routeBuilder) : base(httpClient, routeBuilder)
    {
        RouteBuilder.AddEndpoint("Fetch");
    }

    IPostEndpoint<bool> IFetchResultsEndpoint.FromIracingSubSession(int subSessionId)
    {
        return new PostEndpoint<bool>(HttpClientWrapper, RouteBuilder).AddRouteParameter(subSessionId);
    }
}
