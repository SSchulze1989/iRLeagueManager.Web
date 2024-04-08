using iRLeagueApiCore.Client.Http;
using iRLeagueApiCore.Client.QueryBuilder;
using iRLeagueApiCore.Client.Results;

namespace iRLeagueApiCore.Client.Endpoints;

internal class UpdateEndpoint<TResult, TModel> : EndpointBase, IUpdateEndpoint<TResult, TModel> where TModel : notnull
{
    public UpdateEndpoint(HttpClientWrapper httpClientWrapper, RouteBuilder routeBuilder) :
        base(httpClientWrapper, routeBuilder)
    {
    }

    async Task<ClientActionResult<NoContent>> IDeleteEndpoint.Delete(CancellationToken cancellationToken)
    {
        return await HttpClientWrapper.DeleteAsClientActionResult(QueryUrl, cancellationToken);
    }

    async Task<ClientActionResult<TResult>> IGetEndpoint<TResult>.Get(CancellationToken cancellationToken)
    {
        return await HttpClientWrapper.GetAsClientActionResult<TResult>(QueryUrl, cancellationToken);
    }

    async Task<ClientActionResult<TResult>> IPutEndpoint<TResult, TModel>.Put(TModel model, CancellationToken cancellationToken)
    {
        return await HttpClientWrapper.PutAsClientActionResult<TResult>(QueryUrl, model, cancellationToken);
    }
}
