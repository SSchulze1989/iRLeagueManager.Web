namespace iRLeagueApiCore.Server.Handlers.Reviews;

public record DeleteVoteCategoryRequest(long CatId) : IRequest;

public class DeleteVoteCategoryHandler : VoteCategoriesHandlerBase<DeleteVoteCategoryHandler, DeleteVoteCategoryRequest>, 
    IRequestHandler<DeleteVoteCategoryRequest, Unit>
{
    public DeleteVoteCategoryHandler(ILogger<DeleteVoteCategoryHandler> logger, LeagueDbContext dbContext, IEnumerable<IValidator<DeleteVoteCategoryRequest>> validators) : 
        base(logger, dbContext, validators)
    {
    }

    public async Task<Unit> Handle(DeleteVoteCategoryRequest request, CancellationToken cancellationToken)
    {
        await validators.ValidateAllAndThrowAsync(request, cancellationToken);
        var deleteVoteCategory = await GetVoteCategoryEntityAsync(request.CatId, cancellationToken)
            ?? throw new ResourceNotFoundException();
        dbContext.Remove(deleteVoteCategory);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
