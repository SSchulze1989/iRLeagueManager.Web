using iRLeagueApiCore.Common.Models;

namespace iRLeagueApiCore.Server.Handlers.Events;

public record GetEventsFromScheduleRequest(long ScheduleId, bool IncludeDetails = false) : IRequest<IEnumerable<EventModel>>;

public sealed class GetEventsFromScheduleHandler : EventHandlerBase<GetEventsFromScheduleHandler, GetEventsFromScheduleRequest>,
    IRequestHandler<GetEventsFromScheduleRequest, IEnumerable<EventModel>>
{
    public GetEventsFromScheduleHandler(ILogger<GetEventsFromScheduleHandler> logger, LeagueDbContext dbContext,
        IEnumerable<IValidator<GetEventsFromScheduleRequest>> validators) : base(logger, dbContext, validators)
    {
    }

    public async Task<IEnumerable<EventModel>> Handle(GetEventsFromScheduleRequest request, CancellationToken cancellationToken)
    {
        await validators.ValidateAllAndThrowAsync(request, cancellationToken);
        var getEvents = await MapToGetEventsFromScheduleAsync(request.ScheduleId, request.IncludeDetails, cancellationToken);
        return getEvents;
    }

    private async Task<IEnumerable<EventModel>> MapToGetEventsFromScheduleAsync(long scheduleId, bool includeDetails = false,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Events
            .Where(x => x.ScheduleId == scheduleId)
            .Select(MapToEventModelExpression(includeDetails))
            .ToListAsync(cancellationToken);
    }
}
