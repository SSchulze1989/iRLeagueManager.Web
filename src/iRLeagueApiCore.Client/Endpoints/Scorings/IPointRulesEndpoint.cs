using iRLeagueApiCore.Common.Models;

namespace iRLeagueApiCore.Client.Endpoints.Scorings;

public interface IPointRulesEndpoint : IPostEndpoint<PointRuleModel, PostPointRuleModel>, IWithIdEndpoint<IPointRuleByIdEndpoint>
{
}
