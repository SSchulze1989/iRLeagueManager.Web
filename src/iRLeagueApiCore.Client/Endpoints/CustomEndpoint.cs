using iRLeagueApiCore.Client.Http;
using iRLeagueApiCore.Client.QueryBuilder;
using iRLeagueApiCore.Client.Results;

namespace iRLeagueApiCore.Client.Endpoints;

internal sealed class CustomEndpoint<T> : UpdateEndpoint<T, object>, ICustomEndpoint<T>
{
    public CustomEndpoint(HttpClientWrapper httpClientWrapper, RouteBuilder routeBuilder, string route) : base(httpClientWrapper, routeBuilder)
    {
        RouteBuilder.AddEndpoint(route);
    }

    async Task<ClientActionResult<T>> IPostEndpoint<T, object>.Post(object model, CancellationToken cancellationToken)
    {
        return await HttpClientWrapper.PostAsClientActionResult<T>(QueryUrl, model, cancellationToken);
    }

    async Task<ClientActionResult<T>> IPostEndpoint<T>.Post(CancellationToken cancellationToken)
    {
        return await HttpClientWrapper.PostAsClientActionResult<T>(QueryUrl, null, cancellationToken);
    }

    async Task<ClientActionResult<IEnumerable<T>>> ICustomEndpoint<T>.GetAll(CancellationToken cancellationToken)
    {
        return await HttpClientWrapper.GetAsClientActionResult<IEnumerable<T>>(QueryUrl, cancellationToken);
    }
}
