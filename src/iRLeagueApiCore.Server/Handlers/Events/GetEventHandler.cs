using iRLeagueApiCore.Common.Models;

namespace iRLeagueApiCore.Server.Handlers.Events;

public record GetEventRequest(long EventId, bool IncludeDetails) : IRequest<EventModel>;

public sealed class GetEventHandler : EventHandlerBase<GetEventHandler, GetEventRequest>, IRequestHandler<GetEventRequest, EventModel>
{
    public GetEventHandler(ILogger<GetEventHandler> logger, LeagueDbContext dbContext, IEnumerable<IValidator<GetEventRequest>> validators) :
        base(logger, dbContext, validators)
    {
    }

    public async Task<EventModel> Handle(GetEventRequest request, CancellationToken cancellationToken)
    {
        await validators.ValidateAllAndThrowAsync(request, cancellationToken);
        var getEvent = await MapToEventModelAsync(request.EventId, includeDetails: request.IncludeDetails, cancellationToken: cancellationToken)
            ?? throw new ResourceNotFoundException();
        return getEvent;
    }
}
