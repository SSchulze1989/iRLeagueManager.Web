namespace iRLeagueApiCore.Server.Handlers.Scorings;

public record DeleteScoringRequest(long ScoringId) : IRequest;

public sealed class DeleteScoringHandler : ScoringHandlerBase<DeleteScoringHandler, DeleteScoringRequest>, IRequestHandler<DeleteScoringRequest>
{
    public DeleteScoringHandler(ILogger<DeleteScoringHandler> logger, LeagueDbContext dbContext, IEnumerable<IValidator<DeleteScoringRequest>> validators) : base(logger, dbContext, validators)
    {
    }

    public async Task<Unit> Handle(DeleteScoringRequest request, CancellationToken cancellationToken)
    {
        await validators.ValidateAllAndThrowAsync(request, cancellationToken);
        var scoring = await GetScoringEntityAsync(request.ScoringId) ?? throw new ResourceNotFoundException();
        dbContext.Scorings.Remove(scoring);
        await dbContext.SaveChangesAsync();
        _logger.LogInformation("Removed scoring {ScoringId} inside league {LeagueId} from database",
            scoring.ScoringId, scoring.LeagueId);
        return Unit.Value;
    }
}
