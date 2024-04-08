using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Server.Models;
using System.Linq.Expressions;

namespace iRLeagueApiCore.Server.Handlers.Events;

public class EventHandlerBase<THandler, TRequest> : HandlerBase<THandler, TRequest>
{
    public EventHandlerBase(ILogger<THandler> logger, LeagueDbContext dbContext, IEnumerable<IValidator<TRequest>> validators) :
        base(logger, dbContext, validators)
    {
    }

    protected virtual async Task<EventEntity?> GetEventEntityAsync(long eventId, CancellationToken cancellationToken)
    {
        return await dbContext.Events
            .Include(x => x.Sessions)
            .Include(x => x.Track)
            .Include(x => x.ResultConfigs)
            .Where(x => x.EventId == eventId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    protected virtual async Task<EventEntity> MapToEventEntityAsync(LeagueUser user, PostEventModel postEvent, EventEntity target, CancellationToken cancellationToken)
    {
        target.Date = postEvent.Date;
        target.Duration = postEvent.Duration;
        target.EventType = postEvent.EventType;
        target.Name = postEvent.Name;
        MapToSessionEntityCollection(user, postEvent.Sessions, target.Sessions);
        target.Track = await GetTrackConfigEntityAsync(postEvent.TrackId, cancellationToken);
        target.ResultConfigs = await GetResultConfigEntities(postEvent.ResultConfigs, cancellationToken);
        return UpdateVersionEntity(user, target);
    }

    protected virtual async Task<EventEntity> MapToEventEntityAsync(LeagueUser user, PutEventModel putEvent, EventEntity target, CancellationToken cancellationToken)
    {
        return await MapToEventEntityAsync(user, (PostEventModel)putEvent, target, cancellationToken);
    }

    protected virtual void MapToSessionEntityCollection(LeagueUser user, IEnumerable<SessionModel> putSessions,
        ICollection<SessionEntity> target)
    {
        List<long> keepSubSessionIds = new List<long>();
        foreach (var putSession in putSessions)
        {
            // try to find subsession in target collection
            var sessionEntity = target
                .FirstOrDefault(x => putSession.SessionId != 0 && x.SessionId == putSession.SessionId);
            // create new subsession if no previous id was given
            if (putSession.SessionId == 0)
            {
                sessionEntity = new SessionEntity();
                target.Add(sessionEntity);
            }
            if (sessionEntity == null)
            {
                throw new InvalidOperationException($"Error while mapping SessionEntities to Event: SessionId:{putSession.SessionId} does not exist in target collection Sessions");
            }
            MapToSessionEntity(user, putSession, sessionEntity);
        }
        // remove subsessions that are not referenced
        var removeSessions = target
            .ExceptBy(putSessions.Select(x => x.SessionId), x => x.SessionId);
        foreach (var removeSession in removeSessions)
        {
            target.Remove(removeSession);
        }
    }

    protected async Task<ICollection<ResultConfigurationEntity>> GetResultConfigEntities(IEnumerable<ResultConfigInfoModel> resultConfigModels, CancellationToken cancellationToken)
    {
        var resultConfigIds = resultConfigModels.Select(x => x.ResultConfigId);
        return await dbContext.ResultConfigurations
            .Where(x => resultConfigIds.Contains(x.ResultConfigId))
            .ToListAsync(cancellationToken);
    }
    protected virtual SessionEntity MapToSessionEntity(LeagueUser user, PutSessionModel putSession, SessionEntity target)
    {
        target.Name = putSession.Name;
        target.SessionType = putSession.SessionType;
        target.SessionNr = putSession.SessionNr;
        target.Laps = putSession.Laps;
        target.Duration = putSession.Duration;
        return UpdateVersionEntity(user, target);
    }

    protected virtual async Task<EventModel?> MapToEventModelAsync(long eventId, bool includeDetails = false, CancellationToken cancellationToken = default)
    {
        var query = dbContext.Events
            .Where(x => x.EventId == eventId)
            .Select(MapToEventModelExpression(includeDetails));
        var sql = query.ToQueryString();
        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    protected virtual Expression<Func<EventEntity, EventModel>> MapToEventModelExpression(bool includeDetails = false) => @event => new EventModel()
    {
        Date = TreatAsUTCDateTime(@event.Date),
        Duration = @event.Duration,
        EventType = @event.EventType,
        Id = @event.EventId,
        LeagueId = @event.LeagueId,
        Name = @event.Name,
        ScheduleId = @event.ScheduleId,
        SeasonId = @event.Schedule.SeasonId,
        HasResult = @event.ScoredEventResults.Any(),
        TrackName = @event.Track.TrackGroup.TrackName,
        ConfigName = @event.Track.ConfigName,
        Sessions = @event.Sessions.Select(session => new SessionModel()
        {
            HasResult = session.SessionResult != null,
            SessionNr = session.SessionNr,
            LeagueId = session.LeagueId,
            Name = session.Name,
            Laps = session.Laps,
            Duration = session.Duration,
            SessionId = session.SessionId,
            SessionType = session.SessionType,
            CreatedOn = TreatAsUTCDateTime(session.CreatedOn),
            CreatedByUserId = session.CreatedByUserId,
            CreatedByUserName = session.CreatedByUserName,
            LastModifiedOn = TreatAsUTCDateTime(session.LastModifiedOn),
            LastModifiedByUserId = session.LastModifiedByUserId,
            LastModifiedByUserName = session.LastModifiedByUserName
        }).ToList(),
        TrackId = @event.TrackId,
        ResultConfigs = @event.ResultConfigs.Select(config => new ResultConfigInfoModel()
        {
            LeagueId = config.LeagueId,
            ResultConfigId = config.ResultConfigId,
            ChampSeasonId = config.ChampSeasonId,
            ChampionshipName = config.ChampSeason.Championship.Name,
            Name = config.Name,
            DisplayName = config.DisplayName,
        }).ToList(),
        SimSessionDetails = (includeDetails == false || @event.SimSessionDetails.Any() == false) ? null :
            @event.SimSessionDetails.Take(1).Select(details => new SimSessionDetailsModel()
            {
                EventAverageLap = details.EventAverageLap,
                EndTime = details.EndTime,
                EventLapsComplete = details.EventLapsComplete,
                EventStrengthOfField = details.EventStrengthOfField,
                Fog = details.Fog,
                IRRaceWeek = details.IRRaceWeek,
                IRSessionId = details.IRSessionId,
                IRSubsessionId = details.IRSubsessionId,
                LicenseCategory = details.LicenseCategory,
                RelHumidity = details.RelHumidity,
                SessionDetailsId = details.SessionDetailsId,
                SessionName = details.SessionName,
                SimStartUtcOffset = details.SimStartUtcOffset,
                SimStartUtcTime = details.SimStartUtcTime,
                Skies = details.Skies,
                StartTime = details.StartTime,
                TempUnits = details.TempUnits,
                TempValue = details.TempValue,
                TimeOfDay = details.TimeOfDay,
                WeatherType = details.WeatherType,
                WeatherVarInitial = details.WeatherVarInitial,
                WeatherVarOngoing = details.WeatherVarOngoing,
                WindDir = details.WindDir,
                WindUnits = details.WindUnits,
            }).First(),
    };
}
