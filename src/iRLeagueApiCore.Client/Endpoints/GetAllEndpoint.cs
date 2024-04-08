using iRLeagueApiCore.Client.Http;
using iRLeagueApiCore.Client.QueryBuilder;
using iRLeagueApiCore.Client.Results;

namespace iRLeagueApiCore.Client.Endpoints;

internal class GetAllEndpoint<T> : EndpointBase, IGetAllEndpoint<T>
{
    public GetAllEndpoint(HttpClientWrapper httpClientWrapper, RouteBuilder routeBuilder) :
        base(httpClientWrapper, routeBuilder)
    {
    }

    async Task<ClientActionResult<IEnumerable<T>>> IGetEndpoint<IEnumerable<T>>.Get(CancellationToken cancellationToken)
    {
        return await HttpClientWrapper.GetAsClientActionResult<IEnumerable<T>>(QueryUrl, cancellationToken);
    }
}
