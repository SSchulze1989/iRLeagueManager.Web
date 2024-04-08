using iRLeagueApiCore.Common.Models.Reviews;
using iRLeagueApiCore.Server.Models;

namespace iRLeagueApiCore.Server.Handlers.Reviews;

public record PutReviewCommentRequest(long CommentId, LeagueUser User, PutReviewCommentModel Model) : IRequest<ReviewCommentModel>;

public sealed class PutReviewCommentHandler : CommentHandlerBase<PutReviewCommentHandler, PutReviewCommentRequest>,
    IRequestHandler<PutReviewCommentRequest, ReviewCommentModel>
{
    public PutReviewCommentHandler(ILogger<PutReviewCommentHandler> logger, LeagueDbContext dbContext, IEnumerable<IValidator<PutReviewCommentRequest>> validators) :
        base(logger, dbContext, validators)
    {
    }

    public async Task<ReviewCommentModel> Handle(PutReviewCommentRequest request, CancellationToken cancellationToken)
    {
        await validators.ValidateAllAndThrowAsync(request, cancellationToken);
        var putComment = await GetCommentEntityAsync(request.CommentId, cancellationToken)
            ?? throw new ResourceNotFoundException();
        putComment = await MapToReviewCommentEntityAsync(request.User, request.Model, putComment, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        var getComment = await MapToReviewCommentModelAsync(putComment.CommentId, cancellationToken)
            ?? throw new InvalidOperationException("Created resource was not found");
        return getComment;
    }
}
