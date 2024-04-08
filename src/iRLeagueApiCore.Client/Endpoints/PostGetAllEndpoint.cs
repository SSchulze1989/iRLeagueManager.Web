using iRLeagueApiCore.Client.Http;
using iRLeagueApiCore.Client.QueryBuilder;
using iRLeagueApiCore.Client.Results;

namespace iRLeagueApiCore.Client.Endpoints;

internal class PostGetAllEndpoint<TGet, TPost> : EndpointBase, IPostGetAllEndpoint<TGet, TPost> where TPost : notnull
{
    public PostGetAllEndpoint(HttpClientWrapper httpClientWrapper, RouteBuilder routeBuilder) : base(httpClientWrapper, routeBuilder)
    {
    }

    async Task<ClientActionResult<IEnumerable<TGet>>> IGetEndpoint<IEnumerable<TGet>>.Get(CancellationToken cancellationToken)
    {
        return await HttpClientWrapper.GetAsClientActionResult<IEnumerable<TGet>>(QueryUrl, cancellationToken);
    }

    async Task<ClientActionResult<TGet>> IPostEndpoint<TGet, TPost>.Post(TPost model, CancellationToken cancellationToken)
    {
        return await HttpClientWrapper.PostAsClientActionResult<TGet>(QueryUrl, model, cancellationToken);
    }
}
