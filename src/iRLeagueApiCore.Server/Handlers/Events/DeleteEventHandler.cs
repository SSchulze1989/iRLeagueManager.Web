namespace iRLeagueApiCore.Server.Handlers.Events;

public record DeleteEventRequest(long EventId) : IRequest;

public sealed class DeleteEventHandler : EventHandlerBase<DeleteEventHandler, DeleteEventRequest>, IRequestHandler<DeleteEventRequest>
{
    public DeleteEventHandler(ILogger<DeleteEventHandler> logger, LeagueDbContext dbContext, IEnumerable<IValidator<DeleteEventRequest>> validators) :
        base(logger, dbContext, validators)
    {
    }

    public async Task<Unit> Handle(DeleteEventRequest request, CancellationToken cancellationToken)
    {
        await validators.ValidateAllAndThrowAsync(request, cancellationToken);
        var deleteEvent = await GetEventEntityAsync(request.EventId, cancellationToken)
            ?? throw new ResourceNotFoundException();
        dbContext.Events
            .Remove(deleteEvent);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
