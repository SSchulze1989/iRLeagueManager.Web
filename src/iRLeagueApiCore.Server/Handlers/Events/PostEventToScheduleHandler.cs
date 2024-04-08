using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Server.Models;

namespace iRLeagueApiCore.Server.Handlers.Events;

public record PostEventToScheduleRequest(long ScheduleId, LeagueUser User, PostEventModel Event) : IRequest<EventModel>;

public sealed class PostEventToScheduleHandler : EventHandlerBase<PostEventToScheduleHandler, PostEventToScheduleRequest>, IRequestHandler<PostEventToScheduleRequest, EventModel>
{
    public PostEventToScheduleHandler(ILogger<PostEventToScheduleHandler> logger, LeagueDbContext dbContext, IEnumerable<IValidator<PostEventToScheduleRequest>> validators) :
        base(logger, dbContext, validators)
    {
    }

    public async Task<EventModel> Handle(PostEventToScheduleRequest request, CancellationToken cancellationToken)
    {
        await validators.ValidateAllAndThrowAsync(request, cancellationToken);
        var postEvent = await CreateEventOnScheduleAsync(request.User, request.ScheduleId, cancellationToken);
        postEvent = await MapToEventEntityAsync(request.User, request.Event, postEvent, cancellationToken);
        dbContext.Events.Add(postEvent);
        await dbContext.SaveChangesAsync();
        var getEvent = await MapToEventModelAsync(postEvent.EventId, cancellationToken: cancellationToken)
            ?? throw new ResourceNotFoundException();
        return getEvent;
    }

    private async Task<EventEntity> CreateEventOnScheduleAsync(LeagueUser user, long scheduleId, CancellationToken cancellationToken)
    {
        var schedule = await dbContext.Schedules
            .Include(x => x.Events)
            .SingleOrDefaultAsync(x => x.ScheduleId == scheduleId, cancellationToken)
            ?? throw new ResourceNotFoundException();
        var @event = CreateVersionEntity(user, new EventEntity());
        schedule.Events.Add(@event);
        return @event;
    }
}
