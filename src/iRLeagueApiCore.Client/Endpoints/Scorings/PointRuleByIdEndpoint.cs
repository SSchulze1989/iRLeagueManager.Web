using iRLeagueApiCore.Client.Http;
using iRLeagueApiCore.Client.QueryBuilder;
using iRLeagueApiCore.Common.Models;

namespace iRLeagueApiCore.Client.Endpoints.Scorings;

internal class PointRuleByIdEndpoint : UpdateEndpoint<PointRuleModel, PutPointRuleModel>, IPointRuleByIdEndpoint
{
    public PointRuleByIdEndpoint(HttpClientWrapper httpClientWrapper, RouteBuilder routeBuilder, long pointRuleId) :
        base(httpClientWrapper, routeBuilder)
    {
        routeBuilder.AddParameter(pointRuleId);
    }
}
