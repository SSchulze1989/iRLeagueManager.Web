using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Server.Models;
using System.Linq.Expressions;

namespace iRLeagueApiCore.Server.Handlers.Schedules;

public class ScheduleHandlerBase<THandler, TRequest> : HandlerBase<THandler, TRequest>
{
    public ScheduleHandlerBase(ILogger<THandler> logger, LeagueDbContext dbContext, IEnumerable<IValidator<TRequest>> validators) :
        base(logger, dbContext, validators)
    {
    }

    protected virtual ScheduleEntity MapToScheduleEntity(LeagueUser user, PutScheduleModel postSchedule, ScheduleEntity target)
    {
        target.Name = postSchedule.Name;
        target.LastModifiedByUserId = user.Id;
        target.LastModifiedByUserName = user.Name;
        target.LastModifiedOn = DateTime.UtcNow;
        target.Version++;
        return target;
    }

    protected virtual async Task<ScheduleModel?> MapToGetScheduleModelAsync(long scheduleId, CancellationToken cancellationToken)
    {
        return await dbContext.Schedules
            .Where(x => x.ScheduleId == scheduleId)
            .Select(MapToGetScheduleModelExpression)
            .SingleOrDefaultAsync(cancellationToken);
    }

    protected virtual Expression<Func<ScheduleEntity, ScheduleModel>> MapToGetScheduleModelExpression => x => new ScheduleModel()
    {
        LeagueId = x.LeagueId,
        ScheduleId = x.ScheduleId,
        SeasonId = x.SeasonId,
        Name = x.Name,
        EventIds = x.Events.Select(x => x.EventId),
        CreatedOn = TreatAsUTCDateTime(x.CreatedOn),
        CreatedByUserId = x.CreatedByUserId,
        CreatedByUserName = x.CreatedByUserName,
        LastModifiedOn = TreatAsUTCDateTime(x.LastModifiedOn),
        LastModifiedByUserId = x.LastModifiedByUserId,
        LastModifiedByUserName = x.LastModifiedByUserName
    };
}
