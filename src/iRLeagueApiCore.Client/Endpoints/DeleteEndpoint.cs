using iRLeagueApiCore.Client.Http;
using iRLeagueApiCore.Client.QueryBuilder;
using iRLeagueApiCore.Client.Results;

namespace iRLeagueApiCore.Client.Endpoints;

internal class DeleteEndpoint : EndpointBase, IDeleteEndpoint
{
    public DeleteEndpoint(HttpClientWrapper httpClient, RouteBuilder routeBuilder) : 
        base(httpClient, routeBuilder)
    {
    }

    public async Task<ClientActionResult<NoContent>> Delete(CancellationToken cancellationToken = default)
    {
        return await HttpClientWrapper.DeleteAsClientActionResult(QueryUrl, cancellationToken);
    }
}
