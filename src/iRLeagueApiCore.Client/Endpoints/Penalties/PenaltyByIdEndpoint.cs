using iRLeagueApiCore.Client.Http;
using iRLeagueApiCore.Client.QueryBuilder;
using iRLeagueApiCore.Client.Results;
using iRLeagueApiCore.Common.Models;

namespace iRLeagueApiCore.Client.Endpoints.Penalties;

internal sealed class PenaltyByIdEndpoint : EndpointBase, IPenaltyByIdEndpoint
{
    public PenaltyByIdEndpoint(HttpClientWrapper httpClient, RouteBuilder routeBuilder, long id) 
        : base(httpClient, routeBuilder)
    {
        RouteBuilder.AddParameter(id);
    }

    public async Task<ClientActionResult<NoContent>> Delete(CancellationToken cancellationToken = default)
    {
        return await HttpClientWrapper.DeleteAsClientActionResult(QueryUrl, cancellationToken);
    }

    public async Task<ClientActionResult<PenaltyModel>> Put(PutPenaltyModel model, CancellationToken cancellationToken = default)
    {
        return await HttpClientWrapper.PutAsClientActionResult<PenaltyModel>(QueryUrl, model, cancellationToken);
    }
}