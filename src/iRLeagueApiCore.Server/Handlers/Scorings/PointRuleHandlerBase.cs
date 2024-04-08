using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Server.Models;
using System.Linq.Expressions;

namespace iRLeagueApiCore.Server.Handlers.Scorings;

public class PointRuleHandlerBase<THandler, TRequest> : HandlerBase<THandler, TRequest>
{
    public PointRuleHandlerBase(ILogger<THandler> logger, LeagueDbContext dbContext, IEnumerable<IValidator<TRequest>> validators) :
        base(logger, dbContext, validators)
    {
    }

    protected virtual async Task<PointRuleEntity?> GetPointRuleEntityAsync(long pointRuleId, CancellationToken cancellationToken)
    {
        return await dbContext.PointRules
            .Where(x => x.PointRuleId == pointRuleId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    protected virtual async Task<PointRuleEntity> MapToPointRuleEntityAsync(LeagueUser user, PostPointRuleModel postPointRule, PointRuleEntity pointRuleEntity,
        CancellationToken cancellationToken)
    {
        pointRuleEntity.BonusPoints = postPointRule.BonusPoints;
        pointRuleEntity.FinalSortOptions = postPointRule.FinalSortOptions.ToList();
        pointRuleEntity.MaxPoints = postPointRule.MaxPoints;
        pointRuleEntity.Name = postPointRule.Name;
        pointRuleEntity.PointDropOff = postPointRule.PointDropOff;
        pointRuleEntity.PointsPerPlace = postPointRule.PointsPerPlace.ToList();
        pointRuleEntity.PointsSortOptions = postPointRule.PointsSortOptions.ToList();
        UpdateVersionEntity(user, pointRuleEntity);
        return await Task.FromResult(pointRuleEntity);
    }

    protected virtual async Task<PointRuleModel?> MapToPointRuleModel(long pointRuleId, CancellationToken cancellationToken)
    {
        return await dbContext.PointRules
            .Where(x => x.PointRuleId == pointRuleId)
            .Select(MapToPointRuleModelExpression)
            .FirstOrDefaultAsync(cancellationToken);
    }

    private Expression<Func<PointRuleEntity, PointRuleModel>> MapToPointRuleModelExpression => pointRule => new PointRuleModel()
    {
        BonusPoints = pointRule.BonusPoints,
        FinalSortOptions = pointRule.FinalSortOptions,
        LeagueId = pointRule.LeagueId,
        MaxPoints = pointRule.MaxPoints,
        Name = pointRule.Name,
        PointDropOff = pointRule.PointDropOff,
        PointRuleId = pointRule.PointRuleId,
        PointsPerPlace = pointRule.PointsPerPlace.ToList(),
        PointsSortOptions = pointRule.PointsSortOptions,
    };
}
