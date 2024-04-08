namespace iRLeagueApiCore.Server.Handlers.Results;

public record DeleteResultRequest(long EventId) : IRequest;

public sealed class DeleteResultHandler : ResultHandlerBase<DeleteResultHandler, DeleteResultRequest>,
    IRequestHandler<DeleteResultRequest, Unit>
{
    public DeleteResultHandler(ILogger<DeleteResultHandler> logger, LeagueDbContext dbContext, IEnumerable<IValidator<DeleteResultRequest>> validators) :
        base(logger, dbContext, validators)
    {
    }

    public async Task<Unit> Handle(DeleteResultRequest request, CancellationToken cancellationToken)
    {
        await validators.ValidateAllAndThrowAsync(request, cancellationToken);
        var deleteEventResults = await GetScoredEventResults(request.EventId, cancellationToken);
        var deleteStandings = await GetEventStandings(request.EventId, cancellationToken);
        foreach (var result in deleteEventResults)
        {
            dbContext.ScoredEventResults.Remove(result);
        }
        foreach (var standing in deleteStandings)
        {
            dbContext.Standings.Remove(standing);
        }
        await dbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }

    private async Task<IEnumerable<ScoredEventResultEntity>> GetScoredEventResults(long eventId, CancellationToken cancellationToken)
    {
        return await dbContext.ScoredEventResults
            .Where(x => x.EventId == eventId)
            .ToListAsync(cancellationToken);
    }

    private async Task<IEnumerable<StandingEntity>> GetEventStandings(long eventId, CancellationToken cancellationToken)
    {
        return await dbContext.Standings
            .Where(x => x.EventId == eventId)
            .ToListAsync(cancellationToken);
    }
}
