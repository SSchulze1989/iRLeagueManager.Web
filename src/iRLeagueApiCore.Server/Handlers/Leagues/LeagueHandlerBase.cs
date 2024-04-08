using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Server.Models;
using System.Linq.Expressions;

namespace iRLeagueApiCore.Server.Handlers.Leagues;

public class LeagueHandlerBase<THandler, TRequest> : HandlerBase<THandler, TRequest>

{
    public LeagueHandlerBase(ILogger<THandler> logger, LeagueDbContext dbContext, IEnumerable<IValidator<TRequest>> validators) :
        base(logger, dbContext, validators)
    {
    }

    protected virtual LeagueEntity MapToLeagueEntity(LeagueUser user, PostLeagueModel postLeague, LeagueEntity leagueEntity)
    {
        leagueEntity.Name = postLeague.Name;
        leagueEntity.NameFull = postLeague.NameFull;
        leagueEntity.LeaguePublic = postLeague.LeaguePublic;
        leagueEntity.Description = postLeague.Description;
        leagueEntity.DescriptionPlain = postLeague.DescriptionPlain;
        leagueEntity.ProtestFormAccess = postLeague.ProtestFormAccess;
        leagueEntity.EnableLiveReviews = postLeague.EnableLiveReviews;
        CreateVersionEntity(user, leagueEntity);
        UpdateVersionEntity(user, leagueEntity);
        return leagueEntity;
    }

    protected virtual async Task<LeagueModel?> MapToGetLeagueModelAsync(long leagueId, bool includeSubscriptionDetails, CancellationToken cancellationToken)
    {
        return await dbContext.Leagues
            .IgnoreQueryFilters()
            .Where(x => x.Id == leagueId)
            .Select(MapToGetLeagueModelExpression(includeSubscriptionDetails))
            .SingleOrDefaultAsync(cancellationToken);
    }

    protected Expression<Func<LeagueEntity, LeagueModel>> MapToGetLeagueModelExpression(bool includeSubscriptionDetails = false) => x => new LeagueModel()
    {
        Id = x.Id,
        Name = x.Name,
        NameFull = x.NameFull,
        Description = x.Description,
        DescriptionPlain = x.DescriptionPlain,
        IsInitialized = x.IsInitialized,
        EnableProtests = x.EnableProtests,
        EnableLiveReviews = x.EnableLiveReviews,
        ProtestCoolDownPeriod = x.ProtestCoolDownPeriod,
        ProtestsClosedAfter = x.ProtestsClosedAfter,
        ProtestFormAccess = x.ProtestFormAccess,
        ProtestsPublic = x.ProtestsPublic,
        SeasonIds = x.Seasons
            .Select(season => season.SeasonId)
            .ToList(),
        CreatedByUserId = x.CreatedByUserId,
        CreatedByUserName = x.CreatedByUserName,
        CreatedOn = TreatAsUTCDateTime(x.CreatedOn),
        LastModifiedByUserId = x.LastModifiedByUserId,
        LastModifiedByUserName = x.LastModifiedByUserName,
        LastModifiedOn = TreatAsUTCDateTime(x.LastModifiedOn),
        LeaguePublic = x.LeaguePublic,
        SubscriptionStatus = includeSubscriptionDetails ? x.Subscription : default,
        SubscriptionExpires = includeSubscriptionDetails ? x.Expires : default,
    };

    protected virtual LeagueEntity MapToLeagueEntity(long leagueId, LeagueUser user, PutLeagueModel putLeague, LeagueEntity leagueEntity)
    {
        leagueEntity.NameFull = putLeague.NameFull;
        leagueEntity.Description = putLeague.Description;
        leagueEntity.DescriptionPlain = putLeague.DescriptionPlain;
        leagueEntity.EnableProtests = putLeague.EnableProtests;
        leagueEntity.ProtestCoolDownPeriod = putLeague.ProtestCoolDownPeriod;
        leagueEntity.ProtestsClosedAfter = putLeague.ProtestsClosedAfter;
        leagueEntity.ProtestsPublic = putLeague.ProtestsPublic;
        leagueEntity.LeaguePublic = putLeague.LeaguePublic;
        leagueEntity.ProtestFormAccess = putLeague.ProtestFormAccess;
        leagueEntity.EnableLiveReviews = putLeague.EnableLiveReviews;
        UpdateVersionEntity(user, leagueEntity);
        return leagueEntity;
    }
}
