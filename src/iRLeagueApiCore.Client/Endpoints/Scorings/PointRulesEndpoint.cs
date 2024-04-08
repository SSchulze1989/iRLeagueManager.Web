using iRLeagueApiCore.Client.Http;
using iRLeagueApiCore.Client.QueryBuilder;
using iRLeagueApiCore.Common.Models;

namespace iRLeagueApiCore.Client.Endpoints.Scorings;

internal class PointRulesEndpoint : PostEndpoint<PointRuleModel, PostPointRuleModel>, IPointRulesEndpoint
{
    public PointRulesEndpoint(HttpClientWrapper httpClientWrapper, RouteBuilder routeBuilder) :
        base(httpClientWrapper, routeBuilder)
    {
        routeBuilder.AddEndpoint("PointRules");
    }

    public IPointRuleByIdEndpoint WithId(long id)
    {
        return new PointRuleByIdEndpoint(HttpClientWrapper, RouteBuilder, id);
    }
}
