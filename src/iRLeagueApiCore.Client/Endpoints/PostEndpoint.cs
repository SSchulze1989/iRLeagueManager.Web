using iRLeagueApiCore.Client.Http;
using iRLeagueApiCore.Client.QueryBuilder;
using iRLeagueApiCore.Client.Results;

namespace iRLeagueApiCore.Client.Endpoints;

internal class PostEndpoint<TResult> : EndpointBase, IPostEndpoint<TResult>
{
    public PostEndpoint(HttpClientWrapper httpClient, RouteBuilder routeBuilder) :
        base(httpClient, routeBuilder)
    {
    }

    public async Task<ClientActionResult<TResult>> Post(CancellationToken cancellationToken)
    {
        return await HttpClientWrapper.PostAsClientActionResult<TResult>(QueryUrl, null, cancellationToken);
    }
}

internal class PostEndpoint<TResult, TModel> : EndpointBase, IPostEndpoint<TResult, TModel> where TModel : notnull
{
    public PostEndpoint(HttpClientWrapper httpClientWrapper, RouteBuilder routeBuilder) :
        base(httpClientWrapper, routeBuilder)
    { }

    async Task<ClientActionResult<TResult>> IPostEndpoint<TResult, TModel>.Post(TModel model, CancellationToken cancellationToken)
    {
        return await HttpClientWrapper.PostAsClientActionResult<TResult>(QueryUrl, model, cancellationToken);
    }
}
