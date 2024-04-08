using iRLeagueApiCore.Common.Models;

namespace iRLeagueApiCore.Server.Handlers.Events;

public record GetEventsFromSeasonRequest(long SeasonId, bool IncludeDetails) : IRequest<IEnumerable<EventModel>>;

public sealed class GetEventsFromSeasonHandler : EventHandlerBase<GetEventsFromSeasonHandler, GetEventsFromSeasonRequest>,
    IRequestHandler<GetEventsFromSeasonRequest, IEnumerable<EventModel>>
{
    public GetEventsFromSeasonHandler(ILogger<GetEventsFromSeasonHandler> logger, LeagueDbContext dbContext,
        IEnumerable<IValidator<GetEventsFromSeasonRequest>> validators) : base(logger, dbContext, validators)
    {
    }

    public async Task<IEnumerable<EventModel>> Handle(GetEventsFromSeasonRequest request, CancellationToken cancellationToken)
    {
        await validators.ValidateAllAndThrowAsync(request, cancellationToken);
        var getEvents = await MapToEventsFromSeasonAsync(request.SeasonId, request.IncludeDetails, cancellationToken);
        return getEvents;
    }

    private async Task<IEnumerable<EventModel>> MapToEventsFromSeasonAsync(long seasonId, bool includeDetails = false,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Events
            .Where(x => x.Schedule.SeasonId == seasonId)
            .Select(MapToEventModelExpression(includeDetails))
            .ToListAsync();
    }
}
