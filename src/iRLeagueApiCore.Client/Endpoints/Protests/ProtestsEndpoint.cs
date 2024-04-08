using iRLeagueApiCore.Client.Http;
using iRLeagueApiCore.Client.QueryBuilder;
using iRLeagueApiCore.Common.Models;

namespace iRLeagueApiCore.Client.Endpoints.Protests;

internal sealed class ProtestsEndpoint : PostGetAllEndpoint<ProtestModel, PostProtestModel>, IProtestsEndpoint
{
    public ProtestsEndpoint(HttpClientWrapper httpClientWrapper, RouteBuilder routeBuilder) : 
        base(httpClientWrapper, routeBuilder)
    {
        RouteBuilder.AddEndpoint("Protests");
    }

    IProtestByIdEndpoint IWithIdEndpoint<IProtestByIdEndpoint, long>.WithId(long id)
    {
        return new ProtestByIdEndpoint(HttpClientWrapper, RouteBuilder, id);
    }
}
