namespace iRLeagueApiCore.Server.Handlers.Reviews;

public record DeletePenaltyRequest(long PenaltyId) : IRequest;

public class DeletePenaltyHandler : ReviewsHandlerBase<DeletePenaltyHandler, DeletePenaltyRequest>, 
    IRequestHandler<DeletePenaltyRequest, Unit>
{
    public DeletePenaltyHandler(ILogger<DeletePenaltyHandler> logger, LeagueDbContext dbContext, IEnumerable<IValidator<DeletePenaltyRequest>> validators) 
        : base(logger, dbContext, validators)
    {
    }

    public async Task<Unit> Handle(DeletePenaltyRequest request, CancellationToken cancellationToken)
    {
        await validators.ValidateAllAndThrowAsync(request, cancellationToken);
        var deletePenalty = await GetAddPenaltyEntity(request.PenaltyId, cancellationToken)
            ?? throw new ResourceNotFoundException();
        dbContext.Remove(deletePenalty);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
