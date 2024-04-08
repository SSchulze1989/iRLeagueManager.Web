namespace iRLeagueApiCore.Server.Handlers.Reviews;

public record DeleteProtestRequest(long ProtestId) : IRequest;

public class DeleteProtestHandler : ProtestsHandlerBase<DeleteProtestHandler, DeleteProtestRequest>, 
    IRequestHandler<DeleteProtestRequest, Unit>
{
    public DeleteProtestHandler(ILogger<DeleteProtestHandler> logger, LeagueDbContext dbContext, IEnumerable<IValidator<DeleteProtestRequest>> validators) : 
        base(logger, dbContext, validators)
    {
    }

    public async Task<Unit> Handle(DeleteProtestRequest request, CancellationToken cancellationToken)
    {
        await validators.ValidateAllAndThrowAsync(request, cancellationToken);
        var deleteProtest = await GetProtestEntityAsync(request.ProtestId, cancellationToken)
            ?? throw new ResourceNotFoundException();
        dbContext.Protests.Remove(deleteProtest);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
