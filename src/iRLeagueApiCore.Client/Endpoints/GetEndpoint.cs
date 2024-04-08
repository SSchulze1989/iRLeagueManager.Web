using iRLeagueApiCore.Client.Http;
using iRLeagueApiCore.Client.QueryBuilder;
using iRLeagueApiCore.Client.Results;

namespace iRLeagueApiCore.Client.Endpoints;

internal class GetEndpoint<T> : EndpointBase, IGetEndpoint<T>
{
    public GetEndpoint(HttpClientWrapper httpClient, RouteBuilder routeBuilder) :
        base(httpClient, routeBuilder)
    {
    }

    async Task<ClientActionResult<T>> IGetEndpoint<T>.Get(CancellationToken cancellationToken)
    {
        return await HttpClientWrapper.GetAsClientActionResult<T>(QueryUrl, cancellationToken);
    }
}
