using iRLeagueApiCore.Client.Endpoints.Penalties;
using iRLeagueApiCore.Client.Http;
using iRLeagueApiCore.Client.QueryBuilder;
using iRLeagueApiCore.Common.Models;

namespace iRLeagueApiCore.Client.Endpoints.Results;

internal class ResultByIdEndpoint : GetEndpoint<EventResultModel>, IResultByIdEndpoint
{
    public ResultByIdEndpoint(HttpClientWrapper httpClient, RouteBuilder routeBuilder, long resultId) :
        base(httpClient, routeBuilder)
    {
        RouteBuilder.AddParameter(resultId);
    }

    public IGetAllEndpoint<PenaltyModel> Penalties()
    {
        return new PenaltiesEndpoint(HttpClientWrapper, RouteBuilder);
    }
}
