namespace iRLeagueApiCore.Server.Handlers.Reviews;

public record DeleteReviewRequest(long ReviewId) : IRequest;

public sealed class DeleteReviewHandler : ReviewsHandlerBase<DeleteReviewHandler, DeleteReviewRequest>,
    IRequestHandler<DeleteReviewRequest>
{
    public DeleteReviewHandler(ILogger<DeleteReviewHandler> logger, LeagueDbContext dbContext, IEnumerable<IValidator<DeleteReviewRequest>> validators) :
        base(logger, dbContext, validators)
    {
    }

    public async Task<Unit> Handle(DeleteReviewRequest request, CancellationToken cancellationToken)
    {
        await validators.ValidateAllAndThrowAsync(request, cancellationToken);
        var deleteReview = await GetReviewEntity(request.ReviewId, cancellationToken)
            ?? throw new ResourceNotFoundException();
        dbContext.Remove(deleteReview);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
