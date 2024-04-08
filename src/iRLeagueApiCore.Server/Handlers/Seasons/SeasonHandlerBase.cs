using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Server.Models;
using System.Linq.Expressions;

namespace iRLeagueApiCore.Server.Handlers.Seasons;

public abstract class SeasonHandlerBase<THandler, TRequest> : HandlerBase<THandler, TRequest>
{
    protected SeasonHandlerBase(ILogger<THandler> logger, LeagueDbContext dbContext, IEnumerable<IValidator<TRequest>> validators) :
        base(logger, dbContext, validators)
    {
    }

    protected virtual async Task<SeasonEntity> MapToSeasonEntityAsync(LeagueUser user, PostSeasonModel putSeason,
        SeasonEntity target, CancellationToken cancellationToken)
    {
        target.Finished = putSeason.Finished;
        target.HideCommentsBeforeVoted = putSeason.HideComments;
        target.MainScoring = await GetScoringEntityAsync(putSeason.MainScoringId, cancellationToken);
        target.SeasonName = putSeason.SeasonName;
        return UpdateVersionEntity(user, target);
    }

    protected virtual async Task<SeasonModel?> MapToGetSeasonModel(long seasonId, CancellationToken cancellationToken)
    {
        return await dbContext.Seasons
            .Where(x => x.SeasonId == seasonId)
            .Select(MapToGetSeasonModelExpression)
            .SingleOrDefaultAsync();
    }

    protected Expression<Func<SeasonEntity, SeasonModel>> MapToGetSeasonModelExpression => x => new SeasonModel()
    {
        SeasonId = x.SeasonId,
        Finished = x.Finished,
        HideComments = x.HideCommentsBeforeVoted,
        LeagueId = x.LeagueId,
        ScheduleIds = x.Schedules.Select(x => x.ScheduleId).ToList(),
        SeasonEnd = x.Schedules
            .SelectMany(x => x.Events)
            .Select(x => x.Date)
            .OrderByDescending(x => x)
            .FirstOrDefault(),
        SeasonStart = x.Schedules
            .SelectMany(x => x.Events)
            .Select(x => x.Date)
            .OrderBy(x => x)
            .FirstOrDefault(),
        SeasonName = x.SeasonName,
        CreatedByUserId = x.CreatedByUserId,
        CreatedByUserName = x.CreatedByUserName,
        CreatedOn = TreatAsUTCDateTime(x.CreatedOn),
        LastModifiedByUserId = x.LastModifiedByUserId,
        LastModifiedByUserName = x.LastModifiedByUserName,
        LastModifiedOn = TreatAsUTCDateTime(x.LastModifiedOn),
    };
}
